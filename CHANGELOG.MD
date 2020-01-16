## Last updated: 2020-01-02 11:30

### NEXT

Kipon.Xrm.IService.OnStepFinalized() impl in unitofwork  generated error while cleaning the context if any items was in it.

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
