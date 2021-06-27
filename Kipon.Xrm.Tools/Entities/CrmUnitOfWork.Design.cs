// Tools Version 1.0.4.14 Dynamics 365 svcutil extension tool by Kipon ApS, Kjeld Ingemann Poulsen
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
}
