@echo off
copy ..\..\Kipon.Xrm.Tools\bin\debug\Kipon.Xrm.Tools.* ..\bin\coretools
copy ..\..\Kipon.Xrm.Tools\bin\debug\Microsoft.Xrm.Sdk.Workflow.* ..\bin\coretools
copy ..\..\Kipon.Xrm.Cmd\bin\debug\Kipon.Xrm.Cmd.* ..\bin\coretools

call ..\bin\coretools\CrmSvcUtil.exe "/connectionstring:AuthType=AD;Url=http://kipon-dev/kip;" /out:Context.design.cs /ServiceContextName:SolidContextService /namespace:Kipon.Solid.Plugin.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.PluginCustomizeCodeDomService,Kipon.Xrm.Tools /codewritermessagefilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterMessageFilterService,Kipon.Xrm.Tools /generateActions /debug
rem ..\bin\coretools\Kipon.Xrm.Cmd.exe buildmodel "/connectionstring:AuthType=AD;Url=http://kipon-dev/kip;" /out:Context.design.cs /ServiceContextName:SolidContextService /namespace:Kipon.Solid.Plugin.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.PluginCustomizeCodeDomService,Kipon.Xrm.Tools /codewritermessagefilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterMessageFilterService,Kipon.Xrm.Tools /generateActions /debug

