﻿@echo off
copy ..\..\Kipon.Xrm.Tools\bin\debug\Kipon.Xrm.Tools.* ..\..\Kipon.Xrm.Tools\bin\coretools
..\..\Kipon.Xrm.Tools\bin\coretools\CrmSvcUtil.exe "/connectionstring:AuthType=AD;Url=http://kipon-dev/kip;" /out:Context.design.cs /ServiceContextName:SolidContextService /namespace:Kipon.Solid.Plugin.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.PluginCustomizeCodeDomService,Kipon.Xrm.Tools /generateActions /debug
