@echo off
copy ..\..\Kipon.Solid.SvcFilter\bin\debug\Kipon.Solid.SvcFilter.dll ..\..\Kipon.Solid.SvcFilter\bin\coretools
..\..\Kipon.Solid.SvcFilter\bin\coretools\CrmSvcUtil.exe /url:"http://kipon-dev/kip/XRMServices/2011/Organization.svc" "/username:deploy" "/password:#!dWWk56(<ahjDeQ" /domain:dom /out:Context.design.cs /ServiceContextName:SolidContextService /namespace:Kipon.Solid.Plugin.Entities /codewriterfilter:Kipon.Solid.SvcFilter.CodeWriterFilter,Kipon.Solid.SvcFilter /codecustomization:Kipon.Solid.SvcFilter.CustomizeCodeDomService,Kipon.Solid.SvcFilter
