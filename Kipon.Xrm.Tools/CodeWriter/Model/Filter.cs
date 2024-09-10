using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kipon.Xrm.Tools.CodeWriter.Model
{
    public class Filter
    {
        public readonly Dictionary<string, Model.Entity> ENTITIES = new Dictionary<string, Model.Entity>();
        public readonly Dictionary<string, Model.OptionSet> GLOBAL_OPTIONSET_INDEX = new Dictionary<string, Model.OptionSet>();
        public readonly Dictionary<string, string> ATTRIBUTE_SCHEMANAME_MAP = new Dictionary<string, string>();
        public readonly List<Model.Action> ACTIONS = new List<Model.Action>();
        public readonly Dictionary<string, Kipon.Xrm.Tools.Models.Activity> ACTIVITIES = new Dictionary<string, Models.Activity>();

        public Dictionary<string, string> LOGICALNAME2SCHEMANAME = new Dictionary<string, string>();

        public bool SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES = false;
        public string ACTION_BACKUP_FILENAME = null;

        public Dictionary<string, List<string>> OPTIONSETFIELDS = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> MULTIOPTIONSETFIELDS = new Dictionary<string, List<string>>();

        public Dictionary<string, Model.Entity> _validEntities = new Dictionary<string, Model.Entity>();

        private void SetDefault()
        {
            ENTITIES.Clear();
            GLOBAL_OPTIONSET_INDEX.Clear();
            ATTRIBUTE_SCHEMANAME_MAP.Clear();
            ACTIONS.Clear();
            ACTIVITIES.Clear();
            LOGICALNAME2SCHEMANAME.Clear();
            SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES = false;
            ACTION_BACKUP_FILENAME = null;
            OPTIONSETFIELDS.Clear();
            MULTIOPTIONSETFIELDS.Clear();
            _validEntities.Clear();
        }

        public Filter()
        {

        }

        public Filter(string filename)
        {
            this.Parse(filename);
        }

        public void Parse(string filename)
        {
            this.SetDefault();

            XElement xml = XElement.Load(filename);

            var supress = xml.Attribute("supress-mapped-standard-optionset-properties");
            if (supress != null && supress.Value.ToLower() == "true")
            {
                this.SUPRESSMAPPEDSTANDARDOPTIONSETPROPERTIES = true;
            }

            var actionbackupfilename = xml.Attribute("action-backup-filename");
            if (actionbackupfilename != null && actionbackupfilename.Value != null)
            {
                this.ACTION_BACKUP_FILENAME = actionbackupfilename.Value;
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
                                        Value = int.Parse(optionsetValue.Value.Replace(".", ""))
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
                                Value = int.Parse(optionsetValue.Value.Replace(".", ""))
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
            }
            #endregion
        }
    }
}
