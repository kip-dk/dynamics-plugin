### Kipon Dynamics 365 CE tools

#### This is the documentation site for Kipon.Xrm.Cmd (a small toolbox to support Dynamics 365 CE development related tasks.)

#### Plugins
##### Kipon.Solid.Plugin
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

#### WebResources

##### Generate d.ts definition files based on the DefinitelyTyped, @types/xrm type
If you are writing webresources in typescript and uses the @types/xrm types to help you with intelicense while doing development, this is the framework for you to go one step futher.
This tool allow you to generate .d.ts files that match your forms.

This if for Dynamics 365 CE forms what Data EntityContext is for the database. You will work with strongly typed forms, that only have attributes and controls according to the actual form,
and if you make changes to the form in Dynamics 365 CE, simply regenerate the .d.ts files with this tool, and the ts compiler will guide you to places where attributes or controls has been remove,
because it will generate compile errors in you IDE.

You must setup a filter.xml in the root of your webresources project:

[filter.xml]
```xml
<webresource>
  <forms namespace="Kipon.Forms">
    <form entitylogicalname="account" title="Account" name="Account" />
    <form entitylogicalname="account" title="Sales Insights" name="SalesInsights" />
    <form entitylogicalname="contact" title="Contact" name="Contact" />
    <form entitylogicalname="contact" title="AI for Sales" name="AIforSales" />
  </forms>
  
  <deploys managed-solution="WebResourceManagerKipon">
    <solutions>
      <solution name="KiponToCustomer"/>
    </solutions>
    <deploy file="Scripts/Forms/Account.js" name="kipon_/Forms/Account.js" description="Form script for the standard account form" />
  </deploys>
</webresource>
```


## License 
MIT (c) Kipon ApS, 2019,2020,2021