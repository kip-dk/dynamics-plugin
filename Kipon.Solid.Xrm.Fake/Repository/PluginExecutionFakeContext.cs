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
        private Dictionary<string, EntityShadow> entities = new Dictionary<string, EntityShadow>();
        private string unsecureConfig = null;
        private string secureConfig = null;
        private Microsoft.Xrm.Sdk.IPlugin plugin;
        private Microsoft.Xrm.Sdk.IOrganizationServiceFactory orgServiceFactory;
        private Microsoft.Xrm.Sdk.IOrganizationService orgService;
        private Microsoft.Xrm.Sdk.ITracingService traceService;
        private List<TransactionElement> transaction = new List<TransactionElement>();
        private Guid? userId;
        private object uow;
        private Dictionary<Type, object> queryCache = new Dictionary<Type, object>();
        private static Dictionary<string, Microsoft.Xrm.Sdk.IPlugin> plugins = new Dictionary<string, Microsoft.Xrm.Sdk.IPlugin>();

        private Kipon.Fake.Xrm.Reflection.PluginMethod.Cache pluginMethodCache;

        #endregion

        #region static method for creating an instance
        public static PluginExecutionFakeContext ForType<T>(Guid? userId = null) where T: Microsoft.Xrm.Sdk.IPlugin
        {
            return new PluginExecutionFakeContext(typeof(T), userId);
        }

        public static PluginExecutionFakeContext ForType<T>(string unsecureConfig, string secureConfig, Guid? userId = null) where T : Microsoft.Xrm.Sdk.IPlugin
        {
            return new PluginExecutionFakeContext(typeof(T), userId, unsecureConfig, secureConfig);
        }
        #endregion


        #region constructors
        private PluginExecutionFakeContext(Type pluginType, Guid? userId)
        {
            this.userId = userId;
            this.pluginMethodCache = new Kipon.Fake.Xrm.Reflection.PluginMethod.Cache(pluginType.Assembly);

            this.orgServiceFactory = new Services.OrganizationServiceFactory(this);

            var pt = this.orgServiceFactory as Microsoft.Xrm.Sdk.IProxyTypesAssemblyProvider;
            if (pt != null)
            {
                pt.ProxyTypesAssembly = pluginType.Assembly;
            }

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
                Kipon.Fake.Xrm.Reflection.Types.Instance.SetAssembly(this.plugin.GetType().Assembly);
            }
        }

        private PluginExecutionFakeContext(Type pluginType, Guid? userId, string unsecureConfig, string secureConfig): this(pluginType, userId)
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
            Kipon.Fake.Xrm.Reflection.Types.Instance.SetAssembly(plugin.GetType().Assembly);
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

            var before = new EntityShadow(entities[key]);
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
            var before = new EntityShadow(entities[key]);
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
            transaction.Add(new TransactionElement { Operation = "Create", Entity = new EntityShadow(entity.LogicalName, entity.Id) });
            var et = new EntityShadow(entity);
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

        EntityShadow[] IEntityShadow.AllEntities()
        {
            return this.entities.Values.ToArray();
        }
        #endregion

        #region private helpers
        private Microsoft.Xrm.Sdk.Entity ResolveImage(string logicalName, Guid id, int pre1post2, Kipon.Fake.Xrm.Reflection.PluginMethod[] methods, EntityShadow pre)
        {
            if (pre1post2 == 1 && pre ==null)
            {
                throw new ArgumentException("For pre you must parse the pre entity state");
            }

            if (pre1post2 == 2 && pre != null)
            {
                throw new ArgumentException("For post you cannot parse the pre entity state");
            }

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

            var data = pre;

            if (pre1post2 == 2)
            {
                data = entities[key];
            }

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
            var et = new EntityShadow(crmEntity);
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

        #region public method for access data
        public T GetEntityById<T>(Guid id) where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            var p = new T();
            var key = p.LogicalName + id.ToString();
            if (!this.entities.ContainsKey(key))
            {
                throw new Exceptions.EntityNotFoundException(p.LogicalName, id);
            }
            return this.entities[key].ToEntity().ToEntity<T>();
        }

        public IQueryable<T> GetQuery<T>()
        {
            var type = typeof(T);
            if (this.queryCache.ContainsKey(type))
            {
                return (IQueryable<T>)this.queryCache[type];
            }

            if (this.orgService == null)
            {
                this.orgService = this.orgServiceFactory.CreateOrganizationService(this.userId);
            }

            if (this.uow == null)
            {
                var uowTypeCache = Kipon.Fake.Xrm.Reflection.TypeCache.ForUow(false);
                var pms = new object[1];
                pms[0] = this.orgService;
                this.uow = uowTypeCache.Constructor.Invoke(pms);
            }

            var queryType = typeof(IQueryable<>).GetGenericTypeDefinition().MakeGenericType(typeof(T));
            var queryTypeCache = Kipon.Fake.Xrm.Reflection.TypeCache.ForQuery(queryType);

            var repo = queryTypeCache.RepositoryProperty.GetValue(this.uow);
            var method = queryTypeCache.QueryMethod;
            this.queryCache[type] = method.Invoke(repo, new object[0]);


            var r = (IQueryable<T>)this.queryCache[type];
            return r;
        }
        #endregion

        #region delegates for unit tests
        public Action OnValidation;
        public Action OnPre;
        public Action OnPost;
        public Action OnPostAsync;
        #endregion

        #region execute the plugin code CRETE/UPDATE/DELETE
        public Guid Create(Microsoft.Xrm.Sdk.Entity target)
        {
            this.preImage = null;
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

                this.ExecuteStep(target, 10, "Create", false, null, this.OnValidation);
                this.ExecuteStep(target, 20, "Create", false, () =>
                {
                    ((Repository.IEntityShadow)this).Create(target);
                    ((Repository.IEntityShadow)this).Commit();
                }, this.OnPre);

                this.ExecuteStep(target, 40, "Create", false, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPost);

                this.ExecuteStep(target, 40, "Create", true, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPostAsync);

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

                this.ExecuteStep(target, 10, "Update", false, null, OnValidation);
                this.ExecuteStep(target, 20, "Update", false, () =>
                {
                    ((Repository.IEntityShadow)this).Update(target);
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPre);
                this.ExecuteStep(target, 40, "Update", false, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPost);
                this.ExecuteStep(target, 40, "Update", true, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPostAsync);
            }
            catch (Exception)
            {
                ((IEntityShadow)this).Rollback();
                throw;
            }
        }

        public void Delete(Microsoft.Xrm.Sdk.EntityReference target)
        {
            try
            {
                var key = target.LogicalName + target.Id.ToString();
                if (!entities.ContainsKey(key))
                {
                    throw new Exceptions.EntityNotFoundException(target.LogicalName, target.Id);
                }

                this.ExecuteStep(target, 10, "Delete", false, null, OnValidation);
                this.ExecuteStep(target, 20, "Delete", false, () =>
                {
                    ((Repository.IEntityShadow)this).Delete(target.LogicalName, target.Id);
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPre);
                this.ExecuteStep(target, 40, "Delete", false, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPost);
                this.ExecuteStep(target, 40, "Delete", true, () =>
                {
                    ((Repository.IEntityShadow)this).Commit();
                }, OnPostAsync);
            }
            catch (Exception)
            {
                ((IEntityShadow)this).Rollback();
                throw;
            }
        }
        #endregion

        #region special events
        public void RemoveMember(Guid listId, Guid entityId)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("ListId", listId);
            parameters.Add("EntityId", entityId);

            this.ExecuteStep(parameters, 20, "RemoveMember", "list", null, Guid.Empty, false, null, OnPre);
            this.ExecuteStep(parameters, 40, "RemoveMember", "list", null, Guid.Empty, false, null, OnPost);
            this.ExecuteStep(parameters, 40, "RemoveMember", "list", null, Guid.Empty, true, null, OnPost);
        }
        #endregion

        #region dispose
        void System.IDisposable.Dispose()
        {
            entities.Clear();
        }
        #endregion;

        #region private methods
        private EntityShadow preImage = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The tarket entity</param>
        /// <param name="stage">10=validate,20=pre,40=post</param>
        /// <param name="message">Create,Update,Delete,??</param>
        /// <param name="isAsync">false or true, only apply to stage 40</param>
        /// <param name="finalize">Will be executed end of step, regardless of any plugin execution</param>
        /// <param name="onDone">the valide method provided by the test library</param>
        private void ExecuteStep(Microsoft.Xrm.Sdk.Entity target, int stage, string message, bool isAsync, Action finalize, Action onDone)
        {
            if (message != "Create" && stage == 10)
            {
                var key = target.LogicalName + target.Id.ToString();
                this.preImage = this.entities[key].Clone();
            } 

            var methods = this.pluginMethodCache.ForPlugin(this.plugin.GetType(), stage, message, target.LogicalName, isAsync, false);
            if (methods.Length > 0)
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(stage, 1, message, target.LogicalName, target.Id, isAsync);
                pluginExecutionContext.InputParameters.Add("Target", target);

                if (message != "Create")
                {
                    var imagePre = this.ResolveImage(target.LogicalName, target.Id, 1, methods, preImage);
                    if (imagePre != null)
                    {
                        var imgName = Kipon.Fake.Xrm.Reflection.PluginMethod.ImageSuffixFor(1, stage, isAsync);
                        pluginExecutionContext.PreEntityImages.Add(imgName, imagePre);
                    }
                }

                if (stage == 40 && message != "Delete")
                {
                    var imagePost = this.ResolveImage(target.LogicalName, target.Id, 2, methods, null);
                    if (imagePost != null)
                    {
                        var imgName = Kipon.Fake.Xrm.Reflection.PluginMethod.ImageSuffixFor(2, stage, isAsync);
                        pluginExecutionContext.PostEntityImages.Add(imgName, imagePost);
                    }
                }

                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this, this.plugin.GetType().Assembly);
                this.plugin.Execute(serviceProvider);

                finalize?.Invoke();

                onDone?.Invoke();
            }
            else
            {
                if (onDone != null)
                {
                    throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), message, target.LogicalName, stage, isAsync);
                } else
                {
                    finalize?.Invoke();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The target reference</param>
        /// <param name="stage">10=validate,20=pre,40=post</param>
        /// <param name="message">Create,Update,Delete,??</param>
        /// <param name="isAsync">false or true, only apply to stage 40</param>
        /// <param name="finalize">Will be executed end of step, regardless of any plugin execution</param>
        /// <param name="onDone">the valide method provided by the test library</param>
        private void ExecuteStep(Microsoft.Xrm.Sdk.EntityReference target, int stage, string message, bool isAsync, Action finalize, Action onDone)
        {
            if (stage == 10)
            {
                var key = target.LogicalName + target.Id.ToString();
                this.preImage = this.entities[key].Clone();
            }

            var methods = this.pluginMethodCache.ForPlugin(this.plugin.GetType(), stage, message, target.LogicalName, isAsync, false);
            if (methods.Length > 0)
            {
                var pluginExecutionContext = new Services.PluginExecutionContext(stage, 1, message, target.LogicalName, target.Id, isAsync);
                pluginExecutionContext.InputParameters.Add("Target", target);

                var imagePre = this.ResolveImage(target.LogicalName, target.Id, 1, methods, preImage);
                if (imagePre != null)
                {
                    var imgName = Kipon.Fake.Xrm.Reflection.PluginMethod.ImageSuffixFor(1, stage, isAsync);
                    pluginExecutionContext.PreEntityImages.Add(imgName, imagePre);
                }

                if (stage == 40 && message != "Delete")
                {
                    var imagePost = this.ResolveImage(target.LogicalName, target.Id, 2, methods, null);
                    if (imagePost != null)
                    {
                        var imgName = Kipon.Fake.Xrm.Reflection.PluginMethod.ImageSuffixFor(2, stage, isAsync);
                        pluginExecutionContext.PostEntityImages.Add(imgName, imagePost);
                    }
                }

                var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this, this.plugin.GetType().Assembly);
                this.plugin.Execute(serviceProvider);

                finalize?.Invoke();

                onDone?.Invoke();
            }
            else
            {
                if (onDone != null)
                {
                    throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), message, target.LogicalName, stage, isAsync);
                }
                else
                {
                    finalize?.Invoke();
                }
            }
        }

        private void ExecuteStep(Dictionary<string, object> inputParameters, int stage, string message, string primaryentityname, string logicalName, Guid id, bool isAsync, Action finalize, Action onDone)
        {
            var methods = this.pluginMethodCache.ForPlugin(this.plugin.GetType(), stage, message, logicalName, isAsync, false);
            if (methods.Length > 0)
            {
                if (inputParameters != null && inputParameters.Count > 0)
                {
                    var pluginExecutionContext = new Services.PluginExecutionContext(stage, 1, message, primaryentityname, id, isAsync);
                    foreach (var key in inputParameters.Keys)
                    {
                        pluginExecutionContext.InputParameters.Add(key, inputParameters[key]);
                    }

                    var serviceProvider = new Services.ServiceProvider(pluginExecutionContext, this, this.plugin.GetType().Assembly);
                    this.plugin.Execute(serviceProvider);
                    finalize?.Invoke();
                    onDone?.Invoke();
                }
            }
            else
            {
                if (onDone != null)
                {
                    throw new Exceptions.UnexpectedEventListenerException(plugin.GetType(), message, primaryentityname, stage, isAsync);
                }
                else
                {
                    finalize?.Invoke();
                }
            }
        }

        #endregion

        #region internal classes
        internal class TransactionElement
        {
            internal string Operation { get; set; }
            internal EntityShadow Entity { get; set; }
        }
        #endregion
    }
}
