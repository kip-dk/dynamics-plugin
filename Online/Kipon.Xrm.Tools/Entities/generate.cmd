@echo off
copy ..\\bin\debug\Kipon.Xrm.dll ..\bin\coretools
copy ..\\bin\debug\Kipon.Xrm.Tools.dll ..\bin\coretools
..\bin\coretools\CrmSvcUtil.exe "/connectionstring:AuthType=ClientSecret; url=https://kipon-plugin.crm4.dynamics.com; ClientId=e9d84925-56fb-4b39-814b-5d44bed7332e; ClientSecret=WGW8Q~AIhn5WSB_StqNSs8eiuMk~XVG-G~InQc7N" /out:Context.design.cs /ServiceContextName:ContextService /namespace:Kipon.Xrm.Tools.Entities /codewriterfilter:Kipon.Xrm.Tools.CodeWriter.CodeWriterFilter,Kipon.Xrm.Tools /codecustomization:Kipon.Xrm.Tools.CodeWriter.ToolsCustomizeCodeDomService,Kipon.Xrm.Tools