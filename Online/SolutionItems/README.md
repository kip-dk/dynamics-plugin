# Kipon.Solid.Plugin - 2.n.n - framework for building Dynamics 365 CE plugin SOLID style

## Version 2.n is in BETA and only supported for Dynamics 365 CE online. Use newest version 1.n for Onprem, and any environment where you need to move the solution to a production environment.

[Official documentation site](https://kipon.dk/solidplugin)

## Scope for version 2
The primary purpose of version 2.0 is to support the new PluginPackage deployment model of Dynamis 365 CE. This feature of the Dynamics 365 CE has not been made generally available.

We have seen statment from Microsoft expecting general availability on Marts 2023, but the value is being changed. Please se below linke for details in the Microsoft annoncenemt:

[Dynamics 365 CE PluginPackage deployment](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/dependent-assembly-plugins)
[Microsoft release statements](https://learn.microsoft.com/en-us/power-platform-release-plan/2022wave1/data-platform/planned-features)


## Kipon.Solid.Plugin 2.n: It's a BETA.
Microsoft has yet to make the PluginPackage deployment model generally available. The Kipon.Solid.Plugin 2.n framework will not go out of Beta until Microsoft has made the concept general available.

Be aware of the current limitations:

- It will only work for Dynamics 365 CE Online
- It will not work for Workflow assemblies

Also be aware of the current state (Microsoft issues):

- It does not work for plugin listening in async mode
- You cannot move a plugin steps in same solution as the Plugin Package

## Get started
The version 2.n is being distributed as a DLL (Kipon.Xrm.dll) rather than the raw source code. Otherwise the code pattern to be used are the same.

Before deployment, you need to setup build of a nuget package, because code are deployed to Dynamics 365 CE through a Nuget package, containing all the needed assemblies.

We will soon publish a full installation instruction on the official website of the framework: https://kipon.dk/solidplugin

So stay tuned.
Jan 1. 2023

