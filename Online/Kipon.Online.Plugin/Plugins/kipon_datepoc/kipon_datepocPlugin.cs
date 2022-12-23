using Kipon.Xrm.Attributes;
using Microsoft.Xrm.Sdk;
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

        public void OnPreCreate(Entities.kipon_datepoc target, Kipon.Xrm.IRepository<Entities.kipon_datepoc> repository)
        {
            target.kipon_no = (from r in repository.GetQuery() select r.kipon_datepocId.Value).ToArray().Count() + 1;

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


        [Sort(1)]
        public void OnPreUpdate(Entities.kipon_datepoc.IDateChanged mergedimage, IQueryable<Entities.kipon_datepoc> dateQuery, Kipon.Xrm.ServiceAPI.IEntityCache cache, Entities.IUnitOfWork uow)
        {
            // Cached query, that only fetch a few fields
            var gb = (from d in uow.Datepocs.GetQuery()
                      where d.kipon_dateonly < System.DateTime.Today.AddYears(10)
                        && d.kipon_name != null
                      select new
                      {
                          Id = d.kipon_datepocId.Value,
                          TSDate = d.kipon_timezonefreedatetime
                      }).ToArray();

            foreach (var g in gb)
            {
                cache.Attach(new Entities.kipon_datepoc { kipon_datepocId = g.Id, kipon_timezonefreedatetime = g.TSDate, EntityState = EntityState.Unchanged });
            }

            /// this query will return values where Name is null, even though it does not make sense from a logical point of view
            /// This is due to that fact that we are puttning the elements into the context cache in above query
            var names = (from g in uow.Datepocs.GetQuery()
                         where g.kipon_dateonly < System.DateTime.Today.AddYears(3)
                           && g.kipon_name != null
                         select new
                         {
                             Id = g.kipon_datepocId.Value,
                             Name = g.kipon_name
                         }).ToArray();

            var counts = cache.GetAttachedEntities().Count();

            if (dateQuery is IQueryable<Entities.kipon_datepoc>)
            {
                /// queries injected directly to a service will 

                var older = (from d in dateQuery
                               where d.kipon_datepocId != mergedimage.Id
                                 && d.kipon_dateonly < mergedimage.kipon_dateonly
                                 && d.kipon_name != null
                               select new
                               {
                                   Id = d.kipon_datepocId.Value,
                                   Name = d.kipon_name
                               }).ToArray();

                if (older.Where(r => string.IsNullOrEmpty(r.Name)).Any())
                {
                    throw new InvalidPluginExecutionException("Ups!, we did loos the name in the query running before us.");
                }

                var cn = cache.GetAttachedEntities().Count();

                if (counts != cn)
                {
                    throw new InvalidPluginExecutionException("The process did add things to the context, that must be a bug.");
                }

                mergedimage.kipon_testresult = $"Found { older.Length } people older than your {counts} / {cn}  ({gb.Length}) names: {names.Length} missing names: { names.Where(n => string.IsNullOrEmpty(n.Name)).Count() }";
            }
            else
            {
                throw new InvalidPluginExecutionException("dateQuery is not expected type of query");
            }
        }


        public void OnPostUpdate(Entities.kipon_datepoc.IDateChanged mergedimage)
        {
            if (mergedimage.kipon_dateonly != null && mergedimage.kipon_dateonly.Value == new DateTime(1964, 1, 20))
            {
                if (mergedimage.kipon_name != "Kjeld I. Poulsen")
                {
                    throw new InvalidPluginExecutionException("Name is not as expected");
                }
            }
        }
    }
}
