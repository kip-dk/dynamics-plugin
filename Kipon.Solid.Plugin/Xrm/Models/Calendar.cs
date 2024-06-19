namespace Kipon.Xrm.Models
{
    using Kipon.Xrm.Extensions.DateTimes;
    using System;
    using System.Reflection;

    public class Calendar
    {
        public static readonly Calendar Current = new Calendar();
        private DateTime _yearStart = System.DateTime.UtcNow.StartOfYear();
        private DayOfWeek _startDayOfWeek = DayOfWeek.Monday;
        private DateTime? _timeout;

        private Calendar()
        {

        }

        public DateTime YearStart
        {
            get
            {
                var now = System.DateTime.UtcNow;
                if (now.Year == _yearStart.Year)
                {
                    return _yearStart;
                }
                _yearStart = new DateTime(now.Year, _yearStart.Month, _yearStart.Day, 0, 0, 0, 0, _yearStart.Kind);
                return _yearStart;
            }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return this._startDayOfWeek;
            }
        }

        public DayOfWeek LastDayOfWeek
        {
            get
            {
                if (FirstDayOfWeek == DayOfWeek.Monday)
                {
                    return DayOfWeek.Sunday;
                }
                return DayOfWeek.Saturday;
            }
        }

        private static void SetYearStart(System.DateTime date)
        {
            Current._yearStart = date;
        }

        private static void SetFirstDayOfWeek(DayOfWeek day)
        {
            Current._startDayOfWeek = day;
        }

        internal static void Initialize(Type pluginType, Guid orgId, Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            if (Current._timeout == null || Current._timeout.Value < System.DateTime.UtcNow)
            {
                var fisYear = pluginType.GetCustomAttribute(typeof(Attributes.FiscalAttribute));

                if (fisYear != null)
                {
                    var org = orgService.Retrieve("organization", orgId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new string[] { "weekstartdaycode", "fiscalcalendarstart" }));
                    SetYearStart((DateTime)org["fiscalcalendarstart"]);
                    SetFirstDayOfWeek((DayOfWeek)(int)org["weekstartdaycode"]);
                    Current._timeout = System.DateTime.UtcNow.AddHours(12);
                }
            }
        }
    }
}
