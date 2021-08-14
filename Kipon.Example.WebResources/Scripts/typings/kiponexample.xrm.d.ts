/// <reference path="../../node_modules/@types/xrm/index.d.ts" />

declare namespace KiponExample.Forms {
    interface Tab extends Xrm.Controls.UiStandardElement, Xrm.Controls.UiFocusable {
        addTabStateChange(handler: Xrm.Events.ContextSensitiveHandler): void;
        getDisplayState(): Xrm.DisplayState;
        getName(): string;
        getParent(): Ui;
        removeTabStateChange(handler: Xrm.Events.ContextSensitiveHandler): void;
        setDisplayState(displayState: Xrm.DisplayState): void;
    }

    interface Ui {
        setFormNotification(message: string, level: Xrm.FormNotificationLevel, uniqueId: string): boolean;
        clearFormNotification(uniqueId: string): boolean;
        close(): void;
        getFormType(): XrmEnum.FormType;
        getViewPortHeight(): number;
        getViewPortWidth(): number;
        refreshRibbon(refreshAll?: boolean): void;
        process: Xrm.Controls.ProcessControl;
        controls: Xrm.Collection.ItemCollection<Xrm.Controls.Control>;
        formSelector: Xrm.Controls.FormSelector;
        navigation: Xrm.Controls.Navigation;
        quickForms: Xrm.Collection.ItemCollection<Xrm.Controls.QuickFormControl>;
    }

    module kipon_invoice {

        interface InformationForm {
            getAttribute(name: 'xx'): Xrm.Attributes.StringAttribute;
            getControl(name: "xx"): Xrm.Controls.StringControl;

            getAttribute(name: 'no'): Xrm.Attributes.NumberAttribute;
            getControl(name: "no"): Xrm.Controls.NumberControl;

            getAttribute(name: 'yesno'): Xrm.Attributes.BooleanAttribute;
            getControl(name: "yesno"): Xrm.Controls.OptionSetControl;

            getAttribute(name: 'cust'): Xrm.Attributes.LookupAttribute;
            getControl(name: "cust"): Xrm.Controls.LookupControl;

            getAttribute(name: 'dt'): Xrm.Attributes.DateAttribute;
            getControl(name: "dt"): Xrm.Controls.DateControl;

            getAttribute(name: 'dc'): Xrm.Attributes.DateAttribute;
            getControl(name: "dc"): Xrm.Controls.DateControl;

            getAttribute(name: 'tx'): Xrm.Attributes.StringAttribute;
            getControl(name: "tx"): Xrm.Controls.StringControl;


            getAttribute(name: "a1"): Xrm.Attributes.StringAttribute;
            getControl(name: "a1"): Xrm.Controls.StringControl;

            getAttribute(name: "a1c"): Xrm.Attributes.Attribute;
            getControl(name: "a1c"): Xrm.Controls.OptionSetControl;

            ui: InformationFormUi & Ui;
            data: Xrm.Data;
        }

        interface InformationFormUi {
            tabs: InformationFormTabs;
        }

        interface InformationFormTabs {
            get(name: "tab_t1"): InformationFormTab_t1 & Tab;
            get(name: "tab_t2"): InformationFormTab_t2 & Tab;
            getLength(): number;
            forEach(f: (c: Xrm.Controls.Tab) => void);
        }


        interface InformationFormTab_t1 {
            sections: InformationFormTab_t1Sections;
        }

        interface InformationFormTab_t1Sections {
            get(name: "sec_1_1"): Xrm.Controls.Section;
            get(name: "sec_1_2"): Xrm.Controls.Section;
            getLength(): number;
            forEach(f: (c: Xrm.Controls.Section) => void);
        }


        interface InformationFormTab_t2 {
            sections: InformationFormTab_t2Sections;
        }

        interface InformationFormTab_t2Sections {
            get(name: "sec_2_1"): Xrm.Controls.Section;
            get(name: "sec_2_2"): Xrm.Controls.Section;
            getLength(): number;
            forEach(f: (c: Xrm.Controls.Section) => void);
        }
    }
}