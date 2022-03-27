copy Kipon.Xrm.Tools\bin\Release\Kipon.Xrm.Tools.dll Package\content\bin\coretools\Kipon.Xrm.Tools.dll
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe Package\content\bin\coretools\Kipon.Xrm.Cmd.exe
copy Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd.exe.config Package\content\bin\coretools\Kipon.Xrm.Cmd.exe.config.template
call Kipon.Xrm.Cmd\bin\Release\Kipon.Xrm.Cmd xrmcodecopy Kipon.Solid.Plugin\Xrm Package\content\Kipon\
call ..\nuget pack Package
