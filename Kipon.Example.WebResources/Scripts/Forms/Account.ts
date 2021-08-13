/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />


module Kipon.Account {
    var Information: Kipon.Forms.account.AccountForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        Information = ctx.getFormContext();

        var s = Information.getAttribute("name").getValue();
        var p = Information.getAttribute("donotemail").getValue();

        Information.ui.tabs.forEach(r => {
        });

        Information.ui.tabs.get("SUMMARY_TAB").sections.get("SOCIAL_PANE_TAB").setVisible(false);
    }
}