﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Typescript
{
    public class Writer : IDisposable
    {
        internal readonly System.IO.StreamWriter writer;

        public Writer(System.IO.Stream str)
        {
            this.writer = new System.IO.StreamWriter(str);
        }


        public void Write(Configs.FormList formList, ServiceAPI.ISystemFormService systemformService)
        {
            var entities = (from e in formList
                            group e by e.EntityLogicalName into grp
                            select new
                            {
                                Entity = grp.Key,
                                Forms = grp.ToArray()
                            }).ToArray();

            using (var ns = this.Namespace(formList.Namespace))
            {
                foreach (var entity in entities)
                {
                    using (var md = ns.Module(entity.Entity))
                    {
                        foreach (var form in entity.Forms)
                        {
                            var crmForm = systemformService.GetForm(entity.Entity, form.Title);

                            #region generate attributes, controls, tabs and tabs.sections for the form
                            using (var formInterface = md.Interface($"{form.Name}Form"))
                            {
                                foreach (var attr in crmForm.Attributes)
                                {
                                    formInterface.Stub($"getAttribute(name: '{ attr.LogicalName }'): {attr.TypescriptAttributeType};");
                                    foreach (var ctrl in attr.Controls)
                                    {
                                        formInterface.Stub($"getControl(name: '{ ctrl }'): { attr.TypescriptControlType };");
                                    }
                                }
                            }

                            using (var formUiInterface = md.Interface($"{form.Name}FormUi"))
                            {
                                formUiInterface.Stub($"tabs: {form.Name}FormTabs");
                            }

                            var startIX = 1;
                            using (var formUiTabs = md.Interface($"{form.Name}FormTabs"))
                            {
                                var tabIx = startIX;
                                foreach (var tab in crmForm.Tabs)
                                {
                                    formUiTabs.Stub($"get(name: '{tab.Name}'): {form.Name}FormTab_t{tabIx} & Xrm.Kipon.Tab;");
                                    tabIx++;
                                }

                                formUiTabs.Stub("getLength(): number;");
                                formUiTabs.Stub("forEach(f: (c: Xrm.Controls.Tab) => void);");
                            }

                            {
                                var tabIx = startIX;
                                foreach (var tab in crmForm.Tabs)
                                {
                                    using (var tabInterface = md.Interface($"{form.Name}FormTab_t{tabIx}"))
                                    {
                                        foreach (var section in tab.Sections)
                                        {
                                            tabInterface.Stub($"get(name: '{section.Name}'): Xrm.Controls.Section;");
                                        }
                                        tabInterface.Stub($"getLength(): number;");
                                        tabInterface.Stub($"forEach(f: (c: Xrm.Controls.Section) => void);");
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
        }

        public Module Module(string name)
        {
            return new Module(this, name);
        }

        public Namespace Namespace(string name)
        {
            return new Namespace(this, name);
        }

        public void Dispose()
        {
            writer.Flush();
            writer.Close();
        }
    }
}
