using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.kipon_datepoc
{
    public class kipon_datepocPlugin : Kipon.Xrm.BasePlugin
    {
        private const string label = "Test of: ";

        public void OnPreCreate(Entities.kipon_datepoc target/*, Kipon.Xrm.IRepository<Entities.kipon_datepoc> repository */)
        {
            if (target.kipon_name == "test")
            {
                var testDate = System.DateTime.Now;
                target.kipon_name = $"{ label }{ testDate.ToString("yyyy-MM-dd HH:mm:ss") } / {target.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss")}";
                target.kipon_timezonefreedateonly = testDate;
                target.kipon_timezonefreedatetime = testDate;
                target.kipon_dateonly = testDate;
                target.kipon_localdateonly = testDate;
                target.kipon_localdatetime = testDate;
                target.kipon_wastest = true;
                target.kipon_testresult = "Please wait a few seconds, then update the form to se the result";
            }
        }

        public void OnPostCreateAsync(Entities.kipon_datepoc target, Entities.IUnitOfWork uow)
        {
            if (target.kipon_wastest == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine(target.kipon_name);
                sb.AppendLine($"timezonefreedateonly      : {target.kipon_timezonefreedateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_timezonefreedatetime: {target.kipon_timezonefreedatetime?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_dateonly            : {target.kipon_dateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_localdateonly       : {target.kipon_localdateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_localdatetime       : {target.kipon_localdatetime?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"CreatedOn                 : {target.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"======================================= Fetch below =========================================");
                var me = (from q in uow.Datepocs.GetQuery()
                          where q.kipon_datepocId == target.Id
                          select q).Single();

                sb.AppendLine($"timezonefreedateonly      : {me.kipon_timezonefreedateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_timezonefreedatetime: {me.kipon_timezonefreedatetime?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_dateonly            : {me.kipon_dateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_localdateonly       : {me.kipon_localdateonly?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"kipon_localdatetime       : {me.kipon_localdatetime?.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"CreatedOn                 : {me.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss")}");

                var clean = new Entities.kipon_datepoc
                {
                    kipon_datepocId = target.Id,
                    kipon_testresult = sb.ToString(),
                    kipon_wastest = false
                };
                uow.Update(clean);
            }
        }
    }
}
