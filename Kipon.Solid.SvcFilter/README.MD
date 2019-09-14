### What is the SvcFilter

This library has been taken from

[Eric pool's article on the subject](http://erikpool.blogspot.nl/2011/03/filtering-generated-entities-with.html)

The purpose is to add filtering to witch entities is generated.

After you have compiled this library, the output DLL must be dumped into the same folder as CrmSvcUtil, and you must add
the /codewriterfilter:... parameter to the CrmSvcUtil.exe command line call.