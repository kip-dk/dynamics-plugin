### Building solid and testable plugins.

The purpose of this project is to create a baseline for easy creating plugins for Dynamics 365 that follow some
good helthy patterns with service oriented structure of code, that makes the code easyer to read, maintain and reuse
cross plugins, but also enable us to write unit test that potentially can be executed outside Dynamics 365.

This page will give some guidance on how to build plugins.

### Plugin project structure.

#### Plugins
All plugins should be put in the "Plugins" folder of Kipon.Dynamics.Plugin (or whatever name you renamed this project). For larger project, it is a good idea to create a sub folder
for each entity, ex. in the initial source we already have an "Plugins/Account" folder.

```csharp
using Kipon.Dynamics.Plugin.Attributes;

namespace Kipon.Dynamics.Plugin.Plugins.Account
{
    [Step(EventType = CrmEventType.Create, PrimaryEntity = Entities.Account.EntityLogicalName, Stage = StageEnum.PreOperation)]
    public class AccountCreatePre : AbstractBasePlugin
    {
        protected override void Execute(DI.IPluginContext pluginContext)
        {
            var target = pluginContext.Target.ToEntity<Entities.Account>();
            pluginContext.GetService<ServiceAPI.IAccountService>().UppercaseName(target);
        }
    }
}
```

Line 1: Import the class decoration namespace, so you can decorate you plugin to be deployed using the Kipon.PluginRegistration tool.<br />
Line 4: Decorate the plugin class, in this case, listen on Create for entity type Account in stage PreOperation.<br />
Line 5: Always extend AbstractBasePlugin, all the injection preparation is being done there, so you must extend that cat. <br />
Line 8: Override the Execute method. You will get a pluginContext from where you can get everything else you need.<br />
Line 9 - 10: Write some code that does something on the event.<br />

Remember the following basic rules for plugins. 

* Dynamics 365 engine reuses instance of the plugin, so you cannot add any member properties or fields to the plugin it self, because they would be shared cross several executions on the same plugin.
* You cannot Import services directly on the plugin for the same reason. You have to use pluginContext.GetService<whatever-service-you-need>(); To get a specific service instance in the plugin. When one service
on the other hand, need another service, you can use the Import decoration property. 


### Entities
This folder contains the generated late bound proxy classes for each entity you wish to work with.  If you need to add functionality to generated entities, remember that they are
generated partially, meaning that you can add functionality by creating an additional partial source, and add code there. Below example creates a virtual propety on the 
contact that will shorten the fullname down, ex Kjeld Ingemann Poulsen will become Kjeld I. Poulsen

```csharp
using System.Text;
namespace Kipon.Dynamics.Plugin.Entities
{
    public partial class Contact
    {
        public string ShortFullname
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FullName))
                {
                    var spl = this.FullName.Split(' ');
                    if (spl.Length <= 2) return this.FullName;
                    var sb = new StringBuilder();
                    for (var i =0;i < spl.Length;i++)
                    {
                        if (i == 0) sb.Append(spl[i]);
                        else if (i == (spl.Length - 1)) sb.Append(" " + spl[i]);
                        else sb.Append(" " + spl[i].Substring(0, 1).ToUpper() + ".");
                    }
                    return sb.ToString();
                }
                return this.FullName;
            }
        }
    }
}
```

### Extensions
C# support adding helper methods to any object (or even simple types), using extensions. This is convinient for implementing general things such as string manipulations, type converting and more. I often uses this
pattern, and put these general methods in here.

```csharp
using System.Linq;

namespace Kipon.Dynamics.Plugin.Extensions.String
{
    public static class StringMethods
    {
        public static string UpperCaseWords(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var words = value.Split(new char[] { ' ', '\t' });
                words = (from w in words select w.UppercaseFirst()).ToArray();
                return string.Join(" ", words);
            }
            return null;
        }

        public static string UppercaseFirst(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
                if (value.Length == 1) return value.ToUpper();
                return value.Substring(0,1).ToUpper() + value.Substring(1).ToLower();
            }
            return value;
        }
    }
}
```

Take a look at above example. It basically implements some simple string manipulations that can uppercase the first letter in each word of a string. It will come in handy later in the 
example.


### Services
You should put as little code in the plugin as possible, and create services for whatever requirement you need to support. For each service you should create a service API, and put that interface in 
the ServiceAPI folder, and a corresponding implementation in the Services folder. Creating ServiceAPI for everything will make it easier for you to create unit test, with mocks where needed.

#### Creating a ServiceAPI
```csharp
namespace Kipon.Dynamics.Plugin.ServiceAPI
{
    public interface IAccountService
    {
        void UppercaseName(Entities.Account target);
    }
}
```

The above code show how to create a simple service API. In this case i have a requirement that each word in the account name must be uppercased automatically. The above code just describe
that I need a service that can do that.

We also need an implementation for that service. It could look like this:

```csharp
using Kipon.Dynamics.Plugin.DI;
using Kipon.Dynamics.Plugin.Entities;
using Kipon.Dynamics.Plugin.Extensions.String;

namespace Kipon.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IAccountService))]
    public class AccountService : ServiceAPI.IAccountService
    {
        public void UppercaseName(Account target)
        {
            target.Name = target.Name.UpperCaseWords();
        }
    }
}
```

Line 1: We import the dependency injection namespace so we can export our service to be visible whereever needed.<br />
Line 3: We import the extensions string namespace, so we can use the extension methods described above<br />
Line 7: We export our service, and is telling the framework, that whoever calls for i IAccountService, should get an instance of this cat.<br />
Line 8 - 14: We implement the service, and manipulates the current name of the account to match our uppercase rule<br />

The above example is very simple and can be done without usage of any services like IUnitOfWork or IOrganizationService. But often our services need to lookup data in other entities, or do
reated updated of system maintained redundantcy. In such case, our service need other services. This framework offer a very simple dependency injection mechanism, that allow you to call for a
service, simply by defining a property for the service, and decorate the service with the Import decoration class:

```csharp
using System.Linq;
using Kipon.Dynamics.Plugin.DI;
using Kipon.Dynamics.Plugin.Entities;
using Kipon.Dynamics.Plugin.Extensions.String;
using Microsoft.Xrm.Sdk;
namespace Kipon.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IAccountService))]
    public class AccountService : ServiceAPI.IAccountService
    {
		[Import]
		public Entities.IUnitOfWork uow { get; set; }

		[Import]
		public IOrganizationService orgService { get; set; }

		// implement the Service interface here
    }
}
```

In above example the service is requesting access to IUnitOfWork, and the SDK IOrganizationService. These properties will be populated automatically by the framework, so in 
the implementation of the methods in your ServiceAPI, you can assume that they have a valid value.

The following services are created by the AbstractBasePlugin:

* System.IServiceProvider, This is the service provider given by the Microsoft Dynamics 365 plugin infrastructure, and should rarly be used. The abstract plugin is already extracting whatever neede from that cat.
* Microsoft.Xrm.Sdk.IOrganizationServiceFactory, a factory service than can cresate IOrganizationService. Should rarly be used. The abstract plugin is already preparing an IOrganizationService for you.
* Microsoft.Xrm.Sdk.IPluginExecutionContext, the execution context provided by the CRM plugin infrastructure. Should rarly be used. This framework is exposing the information in here by decorations and properties on the custom PluginContext
* Microsoft.Xrm.Sdk.ITracingService, trace service, convinient if you need to add debugging information to your plugin.
* Microsoft.Xrm.Sdk.IOrganizationService, an organization service that can be used to perform any SDK based operation. You should avoid using the service directly. Instead use IUnitOfWork.
* DI.IPluginContext, The plugin context can be injected anywhere, and thereby you can expose the context to any service. The interface has however mainly be created to support the wiring from an actual plugin implementation, into the 
execute method. Making the interface visible in services might make it harder for you to create unit-test for such service, because you will need to mock the full functionality of this interface.

Any custom build service that you build in your plugin library must be decorated with [Export(typeof(whatever-interface-it-implements))]. That way, the service can be injected in any other service
using the same import pattern, and any plugin can request an instance of the service by calling pluginContext.GetService< whatever-service-interface-needed >();


### Attributes
This folder contains class decoration properties that enable you to deploy the plugins without using the Plugin Registration tool. You should not need to change any code in there. Just leave it as
is an enjoy the functionality offered.

### DI
This folder contains a very simple dependency injection framework that allow you to create services, where other services are injected through property decorations. You should not need to change any code in there.



