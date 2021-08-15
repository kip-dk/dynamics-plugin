copy Kipon.Xrm.Tools\bin\Release\Kipon.Xrm.Tools.dll ToolsPackage\content\bin\coretools\Kipon.Xrm.Tools.dll
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe ToolsPackage\content\bin\coretools\Kipon.Xrm.Cmd.exe
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe.config ToolsPackage\content\bin\coretools\Kipon.Xrm.Cmd.exe.config.template
copy Kipon.Example.WebResources\deploy.cmd.template ToolsPackage\content\deploy.cmd.webresource.template
copy Kipon.Example.WebResources\generate.cmd.template ToolsPackage\content\generate.cmd.webresource.template
copy Kipon.Example.WebResources\filter.xml.template ToolsPackage\content\filter.xml.webresource.template
call nuget pack ToolsPackage
