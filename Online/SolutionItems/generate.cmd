﻿@echo off
"..\bin\coretools\CrmSvcUtil.exe" "/connectionstring:AuthType=ClientSecret; url=https://[?].crm[?].dynamics.com; ClientId=[?]; ClientSecret=[?]" /out:Context.design.cs /ServiceContextName:XrmContextService /namespace:[namespace].Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.PluginCustomizeCodeDomService,Kipon.Xrm.Tools /generateActions