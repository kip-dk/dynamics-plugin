/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />
var Kipon;
(function (Kipon) {
    var Account;
    (function (Account) {
        var Form;
        function onLoad(ctx) {
            Form = ctx.getFormContext();
            var s = Form.getAttribute("name").getValue();
            var p = Form.getAttribute("donotemail").getValue();
            Form.ui.tabs.forEach(function (r) {
            });
            Form.ui.tabs.get("SUMMARY_TAB").sections.get("SOCIAL_PANE_TAB").setVisible(false);
        }
        Account.onLoad = onLoad;
    })(Account = Kipon.Account || (Kipon.Account = {}));
})(Kipon || (Kipon = {}));
//# sourceMappingURL=Account.js.map