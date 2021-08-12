/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/kiponexample.xrm.d.ts" />


module Kiponexport.Account {
    var Information: KiponExample.Forms.kipon_invoice.InformationForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        Information = ctx.getFormContext();

        var s = Information.getAttribute("xx").getValue();
        var p = Information.getAttribute("a1c").getValue();

        Information.ui.tabs.forEach(r => {
        });

    }
}