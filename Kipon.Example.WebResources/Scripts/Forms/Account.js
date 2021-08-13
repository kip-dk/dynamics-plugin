/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/Kipon.Forms.d.ts" />
var Kipon;
(function (Kipon) {
    var Account;
    (function (Account) {
        var Information;
        function onLoad(ctx) {
            Information = ctx.getFormContext();
            var s = Information.getAttribute("name").getValue();
            var p = Information.getAttribute("donotemail").getValue();
            Information.ui.tabs.forEach(function (r) {
            });
            Information.ui.tabs.get("SUMMARY_TAB");
        }
        Account.onLoad = onLoad;
    })(Account = Kipon.Account || (Kipon.Account = {}));
})(Kipon || (Kipon = {}));
//# sourceMappingURL=Account.js.map