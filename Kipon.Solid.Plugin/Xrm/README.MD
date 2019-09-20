## Kipon.Xrm plugin base library, (c) Kipon ApS 2019

The Kipon.Xrm folder in this library was created by Kjeld Ingemann Poulsen, Kipon ApS to help
creating better Dynamics 365 plugins, using SOLID as the based princip of writing plugins.

The code is open-source, however you are not allowed to change the namespace of any elements hosted in the Xrm folder.
That would be a violation of the license terms.

This README.MD file must be a part of any plugin using the Kipon.Xrm implementation.

Together with the Kipon.Xrm code base, witch must be part of your custom assembly due to single assembly deployment model of Dynamics 365,
two additional tools are available for building early-bound-entities and more, and to automate deployment to CRM. These tools are assuming the use 
of the Kipon.Xrm namespace.  Any renaming or removal of elements in the Kipon.Xrm folder will cause these tools to fail and thereby reduce the value
of using the Kipon.Xrm significantly.