using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Tools.XrmOrganization
{
    public class OrganizationService : Microsoft.Xrm.Sdk.IOrganizationService
    {
        public static Microsoft.Xrm.Sdk.IOrganizationService ForcedOrganization { get; set; }

        private Microsoft.Xrm.Sdk.IOrganizationService instance;

        public OrganizationService()
        {
            if (ForcedOrganization != null)
            {
                this.instance = ForcedOrganization;
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var connectionString = ConnectionString.Value;

            if (connectionString.Contains("ClientSecret"))
            {
                this.instance = new OAuthOrganizationService();
            }
            else
            {
                try
                {
                    var client = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);
                    client.OrganizationServiceProxy.EnableProxyTypes(typeof(OrganizationService).Assembly);
                    this.instance = client;
                }
                catch (Exception)
                {
                    ClientCredentials clientCredentials = new ClientCredentials();
                    clientCredentials.UserName.UserName = connectionString.GetParameter("Username");
                    clientCredentials.UserName.Password = connectionString.GetParameter("Password");

                    var uri = new Uri($"{connectionString.GetParameter("Url")}/XRMServices/2011/Organization.svc");

                    var proxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);
                    proxy.EnableProxyTypes(typeof(OrganizationService).Assembly);
                    this.instance = proxy;
                }
            }
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            instance.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public Guid Create(Entity entity)
        {
            return instance.Create(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            instance.Delete(entityName, id);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            instance.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return instance.Execute(request);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return instance.Retrieve(entityName, id, columnSet);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return instance.RetrieveMultiple(query);
        }

        public void Update(Entity entity)
        {
            instance.Update(entity);
        }
    }
}
