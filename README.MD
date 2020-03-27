### Kipon Dynamics 365 SOLID plugin template

This is the second version of the Kipon plugin template. It is a 100%  rewrite and it aims to make is possible to be 100% SOLID when
developing plugin for Dynamics 365 Customer Engagement platform.

### Project offer
* Quick startup of new plugin project with simple installation with nuget
* A base plugin (Kipon.Xrm.BasePlugin) that allow you to write event oriented plugins that get needed things auto injected
* Architecture for building plugins using a service api architecture, including a dependency injection framework. 
* Architecture for building plugins based on "unit of work" pattern. IUnitOfWork interface is included in the code, and crmscvutil is extended to generate repository implementations for all included entities.
* Template and tool for generating early bound entities based on Dynamics 365 Metadata
* Tool to deploy the assembly and related steps to your Dyanmics 365 development instance, and adding these to your solution.

### Installation
1) Create a C#.NET Framework library, at least version 4.5.2. (.NET Core is not yet supported for dynamics 365 plugin development)

2) Add nuget package Kipon.Solid.Plugin to the library

```shell
Install-Package Kipon.Solid.Plugin -MinimumVersion 1.0.1.1
```

3) Rename the following files included in your project, and adjust according to your need

* deploy.cmd.template: - remove ".template" from name, and add your solution name and your Dynamics 365 connectionstring
* Entities\filter.xml.template: - remove ".template", and adjust the list of entities and option set values according to your need
* Entities\generate.cmd.template: - remove ".template", and adjust the [placeholders] in the file according to your need.

4) Now you can use Entities\generate.cmd to generate early bound entities.

Remeber to include the following files to you project after first time early bound entity generated:

* Entities\Context.design.cs
* Entities\CrmUnitOfWork.Design.cs

### Creating a plugin
We are working on a creating a rich documentation side with examples and recommendations as the concept mature. That part is still work in process, so for now
you will have to settle with below small examples to get started.

Creating a plugin is as simple as creating a class. I recommend creating a folder in your project for all plugins (ex. named Plugins), and depending on the expected size
of your library, you can ether create a plugin for each entity you need to listen to, or a folder for each entity, and then below plugins related to that entity

Any plugin must extend from Kipon.Xrm.BasePlugin

Any method that should be called on plugin execution must be public, and methods in the plugin must follow a simple naming convention:

```csharp
public void On[Stage][Message][Async](parameters)
{
}
```


#### where Stage can be
* Validate
* Pre
* Post

#### Message can be
* Create
* Update
* Delete
* RetrieveMultiple
* RemoveMember

##### Below here work in progress messages - might or might not work (most likely they do NOT work yet):
* Associate
* Disassociate
* SetState
* SetStateDynamicEntity
* Retrieve
* AddMember
* AddListMembers

#### Async
Only apply to stage Post, and should simply state Async if the plugin should run Async otherwise blank


#### Resolve entity to listen for
The concept will use the parameters to resolve what entity you are listening for. See example below.

You must add at least one parameter that maps to a specific entity, and the name of this parameter must reflect what you expect to get:

* target (the record being created updated or deleted)
* preimage (only applies to update and delete, and is giving the entity as is looked before this operation)
* postimage (only applices to post stages and is giving the entity as is looks after this operation)
* mergedimage (is really the preimage, but target is merged into this instance so you can see how the entity looks, including any changes made in this process, even before it hits the database)

Target and images can ONLY be injected into plugin methods. When needed in a service, add these as parameters in your service method, and parse them from the plugin method to your service method.

#### Plugins\AccountPlugin example
```csharp
using Kipon.Xrm;
namespace MySolid.Plugins
{
	public class AccountPlugin : BasePlugin
	{
		public void OnPreCreate(Entities.Account target)
		{
			// Do whatever you need to do whenever an account is created
		}
	}
}
```

#### Same step for multi entities
If you need to run the exact same code for multi entities, that is perfectly possible. Define a shared interface and ensure each entity supported is implementing this interface.
Then inject the interface to your plugin as target, preimage, mergedimage or postimage, and finally decorate the plugin method with LogicalName attributes:

```csharp
using Kipon.Xrm;
using Kipon.Xrm.Attributes;
namespace MySolid.Entities 
{
	public interface IShared
	{
		string Subject { get; }
	}

	// Both Email and Task already have a property Subject, so no futher impl. is needed.
	public partial class Email : IShared { }
	public partial class Task : IShared { }
}

namespace MySolid.Plugins
{
	public class MySharedPlugin : BasePlugin
	{
		[LogicalName(Entities.Email.EntityLogicalName)]
		[LogicalName(Entities.Task.EntityLogicalName)]
		public void OnPreCreate(Entities.IShared target)
		{
			// Do whatever you need to do on the task or email pre create
			// you can even typecast the entity to appropriate, because what you get IS the strong type entity.
		}
	}
}
```

#### Special plugin method parameters, related to specific messages (types and names, names is NOT case sensetive):

##### Any message having a target entity
* System.Guid id - the id of the primary entity.

##### RemoveMember
* System.Guid listId - this list an entity instance is being removed from
* System.Guid entityId - the entityId of the entity (account, contact, lead) that is being removed from the list


### Adding services
Standard services like:

* Microsoft.Xrm.Sdk.IPluginExecutionContext
* Microsoft.Xrm.Sdk.IOrganizationService
* Microsoft.Xrm.Sdk.IOrganizationServiceFactory
* Microsoft.Xrm.Sdk.ITracingService

can be injected into the plugin (or custom services) simply by adding them as a parameter to your plugin event method.

You can create you own services and inject these as well. I recommend that you create a folder in your project for serviceapi, and one for
service implementations. You should only inject ServiceAPI into your plugins, never actual implementations.

Each service defined can have one and only one implementation within the plugin library. Service implementations is found with reflection, and the concept
is looking for the "one-and-only" implementation for each service. Service implementations can depend on other services. Services are injected into each other
by constructor injection. Only constructor injection is supported for services, so be carefull on circular references. They are not allowed. As with plugin event methods,
you should only inject interfaces into you services.

You cannot inject target, preimage, postimage or mergedimage directly into your services. Add these as parameters in your interface and let the plugin parse
these information to your service through service method calls.

#### ServiceAPI\IAccountService
```csharp
namespace MySolid.ServiceAPI
{
	public interface IAccountService
	{
		void DoSomething(Entities.Account account);
	}
}
```

#### Services\AccountService
```csharp
namespace MySolid.Services
{
	public class AccountService : ServiceAPI.IAccountService
	{
		public void DoSomething(Entities.Account account)
		{
			// actual impl.
		}
	}
}
```

#### Plugins\AccountPlugin now with service injection
```csharp
using Kipon.Xrm;
using MySolid.ServiceAPI
namespace MySolid.Plugins
{
	public class AccountPlugin : BasePlugin
	{
		public void OnPreCreate(Entities.Account target, IAccountService accountService)
		{
			accountService.DoSomething(target);
		}
	}
}
```


### Deploy your code to Dynamics 365 instance
The Kipon.Solid.Plugin nuget package has placed a deploy.cmd.template file in the root of your project. Rename this file to deploy.cmd and  change the content to match your need
(connectionstring and target solution).

After that, simply open a command line tool, navigate to the root folder of you project and call the command

```shell
C> deploy.cmd
```

This process will create/update the assembly in your solution, remove any plugin, step or image no longer needed according to your code and add plugin, steps and images
needed according to the new version of the code. The tool will ofcause only target plugins related to this assembly.

It is recommended that you always deploy "release" version code to your Dynamics 365 instance and remember to update the AssemblyVersion in the Properties.AssemblyInfo file before
building, so each deployment of the assembly is giving a higher version than the former.

Also remember to sign your library with a strong name key before deployment. Deployment is always created as SANDBOX, allowing you to deploy to any Dynamics 365 instance
including online instances.


## Disclamer
This code, project and documentation is provided as-is. Any use of the project, documentation or code is on your own risk. Kipon ApS does
not take any responsibility in damages cause by the use of this concept.


## License 
MIT (c) Kipon ApS, 2019,2020