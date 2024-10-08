// Dynamics 365 svcutil extension tool by Kipon ApS, Kjeld Ingemann Poulsen
// This file is autogenerated. Do not touch the code manually

namespace Kipon.Xrm.Tools.Entities
{
	public partial interface IUnitOfWork
	{
		#region entity repositories
		IRepository<PluginAssembly> PluginAssemblies { get; }
		IRepository<PluginType> PluginTypes { get; }
		IRepository<SdkMessageProcessingStep> SdkMessageProcessingSteps { get; }
		IRepository<Publisher> Publishers { get; }
		IRepository<Solution> Solutions { get; }
		IRepository<SdkMessage> SdkMessages { get; }
		IRepository<SdkMessagePair> SdkMessagePairs { get; }
		IRepository<SdkMessageFilter> SdkMessageFilters { get; }
		IRepository<SdkMessageRequest> SdkMessageRequests { get; }
		IRepository<SdkMessageProcessingStepImage> SdkMessageProcessingStepImages { get; }
		IRepository<SdkMessageRequestField> SdkMessageRequestFields { get; }
		IRepository<SdkMessageResponse> SdkMessageResponses { get; }
		IRepository<SdkMessageResponseField> SdkMessageResponseFields { get; }
		IRepository<SolutionComponent> SolutionComponents { get; }
		IRepository<WebResource> WebResources { get; }
		IRepository<SystemForm> SystemForms { get; }
		IRepository<Workflow> Workflows { get; }
		#endregion
	}
}
