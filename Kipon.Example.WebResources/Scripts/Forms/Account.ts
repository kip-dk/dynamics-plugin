/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />

//#include Scripts/Services/AccountService

module Kipon.Account {
    var Form: Kipon.Forms.account.AccountForm;

    export function onLoad(ctx: Xrm.Page.EventContext) {

        Form = ctx.getFormContext();

        var s = Form.getAttribute("name").getValue();
        var p = Form.getAttribute("donotemail").getValue();

        Form.ui.tabs.forEach(r => {
        });

        Form.ui.tabs.get("SUMMARY_TAB").sections.get("SOCIAL_PANE_TAB").setVisible(false);

        var ac = new Kipon.AccountService().getSomething();
    }
}