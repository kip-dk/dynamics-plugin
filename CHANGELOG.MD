## Last updated: 2021-08-14

### 1.0.5.2 Kipon.Solid.Plugin entity metadata
Additional properties is now generated for each entity:

This can be helpfull when working with the stronly named properties to know metadata details on the entity, without having to lookup via metadata service.

The following const properties of each entity has been made available:
```csharp
	/// example from generated PhoneCall entity
	public partial class PhoneCall {
		public const bool IsEntityActivityType = true;
		public const string EntitySchemaName = "PhoneCall";
		public const string PrimaryIdAttribute = "activityid";
		public const string PrimaryNameAttribute = "subject";
	}
```

As you can see, valueable information about the entity has been added.

On top, Kipon.Xrm  plugin framework now comes with its first build in service "Kipon.Xrm.ServiceAPI.INamingService"
You can inject this service anywhere, and use it to lookup missing names for EntityReference fields. This is convinient in situations where you need
the name of a reference field, but the client did not populate that part of the EntityReference. The name found will be added to the EntityReference parsed, and returned.

Generating actions is no longer using a specialized version of the IOrganizationServce. The end result is the same, but the good news is that now
it is only crmsvcutil.exe than handles the connection to CRM, metadata for actions is only fetch once, 
and the framework no longer need to align connection string support for this part. 


### 1.0.5.0 Kipon.Xrm.Tools version
This is the first release of the framework where the toolbox of the framework is published as its own package.
The tooling of the framework can now be installed separately from the Kipon.Xrm.Tools nuget package.  This is ONLY relevant outside a Kipon.Solid.Plugin context (because in this context the tools are already included).

Take a look at the official documentation page for the Kipon.Xrm.Tools to se what's in the package:

[Kipon.Xrm.Tools](TOOLS.MD)


### 1.0.4.22 transform output parameters to SDK serializable types + IPluginExecutionContext extension method
Output parameters of type Entity and EntityCollection is now transformed from strongly typed entities to SDK classes before added to the OutputParameters, because dynamics 365 is not able to serialize the strongly typed classes.
    
New extension methods added to IPluginExecutionContext to make it easy to extract input paramters from a parent context.

(now with unit-test that actualy verifies the sanity of the new extension method  [:-(. it is never easer to shortcut test .. sorry for version 19-21, should not happen)]

### 1.0.4.19,20,21 same as 21, but did not work 100% as expected.

### 1.0.4.18 OptionSetValue in actions mappes correctly
Non required InputFields of type OptionSetValue was wrongly mapped to object in the action interface and implementation. This has been fixed.

### 1.0.4.17 QualifyLead
Action on QualifyLead did not work as expected.

### 1.0.4.16 Code generator bug fix.

To get any value of of this patch, you must regenerate entities after you installed this version.

When using target and merge image on same plugin method, with supress-mapped-standard-optionset-properties set to true on filter.xml, 
the mirror of changing values on target caused a null reference exception, because a property raise was 
done on the default naming, instead of the actual naming of the property. This problem only apear if you map optionsetvalues in the filter.xml 
file to another name than the schema name, ex property schema name  kipon_myoptionset,  is mapped to an enum representation named MyoptionsetEnumValue

The propertychange event will now fire on the correct property name, but you must generate entities to get this part of the code in place.

### 1.0.4.15
Added tracelog for each method invoke, that is pushed to tracelog, only if exception is thrown.

### 1.0.4.14 Fix tools
Documentation of new generate tools updated on below change to match reality.

### 1.0.4.13 Extension method added


#### New extension to capture pre value from plugincontext.
The extension namespace Kipon.Solid.Plugin.Extensions.Sdk got a new extension method for the IPluginExecutionContext, PreValueOf<T>(propertyname).
The propertyname must be the name of the property in the stronly typed entity.

The method will diig into the preimage of the context and find the value of the field, and return it as type T. This is convinient when working with mergedimages, where
you need to know the current value (always the value return from the mergedimage - changed or not) and the prevalue at same time, ex. for comparrison. 
This might save you from creating a specific preimage-interfaces. 

the flawor of this method is somehow "less solid" so use it with care. The best solid approach is to define the preimage interfaces and inject that into your plugin, and thereby 
be explicit in you service that the service need to know the pre value of a field as well as the current value, if changed.

#### Kipon.Xrm.Tools.CodeWriter.PlainTxtWriterFilter
You can use the Kipon.Xrm.Tools to generate entites for non Kipon.Xrm.Plugins, with a very simple filter mechanism. This is convinient, if you have old projects
with strong entity type generation, but you wish to start filter entities out, because you upgraded to a Dynamics 365 CE version that adds way to much, if you just generate everything.

The filter.txt file should be placed in the folder from where you run the CrmSvcUtil command (not nessesarrely the folder where you host the CrmSvcUtil.exe), 
and have an entity name on each line. You can state all entities matching a start pattern
by proiding all: keyword infront, ex

[filter.txt]
all:kipon_
contact
account
task

This will limit genration of entities to entities starting with kipon_, and it will also generate account, contact and task.

On the command line prompt, you specify the filter same way as for plugins:

..\..\Kipon.Xrm.Tools\bin\coretools\CrmSvcUtil.exe  [other parameters] /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.PlainTxtCodeWriterFilter,Kipon.Xrm.Tools

Remeber to ensure that the Kipon.Xrm.Tools DLL is copied to the folder where you have CrmSvcUtil.exe


### 1.0.4.12 Tool version, generate QualifyLead action

### 1.0.4.11 Inject secure- and unsecureconfig into plugin or services
By adding a parameter of type string with name UnsecureConfig or SecureConfig (not case sensetive) to a plugin method or service constructor, you will get the
config setting from the current plugin injected to your method/service constructor.

Injecting the config into plugin method does not make a lot of sence, because you have to create the appropriate constructor anyway, but it has been added for consistancy.

Remember, you must add both parameterless constructor and config constructor to your plugin when you need to support config settings from the plugin

```csharp
public class MyPlugin: Kipon.Xrm.BasePlugin
{
   public MyPlugin() {}
   public MyPlugin(string unsecureConfig, string secureConfig): base(unsecureConfig, secureConfig) { }

   public void OnPreCreate(...)
}
```


### 1.0.4.10 Unbounded actions
Unbounded actions did require a minor change in the plugin framework. It has been fixed in this version.

### 1.0.4.9 Tools version only
This version only changed the deploy tool to support auto registration of steps for unbounded actions. At same time it limits registration of bounded actions to only
try to bind for the appropriate primary entity.

### 1.0.4.8 | 2021-02-10 
Map any exception in invoke to InvalidPluginExecutionException, with parse of the original exception, to make errors more transparent.

### 1.0.4.7 | 2021-01-15
Extenson method for IPluginExecutionContext IsChildOf can now resolve actions in a transaction, and id is parsed to child of child (bug fix).

### 1.0.4.6 | 2021-01-11
Extension method add in Sdk to return if an attribute is part of the target payload.

### 1.0.4.5 | 2021-01-10
BasePlugin did not always allow an InvalidPluginExecutionException to drain all the way down to the plugin infrastructure due to reflection Invoke was catching
the exception and wrapping it in a System.Reflection.TargetInvocationException. This problem has been solved so the original InvalidPluginExecutionException
is now drained down to the Dynamics 365 infrastructure, and therefore shown correctly in the UI. The System.Reflection.TargetInvocationException is only mapped back
to the original InvalidPluginExecutionException on its own root Inner excpetion.

All unknown excepts are mapped to InvalidPluginExecutionException with the message of the original exception, and the original exception parsed to InvalidPluginExecutionException to allow
better information to the client for diagnostic.

### 1.0.4.4 | 2021-01-09
Added simple extension method on Sdk to clone an entity without key and without system parameters as createdby, modifiedby, importsequencenumber,..

### 1.0.4.3
Introduction of Kipon.Xrm.ITarget interface. This interface allow you in combination with Kipon.Xrm.Attributes.TargetFilterAttribute to define an inteface to be the target object of an event, and
then implement the interface and decorate each entity implementation of the interface (entity proxy class extending Microsoft.Xrm.Sdk.Entity) with the TargetFilter attributes, 
defining the attributes of relevance for that paticular implementation of the target.

This can reduce the need for dublicate plugins where the intention is to do the same operation on several entities, but the naming and relevant target fields are not the same for all entities.

### 1.0.4.2
Allow mergedimage to be used as parameter in create. In Create, Merged images is the same as the target. This allow you to make mergedimage interfaces that can be shared between the create and the update event.

### 1.0.4.1
Extension method on PluginContext ParentTarget<T>(string message, Guid id) will return the strongly typed entity of the parent event trigger this event. This allow you to take information
from the initiating entity payload directly

### 1.0.4.0
Extension method on PluginContext isChildOf has been improved to also look for message in parent ExecuteTransaction messages. This is a change of behavior that might result in
new behavior on existing plugins. that is why the version goes from ..3.0 to ..4.0. Running through message in a transaction to match only works for search on Create, Update and Delete message.

On top of that, you can now also add the id of the target entity that is suppose to be the parent, so you can ask if a plugin exectuion is a child of a create/update/delete operation for a specific target.

### 1.0.3.0 TypeConverters extension methods - wrong namespace
Some Kipon.Solid... namespaced extension parse it through to release 1.0.2.10. This was unintentional. The Kipon Solid software should only expose method in the Kipon.Xrm namespace.
The extensions methods are still there, but now under correct namespace Kipon.Xrm.Extensions.TypeConverters. Since this is a breaking change if anybody actually took advantage of the
methods, the minor version has been increased to indicate potential breaking change.

### 1.0.2.10 Multi OnRetrieve and OnRetrieveMultiple on virtual entity plugins
To support datasource providers that server more than one virtual entity in an external datasource, the virtualentity plugin can now
have multiple on.. methods. You MUST decorate each method with logical name of the entity, using the LogicalName attribute to allow the frame
to distinct. You can have one set off methods without logical name that will be used as fallback if a request for an non explicit logical name is received.
For the sake of simplicity, and the cool thing about being explicit, i recommend that you always decorate your virtual entity plugin methods with logical names.

### 1.0.2.9 fixe in new extension methods

### 1.0.2.8 paging info returned for virtual entities
if underlying plugin is providing paging info, it is now returned in the EntityCollection.

A number of convinient extensions has been added to the Kipon.Xrm.Extensions namespace. 

### 1.0.2.7 fix datasource injection in virtual entity plugin
See description on version 1.0.2.6

### 1.0.2.6 inject datasource (virtual entity plugin do not work in this version)
you can now ask for the datasource entity by adding an entity parameter to your Retrieve RetrieveMultiple method with type of Microsoft.Xrm.Sdk.Entity  or an extension of same (strongly typed version)
datasource can only be injected into the plugin for now. The parameter name must be "datasource", otherwise the framework will think it is target, preimage, mergedimage or postimage.

### 1.0.2.5 virtual entity plugin fix
When a provider is populating null in an entity reference, the plugin works, but when receiving the result from a client using the SDK, this will fail, because null values for entityreferences is not supported
by the linq query provider. The Kipon.Xrm.VirtualEntityPlugin now remove all null values from the result entity and result entity collection, witch looking from the client perspective is the same, so your plugin
can set attributes to null, without the consequence of query fail for a client using the linq provider.

### 1.0.2.4
Support for virtual entity plugin.

### 1.0.2.3
Fix for virtuel entity support

### 1.0.2.2
Fix for virtuel entity support.

### 1.0.2.1
Experimental support for virtual entity plugin. A new base plugin type "Kipon.Xrm.VirtualEntityPlugin" has been put in place to support development of virtual entity data providers with this framework.

A virtual entity plugin does not have any steps due to the design of virtual entities. It is called on stage 30 when entity and provider is linked togehter. For now, you must create the Data Provider manually 
using the Microsoft standard PRT, but you can develop the plugin with Kipon.Solid.Plugin tool. Extend Kipon.Xrm.VirtualEntityPlugin, and write two methods in your plugin:  OnRetrieve and OnRetrieveMultiple.
For the first method, return and Entity, for the second, return an EntityCollection. To get the query parsed from dynamics 365, inject an instance of Microsoft.Xrm.Sdk.Query.QueryExpression and vist the
expression three to get the actual filter criteria for your query.

### 1.0.1.9 
Allow use of Mergedimage interfaces in plugin method for message Create. I nthis case, the mergedimage will just be a copy of the target. Two way sync in pre-stage in pre stage is also supported on pre-create

### 1.0.1.8
Better message when plugin is unable to resolve a plugin parameter

### 1.0.1.7
Map framework exceptions to invaludpluginexecutionexception to allow serialization of message to the client. 

### 1.0.1.6
Exceptions thrown by the framework has been marked serializable

### 1.0.1.5
The actions request server interface is not generated partially.

### 1.0.1.4
The Kipon.Xrm.Tools crmsvcutil extension now generate request interface, request impl. and response impl. for actions. Please review the documentation on the readme page for details on the topic.

REMARK: Mergedimage<T> now defined Guid Id { get;} and LogicalName { get; } as assumed properties same way as Target, Preimage and Postimage. The workaround so
far has been to add them to each interface inheriting from the Mergedimage interface. If you did so, you MUST remove these specific implementation after next
entity code generation. You will get warning in Visual Studio, so just fix these by removing the declarations from your interfaces.
(sorry that the version did not increase to 1.1.0.0 as breaking changes should, but we only found out after publish.)

### 1.0.1.3
Added support for bounded custom actions (did not test unbounded - so for now, not supported). A custom action is defined in the solution Process as an action, and bound to an entity.
You can on top of that, define input and output parameters. As all plugin, it must inherit from Kipon.Xrm.BasePlugin, and you must add event methods on the form

On[stage][ProcessLogicalName],  ex. OnPostkipon_AccountCountContacts, remember to prefix with your solution prefix as demonstrated here.

where stage can be

Validate
Pre
Post

Async has NOT been tested.

Parameters are injected by naming convention, so if you have an input parameter of type string, and you named it Name,  you can add a parameter to your On.. method On....(.., string Name). 
Alternativly you can also inject the Microsoft.Xrm.Sdk.IPluginExecutionContext and extract them from there.

The references to the bounded entity is injected as a TargetReferences, same pattern as for Delete message, ex On....(Entities.AccountReference target).

### 1.0.1.2
All metadata cache is now build with lock when generating relevant cached metadata to avoid Dictionary.Insert nullpointer exception, cause by multi plugins trying to cache metadata
at same time.

### 1.0.1.1
Better message on unable to resolve type exception from service type cache

### 1.0.1.0
The project has been upgraded to .NET 4.6.2, and references to newest SDK packages. (this might impact onpremise installations)

### 1.0.0.33
This is a tools version only. Still trying to get better messages on error.

### 1.0.0.32
This is a tools version only. Tools are now giving better indication of errors based on exceptions.

### 1.0.0.31
Version of generated entities will now be compared with the tools version used to deploy, to avoid deploying code to CRM
where the entities has been generated with a tool version lower than the used BasePlugin. This is to ensure consistant behavior
when the framework is making changes by changing the code generator. Because you get Kipon.Xrm.BasePlugin, simply by updating nuget,
this process ensures that you remember to regenerate your entities, so the entity model is consistant with the expectations in the BasePlugin.

### 1.0.0.30
mergedimage is keept in sync with target in stage 20 or less (it does not make
sense to make any changes to a target in stage above 20, because the changes will not make
it into the CRM database anyway). This means that any property set
on target will reflect into mergedimage, and any change in mergedimage will reflect into target
and as a consequence the later will cause update of the field in CRM. Be aware that the propertychanged
event will only trigger on the instance that triggers the change, the counter party will be updated silent.

To simplify the needed interfaces you must define and handle on update, and additional decoration attribute [TargetFilter]
has been added. This attribute should be used to decorate properties on pre-, merged- and postimages interfaces to tell that 
even though the image is not a target images the property should still be listed in the filtered properties on the
update operation. Together with the two way mergedimage impl. mentioned above, this means that you often only have
to ask for a mergedimage in your plugin method, becuase it can basically represent the target and the merged image 
at same time.

### 1.0.0.29
Kipon.Xrm.IService.OnStepFinalized() impl in crmunitofwork  generated error while cleaning the context if any items was in it.

### 1.0.0.28
IMPORTANT: You must regenerate your entities after installatin this package. Changes has been made to the code generator.

IUnitFoWork.ClearChanges() and SaveChanges() has been removed. These methods belongs into a context that is not transactional by default
(IUnitOfWork in a plugin is just participating in the current IOrganizationService transaction, so we are already in a transaction) so the methods did not make sense.

The generated CrmRepository implementation has been updated accordingly, so Add, Update, Delete is operating directly on the IOrganizationService, but the underlying
CrmContext is still keept in sync. Add will add the object to the context after create, Delete will remove the object from the context, and Update will add or update
the object in the CrmContext according to the updates, after the operation has been executed on the organization service.

Delete references was not resolved correctly when listening to delete events.

Generic shared interfaces for preimage, mergedimage and postimage did not work as expected. Now these images can be based on a shared interfaces that is impl. by the entity.
At least one parameter must target a specific entity ether by reference or implementation, or the method must be decorated with logicalname attribute to identify what to listen for.

### 1.0.0.27
Release build of tools added, 1.0.0.26 was unable to deploy new step.

### 1.0.0.26
Added and tested support for RemoveMember  (event to listen on single member being removed from marketing list)

### 1.0.0.25
Added support for multi entities implementing same interface, and allow registre the relevant entities using the [LogicalName(name)] attribute on the
plugin method to identify relevant entities.

Inform on update of step.

### 1.0.0.17
Deployment tools is outputting when a step is deleted.

interface properties without a getter is no longer added to filteredattriutes for update event listener

### 2019-12-31
The SOLID version of this concept has been put on top of the list, and reference to old version has been hidden.
