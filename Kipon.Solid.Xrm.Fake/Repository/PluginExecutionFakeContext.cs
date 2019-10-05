using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository
{
    public class PluginExecutionFakeContext : System.IDisposable, IEntityShadow
    {
        #region private members
        private Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        private string unsecureConfig = null;
        private string secureConfig = null;
        private Microsoft.Xrm.Sdk.IPlugin plugin;
        private Microsoft.Xrm.Sdk.IOrganizationServiceFactory orgServiceFactory;
        private Microsoft.Xrm.Sdk.ITracingService traceService;
        private List<TransactionElement> transaction = new List<TransactionElement>();

        private static Dictionary<string, Microsoft.Xrm.Sdk.IPlugin> plugins = new Dictionary<string, Microsoft.Xrm.Sdk.IPlugin>();

        #endregion

        #region constructors
        public PluginExecutionFakeContext(Type pluginType)
        {
            this.orgServiceFactory = new Services.OrganizationServiceFactory(this);
            this.traceService = new Services.TracingService();

            var key = $"{pluginType.FullName}:null:null";

            if (plugins.ContainsKey(key))
            {
                this.plugin = plugins[key];
            }
            else
            {
                this.plugin = (Microsoft.Xrm.Sdk.IPlugin)System.Activator.CreateInstance(pluginType);
                plugins[key] = this.plugin;
            }
        }

        public PluginExecutionFakeContext(Type pluginType, string unsecureConfig, string secureConfig): this(pluginType)
        {
            this.unsecureConfig = unsecureConfig;
            this.secureConfig = secureConfig;

            var key = $"{pluginType.FullName}:{(this.unsecureConfig == null ? "null":this.unsecureConfig)}:{(this.secureConfig != null ? this.secureConfig : "null")}";

            if (plugins.ContainsKey(key))
            {
                this.plugin = plugins[key];
            }
            else
            {
                this.plugin = (Microsoft.Xrm.Sdk.IPlugin)System.Activator.CreateInstance(pluginType);
                plugins[key] = this.plugin;
            }

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

            var before = new Entity(entities[key]);
            transaction.Add(new TransactionElement { Operation = "Update", Entity = before });
            entities[key].UpdateFrom(entity);
        }

        void IEntityShadow.Delete(string logicalName, Guid id)
        {
            var key = logicalName + id.ToString();
            if (!entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(logicalName, id);
            }
            var before = new Entity(entities[key]);
            transaction.Add(new TransactionElement { Operation = "Delete", Entity = before });
            this.entities.Remove(key);
        }

        void IEntityShadow.Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            var key = entity.LogicalName + entity.Id.ToString();
            if (this.entities.ContainsKey(key))
            {
                throw new Exceptions.EntityExistsException(entity);
            }
            transaction.Add(new TransactionElement { Operation = "Create", Entity = new Entity(entity.LogicalName, entity.Id) });
            var et = new Entity(entity);
            entities.Add(et.Key, et);
        }

        void IEntityShadow.Rollback()
        {
            var slut = this.transaction.Count - 1;
            while (slut >= 0)
            {
                var trans = this.transaction[slut];

                switch (trans.Operation)
                {
                    case "Create":
                        {
                            this.entities.Remove(trans.Entity.Key);
                            break;
                        }
                    case "Delete":
                        {
                            this.entities[trans.Entity.Key] = trans.Entity;
                            break;
                        }
                    case "Update":
                        {
                            this.entities[trans.Entity.Key] = trans.Entity;
                            break;
                        }
                }
                slut--;
            }
        }

        void IEntityShadow.Commit()
        {
            this.transaction.Clear();
        }
        #endregion


        #region public methods for preparation of conext
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
            try
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
                    var pluginExecutionContext = new Services.PluginExecutionContext(10, 1, "Create", target.LogicalName, target.Id, false);
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

                ((IEntityShadow)this).Commit();

                // post async
                {
                    var pluginExecutionContext = new Services.PluginExecutionContext(40, 1, "Create", target.LogicalName, target.Id, true);
#warning make post image available
                    var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                    this.plugin.Execute(serviceProvider);

                    ((IEntityShadow)this).Commit();
                }
                return target.Id;
            } catch (Exception)
            {
                ((IEntityShadow)this).Rollback();
                throw;
            }
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

        #region internal classes
        internal class TransactionElement
        {
            internal string Operation { get; set; }
            internal Entity Entity { get; set; }
        }
        #endregion
    }
}
