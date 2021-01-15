// Plugin Version: 1.0.4.7, Dynamics 365 svcutil solid extension tool by Kipon ApS (c) 2019, Kjeld Poulsen
// This file is autogenerated. Do not touch the code manually.

using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
namespace Kipon.Xrm
{
	public sealed class Version
	{
		public const string No = "1.0.4.7";
	}
}
namespace Kipon.Solid.Plugin.Entities
{
	[Kipon.Xrm.Attributes.Export(typeof(IUnitOfWork))]
	[Kipon.Xrm.Attributes.Export(typeof(Kipon.Xrm.IUnitOfWork))]
	public sealed partial class CrmUnitOfWork: IUnitOfWork, IDisposable, Kipon.Xrm.IService
	{
		private SolidContextService context;
		private IOrganizationService _service;
		public CrmUnitOfWork(IOrganizationService orgService)
		{
			this._service = orgService;
			this.context = new SolidContextService(_service);
		}

        public void Dispose()
        {
            context.Dispose();
        }

        public R ExecuteRequest<R>(OrganizationRequest request) where R : OrganizationResponse
        {
            return (R)this.context.Execute(request);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return this.context.Execute(request);
        }


        public Guid Create(Entity entity)
        {
            return this._service.Create(entity);
        }

        public void Update(Entity entity)
        {
            this._service.Update(entity);
        }

        public void Delete(Entity entity)
        {
            this._service.Delete(entity.LogicalName, entity.Id);
        }

        public void ClearContext()
        {
            var candidates = this.context.GetAttachedEntities().ToArray();
            foreach (var can in candidates) 
            {
                context.Detach(can);
            }
        }

        public void Detach(string logicalName, params Guid[] ids)
        {
            if (this.context != null)
            {
                var candidates = (from c in this.context.GetAttachedEntities() where c.LogicalName == logicalName select c);
                if (ids != null && ids.Length > 0)
                {
                    candidates = (from c in candidates where ids.Contains(c.Id) select c);
                }
                foreach (var r in candidates.ToArray())
                {
                    context.Detach(r);
                }
            }
        }

        public void Detach(Microsoft.Xrm.Sdk.EntityReference eref)
        {
            this.Detach(eref.LogicalName, eref.Id);
        }

        public void Detach(Microsoft.Xrm.Sdk.Entity ent)
        {
            this.Detach(ent.LogicalName, ent.Id);
        }

		void Kipon.Xrm.IService.OnStepFinalized()
		{
			foreach (var e in this.context.GetAttachedEntities().ToArray()) this.context.Detach(e);
		}

		private Kipon.Xrm.IRepository<Account> _accounts; 
		public Kipon.Xrm.IRepository<Account> Accounts
		{
			get
			{
				if (_accounts == null)
					{
						_accounts = new CrmRepository<Account>(this.context, this._service);
					}
				return _accounts;
			}
		}
		private Kipon.Xrm.IRepository<PhoneCall> _phonecalls; 
		public Kipon.Xrm.IRepository<PhoneCall> Phonecalls
		{
			get
			{
				if (_phonecalls == null)
					{
						_phonecalls = new CrmRepository<PhoneCall>(this.context, this._service);
					}
				return _phonecalls;
			}
		}
		private Kipon.Xrm.IRepository<Contact> _contacts; 
		public Kipon.Xrm.IRepository<Contact> Contacts
		{
			get
			{
				if (_contacts == null)
					{
						_contacts = new CrmRepository<Contact>(this.context, this._service);
					}
				return _contacts;
			}
		}
		private Kipon.Xrm.IRepository<Opportunity> _opportunities; 
		public Kipon.Xrm.IRepository<Opportunity> Opportunities
		{
			get
			{
				if (_opportunities == null)
					{
						_opportunities = new CrmRepository<Opportunity>(this.context, this._service);
					}
				return _opportunities;
			}
		}
		private Kipon.Xrm.IRepository<SalesOrder> _salesorders; 
		public Kipon.Xrm.IRepository<SalesOrder> Salesorders
		{
			get
			{
				if (_salesorders == null)
					{
						_salesorders = new CrmRepository<SalesOrder>(this.context, this._service);
					}
				return _salesorders;
			}
		}
		private Kipon.Xrm.IRepository<Quote> _quotes; 
		public Kipon.Xrm.IRepository<Quote> Quotes
		{
			get
			{
				if (_quotes == null)
					{
						_quotes = new CrmRepository<Quote>(this.context, this._service);
					}
				return _quotes;
			}
		}
		private Kipon.Xrm.IRepository<SystemUser> _systemusers; 
		public Kipon.Xrm.IRepository<SystemUser> Systemusers
		{
			get
			{
				if (_systemusers == null)
					{
						_systemusers = new CrmRepository<SystemUser>(this.context, this._service);
					}
				return _systemusers;
			}
		}
	}
	[Kipon.Xrm.Attributes.Export(typeof(IAdminUnitOfWork))]
	[Kipon.Xrm.Attributes.Export(typeof(Kipon.Xrm.IAdminUnitOfWork))]
	public sealed partial class AdminCrmUnitOfWork : IAdminUnitOfWork, IDisposable, Kipon.Xrm.IService
	{
		private SolidContextService context;
		private IOrganizationService _service;
		public AdminCrmUnitOfWork(IOrganizationService orgService)
		{
			this._service = orgService;
			this.context = new SolidContextService(_service);
		}

        public void Dispose()
        {
            context.Dispose();
        }

        public R ExecuteRequest<R>(OrganizationRequest request) where R : OrganizationResponse
        {
            return (R)this.context.Execute(request);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return this.context.Execute(request);
        }


        public Guid Create(Entity entity)
        {
            return this._service.Create(entity);
        }

        public void Update(Entity entity)
        {
            this._service.Update(entity);
        }

        public void Delete(Entity entity)
        {
            this._service.Delete(entity.LogicalName, entity.Id);
        }

        public void ClearContext()
        {
            var candidates = this.context.GetAttachedEntities().ToArray();
            foreach (var can in candidates) 
            {
                context.Detach(can);
            }
        }

        public void Detach(string logicalName, params Guid[] ids)
        {
            if (this.context != null)
            {
                var candidates = (from c in this.context.GetAttachedEntities() where c.LogicalName == logicalName select c);
                if (ids != null && ids.Length > 0)
                {
                    candidates = (from c in candidates where ids.Contains(c.Id) select c);
                }
                foreach (var r in candidates.ToArray())
                {
                    context.Detach(r);
                }
            }
        }

        public void Detach(Microsoft.Xrm.Sdk.EntityReference eref)
        {
            this.Detach(eref.LogicalName, eref.Id);
        }

        public void Detach(Microsoft.Xrm.Sdk.Entity ent)
        {
            this.Detach(ent.LogicalName, ent.Id);
        }

		void Kipon.Xrm.IService.OnStepFinalized()
		{
			foreach (var e in this.context.GetAttachedEntities().ToArray()) this.context.Detach(e);
		}

		private Kipon.Xrm.IRepository<Account> _accounts; 
		public Kipon.Xrm.IRepository<Account> Accounts
		{
			get
			{
				if (_accounts == null)
					{
						_accounts = new CrmRepository<Account>(this.context, this._service);
					}
				return _accounts;
			}
		}
		private Kipon.Xrm.IRepository<PhoneCall> _phonecalls; 
		public Kipon.Xrm.IRepository<PhoneCall> Phonecalls
		{
			get
			{
				if (_phonecalls == null)
					{
						_phonecalls = new CrmRepository<PhoneCall>(this.context, this._service);
					}
				return _phonecalls;
			}
		}
		private Kipon.Xrm.IRepository<Contact> _contacts; 
		public Kipon.Xrm.IRepository<Contact> Contacts
		{
			get
			{
				if (_contacts == null)
					{
						_contacts = new CrmRepository<Contact>(this.context, this._service);
					}
				return _contacts;
			}
		}
		private Kipon.Xrm.IRepository<Opportunity> _opportunities; 
		public Kipon.Xrm.IRepository<Opportunity> Opportunities
		{
			get
			{
				if (_opportunities == null)
					{
						_opportunities = new CrmRepository<Opportunity>(this.context, this._service);
					}
				return _opportunities;
			}
		}
		private Kipon.Xrm.IRepository<SalesOrder> _salesorders; 
		public Kipon.Xrm.IRepository<SalesOrder> Salesorders
		{
			get
			{
				if (_salesorders == null)
					{
						_salesorders = new CrmRepository<SalesOrder>(this.context, this._service);
					}
				return _salesorders;
			}
		}
		private Kipon.Xrm.IRepository<Quote> _quotes; 
		public Kipon.Xrm.IRepository<Quote> Quotes
		{
			get
			{
				if (_quotes == null)
					{
						_quotes = new CrmRepository<Quote>(this.context, this._service);
					}
				return _quotes;
			}
		}
		private Kipon.Xrm.IRepository<SystemUser> _systemusers; 
		public Kipon.Xrm.IRepository<SystemUser> Systemusers
		{
			get
			{
				if (_systemusers == null)
					{
						_systemusers = new CrmRepository<SystemUser>(this.context, this._service);
					}
				return _systemusers;
			}
		}
	}
	public partial interface IAccountTarget : Kipon.Xrm.Target<Account>{ }
	public partial interface IAccountPreimage : Kipon.Xrm.Preimage<Account>{ }
	public partial interface IAccountPostimage : Kipon.Xrm.Postimage<Account>{ }
	public partial interface IAccountMergedimage : Kipon.Xrm.Mergedimage<Account>{ }
	public sealed partial class Account :
		IAccountTarget,
		IAccountPreimage,
		IAccountPostimage,
		IAccountMergedimage
	{
	}
	public partial interface IPhoneCallTarget : Kipon.Xrm.Target<PhoneCall>{ }
	public partial interface IPhoneCallPreimage : Kipon.Xrm.Preimage<PhoneCall>{ }
	public partial interface IPhoneCallPostimage : Kipon.Xrm.Postimage<PhoneCall>{ }
	public partial interface IPhoneCallMergedimage : Kipon.Xrm.Mergedimage<PhoneCall>{ }
	public sealed partial class PhoneCall :
		IPhoneCallTarget,
		IPhoneCallPreimage,
		IPhoneCallPostimage,
		IPhoneCallMergedimage
	{
	}
	public partial interface IContactTarget : Kipon.Xrm.Target<Contact>{ }
	public partial interface IContactPreimage : Kipon.Xrm.Preimage<Contact>{ }
	public partial interface IContactPostimage : Kipon.Xrm.Postimage<Contact>{ }
	public partial interface IContactMergedimage : Kipon.Xrm.Mergedimage<Contact>{ }
	public sealed partial class Contact :
		IContactTarget,
		IContactPreimage,
		IContactPostimage,
		IContactMergedimage
	{
	}
	public partial interface IOpportunityTarget : Kipon.Xrm.Target<Opportunity>{ }
	public partial interface IOpportunityPreimage : Kipon.Xrm.Preimage<Opportunity>{ }
	public partial interface IOpportunityPostimage : Kipon.Xrm.Postimage<Opportunity>{ }
	public partial interface IOpportunityMergedimage : Kipon.Xrm.Mergedimage<Opportunity>{ }
	public sealed partial class Opportunity :
		IOpportunityTarget,
		IOpportunityPreimage,
		IOpportunityPostimage,
		IOpportunityMergedimage
	{
	}
	public partial interface ISalesOrderTarget : Kipon.Xrm.Target<SalesOrder>{ }
	public partial interface ISalesOrderPreimage : Kipon.Xrm.Preimage<SalesOrder>{ }
	public partial interface ISalesOrderPostimage : Kipon.Xrm.Postimage<SalesOrder>{ }
	public partial interface ISalesOrderMergedimage : Kipon.Xrm.Mergedimage<SalesOrder>{ }
	public sealed partial class SalesOrder :
		ISalesOrderTarget,
		ISalesOrderPreimage,
		ISalesOrderPostimage,
		ISalesOrderMergedimage
	{
	}
	public partial interface IQuoteTarget : Kipon.Xrm.Target<Quote>{ }
	public partial interface IQuotePreimage : Kipon.Xrm.Preimage<Quote>{ }
	public partial interface IQuotePostimage : Kipon.Xrm.Postimage<Quote>{ }
	public partial interface IQuoteMergedimage : Kipon.Xrm.Mergedimage<Quote>{ }
	public sealed partial class Quote :
		IQuoteTarget,
		IQuotePreimage,
		IQuotePostimage,
		IQuoteMergedimage
	{
	}
	public partial interface ISystemUserTarget : Kipon.Xrm.Target<SystemUser>{ }
	public partial interface ISystemUserPreimage : Kipon.Xrm.Preimage<SystemUser>{ }
	public partial interface ISystemUserPostimage : Kipon.Xrm.Postimage<SystemUser>{ }
	public partial interface ISystemUserMergedimage : Kipon.Xrm.Mergedimage<SystemUser>{ }
	public sealed partial class SystemUser :
		ISystemUserTarget,
		ISystemUserPreimage,
		ISystemUserPostimage,
		ISystemUserMergedimage
	{
	}
	public sealed class AccountReference : Kipon.Xrm.TargetReference<Account>
	{
		public AccountReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => Account.EntityLogicalName;
	}
	public sealed class PhoneCallReference : Kipon.Xrm.TargetReference<PhoneCall>
	{
		public PhoneCallReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => PhoneCall.EntityLogicalName;
	}
	public sealed class ContactReference : Kipon.Xrm.TargetReference<Contact>
	{
		public ContactReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => Contact.EntityLogicalName;
	}
	public sealed class OpportunityReference : Kipon.Xrm.TargetReference<Opportunity>
	{
		public OpportunityReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => Opportunity.EntityLogicalName;
	}
	public sealed class SalesOrderReference : Kipon.Xrm.TargetReference<SalesOrder>
	{
		public SalesOrderReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => SalesOrder.EntityLogicalName;
	}
	public sealed class QuoteReference : Kipon.Xrm.TargetReference<Quote>
	{
		public QuoteReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => Quote.EntityLogicalName;
	}
	public sealed class SystemUserReference : Kipon.Xrm.TargetReference<SystemUser>
	{
		public SystemUserReference(EntityReference target): base(target){ }
		protected sealed override string _logicalName => SystemUser.EntityLogicalName;
	}
	public partial interface IUnitOfWork : Kipon.Xrm.IUnitOfWork
	{
		#region entity repositories
		Kipon.Xrm.IRepository<Account> Accounts { get; }
		Kipon.Xrm.IRepository<PhoneCall> Phonecalls { get; }
		Kipon.Xrm.IRepository<Contact> Contacts { get; }
		Kipon.Xrm.IRepository<Opportunity> Opportunities { get; }
		Kipon.Xrm.IRepository<SalesOrder> Salesorders { get; }
		Kipon.Xrm.IRepository<Quote> Quotes { get; }
		Kipon.Xrm.IRepository<SystemUser> Systemusers { get; }
		#endregion
	}
	public partial interface IAdminUnitOfWork : Kipon.Xrm.IAdminUnitOfWork, IUnitOfWork { }
   public class CrmRepository<T> : Kipon.Xrm.IRepository<T> where T: Microsoft.Xrm.Sdk.Entity, new() 
    {
        private SolidContextService context;
        private Microsoft.Xrm.Sdk.IOrganizationService _service;

        public CrmRepository(SolidContextService context, Microsoft.Xrm.Sdk.IOrganizationService service)
        {
            this.context = context;
            this._service = service;
        }

        public IQueryable<T> GetQuery()
        {
            return context.CreateQuery<T>();
        }

        public void Delete(T entity)
        {
            this._service.Delete(entity.LogicalName, entity.Id);
            this.context.Detach(entity);
        }

        public void Add(T entity)
        {
            this._service.Create(entity);
            this.context.Attach(entity);
        }

        public void Attach(T entity)
        {
            this.context.Attach(entity);
        }

        public void Detach(T entity)
        {
            this.context.Detach(entity);
        }

        public void Update(T entity)
        {
            this._service.Update(entity);
            if (!this.context.IsAttached(entity))
            {
                this.context.Attach(entity);
            } else 
            {
                var ch = (from c in this.context.GetAttachedEntities() 
                          where c.LogicalName == entity.LogicalName && 
                                c.Id == entity.Id 
                          select c).Single();

                foreach (var key in entity.Attributes.Keys)
                {
                    // update the cache silent
                    ch.Attributes.Remove(key);
                    ch.Attributes.Add(key, entity[key]);
                }
            }
        }

        public T GetById(Guid id)
        {
            return (from q in this.GetQuery()
                    where q.Id == id
                    select q).Single();
        }
    }
	public enum BudgetEnum
	{
		NoCommittedBudget = 0,
		MAyBuy = 1,
		CanBuy = 2,
		WillBuy = 3
	}
	public partial class Account
	{
		public enum PreferredContactMethodCodeEnum
		{
			Any = 1,
			Email = 2,
			Phone = 3,
			Fax = 4,
			Mail = 5,
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("preferredcontactmethodcode")]
		public PreferredContactMethodCodeEnum? PreferredContactMethodCode
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("preferredcontactmethodcode");
				if (optionSet != null)
				{
					return (PreferredContactMethodCodeEnum)optionSet.Value;
				}
				return null;
			}
			set
			{
				this.OnPropertyChanging("PreferredContactMethodCode");
				if (value != null)
				{
					this.SetAttributeValue("preferredcontactmethodcode", new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value));
					this.OnPropertyChanged("PreferredContactMethodCode");
					return;
				}
				this.SetAttributeValue("preferredcontactmethodcode", null);
				this.OnPropertyChanged("PreferredContactMethodCode");
			}
		}
		[Microsoft.Xrm.Sdk.AttributeLogicalName("new_multiselectbudget")]
		public BudgetEnum[] MultiSelectBudget
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValueCollection optionSetValues = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>("new_multiselectbudget");
				if (optionSetValues != null && optionSetValues.Count > 0)
				{
					return (from v in optionSetValues select (BudgetEnum)v.Value).ToArray();
				}
				return null;
			}
			set
			{
				this.OnPropertyChanging("new_multiselectbudget");
				if (value != null && value.Length > 0)
				{
					var result = new Microsoft.Xrm.Sdk.OptionSetValueCollection();
					foreach (var v in value) result.Add(new Microsoft.Xrm.Sdk.OptionSetValue((int)v));
					this.SetAttributeValue("new_multiselectbudget", result);
					this.OnPropertyChanged("new_multiselectbudget");
					return;
				}
				this.SetAttributeValue("new_multiselectbudget", null);
				this.OnPropertyChanged("new_multiselectbudget");
			}
		}
	}
	public partial class Opportunity
	{
		[Microsoft.Xrm.Sdk.AttributeLogicalName("budgetstatus")]
		public BudgetEnum? Budgetstatus
		{
			get
			{
				Microsoft.Xrm.Sdk.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("budgetstatus");
				if (optionSet != null)
				{
					return (BudgetEnum)optionSet.Value;
				}
				return null;
			}
			set
			{
				this.OnPropertyChanging("BudgetStatus");
				if (value != null)
				{
					this.SetAttributeValue("budgetstatus", new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value));
					this.OnPropertyChanged("BudgetStatus");
					return;
				}
				this.SetAttributeValue("budgetstatus", null);
				this.OnPropertyChanged("BudgetStatus");
			}
		}
	}
}
namespace Kipon.Xrm.Extensions.Sdk
{
	public static partial class KiponSdkGeneratedExtensionMethods
	{
		static KiponSdkGeneratedExtensionMethods()
		{
			entittypes[Kipon.Solid.Plugin.Entities.Account.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.Account);
			entittypes[Kipon.Solid.Plugin.Entities.PhoneCall.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.PhoneCall);
			entittypes[Kipon.Solid.Plugin.Entities.Contact.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.Contact);
			entittypes[Kipon.Solid.Plugin.Entities.Opportunity.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.Opportunity);
			entittypes[Kipon.Solid.Plugin.Entities.SalesOrder.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.SalesOrder);
			entittypes[Kipon.Solid.Plugin.Entities.Quote.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.Quote);
			entittypes[Kipon.Solid.Plugin.Entities.SystemUser.EntityLogicalName] = typeof(Kipon.Solid.Plugin.Entities.SystemUser);
		}
	}
}
namespace Kipon.Solid.Plugin.Actions
{
	public partial interface IAccountCountContactsRequest: Kipon.Xrm.ActionTarget<Kipon.Solid.Plugin.Entities.Account>
	{
		string Name { get; }
	}
	public partial class AccountCountContactsResponse
	{
		[Kipon.Xrm.Attributes.Output("Count", true)]
		 public int Count { get; set; }
		[Kipon.Xrm.Attributes.Output("AMoney", true)]
		 public Microsoft.Xrm.Sdk.Money AMoney { get; set; }
	}
	public partial interface IAnunboundedactionRequest
	{
		string Name { get; }
		string Document { get; }
	}
	public partial class AnunboundedactionResponse
	{
		[Kipon.Xrm.Attributes.Output("Id", true)]
		 public string Id { get; set; }
	}
}
namespace Kipon.Solid.Plugin.Actions.Implement
{
	using Kipon.Solid.Plugin.Actions;
	using Kipon.Xrm.Actions;
	public partial class AccountCountContactsRequest : AbstractActionRequest, IAccountCountContactsRequest
	{
		public AccountCountContactsRequest(Microsoft.Xrm.Sdk.IPluginExecutionContext ctx): base(ctx){ }
		public Microsoft.Xrm.Sdk.EntityReference Target { get => this.ValueOf<Microsoft.Xrm.Sdk.EntityReference>("Target"); }
		public string Name {get => this.ValueOf<string>("Name");}
	}
	public partial class AnunboundedactionRequest : AbstractActionRequest, IAnunboundedactionRequest
	{
		public AnunboundedactionRequest(Microsoft.Xrm.Sdk.IPluginExecutionContext ctx): base(ctx){ }
		public string Name {get => this.ValueOf<string>("Name");}
		public string Document {get => this.ValueOf<string>("Document");}
	}
}
