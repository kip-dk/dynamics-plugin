### Kipon Dynamics 365 plugin template
This project is a Visual Studio template solution for builing Dynamics 365 plugins

It has three sub projects

#### Kipon.Dynamics.Plugin
This is the assembly project. You are suppose to rename this to match your customer and needs, and build you own plugins here.

#### Kipon.Dynamics.SvcFilter
This is a very simple filter DLL, that can be added to the CrmSvcUtil.exe command line tool as /codewriterfilter to ensure that
the tool only generate entities that is needed.

#### Kipon.PluginRegistration
Normally you would use the Dynamics 365 plugin registration tool to registre your plugins in Dynamics 365. This simple command line tool
can registre you plugins if you decorate you plugin classes with Step(..) attributes, defined in Kipon.Dynamics.Plugin.Attributes.

This allow you to write codes that is telling how the assembly plugin type should be deployed, so your code and the actuall configuration is
always in sync.

#### Installation guide

##### 1) Download the project from GIT

![Download image](images/download.PNG)

