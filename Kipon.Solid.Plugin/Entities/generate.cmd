@echo off
copy ..\..\Kipon.Xrm.Tools\bin\debug\Kipon.Xrm.Tools.dll ..\..\Kipon.Xrm.Tools\bin\coretools
..\..\Kipon.Xrm.Tools\bin\coretools\CrmSvcUtil.exe /url:"http://kipon-dev/kip/XRMServices/2011/Organization.svc" "/username:deploy" "/password:#!dWWk56(<ahjDeQ" /domain:dom /out:Context.design.cs /ServiceContextName:SolidContextService /namespace:Kipon.Solid.Plugin.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.PluginCustomizeCodeDomService,Kipon.Xrm.Tools
