using Kipon.Xrm.Tools.Entities;
using Kipon.Xrm.Tools.Extensions.TypeConverters;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("export-sdk-messages", typeof(ICmd))]
    public class ExportSdkMessages : ICmd
    {
        private readonly IUnitOfWork uow;

        [ImportingConstructor]
        public ExportSdkMessages(Kipon.Xrm.Tools.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (!System.IO.File.Exists("filter.xml"))
            {
                Console.WriteLine($"No filter.xml fil found in current folder");
                return Task.CompletedTask;
            }


            var filterFile = new Kipon.Xrm.Tools.CodeWriter.Model.Filter("filter.xml");

            if (string.IsNullOrEmpty(filterFile.ACTION_BACKUP_FILENAME))
            {
                Console.WriteLine($"Please add attribute: action-backup-filename to the root tag of the xml, containing name of file to use as export name");
                return Task.CompletedTask;
            }

            if (filterFile.ACTIONS == null || filterFile.ACTIONS.Count == 0)
            {
                Console.WriteLine($"You did not configure any actions to be generated. You do not need an action-backup-file");
                return Task.CompletedTask;
            }

            var meta = new RetrieveAllEntitiesRequest
            {
                EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity,
                RetrieveAsIfPublished = false
            };

            var response = this.uow.ExecuteRequest<RetrieveAllEntitiesResponse>(meta);

            var map = response.EntityMetadata.Where(r => r.ObjectTypeCode != null && !string.IsNullOrEmpty(r.LogicalName)).ToDictionary(r => r.LogicalName, v => v.ObjectTypeCode.Value);
            map.Add("none", 0);

            var mess = (from s in uow.SdkMessages.GetQuery()
                        orderby s.Name
                        select s).ToArray();

            var requests = (from r in uow.SdkMessageRequests.GetQuery()
                            select r).ToArray();

            var responses = (from r in uow.SdkMessageResponses.GetQuery()
                             select r).ToArray();

            var pairs = (from p in uow.SdkMessagePairs.GetQuery()
                         select p).ToArray();

            var filter = (from f in uow.SdkMessageFilters.GetQuery()
                          select f).ToArray();

            var inputs = (from r in uow.SdkMessageRequestFields.GetQuery()
                          select r).ToArray();

            var outputs = (from r in uow.SdkMessageResponseFields.GetQuery()
                           select r).ToArray();


            Console.WriteLine($"Found: Message: {mess.Length}\nFilter: { filter.Length }\nInputs: {inputs.Length}\nOutputs: {outputs.Length}\nPairs: { pairs.Length }\nRequests: { requests.Length }\nResponses: { responses.Length }");

            var result = (from m in mess
                          select new Kipon.Xrm.Tools.Models.SdkMessageWrapper
                          {
                              Code = map[filter.Where(r => r.SdkMessageId.Id == m.SdkMessageId).Select(r => r.PrimaryObjectTypeCode).DefaultIfEmpty("none").First()],
                              Name = m.Name,
                              InputFields = (from i in inputs
                                             join r in requests on i.SdkMessageRequestId.Id equals r.SdkMessageRequestId
                                             join p in pairs on r.SdkMessagePairId.Id equals p.SdkMessagePairId
                                             where p.SdkMessageId.Id == m.SdkMessageId
                                               && i.ClrParser != null
                                               && !i.ClrParser.StartsWith("Microsoft.Crm.Sdk.Query.ColumnSetBase")
                                             select new Kipon.Xrm.Tools.Models.SdkMessageWrapper.Field
                                             {
                                                 CLRFormatter = i.ClrParser,
                                                 IsOptional = i.Optional ?? false,
                                                 Name = i.Name
                                             }).ToArray().Distinct().OrderBy(r => r.Name).ToArray().BestOf(),
                              OutputFields = (from i in outputs
                                              join s in responses on i.SdkMessageResponseId.Id equals s.SdkMessageResponseId
                                              join r in requests on s.SdkMessageRequestId.Id equals r.SdkMessageRequestId
                                              join p in pairs on r.SdkMessagePairId.Id equals p.SdkMessagePairId
                                              where p.SdkMessageId.Id == m.SdkMessageId
                                                && i.ClrFormatter != null
                                              select new Kipon.Xrm.Tools.Models.SdkMessageWrapper.Field
                                              {
                                                  Name = i.Name,
                                                  CLRFormatter = i.ClrFormatter
                                              }).ToArray().Distinct().OrderBy(r => r.Name).ToArray().BestOf()
                          }).ToArray();

            result = result.Where(r => filterFile.ACTIONS.Where(a => a.LogicalName == r.Name).Any()).ToArray().OrderBy(r => r.Name).ToArray();

            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(result.GetType());

            using (var file = new System.IO.FileStream(filterFile.ACTION_BACKUP_FILENAME, System.IO.FileMode.Create))
            {
                ser.WriteObject(file, result);
            }

            return Task.CompletedTask;
        }
    }

    public static class ExportSdkMessageLocalExtensions
    {
        public static Xrm.Tools.Models.SdkMessageWrapper.Field[] BestOf(this Xrm.Tools.Models.SdkMessageWrapper.Field[] fields)
        {
            var result = new List<Xrm.Tools.Models.SdkMessageWrapper.Field>();

            var groups = (from f in fields
                          group f by f.Name into grp
                          select new
                          {
                              Name = grp.Key,
                              Rows = grp.ToArray()
                          }).ToArray();

            void add(Xrm.Tools.Models.SdkMessageWrapper.Field f)
            {
                if (f.IsOptional == null)
                {
                    f.IsOptional = false;
                }
                result.Add(f);
            }

            foreach (var group in groups)
            {
                if (group.Rows.Length == 1)
                {
                    add(group.Rows[0]);
                    continue;
                }


                var rows = group.Rows.Where(r => !string.IsNullOrEmpty(r.CLRFormatter)).ToArray();
                if (rows.Length == 1)
                {
                    add(rows[0]);
                    continue;
                }

                rows = group.Rows.Where(r => r.IsOptional.HasValue).ToArray();
                if (rows.Length == 1)
                {
                    add(rows[0]);
                    continue;
                }

                rows = group.Rows.Where(r => !string.IsNullOrEmpty(r.CLRFormatter) && r.IsOptional.HasValue).ToArray();
                if (rows.Length == 1)
                {
                    add(rows[0]);
                    continue;
                }

                add(group.Rows[0]);
            }
            return result.ToArray();
        }
    }
}
