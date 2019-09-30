using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository
{
    public class PluginEntityContext : System.IDisposable, IEntityShadow
    {
        #region private members
        private Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        private string unsecureConfig = null;
        private string secureConfig = null;
        private Microsoft.Xrm.Sdk.IPlugin plugin;
        private Microsoft.Xrm.Sdk.IOrganizationServiceFactory orgServiceFactory;
        private Microsoft.Xrm.Sdk.ITracingService traceService;
        #endregion

        #region constructors
        public PluginEntityContext(Type pluginType)
        {
            this.orgServiceFactory = new Services.OrganizationServiceFactory(this);
            this.traceService = new Services.TracingService();

            this.plugin = (Microsoft.Xrm.Sdk.IPlugin)System.Activator.CreateInstance(pluginType);
        }

        public PluginEntityContext(Type pluginType, string unsecureConfig, string secureConfig): this(pluginType)
        {
            this.unsecureConfig = unsecureConfig;
            this.secureConfig = secureConfig;

            this.plugin = (Microsoft.Xrm.Sdk.IPlugin)System.Activator.CreateInstance(pluginType, new object[] { unsecureConfig, secureConfig });
        }
        #endregion

        #region impl. entityshadow
        Microsoft.Xrm.Sdk.Entity IEntityShadow.Get(string logicalName, Guid id)
        {
            var key = logicalName + id.ToString();
            if (!this.entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(logicalName, id);
            }
            return entities[key].ToEntity();
        }

        void IEntityShadow.Update(Microsoft.Xrm.Sdk.Entity entity)
        {
            var key = entity.LogicalName + entity.Id.ToString();
            if (!entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(entity.LogicalName, entity.Id);
            }
            entities[key].UpdateFrom(entity);
        }

        void IEntityShadow.Delete(string logicalName, Guid id)
        {
            var key = logicalName + id.ToString();
            if (!entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(logicalName, id);
            }
            this.entities.Remove(key);
        }

        void IEntityShadow.Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            var key = entity.LogicalName + entity.Id.ToString();
            if (this.entities.ContainsKey(key))
            {
                throw new Exceptions.EntityExistsException(entity);
            }
            var et = new Entity(entity);
            entities.Add(et.Key, et);
        }
        #endregion


        #region methods
        public void AddEntity(Microsoft.Xrm.Sdk.Entity crmEntity)
        {
            var et = new Entity(crmEntity);
            entities.Add(et.Key, et);
        }

        public void AddEntityRange(IEnumerable<Microsoft.Xrm.Sdk.Entity> crmEntities)
        {
            foreach (var e in crmEntities)
            {
                this.AddEntity(e);
            }
        }
        #endregion

        #region execute the plugin code
        public Guid Create(Microsoft.Xrm.Sdk.Entity target)
        {
            if (target.Id == Guid.Empty)
            {
                target.Id = Guid.NewGuid();
            }

            var key = target.LogicalName + target.Id.ToString();
            if (entities.ContainsKey(key))
            {
                throw new Exceptions.EntityExistsException(target);
            }

            // Validate
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(10, 1,"Create", target.LogicalName, target.Id, false);
                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                this.plugin.Execute(serviceProvider);
            }
            // pre
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(20, 1, "Create", target.LogicalName, target.Id, false);
                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                this.plugin.Execute(serviceProvider);
                ((Repository.IEntityShadow)this).Create(target);
            }

            // post
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(40, 1, "Create", target.LogicalName, target.Id, false);
#warning make post image available
                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                this.plugin.Execute(serviceProvider);
            }

            // post async
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(40, 1, "Create", target.LogicalName, target.Id, true);
#warning make post image available
                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                this.plugin.Execute(serviceProvider);
            }
            return target.Id;
        }

        public void Update(Microsoft.Xrm.Sdk.Entity target)
        {
            plugin.Execute(null);
        }

        public void Delete(Microsoft.Xrm.Sdk.EntityReference target)
        {
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            entities.Clear();
        }
        #endregion;

        #region private methods
        #endregion
    }
}
