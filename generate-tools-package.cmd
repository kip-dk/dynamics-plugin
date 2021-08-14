copy Kipon.Xrm.Tools\bin\Release\Kipon.Xrm.Tools.dll ToolsPackage\content\bin\coretools\Kipon.Xrm.Tools.dll
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe ToolsPackage\content\bin\coretools\Kipon.Xrm.Cmd.exe
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe.config ToolsPackage\content\bin\coretools\Kipon.Xrm.Cmd.exe.config.template
call nuget pack ToolsPackage
