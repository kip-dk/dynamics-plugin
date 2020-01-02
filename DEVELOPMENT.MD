## Instruction for development

### Kipon.Solid.Plugin

This is the base library for developing the Kipon.Solid.Plugin  concept.

All code in the Xrm folder is distributed to Kipon.Xrm.cs in the nuget package. 

All code outside the Xrm folder is just simple test code. It is only relevant for testing the concept, and is NOT distributed as part of the 
Kipon.Solid.Plugin package. The code does however reflect a realistic use of the Kipon.Xrm and serve as base source for most on the unit-tests.

In the Xrm folder, it is important to have using statements within the namespace, and
never outside because all files under Xrm is distributed as a single C# source file.


#### Correct use of using statement in all files below the Xrm folder.
```csharp
namespace Kipon.Xrm
{
	using System;
	etc.
}
```

### Kipon.Xrm
The Kipon.Xrm namespace is reserved for code that must be distributed as source code to the target library and it cannot be changed. Any code below the
Xrm folder should use this namespace.

Do NOT use the full Kipon.Xrm namespace anywhere in source code under the Kipon.Xrm, only use relative namespace. This restriction is due to the fact
that the Kipon.Xrm source code is also used in the Kipon.Xrm.Tools for deployment and in the Kipon.Solid.Xrm.Fake tool for unit test. In these libraries
the code is the same, however the root namespace is changed to avoid namespace conflict with the Dyanmics plugin library.

Both deploy tool and unit test tool need to know how to identify plugins and map these to relevant methods. All this code is hosted in the Kipon.Xrm namespace
and this way these 3 tools (the actual plugin, the deployment tool and the unit test tool) can be keept in sync. on the basic logic without 
having any dependencies to each other.

### Kipon.Xrm.Tools
This library contains code for tooling around the Kipon.Solid.Plugin context. The primary tool is the deployment tool, however it also host the code
generator for extending early bound entities and more. 

The Xrm folder contains a single source Kipon.Xrm.cs. This is an accumulation of the Xrm folder in the Kipon.Solid.Plugin library, but the namespace has
been changed to avoid any conflicts on runtime. The Xrm folder has a generate.cmd command. On changes in the Xrm of the Kipon.Solid.Plugin, this code must
be regenerated. That is done using the generate.cmd command.

### Kipon.Solid.Xrm.Fake
This library is a base library for creating unit test on top of a plugin using the Kipon.Solid.Plugin concept. Especially the query part of this project is 
"WORK-IN-PROGRESS", and the Kipon.Solid.Xrm.Fake library has not been officially release yet. 
It will, when the state is closer to a usable unit test framework be released as a separate nuget package.

This library also have an Xrm folder with a single Kipon.Xrm.cs source. The file i accumulated from the Kipon.Solid.Plugin using the generate.cmd tool.
The reason for copy and rename namespace is to avoid namespace conflict on the Kipon.Xrm namespace, when creating unit test based on this library.

### Development
All development is done in visual studio "Debug". State. Be sure your visual studio are in "Debug" before using any of the generate.cmd scripts mentionend above.

### Prepare a package for nuget update:

* Update the Solution Items/Package/Kipon.Solid.Plugin.nuspec package to the next version number
* Search for the old version in all source, and make sure the new version reflects everywhere (you might need to generate Kipon.Xrm.cs in tools and unit test library)
* Build while in "Debug" mode
* Run all unit test, verify that no unit test is failing
* Update the CHANGELOG.MD with appropriate descriptions of changes
* Change the visual studio to "Release"
* Build all
* run all unit test, now in release mode, and verify no unit test is failing
* Call generate-package.cmd
* upload the package to nuget