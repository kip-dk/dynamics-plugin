using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("wr-deploy", typeof(ICmd))]
    public class WebResourceDeploy : ICmd
    {
        private readonly Xrm.Tools.Entities.IUnitOfWork uow;
        private readonly IMessageService messageService;

        [ImportingConstructor]
        public WebResourceDeploy(Kipon.Xrm.Tools.Entities.IUnitOfWork uow,  Kipon.Xrm.Tools.ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (System.IO.File.Exists("filter.xml"))
            {
                using (var file = new System.IO.FileStream("filter.xml", System.IO.FileMode.Open))
                {
                    #region initialize the process
                    var merger = new Kipon.Xrm.Tools.Models.WebResourceMerge();

                    var config = new Kipon.Xrm.Tools.Configs.Webresource(file);
                    var solution = (from s in uow.Solutions.GetQuery()
                                    where s.UniqueName == config.Deploys.ManagedSolution
                                    select new
                                    {
                                        Id = s.SolutionId.Value,
                                        Name = s.FriendlyName
                                    }).Single();

                    uow.Detach(Kipon.Xrm.Tools.Entities.Solution.EntityLogicalName, solution.Id);

                    var managedWebResources = (from wr in uow.WebResources.GetQuery()
                                               join sc in uow.SolutionComponents.GetQuery() on wr.WebResourceId equals sc.ObjectId
                                               where sc.SolutionId.Id == solution.Id
                                               select new
                                               {
                                                   Id = wr.WebResourceId.Value,
                                                   Name = wr.Name,
                                                   CanBeDeleted = wr.CanBeDeleted != null ? wr.CanBeDeleted.CanBeChanged : true,
                                                   InManagerSolution = true,
                                                   ModifiedOn = wr.ModifiedOn
                                               }).ToList();

                    Dictionary<Guid, string[]> webresourceSolutionIndex = null;

                    if (config.Deploys.Solutions != null && config.Deploys.Solutions.Length > 0)
                    {
                        var cpIndex = (from sc in uow.SolutionComponents.GetQuery()
                                        join wr in uow.WebResources.GetQuery() on sc.ObjectId equals wr.WebResourceId.Value
                                        join so in uow.Solutions.GetQuery() on sc.SolutionId.Id equals so.SolutionId
                                        where sc.ComponentType.Value == 61
                                        select new
                                        {
                                            Solution = so.UniqueName,
                                            WebResourceId = wr.WebResourceId.Value
                                        })
                                        .Distinct()
                                        .ToArray()
                                        .Where(r => config.Deploys.Solutions.Contains(r.Solution))
                                        .ToArray();

                        uow.Detach(Kipon.Xrm.Tools.Entities.SolutionComponent.EntityLogicalName);

                        webresourceSolutionIndex = (from cp in cpIndex
                                                    group cp by cp.WebResourceId into grp
                                                    select new
                                                    {
                                                        Id = grp.Key,
                                                        Solutions = grp.Select(r => r.Solution).ToArray()
                                                    }).ToDictionary(r => r.Id, v => v.Solutions);
                    }



                    uow.Detach(Kipon.Xrm.Tools.Entities.WebResource.EntityLogicalName, managedWebResources.Select(r => r.Id));

                    List<Guid> toBePublished = new List<Guid>();
                    #endregion

                    #region create/update webresources
                    foreach (var deploy in config.Deploys)
                    {
                        var managedWr = managedWebResources.Where(r => r.Name == deploy.Name).SingleOrDefault();

                        if (managedWr == null)
                        {
                            managedWr = (from wr in uow.WebResources.GetQuery()
                                         where wr.Name == deploy.Name
                                         select new
                                         {
                                             Id = wr.WebResourceId.Value,
                                             Name = wr.Name,
                                             CanBeDeleted = wr.CanBeDeleted != null ? wr.CanBeDeleted.CanBeChanged : true,
                                             InManagerSolution = false,
                                             ModifiedOn = wr.ModifiedOn
                                         }).SingleOrDefault();

                            if (managedWr != null)
                            {
                                uow.Detach(Kipon.Xrm.Tools.Entities.WebResource.EntityLogicalName, managedWr.Id);
                            } else
                            {
                                managedWr = new
                                {
                                    Id = Guid.NewGuid(),
                                    Name = deploy.Name,
                                    CanBeDeleted = true,
                                    InManagerSolution = false,
                                    ModifiedOn = (DateTime?)null
                                };
                            }
                        }

                        var resourceType = deploy.Filename.ToFileType();

                        if (resourceType == Xrm.Tools.Entities.WebResourceTypeEnum.Unknown)
                        {
                            this.messageService.Inform($"Unable to resolve webresource type for source: { deploy.Filename }. The item is ignored");
                            continue;
                        }

                        if (merger.IsChanged(deploy.Filename, managedWr.ModifiedOn))
                        {
                            toBePublished.Add(managedWr.Id);

                            var content = merger.GetMergedFileContent(deploy.Filename, resourceType);

                            if (managedWr.ModifiedOn != null)
                            {
                                var clean = new Kipon.Xrm.Tools.Entities.WebResource
                                {
                                    WebResourceId = managedWr.Id,
                                    Content = System.Convert.ToBase64String(content),
                                    Description = deploy.Description,
                                    WebResourceType = new Microsoft.Xrm.Sdk.OptionSetValue((int)resourceType)
                                };

                                var updateReq = new Microsoft.Xrm.Sdk.Messages.UpdateRequest
                                {
                                    Target = clean
                                };

                                if (!managedWr.InManagerSolution)
                                {
                                    updateReq.Parameters.Add("SolutionUniqueName", config.Deploys.ManagedSolution);
                                }
                                uow.Execute(updateReq);
                                this.messageService.Inform($"{ deploy.Name } was updated.");
                            }
                            else
                            {
                                var clean = new Kipon.Xrm.Tools.Entities.WebResource
                                {
                                    WebResourceId = managedWr.Id,
                                    Name = deploy.Name,
                                    DisplayName = deploy.Filename,
                                    Content = System.Convert.ToBase64String(content),
                                    Description = deploy.Description,
                                    WebResourceType = new Microsoft.Xrm.Sdk.OptionSetValue((int)resourceType)
                                };

                                CreateRequest createRequest = new CreateRequest
                                {
                                    Target = clean
                                };
                                createRequest.Parameters.Add("SolutionUniqueName", config.Deploys.ManagedSolution);
                                uow.Execute(createRequest);
                                this.messageService.Inform($"{ deploy.Name } was created.");
                            }

                            if (config.Deploys.Solutions != null && config.Deploys.Solutions.Length > 0)
                            {
                                foreach (var sol in config.Deploys.Solutions)
                                {
                                    if (webresourceSolutionIndex.TryGetValue(managedWr.Id, out string[] solutions) && solutions.Contains(sol)) continue;

                                    var req = new AddSolutionComponentRequest
                                    {
                                        AddRequiredComponents = false,
                                        ComponentId = managedWr.Id,
                                        ComponentType = 61,
                                        SolutionUniqueName = sol
                                    };
                                    uow.Execute(req);
                                }
                            }
                        } else
                        {
                            this.messageService.Inform($"{ deploy.Name } was unchanged.");
                        }

                        if (managedWr.InManagerSolution)
                        {
                            managedWebResources.Remove(managedWr);
                        }
                    }
                    #endregion

                    #region cleanup webresource no longer needed
                    foreach (var wr in managedWebResources)
                    {
                        if (wr.CanBeDeleted)
                        {
                            uow.Delete(new Kipon.Xrm.Tools.Entities.WebResource { WebResourceId = wr.Id });
                            this.messageService.Inform($"{wr.Name} was deleted. It was in the managed-solution, but did not exists in the source.");
                        } else
                        {
                            this.messageService.Inform($"{wr.Name} is not longer in the code and should be deleted, but it has dependencies. Delete was ignored");
                        }
                    }
                    #endregion

                    #region publish the managed solution
                    if (toBePublished.Count > 0)
                    {
                        this.publish(toBePublished.ToArray());
                    }
                    #endregion
                }
            }
            else
            {
                this.messageService.Inform($"Could not find file: filter.xml in current directory.");
            }
            return Task.CompletedTask;
        }

        private void publish(Guid[] webresourceIds)
        {
            this.messageService.Inform("Publishing Changes");
            OrganizationRequest organizationRequest = new OrganizationRequest
            {
                RequestName = "PublishXml"
            };
            organizationRequest.Parameters = new ParameterCollection();
            organizationRequest.Parameters.Add(new KeyValuePair<string, object>("ParameterXml", string.Format("<importexportxml><webresources>{0}</webresources></importexportxml>", string.Join("", from a in webresourceIds
                                                                                                                                                                                                     select $"<webresource>{a}</webresource>"))));
            uow.Execute(organizationRequest);
        }

    }
}
