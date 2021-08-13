/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/kiponexample.xrm.d.ts" />
var Kipon;
(function (Kipon) {
    var Account;
    (function (Account) {
        var Information;
        function onLoad(ctx) {
            Information = ctx.getFormContext();
            var s = Information.getAttribute("xx").getValue();
            var p = Information.getAttribute("a1c").getValue();
            Information.ui.tabs.forEach(function (r) {
            });
            Information.ui.tabs.get("tab_t1").sections.forEach(function (s) {
            });
        }
        Account.onLoad = onLoad;
    })(Account = Kipon.Account || (Kipon.Account = {}));
})(Kipon || (Kipon = {}));
//# sourceMappingURL=Account.js.map