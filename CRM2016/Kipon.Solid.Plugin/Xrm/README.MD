## Kipon.Xrm plugin base library, (c) Kipon ApS 2019,2020

The Kipon.Xrm folder in this library was created by Kjeld Ingemann Poulsen, Kipon ApS to help
creating better Dynamics 365 plugins, using SOLID as the based princip of writing plugins.

The code is open-source, however you are not allowed to change the namespace of any elements hosted in the Kipon/Xrm folder.
That would be a violation of the license terms.

This README.MD file must be a part of any plugin using the Kipon.Xrm implementation, and it must be visisble in the Xrm folder hosting the code 
provided by the library.

Together with the Kipon.Xrm code base, that must be part of your custom assembly due to single assembly deployment model of Dynamics 365 plugins
and additional tools is available (Kipon.Xrm.Tools). 

This tool provide functionality:

   1) For building early-bound-entities and more 
   2) for automate deployment into your CRM development instance 

These tools are assuming the use of the Kipon.Xrm namespace.  Any renaming or removal of elements in the Kipon.Xrm folder will cause these tools to 
fail and thereby reduce the value of using the Kipon.Xrm aseembly plugin framework significantly.

Please goto the official documentation site for the Kipon Dynamics 365 plugin development model:

TO-DO LINK to official documentation site.