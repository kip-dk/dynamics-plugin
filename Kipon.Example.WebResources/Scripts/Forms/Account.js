/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../typings/kiponexample.xrm.d.ts" />
var Kiponexport;
(function (Kiponexport) {
    var Account;
    (function (Account) {
        var Information;
        function onLoad(ctx) {
            Information = ctx.getFormContext();
            var s = Information.getAttribute("xx").getValue();
            var p = Information.getAttribute("a1c").getValue();
            Information.ui.tabs.forEach(function (r) {
            });
        }
        Account.onLoad = onLoad;
    })(Account = Kiponexport.Account || (Kiponexport.Account = {}));
})(Kiponexport || (Kiponexport = {}));
//# sourceMappingURL=Account.js.map