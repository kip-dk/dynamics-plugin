﻿@echo off
copy ..\\bin\debug\Kipon.Xrm.Tools.dll ..\bin\coretools
..\bin\coretools\CrmSvcUtil.exe "/connectionstring:AuthType=AD;Url=http://kipon-dev/kip;" /out:Context.design.cs /ServiceContextName:ContextService /namespace:Kipon.Xrm.Tools.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.ToolsCustomizeCodeDomService,Kipon.Xrm.Tools