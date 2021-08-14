## Kipon Dynamics 365 CE tools

### This is the documentation site for Kipon.Xrm.Cmd (a small toolbox to support Dynamics 365 CE development related tasks.)

### Plugins
#### Kipon.Solid.Plugin
This tool is used to generate plugins and deploy the plugins when you are using the Kipon.Solid.Plugin framework. You do NOT need to install this package if you use the
Kipon.Solid.Plugin nuget package. Everything you need is already included in that package. 

##### Simple text file to filter entities to be generated
You can use the Kipon.Xrm.Tools to generate entites for non Kipon.Xrm.BasePlugin, with a very simple filter mechanism. This is convinient, if you have old projects
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

(assuming you generate command is hosted in a folder a single level down from the root of your plugin/tooling project)

..\bin\coretools\CrmSvcUtil.exe  [other parameters] /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.PlainTxtCodeWriterFilter,Kipon.Xrm.Tools

Installing the kipon.xrm.tools package ensure that the filter mentioned above is in the coretools folder.

### WebResources

#### Generate d.ts definition files based on the DefinitelyTyped, @types/xrm type
If you are writing webresources in typescript and uses the @types/xrm types to help you with intelicense while doing development, this is the framework for you to go one step futher.
This tool allow you to generate .d.ts files that match your Dynamics 365 CE entityforms.

If you are new to @types/xrm, and this is your first typescript based webresource library, follow these simple steps:

* Create a new project of type ASP.NET Core
* Use nuget tool to add the Kipon.Xrm.Tools to your newly created ASP.NET Core web application
* After install of the Kipon.Xrm.Tools, rightclick on the package.json file in the root of the project and click Restore packages

Now your solution is ready for typescript based webresources.

This generator tool is for Dynamics 365 CE forms what Data EntityContext (CrmSvcUtil.exe) is for the database. You will work with strongly typed forms, that only have attributes and controls according to the actual form,
and if you make changes to the form in Dynamics 365 CE, simply regenerate the .d.ts files with this tool, and the ts compiler will guide you to places where attributes or controls has been remove,
because it will generate compile errors in you IDE.

You must setup a filter.xml in the root of your webresources project:

[filter.xml]
```xml
<webresource>
  <forms namespace="Kipon.Forms" typings="Scripts\typings\">
    <form entitylogicalname="account" title="Account" name="Account" />
    <form entitylogicalname="account" title="Sales Insights" name="SalesInsights" />
    <form entitylogicalname="contact" title="Contact" name="Contact" />
    <form entitylogicalname="contact" title="AI for Sales" name="AIforSales" />
  </forms>
  
  <deploys ..>
     ...
  </deploys>
</webresource>
```
For each form, add a form tag to the forms container, specifying the entitylogicalname, the title of the form, and the TypeScript conformed name to be used the generating the .d.ts fil for the form.

the Above definition will generate d.ts files for the account entity, form Account, and form SalesInsights, and for the contact entity it will generate Contact and AiForSales form definitions.
Be aware the the word "Form" wil be added to the end of the interface.

After the d.ts file has been generated, it can be imported and used within your forms:

```typescript
/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />

module Kipon.Account {
    var Form: Kipon.Forms.account.AccountForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        Form = ctx.getFormContext();

        var s = Form.getAttribute("name").getValue();
        var p = Form.getAttribute("donotemail").getValue();

        Form.ui.tabs.forEach(r => {
        });

        Form.ui.tabs.get("SUMMARY_TAB").sections.get("SOCIAL_PANE_TAB").setVisible(false);
    }
}
```
Above code is just to demonstrate that "casting" the formcontext into a specific interface (Kipon.Forms.account.AccountForm) allow us to work on the formcontext business-as-usual, if you
are using @types/xrm, but the difference is, if you address attributes through Form.getAttribute("xxx") or Form.getControl("xxx") that does not sit on the form, you will get compile time errors.

On to of that, you IDE can give you intelise when working with the Form instance. It is from a development perspective easer to work with, and causes fewer bugs, and help you keep your code in sync.
with whatever changes are made to the form through the Dynamis 365 CE customization interface.

The files generated is pure d.ts files (interface only), so it will have zero impact on the size of the scripts deployed to the Dynamics 365 CE platform

### Using the genrate tool.

Assuming the filter.xml is hosted in the root of you webresource project:

The folder must contain a folder structure before you generate correspinding to the typings attribute of the forms tag. this folder is NOT created on the fly

.\bin\coretools\Kipon.Xrm.Cmd wr-generate "/connectionstring:[Dynamics365CEConnectionString]"

[Dynamics365CEConnectionString] supported the Dynamics 365 CE Tools pattern for connectionstring.

The generated d.ts file is added or updated in the typings folder.

#### Deploy webresouce files from you source repository into Dynamics 365 CE
The Kipon.Xrm.Tools can help you deploy your "code-managed" webresource into your Dynamics 365 development instance.

This makes a lot of sence if you are using the above "generate" tool to write typescript forms based on strongly typed formdefinitions.

First you need to configure what to deploy. This is done within the shared filter.xml til in the deploys section of the xml:

[filter.xml]
```xml
<webresource>
  <forms  ..>
  ---
  </forms>
  
  <deploys managed-solution="WebResourceManagerKipon">
    <solutions>
      <solution name="KiponToCustomer"/>
    </solutions>
    <deploy file="Scripts/Forms/Account.js" name="kipon_/Forms/Account.js" description="Form script for the standard account form" />
  </deploys>
</webresource>
```

You should setup an unmaned solution in your development instanse that has the single purpose of hosting all webresources that is deployed from you webresource source projekt.
In ahove example i have named that solution [WebResourceManagerKipon]. This solution should be "owned" by the Kipon.Xrm.Tools deployment context. This allow the tool to cleanup
if scripts are removed. 

If you need the scripts to be deployed in additional solutions, (ex. thouse you use to move the DEV to TEST and PROD environments), simply add more solutions names under the solutions container in the xml.

The difference between the managed-solution, and the list of solutions is, that this tool assume to have full control on all elements in the "managed-solution", while it does not assume anything about elements
in the solutions list, beside the fact that webresources deployed through this tool, also need to be added to these solutions.

#### Run the tool

.\bin\coretools\Kipon.Xrm.Cmd wr-deploy "/connectionstring:[Dynamics365CEConnectionString]"

[Dynamics365CEConnectionString] supported the Xrm.Tools pattern for connectionstring.

This will create/update webresource mentioned in the deploy section, delete components that are in the WebResourceManagerKipon that is no longer mentioned in the deploys section, and finally attach
the webresources to the solutions.

Finally the tool will publish all resource that was created or updated in the process.


#### Merging scripts on deploy #include

Instead of deploying each and every javascript as a separate file, you can organize the scripts in "natural" script containers, describing the dependencies within the root script.

```typescript
/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />

//#include Scripts/Services/AccountService

module Kipon.Account {
    var Form: Kipon.Forms.account.AccountForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        var ac = new Kipon.AccountService().getSomething();
    }
}
```
Take a look at above example. In the code i need to work with a "generic" service that can be used cross form scripts, ex. to fetch som account data.
The AccountService is a typescript that will be compiled into AccountService.js, and normally we would deploy this script to Dynamics 365 CE, and then include
both the AccountService.js script and the Account.js on the form.

But instead of using such deploy model, i can ask the deployment tool to merge the scripts into a single script that is merged and deployed. That way I only need to deploy the
Account.js script, and the AccountService will be embedded in any formscript adding the above include.

//#include also works for nested scrpts. So if the AccountService script also have includes, these will also be expanded into the single script deployment Account.js

## License 
MIT (c) Kipon ApS, 2019,2020,2021