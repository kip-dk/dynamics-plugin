## Last updated: 2020-09-28 09:20

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
