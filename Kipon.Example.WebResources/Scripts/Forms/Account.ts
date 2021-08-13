/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/kiponexample.xrm.d.ts" />


module Kipon.Account {
    var Information: KiponExample.Forms.kipon_invoice.InformationForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        Information = ctx.getFormContext();

        var s = Information.getAttribute("xx").getValue();
        var p = Information.getAttribute("a1c").getValue();

        Information.ui.tabs.forEach(r => {
        });

        Information.ui.tabs.get("tab_t1").sections.forEach(s => {
        });
    }
}