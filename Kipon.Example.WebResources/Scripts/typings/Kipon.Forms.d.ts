/// <reference path="../../node_modules/@types/xrm/index.d.ts" />

declare namespace Kipon.Forms {
    module account {
            interface AccountFormTab_t1 {
                get(name: "ACCOUNT_INFORMATION"): Xrm.Controls.Section;
                get(name: "ADDRESS"): Xrm.Controls.Section;
                get(name: "MapSection"): Xrm.Controls.Section;
                get(name: "SOCIAL_PANE_TAB"): Xrm.Controls.Section;
                get(name: "Summary_section_6"): Xrm.Controls.Section;
                get(name: "SUMMARY_TAB_section_6"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AccountFormTab_t2 {
                get(name: "COMPANY_PROFILE"): Xrm.Controls.Section;
                get(name: "DETAILS_TAB_section_6"): Xrm.Controls.Section;
                get(name: "MARKETING"): Xrm.Controls.Section;
                get(name: "CONTACT_PREFERENCES"): Xrm.Controls.Section;
                get(name: "BILLING"): Xrm.Controls.Section;
                get(name: "SHIPPING"): Xrm.Controls.Section;
                get(name: "ChildAccounts"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AccountFormTab_t3 {
                get(name: "tab_3_section_1"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AccountFormTab_t4 {
                get(name: "documents_sharepoint_section"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AccountFormTabs {
                get(name: "SUMMARY_TAB"): AccountFormTab_t1 & Xrm.Kipon.Tab;
                get(name: "DETAILS_TAB"): AccountFormTab_t2 & Xrm.Kipon.Tab;
                get(name: "tab_3"): AccountFormTab_t3 & Xrm.Kipon.Tab;
                get(name: "documents_sharepoint"): AccountFormTab_t4 & Xrm.Kipon.Tab;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Tab) => void);
            }

            interface AccountFormUi {
                tabs: AccountFormTabs;
            }

            interface AccountForm {
                getAttribute(name: "name"): Xrm.Attributes.StringAttribute;
                getControl(name: "name"): Xrm.Controls.StringControl;
                getAttribute(name: "telephone1"): Xrm.Attributes.StringAttribute;
                getControl(name: "telephone1"): Xrm.Controls.StringControl;
                getAttribute(name: "fax"): Xrm.Attributes.StringAttribute;
                getControl(name: "fax"): Xrm.Controls.StringControl;
                getAttribute(name: "websiteurl"): Xrm.Attributes.StringAttribute;
                getControl(name: "websiteurl"): Xrm.Controls.StringControl;
                getAttribute(name: "parentaccountid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "parentaccountid"): Xrm.Controls.LookupControl;
                getAttribute(name: "tickersymbol"): Xrm.Attributes.StringAttribute;
                getControl(name: "tickersymbol"): Xrm.Controls.StringControl;
                getAttribute(name: "lastonholdtime"): Xrm.Attributes.DateAttribute;
                getControl(name: "lastonholdtime"): Xrm.Controls.DateControl;
                getAttribute(name: "address1_composite"): Xrm.Attributes.StringAttribute;
                getControl(name: "address1_composite"): Xrm.Controls.StringControl;
                getAttribute(name: "primarycontactid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "primarycontactid"): Xrm.Controls.LookupControl;
                getAttribute(name: "industrycode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "industrycode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "sic"): Xrm.Attributes.StringAttribute;
                getControl(name: "sic"): Xrm.Controls.StringControl;
                getAttribute(name: "ownershipcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "ownershipcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "description"): Xrm.Attributes.StringAttribute;
                getControl(name: "description"): Xrm.Controls.StringControl;
                getAttribute(name: "originatingleadid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "originatingleadid"): Xrm.Controls.LookupControl;
                getAttribute(name: "lastusedincampaign"): Xrm.Attributes.DateAttribute;
                getControl(name: "lastusedincampaign"): Xrm.Controls.DateControl;
                getAttribute(name: "donotsendmm"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotsendmm"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "preferredcontactmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "followemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "followemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotbulkemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotbulkemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotphone"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotphone"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotfax"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotfax"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotpostalmail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotpostalmail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "transactioncurrencyid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "transactioncurrencyid"): Xrm.Controls.LookupControl;
                getAttribute(name: "creditlimit"): Xrm.Attributes.Attribute;
                getControl(name: "creditlimit"): Xrm.Controls.Control;
                getAttribute(name: "creditonhold"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "creditonhold"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "paymenttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "paymenttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_shippingmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_shippingmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_freighttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_freighttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "revenue"): Xrm.Attributes.Attribute;
                getControl(name: "header_revenue"): Xrm.Controls.Control;
                getAttribute(name: "numberofemployees"): Xrm.Attributes.NumberAttribute;
                getControl(name: "header_numberofemployees"): Xrm.Controls.NumberControl;
                getAttribute(name: "ownerid"): Xrm.Attributes.Attribute;
                getControl(name: "header_ownerid"): Xrm.Controls.Control;
                ui: AccountFormUi & Xrm.Kipon.Ui;
                data: Xrm.Data;
            }

            interface SalesInsightsFormTab_t1 {
                get(name: "ACCOUNT_INFORMATION"): Xrm.Controls.Section;
                get(name: "ADDRESS"): Xrm.Controls.Section;
                get(name: "MapSection"): Xrm.Controls.Section;
                get(name: "SOCIAL_PANE_TAB"): Xrm.Controls.Section;
                get(name: "Summary_section_6"): Xrm.Controls.Section;
                get(name: "SUMMARY_TAB_section_6"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface SalesInsightsFormTab_t2 {
                get(name: "COMPANY_PROFILE"): Xrm.Controls.Section;
                get(name: "DETAILS_TAB_section_6"): Xrm.Controls.Section;
                get(name: "MARKETING"): Xrm.Controls.Section;
                get(name: "CONTACT_PREFERENCES"): Xrm.Controls.Section;
                get(name: "BILLING"): Xrm.Controls.Section;
                get(name: "SHIPPING"): Xrm.Controls.Section;
                get(name: "ChildAccounts"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface SalesInsightsFormTabs {
                get(name: "SUMMARY_TAB"): SalesInsightsFormTab_t1 & Xrm.Kipon.Tab;
                get(name: "DETAILS_TAB"): SalesInsightsFormTab_t2 & Xrm.Kipon.Tab;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Tab) => void);
            }

            interface SalesInsightsFormUi {
                tabs: SalesInsightsFormTabs;
            }

            interface SalesInsightsForm {
                getAttribute(name: "name"): Xrm.Attributes.StringAttribute;
                getControl(name: "name"): Xrm.Controls.StringControl;
                getAttribute(name: "telephone1"): Xrm.Attributes.StringAttribute;
                getControl(name: "telephone1"): Xrm.Controls.StringControl;
                getAttribute(name: "fax"): Xrm.Attributes.StringAttribute;
                getControl(name: "fax"): Xrm.Controls.StringControl;
                getAttribute(name: "websiteurl"): Xrm.Attributes.StringAttribute;
                getControl(name: "websiteurl"): Xrm.Controls.StringControl;
                getAttribute(name: "parentaccountid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "parentaccountid"): Xrm.Controls.LookupControl;
                getAttribute(name: "tickersymbol"): Xrm.Attributes.StringAttribute;
                getControl(name: "tickersymbol"): Xrm.Controls.StringControl;
                getAttribute(name: "address1_composite"): Xrm.Attributes.StringAttribute;
                getControl(name: "address1_composite"): Xrm.Controls.StringControl;
                getAttribute(name: "primarycontactid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "primarycontactid"): Xrm.Controls.LookupControl;
                getAttribute(name: "industrycode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "industrycode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "sic"): Xrm.Attributes.StringAttribute;
                getControl(name: "sic"): Xrm.Controls.StringControl;
                getAttribute(name: "ownershipcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "ownershipcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "description"): Xrm.Attributes.StringAttribute;
                getControl(name: "description"): Xrm.Controls.StringControl;
                getAttribute(name: "originatingleadid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "originatingleadid"): Xrm.Controls.LookupControl;
                getAttribute(name: "lastusedincampaign"): Xrm.Attributes.DateAttribute;
                getControl(name: "lastusedincampaign"): Xrm.Controls.DateControl;
                getAttribute(name: "donotsendmm"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotsendmm"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "preferredcontactmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "followemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "followemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotbulkemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotbulkemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotphone"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotphone"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotfax"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotfax"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotpostalmail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotpostalmail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "transactioncurrencyid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "transactioncurrencyid"): Xrm.Controls.LookupControl;
                getAttribute(name: "creditlimit"): Xrm.Attributes.Attribute;
                getControl(name: "creditlimit"): Xrm.Controls.Control;
                getAttribute(name: "creditonhold"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "creditonhold"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "paymenttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "paymenttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_shippingmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_shippingmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_freighttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_freighttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "revenue"): Xrm.Attributes.Attribute;
                getControl(name: "header_revenue"): Xrm.Controls.Control;
                getAttribute(name: "numberofemployees"): Xrm.Attributes.NumberAttribute;
                getControl(name: "header_numberofemployees"): Xrm.Controls.NumberControl;
                getAttribute(name: "ownerid"): Xrm.Attributes.Attribute;
                getControl(name: "header_ownerid"): Xrm.Controls.Control;
                ui: SalesInsightsFormUi & Xrm.Kipon.Ui;
                data: Xrm.Data;
            }

    }

    module contact {
            interface ContactFormTab_t1 {
                get(name: "CONTACT_INFORMATION"): Xrm.Controls.Section;
                get(name: "MapSection"): Xrm.Controls.Section;
                get(name: "BusinessCard"): Xrm.Controls.Section;
                get(name: "SOCIAL_PANE_TAB"): Xrm.Controls.Section;
                get(name: "TalkingPoints_section"): Xrm.Controls.Section;
                get(name: "Summary_section_6"): Xrm.Controls.Section;
                get(name: "CUSTOMER_DETAILS_TAB"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface ContactFormTab_t2 {
                get(name: "PERSONAL INFORMATION"): Xrm.Controls.Section;
                get(name: "PERSONAL_NOTES_SECTION"): Xrm.Controls.Section;
                get(name: "marketing information"): Xrm.Controls.Section;
                get(name: "CONTACT_PREFERENCES"): Xrm.Controls.Section;
                get(name: "billing information"): Xrm.Controls.Section;
                get(name: "shipping information"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface ContactFormTab_t3 {
                get(name: "documents_sharepoint_section"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface ContactFormTabs {
                get(name: "SUMMARY_TAB"): ContactFormTab_t1 & Xrm.Kipon.Tab;
                get(name: "DETAILS_TAB"): ContactFormTab_t2 & Xrm.Kipon.Tab;
                get(name: "documents_sharepoint"): ContactFormTab_t3 & Xrm.Kipon.Tab;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Tab) => void);
            }

            interface ContactFormUi {
                tabs: ContactFormTabs;
            }

            interface ContactForm {
                getAttribute(name: "fullname"): Xrm.Attributes.StringAttribute;
                getControl(name: "fullname"): Xrm.Controls.StringControl;
                getAttribute(name: "jobtitle"): Xrm.Attributes.StringAttribute;
                getControl(name: "jobtitle"): Xrm.Controls.StringControl;
                getAttribute(name: "parentcustomerid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "parentcustomerid"): Xrm.Controls.LookupControl;
                getControl(name: "parentcustomerid"): Xrm.Controls.LookupControl;
                getAttribute(name: "emailaddress1"): Xrm.Attributes.StringAttribute;
                getControl(name: "emailaddress1"): Xrm.Controls.StringControl;
                getAttribute(name: "telephone1"): Xrm.Attributes.StringAttribute;
                getControl(name: "telephone1"): Xrm.Controls.StringControl;
                getAttribute(name: "mobilephone"): Xrm.Attributes.StringAttribute;
                getControl(name: "mobilephone"): Xrm.Controls.StringControl;
                getAttribute(name: "fax"): Xrm.Attributes.StringAttribute;
                getControl(name: "fax"): Xrm.Controls.StringControl;
                getAttribute(name: "preferredcontactmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_composite"): Xrm.Attributes.StringAttribute;
                getControl(name: "address1_composite"): Xrm.Controls.StringControl;
                getAttribute(name: "businesscard"): Xrm.Attributes.StringAttribute;
                getControl(name: "businesscard"): Xrm.Controls.StringControl;
                getAttribute(name: "gendercode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "gendercode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "familystatuscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "familystatuscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "spousesname"): Xrm.Attributes.StringAttribute;
                getControl(name: "spousesname"): Xrm.Controls.StringControl;
                getAttribute(name: "birthdate"): Xrm.Attributes.DateAttribute;
                getControl(name: "birthdate"): Xrm.Controls.DateControl;
                getAttribute(name: "anniversary"): Xrm.Attributes.DateAttribute;
                getControl(name: "anniversary"): Xrm.Controls.DateControl;
                getAttribute(name: "description"): Xrm.Attributes.StringAttribute;
                getControl(name: "description"): Xrm.Controls.StringControl;
                getAttribute(name: "originatingleadid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "originatingleadid"): Xrm.Controls.LookupControl;
                getAttribute(name: "lastusedincampaign"): Xrm.Attributes.DateAttribute;
                getControl(name: "lastusedincampaign"): Xrm.Controls.DateControl;
                getAttribute(name: "donotsendmm"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotsendmm"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "followemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "followemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotbulkemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotbulkemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotphone"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotphone"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotfax"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotfax"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotpostalmail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotpostalmail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "transactioncurrencyid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "transactioncurrencyid"): Xrm.Controls.LookupControl;
                getAttribute(name: "creditlimit"): Xrm.Attributes.Attribute;
                getControl(name: "creditlimit"): Xrm.Controls.Control;
                getAttribute(name: "creditonhold"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "creditonhold"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "paymenttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "paymenttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_shippingmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_shippingmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_freighttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_freighttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "ownerid"): Xrm.Attributes.Attribute;
                getControl(name: "header_ownerid"): Xrm.Controls.Control;
                ui: ContactFormUi & Xrm.Kipon.Ui;
                data: Xrm.Data;
            }

            interface AIforSalesFormTab_t1 {
                get(name: "CONTACT_INFORMATION"): Xrm.Controls.Section;
                get(name: "MapSection"): Xrm.Controls.Section;
                get(name: "SOCIAL_PANE_TAB"): Xrm.Controls.Section;
                get(name: "TalkingPoints_section"): Xrm.Controls.Section;
                get(name: "Summary_section_6"): Xrm.Controls.Section;
                get(name: "CUSTOMER_DETAILS_TAB"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AIforSalesFormTab_t2 {
                get(name: "PERSONAL INFORMATION"): Xrm.Controls.Section;
                get(name: "PERSONAL_NOTES_SECTION"): Xrm.Controls.Section;
                get(name: "marketing information"): Xrm.Controls.Section;
                get(name: "CONTACT_PREFERENCES"): Xrm.Controls.Section;
                get(name: "billing information"): Xrm.Controls.Section;
                get(name: "shipping information"): Xrm.Controls.Section;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Section) => void);
            }

            interface AIforSalesFormTabs {
                get(name: "SUMMARY_TAB"): AIforSalesFormTab_t1 & Xrm.Kipon.Tab;
                get(name: "DETAILS_TAB"): AIforSalesFormTab_t2 & Xrm.Kipon.Tab;
                getLength(): number;
                forEach(f: (c: Xrm.Controls.Tab) => void);
            }

            interface AIforSalesFormUi {
                tabs: AIforSalesFormTabs;
            }

            interface AIforSalesForm {
                getAttribute(name: "fullname"): Xrm.Attributes.StringAttribute;
                getControl(name: "fullname"): Xrm.Controls.StringControl;
                getAttribute(name: "jobtitle"): Xrm.Attributes.StringAttribute;
                getControl(name: "jobtitle"): Xrm.Controls.StringControl;
                getAttribute(name: "parentcustomerid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "parentcustomerid"): Xrm.Controls.LookupControl;
                getControl(name: "parentcustomerid"): Xrm.Controls.LookupControl;
                getAttribute(name: "emailaddress1"): Xrm.Attributes.StringAttribute;
                getControl(name: "emailaddress1"): Xrm.Controls.StringControl;
                getAttribute(name: "telephone1"): Xrm.Attributes.StringAttribute;
                getControl(name: "telephone1"): Xrm.Controls.StringControl;
                getAttribute(name: "mobilephone"): Xrm.Attributes.StringAttribute;
                getControl(name: "mobilephone"): Xrm.Controls.StringControl;
                getAttribute(name: "fax"): Xrm.Attributes.StringAttribute;
                getControl(name: "fax"): Xrm.Controls.StringControl;
                getAttribute(name: "preferredcontactmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getControl(name: "preferredcontactmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_composite"): Xrm.Attributes.StringAttribute;
                getControl(name: "address1_composite"): Xrm.Controls.StringControl;
                getAttribute(name: "gendercode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "gendercode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "familystatuscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "familystatuscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "spousesname"): Xrm.Attributes.StringAttribute;
                getControl(name: "spousesname"): Xrm.Controls.StringControl;
                getAttribute(name: "birthdate"): Xrm.Attributes.DateAttribute;
                getControl(name: "birthdate"): Xrm.Controls.DateControl;
                getAttribute(name: "anniversary"): Xrm.Attributes.DateAttribute;
                getControl(name: "anniversary"): Xrm.Controls.DateControl;
                getAttribute(name: "description"): Xrm.Attributes.StringAttribute;
                getControl(name: "description"): Xrm.Controls.StringControl;
                getAttribute(name: "originatingleadid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "originatingleadid"): Xrm.Controls.LookupControl;
                getAttribute(name: "lastusedincampaign"): Xrm.Attributes.DateAttribute;
                getControl(name: "lastusedincampaign"): Xrm.Controls.DateControl;
                getAttribute(name: "donotsendmm"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotsendmm"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "followemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "followemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotbulkemail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotbulkemail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotphone"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotphone"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotfax"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotfax"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "donotpostalmail"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "donotpostalmail"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "transactioncurrencyid"): Xrm.Attributes.LookupAttribute;
                getControl(name: "transactioncurrencyid"): Xrm.Controls.LookupControl;
                getAttribute(name: "creditlimit"): Xrm.Attributes.Attribute;
                getControl(name: "creditlimit"): Xrm.Controls.Control;
                getAttribute(name: "creditonhold"): Xrm.Attributes.BooleanAttribute;
                getControl(name: "creditonhold"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "paymenttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "paymenttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_shippingmethodcode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_shippingmethodcode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "address1_freighttermscode"): Xrm.Attributes.OptionSetAttribute;
                getControl(name: "address1_freighttermscode"): Xrm.Controls.OptionSetControl;
                getAttribute(name: "ownerid"): Xrm.Attributes.Attribute;
                getControl(name: "header_ownerid"): Xrm.Controls.Control;
                ui: AIforSalesFormUi & Xrm.Kipon.Ui;
                data: Xrm.Data;
            }

    }

}

