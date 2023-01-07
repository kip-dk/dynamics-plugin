// Tools Version 2.0.0.10 Dynamics 365 svcutil extension tool by Kipon ApS, Kjeld Ingemann Poulsen
// This file is autogenerated. Do not touch the code manually

namespace Kipon.Xrm.Tools.Entities
{
	public partial class CrmUnitOfWork
	{
		private IRepository<PluginAssembly> _pluginassemblies; 
		public IRepository<PluginAssembly> PluginAssemblies
		{
			get
			{
				if (_pluginassemblies == null)
					{
						_pluginassemblies = new CrmRepository<PluginAssembly>(this.context);
					}
				return _pluginassemblies;
			}
		}
		private IRepository<PluginType> _plugintypes; 
		public IRepository<PluginType> PluginTypes
		{
			get
			{
				if (_plugintypes == null)
					{
						_plugintypes = new CrmRepository<PluginType>(this.context);
					}
				return _plugintypes;
			}
		}
		private IRepository<PluginPackage> _pluginpackages; 
		public IRepository<PluginPackage> PluginPackages
		{
			get
			{
				if (_pluginpackages == null)
					{
						_pluginpackages = new CrmRepository<PluginPackage>(this.context);
					}
				return _pluginpackages;
			}
		}
		private IRepository<SdkMessageProcessingStep> _sdkmessageprocessingsteps; 
		public IRepository<SdkMessageProcessingStep> SdkMessageProcessingSteps
		{
			get
			{
				if (_sdkmessageprocessingsteps == null)
					{
						_sdkmessageprocessingsteps = new CrmRepository<SdkMessageProcessingStep>(this.context);
					}
				return _sdkmessageprocessingsteps;
			}
		}
		private IRepository<Publisher> _publishers; 
		public IRepository<Publisher> Publishers
		{
			get
			{
				if (_publishers == null)
					{
						_publishers = new CrmRepository<Publisher>(this.context);
					}
				return _publishers;
			}
		}
		private IRepository<Solution> _solutions; 
		public IRepository<Solution> Solutions
		{
			get
			{
				if (_solutions == null)
					{
						_solutions = new CrmRepository<Solution>(this.context);
					}
				return _solutions;
			}
		}
		private IRepository<SdkMessage> _sdkmessages; 
		public IRepository<SdkMessage> SdkMessages
		{
			get
			{
				if (_sdkmessages == null)
					{
						_sdkmessages = new CrmRepository<SdkMessage>(this.context);
					}
				return _sdkmessages;
			}
		}
		private IRepository<SdkMessageFilter> _sdkmessagefilters; 
		public IRepository<SdkMessageFilter> SdkMessageFilters
		{
			get
			{
				if (_sdkmessagefilters == null)
					{
						_sdkmessagefilters = new CrmRepository<SdkMessageFilter>(this.context);
					}
				return _sdkmessagefilters;
			}
		}
		private IRepository<SdkMessageProcessingStepImage> _sdkmessageprocessingstepimages; 
		public IRepository<SdkMessageProcessingStepImage> SdkMessageProcessingStepImages
		{
			get
			{
				if (_sdkmessageprocessingstepimages == null)
					{
						_sdkmessageprocessingstepimages = new CrmRepository<SdkMessageProcessingStepImage>(this.context);
					}
				return _sdkmessageprocessingstepimages;
			}
		}
		private IRepository<SolutionComponent> _solutioncomponents; 
		public IRepository<SolutionComponent> SolutionComponents
		{
			get
			{
				if (_solutioncomponents == null)
					{
						_solutioncomponents = new CrmRepository<SolutionComponent>(this.context);
					}
				return _solutioncomponents;
			}
		}
		private IRepository<WebResource> _webresources; 
		public IRepository<WebResource> WebResources
		{
			get
			{
				if (_webresources == null)
					{
						_webresources = new CrmRepository<WebResource>(this.context);
					}
				return _webresources;
			}
		}
		private IRepository<SystemForm> _systemforms; 
		public IRepository<SystemForm> SystemForms
		{
			get
			{
				if (_systemforms == null)
					{
						_systemforms = new CrmRepository<SystemForm>(this.context);
					}
				return _systemforms;
			}
		}
		private IRepository<Workflow> _workflows; 
		public IRepository<Workflow> Workflows
		{
			get
			{
				if (_workflows == null)
					{
						_workflows = new CrmRepository<Workflow>(this.context);
					}
				return _workflows;
			}
		}
	}
	public partial class PluginAssembly
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("authtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue AuthType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("authtype");
			set
			{
				this.OnPropertyChanging("AuthType");
					this.SetAttributeValue("authtype", value);
				this.OnPropertyChanged("AuthType");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("isolationmode")]
		public Microsoft.Xrm.Sdk.OptionSetValue IsolationMode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("isolationmode");
			set
			{
				this.OnPropertyChanging("IsolationMode");
					this.SetAttributeValue("isolationmode", value);
				this.OnPropertyChanged("IsolationMode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("sourcetype")]
		public Microsoft.Xrm.Sdk.OptionSetValue SourceType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("sourcetype");
			set
			{
				this.OnPropertyChanging("SourceType");
					this.SetAttributeValue("sourcetype", value);
				this.OnPropertyChanged("SourceType");
			}
		}
	}
	public partial class PluginType
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
	}
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Kipon.Solid.Plugin", "2.0.0.10")]
	public enum PluginPackageState
	{
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	public partial class PluginPackage
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("statuscode")]
		public Microsoft.Xrm.Sdk.OptionSetValue statuscode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statuscode");
			set
			{
				this.OnPropertyChanging("statuscode");
					this.SetAttributeValue("statuscode", value);
				this.OnPropertyChanged("statuscode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public PluginPackageState? statecode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((PluginPackageState)(System.Enum.ToObject(typeof(PluginPackageState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.OnPropertyChanging("statecode");
				if ((value == null))
				{
					this.SetAttributeValue("statecode", null);
				}
				else
				{
					this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));
				}
				this.OnPropertyChanged("statecode");
			}
		}
	}
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Kipon.Solid.Plugin", "2.0.0.10")]
	public enum SdkMessageProcessingStepState
	{
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Enabled = 0,
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Disabled = 1,
	}
	public partial class SdkMessageProcessingStep
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("invocationsource")]
		public Microsoft.Xrm.Sdk.OptionSetValue InvocationSource
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("invocationsource");
			set
			{
				this.OnPropertyChanging("InvocationSource");
					this.SetAttributeValue("invocationsource", value);
				this.OnPropertyChanged("InvocationSource");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("mode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Mode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("mode");
			set
			{
				this.OnPropertyChanging("Mode");
					this.SetAttributeValue("mode", value);
				this.OnPropertyChanged("Mode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("stage")]
		public Microsoft.Xrm.Sdk.OptionSetValue Stage
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("stage");
			set
			{
				this.OnPropertyChanging("Stage");
					this.SetAttributeValue("stage", value);
				this.OnPropertyChanged("Stage");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("statuscode")]
		public Microsoft.Xrm.Sdk.OptionSetValue StatusCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statuscode");
			set
			{
				this.OnPropertyChanging("StatusCode");
					this.SetAttributeValue("statuscode", value);
				this.OnPropertyChanged("StatusCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("supporteddeployment")]
		public Microsoft.Xrm.Sdk.OptionSetValue SupportedDeployment
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("supporteddeployment");
			set
			{
				this.OnPropertyChanging("SupportedDeployment");
					this.SetAttributeValue("supporteddeployment", value);
				this.OnPropertyChanged("SupportedDeployment");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public SdkMessageProcessingStepState? StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((SdkMessageProcessingStepState)(System.Enum.ToObject(typeof(SdkMessageProcessingStepState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.OnPropertyChanging("StateCode");
				if ((value == null))
				{
					this.SetAttributeValue("statecode", null);
				}
				else
				{
					this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));
				}
				this.OnPropertyChanged("StateCode");
			}
		}
	}
	public partial class Publisher
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("address1_addresstypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Address1_AddressTypeCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("address1_addresstypecode");
			set
			{
				this.OnPropertyChanging("Address1_AddressTypeCode");
					this.SetAttributeValue("address1_addresstypecode", value);
				this.OnPropertyChanged("Address1_AddressTypeCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("address1_shippingmethodcode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Address1_ShippingMethodCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("address1_shippingmethodcode");
			set
			{
				this.OnPropertyChanging("Address1_ShippingMethodCode");
					this.SetAttributeValue("address1_shippingmethodcode", value);
				this.OnPropertyChanged("Address1_ShippingMethodCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("address2_addresstypecode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Address2_AddressTypeCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("address2_addresstypecode");
			set
			{
				this.OnPropertyChanging("Address2_AddressTypeCode");
					this.SetAttributeValue("address2_addresstypecode", value);
				this.OnPropertyChanged("Address2_AddressTypeCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("address2_shippingmethodcode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Address2_ShippingMethodCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("address2_shippingmethodcode");
			set
			{
				this.OnPropertyChanging("Address2_ShippingMethodCode");
					this.SetAttributeValue("address2_shippingmethodcode", value);
				this.OnPropertyChanged("Address2_ShippingMethodCode");
			}
		}
	}
	public partial class Solution
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("solutiontype")]
		public Microsoft.Xrm.Sdk.OptionSetValue SolutionType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("solutiontype");
			set
			{
				this.OnPropertyChanging("SolutionType");
					this.SetAttributeValue("solutiontype", value);
				this.OnPropertyChanged("SolutionType");
			}
		}
	}
	public partial class SdkMessage
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
	}
	public partial class SdkMessageFilter
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
	}
	public partial class SdkMessageProcessingStepImage
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("imagetype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ImageType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("imagetype");
			set
			{
				this.OnPropertyChanging("ImageType");
					this.SetAttributeValue("imagetype", value);
				this.OnPropertyChanged("ImageType");
			}
		}
	}
	public partial class SolutionComponent
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componenttype")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componenttype");
			set
			{
				this.OnPropertyChanging("ComponentType");
					this.SetAttributeValue("componenttype", value);
				this.OnPropertyChanged("ComponentType");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("rootcomponentbehavior")]
		public Microsoft.Xrm.Sdk.OptionSetValue RootComponentBehavior
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("rootcomponentbehavior");
			set
			{
				this.OnPropertyChanging("RootComponentBehavior");
					this.SetAttributeValue("rootcomponentbehavior", value);
				this.OnPropertyChanged("RootComponentBehavior");
			}
		}
	}
	public partial class WebResource
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("webresourcetype")]
		public Microsoft.Xrm.Sdk.OptionSetValue WebResourceType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("webresourcetype");
			set
			{
				this.OnPropertyChanging("WebResourceType");
					this.SetAttributeValue("webresourcetype", value);
				this.OnPropertyChanged("WebResourceType");
			}
		}
	}
	public partial class SystemForm
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("formactivationstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue FormActivationState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("formactivationstate");
			set
			{
				this.OnPropertyChanging("FormActivationState");
					this.SetAttributeValue("formactivationstate", value);
				this.OnPropertyChanged("FormActivationState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("formpresentation")]
		public Microsoft.Xrm.Sdk.OptionSetValue FormPresentation
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("formpresentation");
			set
			{
				this.OnPropertyChanging("FormPresentation");
					this.SetAttributeValue("formpresentation", value);
				this.OnPropertyChanged("FormPresentation");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("type")]
		public Microsoft.Xrm.Sdk.OptionSetValue Type
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("type");
			set
			{
				this.OnPropertyChanging("Type");
					this.SetAttributeValue("type", value);
				this.OnPropertyChanged("Type");
			}
		}
	}
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Kipon.Solid.Plugin", "2.0.0.10")]
	public enum WorkflowState
	{
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Draft = 0,
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Activated = 1,
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Suspended = 2,
	}
	public partial class Workflow
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("businessprocesstype")]
		public Microsoft.Xrm.Sdk.OptionSetValue BusinessProcessType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("businessprocesstype");
			set
			{
				this.OnPropertyChanging("BusinessProcessType");
					this.SetAttributeValue("businessprocesstype", value);
				this.OnPropertyChanged("BusinessProcessType");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("category")]
		public Microsoft.Xrm.Sdk.OptionSetValue Category
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("category");
			set
			{
				this.OnPropertyChanging("Category");
					this.SetAttributeValue("category", value);
				this.OnPropertyChanged("Category");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("componentstate")]
		public Microsoft.Xrm.Sdk.OptionSetValue ComponentState
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("componentstate");
			set
			{
				this.OnPropertyChanging("ComponentState");
					this.SetAttributeValue("componentstate", value);
				this.OnPropertyChanged("ComponentState");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("createstage")]
		public Microsoft.Xrm.Sdk.OptionSetValue CreateStage
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("createstage");
			set
			{
				this.OnPropertyChanging("CreateStage");
					this.SetAttributeValue("createstage", value);
				this.OnPropertyChanged("CreateStage");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("deletestage")]
		public Microsoft.Xrm.Sdk.OptionSetValue DeleteStage
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("deletestage");
			set
			{
				this.OnPropertyChanging("DeleteStage");
					this.SetAttributeValue("deletestage", value);
				this.OnPropertyChanged("DeleteStage");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("mode")]
		public Microsoft.Xrm.Sdk.OptionSetValue Mode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("mode");
			set
			{
				this.OnPropertyChanging("Mode");
					this.SetAttributeValue("mode", value);
				this.OnPropertyChanged("Mode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("processtriggerscope")]
		public Microsoft.Xrm.Sdk.OptionSetValue ProcessTriggerScope
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("processtriggerscope");
			set
			{
				this.OnPropertyChanging("ProcessTriggerScope");
					this.SetAttributeValue("processtriggerscope", value);
				this.OnPropertyChanged("ProcessTriggerScope");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("runas")]
		public Microsoft.Xrm.Sdk.OptionSetValue RunAs
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("runas");
			set
			{
				this.OnPropertyChanging("RunAs");
					this.SetAttributeValue("runas", value);
				this.OnPropertyChanged("RunAs");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("scope")]
		public Microsoft.Xrm.Sdk.OptionSetValue Scope
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("scope");
			set
			{
				this.OnPropertyChanging("Scope");
					this.SetAttributeValue("scope", value);
				this.OnPropertyChanged("Scope");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("statuscode")]
		public Microsoft.Xrm.Sdk.OptionSetValue StatusCode
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statuscode");
			set
			{
				this.OnPropertyChanging("StatusCode");
					this.SetAttributeValue("statuscode", value);
				this.OnPropertyChanged("StatusCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("type")]
		public Microsoft.Xrm.Sdk.OptionSetValue Type
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("type");
			set
			{
				this.OnPropertyChanging("Type");
					this.SetAttributeValue("type", value);
				this.OnPropertyChanged("Type");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("uiflowtype")]
		public Microsoft.Xrm.Sdk.OptionSetValue UIFlowType
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("uiflowtype");
			set
			{
				this.OnPropertyChanging("UIFlowType");
					this.SetAttributeValue("uiflowtype", value);
				this.OnPropertyChanged("UIFlowType");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("updatestage")]
		public Microsoft.Xrm.Sdk.OptionSetValue UpdateStage
		{
			get => this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("updatestage");
			set
			{
				this.OnPropertyChanging("UpdateStage");
					this.SetAttributeValue("updatestage", value);
				this.OnPropertyChanged("UpdateStage");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public WorkflowState? StateCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
				if ((optionSet != null))
				{
					return ((WorkflowState)(System.Enum.ToObject(typeof(WorkflowState), optionSet.Value)));
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.OnPropertyChanging("StateCode");
				if ((value == null))
				{
					this.SetAttributeValue("statecode", null);
				}
				else
				{
					this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.OptionSetValue(((int)(value))));
				}
				this.OnPropertyChanged("StateCode");
			}
		}
	}
}
