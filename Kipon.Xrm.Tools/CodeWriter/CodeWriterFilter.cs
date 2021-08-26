using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

// To acknowledge the initial author, this sources comes from
// http://erikpool.blogspot.nl/2011/03/filtering-generated-entities-with.html


namespace Kipon.Xrm.Tools.CodeWriter
 {
    /// <summary>
    /// CodeWriterFilter for CrmSvcUtil that reads list of entities from an xml file to
    /// determine whether or not the entity class should be generated.
    /// </summary>
    public class CodeWriterFilter : ICodeWriterFilterService
    {
        public static readonly Dictionary<string, Model.Entity> ENTITIES = new Dictionary<string, Model.Entity>();
        public static readonly Dictionary<string, Model.OptionSet> GLOBAL_OPTIONSET_INDEX = new Dictionary<string, Model.OptionSet>();
        public static readonly Dictionary<string, string> ATTRIBUTE_SCHEMANAME_MAP = new Dictionary<string, string>();
        public static readonly List<Model.Action> ACTIONS = new List<Model.Action>();
        public static readonly Dictionary<string, Kipon.Xrm.Tools.Models.Activity> ACTIVITIES = new Dictionary<string, Models.Activity>();

        public static Dictionary<string, string> LOGICALNAME2SCHEMANAME = new Dictionary<string, string>();

        public static bool SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES = false;

        //list of entity names to generate classes for.
        private Dictionary<string, Model.Entity> _validEntities = new Dictionary<string, Model.Entity>();
        private bool initialized = false;

        //reference to the default service.
        private ICodeWriterFilterService _defaultService = null;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="defaultService">default implementation</param>
        public CodeWriterFilter(ICodeWriterFilterService defaultService)
        {
            if (System.Environment.GetCommandLineArgs()?.Where(r => r == "/debug").Any() == true)
            {
                Console.WriteLine("Please attach debugger to crmsvcutil.exe process");
                Console.WriteLine("Press [Enter] when ready");
                Console.ReadLine();
                Console.WriteLine("Continues...");
            }
            this._defaultService = defaultService;
        }

        /// <summary>
        /// loads the entity filter data from the filter.xml file
        /// </summary>
        private void LoadFilterData(IServiceProvider services)
        {
            if (this.initialized) return;

            var metaService = services.GetService(typeof(Microsoft.Crm.Services.Utility.IMetadataProviderService)) as Microsoft.Crm.Services.Utility.IMetadataProviderService;
            var meta = metaService.LoadMetadata();

            XElement xml = XElement.Load("filter.xml");

            var supress = xml.Attribute("supress-mapped-standard-optionset-properties");
            if (supress != null && supress.Value.ToLower() == "true")
            {
                SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES = true;
            }

            #region parse entity definitions
            {
                XElement entitiesElement = xml.Element("entities");

                var row = 0;
                foreach (XElement entityElement in entitiesElement.Elements("entity"))
                {
                    row++;
                    var uowName = entityElement.Attribute("servicename");
                    if (uowName == null)
                    {
                        throw new Exception($"No servicename on entity number {row}");
                    }

                    var logicalname = entityElement.Attribute("logicalname");
                    if (logicalname == null)
                    {
                        throw new Exception($"No logical name on entity number {row} : {uowName.Value}");
                    }

                    string primaryfield = null;
                    {
                        var pf = entityElement.Attribute("primaryfield");
                        if (pf != null)
                        {
                            primaryfield = pf.Value;
                        }
                    }

                    string primaryfieldvaluetemplate = null;
                    {
                        var pfv = entityElement.Attribute("primaryfieldvaluetemplate");
                        if (pfv != null)
                        {
                            primaryfieldvaluetemplate = pfv.Value;
                        }
                    }

                    List<Model.OptionSet> optionsets = new List<Model.OptionSet>();
                    if (optionsets != null)
                    {
                        foreach (XElement optionset in entityElement.Elements("optionset"))
                        {
                            var optionsetLogicalname = optionset.Attribute("logicalname");
                            var optionsetName = optionset.Attribute("name");
                            var optionsetId = optionset.Attribute("id");
                            var optionsetMulti = optionset.Attribute("multi");
                            var next = new Model.OptionSet
                            {
                                Id = optionsetId?.Value,
                                Name = optionsetName.Value,
                                Logicalname = optionsetLogicalname.Value,
                                Multi = optionsetMulti != null && optionsetMulti.Value.ToLower() == "true"
                            };

                            if (next.Id == null)
                            {
                                var values = new List<Model.OptionSetValue>();
                                foreach (XElement optionsetValue in optionset.Elements("value"))
                                {
                                    values.Add(new Model.OptionSetValue
                                    {
                                        Name = optionsetValue.Attribute("name").Value,
                                        Value = int.Parse(optionsetValue.Value)
                                    });
                                }
                                next.Values = values.ToArray();
                                if (next.Values.Length == 0)
                                {
                                    throw new Exception($"Local optionset on {logicalname.Value} {next.Name} does not define any values");
                                }
                            }
                            optionsets.Add(next);
                        }
                    }

                    var entity = new Model.Entity
                    {
                        LogicalName = logicalname.Value,
                        ServiceName = uowName.Value,
                        Primaryfield = primaryfield,
                        PrimaryfieldValuetemplate = primaryfieldvaluetemplate,
                        Optionsets = optionsets.ToArray()
                    };

                    _validEntities.Add(entity.LogicalName, entity);
                }
            }
            #endregion

            #region parse global optionsets
            {
                XElement optionsetsElement = xml.Element("optionsets");
                var row = 0;
                if (optionsetsElement != null)
                {
                    foreach (XElement optionset in optionsetsElement.Elements("optionset"))
                    {
                        var optionsetName = optionset.Attribute("name");
                        if (optionsetName == null)
                        {
                            throw new Exception($"Global optionset definition {row} does not have a name");
                        }
                        var optionsetId = optionset.Attribute("id");
                        if (optionsetId == null)
                        {
                            throw new Exception($"Global optionset definition {row} does not have an id");
                        }

                        if (GLOBAL_OPTIONSET_INDEX.ContainsKey(optionsetId.Value))
                        {
                            throw new Exception($"Global optionset definition {row} id is not unique");
                        }

                        var next = new Model.OptionSet
                        {
                            Id = optionsetId.Value,
                            Name = optionsetName.Value
                        };

                        var values = new List<Model.OptionSetValue>();
                        foreach (XElement optionsetValue in optionset.Elements("value"))
                        {
                            values.Add(new Model.OptionSetValue
                            {
                                Name = optionsetValue.Attribute("name").Value,
                                Value = int.Parse(optionsetValue.Value)
                            });
                        }
                        next.Values = values.ToArray();
                        if (next.Values.Length == 0)
                        {
                            throw new Exception($"Global optionset {row} does not define any values");
                        }
                        GLOBAL_OPTIONSET_INDEX.Add(next.Id, next);
                    }
                }
            }
            #endregion

            #region parse actions
            {
                XElement actionElements = xml.Element("actions");
                if (actionElements != null)
                {
                    foreach (XElement action in actionElements.Elements("action"))
                    {
                        var name = action.Attribute("name");
                        if (name == null || string.IsNullOrEmpty(name.Value))
                        {
                            throw new Exception("actions must have a name attribute");
                        }
                        var logicalName = action.Value;
                        if (string.IsNullOrEmpty(logicalName))
                        {
                            throw new Exception("action logical name must be set inside the action tag");
                        }
                        var nextaction = new Model.Action { Name = name.Value, LogicalName = logicalName };
                        ACTIONS.Add(nextaction);
                    }
                }

                if (ACTIONS.Count > 0)
                {

                    foreach (var action in ACTIONS)
                    {
                        var sdkMessage = meta.Messages.MessageCollection.Values.Where(r => r.Name == action.LogicalName).Single();

                        string entityLogicalName = null;
                        var isActivity = false;
                        if (sdkMessage.SdkMessageFilters != null && sdkMessage.SdkMessageFilters.Values.Count == 1)
                        {
                            var code = sdkMessage.SdkMessageFilters.Values.First().PrimaryObjectTypeCode;
                            if (code > 0)
                            {
                                // even unbound entities has a filter with the primparyobjecttypecode equals 0
                                var ent = meta.Entities.Where(r => r.ObjectTypeCode == code).Single();
                                entityLogicalName = ent.LogicalName;
                                isActivity = ent.IsActivity ?? false;
                            }
                        }

                        var na = new Kipon.Xrm.Tools.Models.Activity(sdkMessage, entityLogicalName, isActivity);
                        ACTIVITIES.Add(action.LogicalName, na);

                        if (!string.IsNullOrEmpty(entityLogicalName))
                        {
                            LOGICALNAME2SCHEMANAME[action.LogicalName] = entityLogicalName;
                        }
                    }
                }
            }
            #endregion

            initialized = true;
        }

        /// <summary>
        /// /Use filter entity list to determine if the entity class should be generated.
        /// </summary>
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            this.LoadFilterData(services);
            var generate = _validEntities.ContainsKey(entityMetadata.LogicalName.ToLowerInvariant());

            if (generate)
            {
                ENTITIES[entityMetadata.SchemaName] = _validEntities[entityMetadata.LogicalName.ToLowerInvariant()];
                LOGICALNAME2SCHEMANAME[entityMetadata.LogicalName.ToLowerInvariant()] = entityMetadata.SchemaName;
                return true;
            }

            var result =  ACTIVITIES.Values.Where(r => r.RequireEntity(entityMetadata.LogicalName.ToLowerInvariant())).Any();

            if (result)
            {
                LOGICALNAME2SCHEMANAME[entityMetadata.LogicalName.ToLowerInvariant()] = entityMetadata.SchemaName;
            }
            return result;
        }

        //All other methods just use default implementation:

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            this.LoadFilterData(services);
            if (!_validEntities.ContainsKey(attributeMetadata.EntityLogicalName.ToLowerInvariant()))
            {
                return _defaultService.GenerateAttribute(attributeMetadata, services);
            }

            if (SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES)
            {
                var entity = _validEntities[attributeMetadata.EntityLogicalName.ToLowerInvariant()];
                if (entity.Optionsets != null && entity.Optionsets.Length > 0)
                {
                    var me = (from op in entity.Optionsets
                              where op.Logicalname == attributeMetadata.LogicalName
                              select op).SingleOrDefault();
                    if (me != null)
                    {
                        if (attributeMetadata is Microsoft.Xrm.Sdk.Metadata.MultiSelectPicklistAttributeMetadata)
                        {
                            me.Multi = true;
                        } else
                        {
                            me.Multi = false;
                        }

                        ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                        return false;
                    } else
                    {
                        return _defaultService.GenerateAttribute(attributeMetadata, services);
                    }
                }
            }

            ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
            return _defaultService.GenerateAttribute(attributeMetadata, services);
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOptionSet(optionSetMetadata, services);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return _defaultService.GenerateServiceContext(services);
        }
    }
}