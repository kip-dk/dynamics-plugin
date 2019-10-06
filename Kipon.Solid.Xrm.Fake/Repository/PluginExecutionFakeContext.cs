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
                Kipon.Xrm.Reflection.Types.Instance.SetAssembly(this.plugin.GetType().Assembly);
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
            Kipon.Xrm.Reflection.Types.Instance.SetAssembly(plugin.GetType().Assembly);
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

        #region private helpers
        private Microsoft.Xrm.Sdk.Entity ResolveImage(string logicalName, Guid id, int pre1post2, Reflection.PluginMethodCache[] methods)
        {
            var needAll = false;
            string[] neededProperties = null;

            if (pre1post2 == 1)
            {
                var need = (from m in methods where m.NeedPreimage select m).Any();
                if (!need)
                {
                    return null;
                }
                needAll = (from m in methods where m.AllPreimageProperties select m).Any();

                if (!needAll)
                {
                    var np = new List<string>();
                    foreach (var m in methods)
                    {
                        if (m.PreimageProperties != null && m.PreimageProperties.Length > 0)
                        {
                            np.AddRange((from p in m.PreimageProperties select p.LogicalName).Distinct());
                        }
                    }
                    neededProperties = np.Distinct().ToArray();
                }

            }

            if (pre1post2 == 2)
            {
                var need = (from m in methods where m.NeedPostimage select m).Any();
                if (!need)
                {
                    return null;
                }

                needAll = (from m in methods where m.AllPostimageProperties select m).Any();
                if (!needAll)
                {
                    var np = new List<string>();
                    foreach (var m in methods)
                    {
                        if (m.PostimageProperties != null && m.PostimageProperties.Length > 0)
                        {
                            np.AddRange((from p in m.PostimageProperties select p.LogicalName).Distinct());
                        }
                    }
                    neededProperties = np.Distinct().ToArray();
                }
            }

            if (needAll == false && (neededProperties == null || neededProperties.Length == 0))
            {
                return null;
            }

            var key = logicalName + id.ToString();
            if (!this.entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(logicalName, id);
            }

            var data = entities[key];

            if (needAll)
            {
                return data.ToEntity();
            }

            var result = new Microsoft.Xrm.Sdk.Entity { LogicalName = logicalName, Id = id };
            foreach (var prop in neededProperties)
            {
                result[prop] = data.ValueOf(prop);
            }
            return result;
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

        #region delegates for unit tests
        public Action OnValidationCreate;
        public Action OnPreCreate;
        public Action OnPostCreate;
        public Action OnPostCreateAsync;
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
                    var methods = Kipon.Xrm.Reflection.PluginMethodCache.ForPlugin(this.plugin.GetType(), 10, "Create", target.LogicalName, false, false);
                    if (methods.Length > 0)
                    {
                        var pluginExecutionContext = new Services.PluginExecutionContext(10, 1, "Create", target.LogicalName, target.Id, false);
                        pluginExecutionContext.InputParameters.Add("Target", target);

                        var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                        this.plugin.Execute(serviceProvider);

                        this.OnValidationCreate?.Invoke();
                    }
                    else
                    {
                        if (this.OnValidationCreate != null)
                        {
                            throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), "Create", 10);
                        }
                    }
                }
                // pre
                {
                    var methods = Kipon.Xrm.Reflection.PluginMethodCache.ForPlugin(this.plugin.GetType(), 20, "Create", target.LogicalName, false, false);
                    if (methods.Length > 0)
                    {
                        var pluginExecutionContext = new Services.PluginExecutionContext(20, 1, "Create", target.LogicalName, target.Id, false);
                        pluginExecutionContext.InputParameters.Add("Target", target);

                        var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                        this.plugin.Execute(serviceProvider);
                        ((Repository.IEntityShadow)this).Create(target);

                        this.OnPreCreate?.Invoke();

                    } else
                    {
                        if (this.OnPreCreate != null)
                        {
                            throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), "Create", 20);
                        }
                    }
                }

                {
                    // post
                    var hasPostSync = false;
                    {
                        var methods = Kipon.Xrm.Reflection.PluginMethodCache.ForPlugin(this.plugin.GetType(), 40, "Create", target.LogicalName, false, false);

                        if (methods.Length > 0) {
                            var pluginExecutionContext = new Services.PluginExecutionContext(40, 1, "Create", target.LogicalName, target.Id, false);

                            var postTarget = ((IEntityShadow)this).Get(target.LogicalName, target.Id);
                            pluginExecutionContext.InputParameters.Add("Target", postTarget);

                            var imagePost = this.ResolveImage(target.LogicalName, target.Id, 2, methods);
                            if (imagePost != null)
                            {
                                var imgName = Reflection.PluginMethodCache.ImageSuffixFor(2, 40, false);
                                pluginExecutionContext.PostEntityImages.Add(imgName, imagePost);
                            }

                            var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                            this.plugin.Execute(serviceProvider);
                            hasPostSync = true;
                        }
                    }
                ((IEntityShadow)this).Commit();

                    if (hasPostSync)
                    {
                        this.OnPostCreate?.Invoke();
                    } else
                    {
                        if (this.OnPostCreate != null)
                        {
                            throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), "Create", 40);
                        }
                    }
                }

                {
                    // post async
                    {
                        var methods = Kipon.Xrm.Reflection.PluginMethodCache.ForPlugin(this.plugin.GetType(), 40, "Create", target.LogicalName, false, true);

                        if (methods.Length > 0) {
                            var pluginExecutionContext = new Services.PluginExecutionContext(40, 1, "Create", target.LogicalName, target.Id, true);

                            var postTarget = ((IEntityShadow)this).Get(target.LogicalName, target.Id);
                            pluginExecutionContext.InputParameters.Add("Target", postTarget);

                            var imagePost = this.ResolveImage(target.LogicalName, target.Id, 2, methods);
                            if (imagePost != null)
                            {
                                var imgName = Reflection.PluginMethodCache.ImageSuffixFor(2, 40, true);
                                pluginExecutionContext.PostEntityImages.Add(imgName, imagePost);
                            }

                            var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this);
                            this.plugin.Execute(serviceProvider);
                            ((IEntityShadow)this).Commit();

                            this.OnPostCreateAsync?.Invoke();
                        } else
                        {
                            if (this.OnPostCreateAsync != null)
                            {
                                throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), "Create", 40, true);
                            }
                        }
                    }
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
            try
            {
                var key = target.LogicalName + target.Id.ToString();
                if (!entities.ContainsKey(key))
                {
                    throw new Exceptions.EntityNotFoundException(target.LogicalName, target.Id);
                }
            } catch (Exception)
            {
                ((IEntityShadow)this).Rollback();
                throw;
            }
        }

        public void Delete(Microsoft.Xrm.Sdk.EntityReference target)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region dispose
        void System.IDisposable.Dispose()
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
