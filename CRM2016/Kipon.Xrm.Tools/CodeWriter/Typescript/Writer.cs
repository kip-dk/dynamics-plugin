using System;
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
            this.writer.WriteLine("/// <reference path=\"../../node_modules/@types/xrm/index.d.ts\" />");
            this.writer.WriteLine("");

            var entities = (from e in formList
                            group e by e.EntityLogicalName into grp
                            select new
                            {
                                Entity = grp.Key,
                                Forms = grp.ToArray()
                            }).ToArray();

            var startIX = 1;

            using (var ns = this.Namespace(formList.Namespace))
            {
                #region generic interfaces that are copies of the defenetlytyped interface omitting the part that is form specific
                using (var tabIf = ns.Interface("Tab"))
                {
                    tabIf.Stub("addTabStateChange(handler: Xrm.Events.ContextSensitiveHandler): void;");
                    tabIf.Stub("getDisplayState(): Xrm.DisplayState;");
                    tabIf.Stub("getName(): string;");
                    tabIf.Stub("getParent(): Ui;");
                    tabIf.Stub("removeTabStateChange(handler: Xrm.Events.ContextSensitiveHandler): void;");
                    tabIf.Stub("setDisplayState(displayState: Xrm.DisplayState): void;");
                }

                using (var UiIf = ns.Interface("Ui"))
                {
                    UiIf.Stub("setFormNotification(message: string, level: Xrm.FormNotificationLevel, uniqueId: string): boolean;");
                    UiIf.Stub("clearFormNotification(uniqueId: string): boolean;");
                    UiIf.Stub("close(): void;");
                    UiIf.Stub("getFormType(): XrmEnum.FormType;");
                    UiIf.Stub("getViewPortHeight(): number;");
                    UiIf.Stub("getViewPortWidth(): number;");
                    UiIf.Stub("refreshRibbon(refreshAll?: boolean): void;");
                    UiIf.Stub("process: Xrm.Controls.ProcessControl;");
                    UiIf.Stub("controls: Xrm.Collection.ItemCollection<Xrm.Controls.Control>;");
                    UiIf.Stub("formSelector: Xrm.Controls.FormSelector;");
                    UiIf.Stub("navigation: Xrm.Controls.Navigation;");
                    UiIf.Stub("quickForms: Xrm.Collection.ItemCollection<Xrm.Controls.QuickFormControl>;");
                }
                #endregion

                foreach (var entity in entities)
                {
                    using (var md = ns.Module(entity.Entity))
                    {
                        foreach (var form in entity.Forms)
                        {
                            var crmForm = systemformService.GetForm(entity.Entity, form.Title);

                            #region generate attributes, controls, tabs and tabs.sections for the form

                            #region first we generate the sections details
                            {
                                var tabIx = startIX;
                                foreach (var tab in crmForm.Tabs)
                                {
                                    using (var tabSectionsInterface = md.Interface($"{form.Name}FormTab_t{tabIx}Sections"))
                                    {
                                        foreach (var section in tab.Sections)
                                        {
                                            tabSectionsInterface.Stub($"get(name: '{section.Name}'): Xrm.Controls.Section;");
                                        }
                                        tabSectionsInterface.Stub($"getLength(): number;");
                                        tabSectionsInterface.Stub($"forEach(f: (c: Xrm.Controls.Section) => void);");
                                    }

                                    using (var tabInterface = md.Interface($"{form.Name}FormTab_t{tabIx}"))
                                    {
                                        tabInterface.Stub($"sections: {form.Name}FormTab_t{tabIx}Sections;");
                                    }
                                    tabIx++;
                                }
                            }
                            #endregion

                            #region then we generate the tabs
                            using (var formUiTabs = md.Interface($"{form.Name}FormTabs"))
                            {
                                var tabIx = startIX;
                                foreach (var tab in crmForm.Tabs)
                                {
                                    formUiTabs.Stub($"get(name: '{tab.Name}'): {form.Name}FormTab_t{tabIx} & Tab;");
                                    tabIx++;
                                }

                                formUiTabs.Stub("getLength(): number;");
                                formUiTabs.Stub("forEach(f: (c: Xrm.Controls.Tab) => void);");
                            }
                            #endregion

                            #region the we generate the Ui
                            using (var formUiInterface = md.Interface($"{form.Name}FormUi"))
                            {
                                formUiInterface.Stub($"tabs: {form.Name}FormTabs;");
                            }
                            #endregion

                            #region finally we generate the form it self.
                            using (var formInterface = md.Interface($"{form.Name}Form"))
                            {
                                foreach (var attr in crmForm.Attributes)
                                {
                                    formInterface.Stub($"getAttribute(name: '{ attr.LogicalName }'): {attr.TypescriptAttributeType};");
                                    foreach (var ctrl in attr.Controls)
                                    {
                                        formInterface.Stub($"getControl(name: '{ ctrl.Name }'): { attr.TypescriptControlType };");
                                    }
                                }
                                formInterface.Stub($"ui: {form.Name}FormUi & Ui;");
                                formInterface.Stub($"data: Xrm.Data;");
                            }
                            #endregion
                            #endregion
                        }
                    }
                }
            }
        }

        public Module Module(string name)
        {
            return new Module(0, this, name);
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
