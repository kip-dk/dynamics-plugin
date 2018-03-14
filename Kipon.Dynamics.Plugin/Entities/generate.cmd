@echo off
copy ..\..\Kipon.Dynamics.SvcFilter\bin\debug\Kipon.Dynamics.SvcFilter.dll ..\..\Kipon.Dynamics.SvcFilter\bin\coretools
..\..\Kipon.Dynamics.SvcFilter\bin\coretools\CrmSvcUtil.exe /url:"http://kipon-dev/kip/XRMServices/2011/Organization.svc" "/username:deploy" "/password:#!dWWk56(<ahjDeQ" /domain:dom /out:Context.design.cs /ServiceContextName:ContextService /namespace:Kipon.Dynamics.Plugin.Entities /codewriterfilter:Kipon.Dynamics.SvcFilter.CodeWriterFilter,Kipon.Dynamics.SvcFilter /codecustomization:Kipon.Dynamics.SvcFilter.CustomizeCodeDomService,Kipon.Dynamics.SvcFilter
