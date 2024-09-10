using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System.Web.Caching;

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
        public static Model.Filter FILTER;
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

            FILTER = new Model.Filter("filter.xml");

            #region parse actions
            {
                if (FILTER.ACTIONS.Count > 0)
                {
                    var hasMissing = false;


                    var messageCollection = meta.Messages.MessageCollection.Values;

                    var result = new List<Models.SdkMessageWrapper>();
                    foreach (var m in messageCollection)
                    {
                        var next = new Models.SdkMessageWrapper
                        {
                            Code = (m.SdkMessageFilters != null && m.SdkMessageFilters?.Values?.Count == 1) ? m.SdkMessageFilters.Values.First().PrimaryObjectTypeCode : 0,
                            Name = m.Name
                        };

                        if (m.SdkMessagePairs != null
                            && m.SdkMessagePairs.Values != null
                            && m.SdkMessagePairs.Values.Count > 0
                            && m.SdkMessagePairs.Values.First().Request != null
                            && m.SdkMessagePairs.Values.First().Request.RequestFields != null
                            && m.SdkMessagePairs.Values.First().Request.RequestFields.Count > 0)
                        {
                            next.InputFields = (from input in m.SdkMessagePairs.Values.FirstOrDefault().Request.RequestFields.Values
                                                select new Models.SdkMessageWrapper.Field
                                                {
                                                    Name = input.Name,
                                                    CLRFormatter = input.CLRFormatter,
                                                    IsOptional = input.IsOptional
                                                }).OrderBy(r => r.Name).ToArray();
                        }

                        if (m.SdkMessagePairs != null
                            && m.SdkMessagePairs.Values != null
                            && m.SdkMessagePairs.Values.Count > 0
                            && m.SdkMessagePairs.Values.First().Response != null
                            && m.SdkMessagePairs.Values.First().Response.ResponseFields != null
                            && m.SdkMessagePairs.Values.First().Response.ResponseFields.Values != null
                            && m.SdkMessagePairs.Values.First().Response.ResponseFields.Values.Count > 0)
                        {

                            next.OutputFields = (from input in m.SdkMessagePairs.Values.FirstOrDefault().Response.ResponseFields.Values
                                                 select new Models.SdkMessageWrapper.Field
                                                 {
                                                     Name = input.Name,
                                                     CLRFormatter = input.CLRFormatter,
                                                     IsOptional = false
                                                 }).OrderBy(r => r.Name).ToArray();
                        }

                        result.Add(next);
                    }

                    var allMessage = result.ToArray();

                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(allMessage.GetType());
                    var toFile = allMessage.Where(r => FILTER.ACTIONS.Where(a => a.LogicalName == r.Name).Any()).ToArray();
                    using (var fs = new System.IO.FileStream(@"C:\Temp\kipon-plugin-message-example.json", System.IO.FileMode.Create))
                    {
                        ser.WriteObject(fs, toFile);
                    }

                    #region backup due to bug in crmsvcutil on some online environments
                    Models.SdkMessageWrapper[] fileBackup = null;
                    Models.SdkMessageWrapper GetFromFile(string logicalName)
                    {
                        if (fileBackup == null)
                        {

                            if (string.IsNullOrEmpty(FILTER.ACTION_BACKUP_FILENAME))
                            {
                                fileBackup = new Models.SdkMessageWrapper[0];
                                return null;
                            }

                            var wrapperSer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Models.SdkMessageWrapper[]));
                            using (var wrapperFile = new System.IO.FileStream(FILTER.ACTION_BACKUP_FILENAME, System.IO.FileMode.Open))
                            {
                                fileBackup = (Models.SdkMessageWrapper[])wrapperSer.ReadObject(wrapperFile);
                            }
                        }
                        return fileBackup.Where(r => r.Name == logicalName).SingleOrDefault();
                    }
                    #endregion

                    foreach (var action in FILTER.ACTIONS)
                    {
                        var sdkMessage = allMessage.Where(r => r.Name == action.LogicalName).SingleOrDefault();

                        if (sdkMessage == null)
                        {
                            sdkMessage = GetFromFile(action.LogicalName);
                        }

                        if (sdkMessage == null)
                        {
                            Console.WriteLine($"Missing: { action.LogicalName }");
                            hasMissing = true;
                            continue;
                        }

                        string entityLogicalName = null;
                        var isActivity = false;
                        if (sdkMessage.Code > 0)
                        {
                            // even unbound entities has a filter with the primparyobjecttypecode equals 0
                            var ent = meta.Entities.Where(r => r.ObjectTypeCode == sdkMessage.Code).SingleOrDefault();

                            if (ent == null)
                            {
                                throw new Exception($"No entity with objecttypecode: [{sdkMessage.Code}] was found.");
                            }

                            entityLogicalName = ent.LogicalName;
                            isActivity = ent.IsActivity ?? false;
                        }

                        if (Kipon.Xrm.Tools.Models.Activity.CUSTOMIZED.Contains(sdkMessage.Name))
                        {
                            var na = new Kipon.Xrm.Tools.Models.Activity(sdkMessage.Name);
                            FILTER.ACTIVITIES.Add(action.LogicalName, na);
                        }
                        else
                        {
                            var na = new Kipon.Xrm.Tools.Models.Activity(sdkMessage, entityLogicalName, isActivity);
                            FILTER.ACTIVITIES.Add(action.LogicalName, na);
                        }

                        if (!string.IsNullOrEmpty(entityLogicalName))
                        {
                            FILTER.LOGICALNAME2SCHEMANAME[action.LogicalName] = entityLogicalName;
                        }
                    }

                    if (hasMissing)
                    {
                        Console.WriteLine("******************************************************************************************************************************************************************************************");
                        Console.WriteLine("Some actions are not returned from crmsvcutil Metadata service. If you know for sure, that your registred actions do endeed exists in the environment, you can use the export-sdk-messages");
                        Console.WriteLine("tool to generate a json file that can be used as backup to enable this tool to generate the interfaces anyway.");
                        Console.WriteLine("Please look into the changelog on the GIT repository on version 1.0.10.24 for a detailed description on how this works.");
                        Console.WriteLine("******************************************************************************************************************************************************************************************");

                        throw new Exception("At least one action is missing in SdkMessage, se above for full list.");
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
            var generate = FILTER._validEntities.ContainsKey(entityMetadata.LogicalName.ToLowerInvariant());

            if (generate)
            {
                FILTER.ENTITIES[entityMetadata.SchemaName] = FILTER._validEntities[entityMetadata.LogicalName.ToLowerInvariant()];
                FILTER.LOGICALNAME2SCHEMANAME[entityMetadata.LogicalName.ToLowerInvariant()] = entityMetadata.SchemaName;
                return true;
            }

            var result = FILTER.ACTIVITIES.Values.Where(r => r.RequireEntity(entityMetadata.LogicalName.ToLowerInvariant())).Any();

            if (result)
            {
                FILTER.LOGICALNAME2SCHEMANAME[entityMetadata.LogicalName.ToLowerInvariant()] = entityMetadata.SchemaName;
            }
            return result;
        }

        //All other methods just use default implementation:

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            this.LoadFilterData(services);
            if (!FILTER._validEntities.ContainsKey(attributeMetadata.EntityLogicalName.ToLowerInvariant()))
            {
                return _defaultService.GenerateAttribute(attributeMetadata, services);
            }

            if (FILTER.SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES)
            {
                var entity = FILTER._validEntities[attributeMetadata.EntityLogicalName.ToLowerInvariant()];
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

                        FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                        return false;
                    } 
                }
            }

            if (attributeMetadata.LogicalName.ToLower() == "statecode")
            {
                FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                return false;
            }

            if (attributeMetadata.LogicalName.ToLower() == "statuscode")
            {
                FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                AddOptionSetField(attributeMetadata.EntityLogicalName.ToLower(), attributeMetadata.SchemaName);
                return false;
            }


            if (attributeMetadata is Microsoft.Xrm.Sdk.Metadata.MultiSelectPicklistAttributeMetadata)
            {
                FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                AddMultiOptionSetField(attributeMetadata.EntityLogicalName.ToLower(), attributeMetadata.SchemaName);
                return false;
            }

            if (attributeMetadata is Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)
            {

                FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
                AddOptionSetField(attributeMetadata.EntityLogicalName.ToLower(), attributeMetadata.SchemaName);
                return false;
            }

            FILTER.ATTRIBUTE_SCHEMANAME_MAP.Add($"{attributeMetadata.EntityLogicalName}.{attributeMetadata.LogicalName}", attributeMetadata.SchemaName);
            return _defaultService.GenerateAttribute(attributeMetadata, services);
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return false;
            // return _defaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return false;
            // return _defaultService.GenerateOptionSet(optionSetMetadata, services);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return _defaultService.GenerateServiceContext(services);
        }

        private static void AddOptionSetField(string entitylogicalname, string attrlogicalname)
        {
            if (FILTER.OPTIONSETFIELDS.TryGetValue(entitylogicalname, out List<string> fields))
            {
                fields.Add(attrlogicalname);
            }
            else
            {
                var list = new List<string>();
                list.Add(attrlogicalname);
                FILTER.OPTIONSETFIELDS.Add(entitylogicalname, list);
            }
        }

        private static void AddMultiOptionSetField(string entitylogicalname, string attrlogicalname)
        {
            if (FILTER.MULTIOPTIONSETFIELDS.TryGetValue(entitylogicalname, out List<string> fields))
            {
                fields.Add(attrlogicalname);
            }
            else
            {
                var list = new List<string>();
                list.Add(attrlogicalname);
                FILTER.MULTIOPTIONSETFIELDS.Add(entitylogicalname, list);
            }
        }
    }
}