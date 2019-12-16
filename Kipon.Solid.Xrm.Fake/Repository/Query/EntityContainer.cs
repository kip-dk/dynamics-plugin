using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Fake.Extensions.Query;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class EntityContainer
    {
        private Dictionary<string, EntityShadow> entities = new Dictionary<string, EntityShadow>();
        public Guid Id { get; private set; }
        public string LogicalName { get; private set; }

        internal EntityContainer(EntityShadow entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("root entity cannot be null.");
            }
            this[string.Empty] = entity;
            this.Id = entity.Id;
            this.LogicalName = entity.LogicalName;
        }

        #region properties
        internal EntityShadow this[string alias]
        {
            get
            {
                if (this.entities.ContainsKey(alias))
                {
                    return this.entities[alias];
                }
                return null;
            }
            private set
            {
                if (value != null)
                {
                    this.entities[alias] = value;
                }
            }
        }
        #endregion

        internal EntityContainer Add(string alias, EntityShadow entity)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentException("alias cannot be null");
            }

            if (entities.ContainsKey(alias))
            {
                throw new ArgumentException("Dublicate alias, alias can only be used once.");
            }

            var result = new EntityContainer(this[string.Empty]);

            foreach (var key in this.entities.Keys)
            {
                if (key != string.Empty)
                {
                    result[key] = this.entities[key];
                }
            }
            result[alias] = entity;
            return result;
        }

        internal bool Match(string alias, Microsoft.Xrm.Sdk.Query.LogicalOperator opr, Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Query.ConditionExpression> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }

            switch (opr)
            {
                case Microsoft.Xrm.Sdk.Query.LogicalOperator.And:
                    {
                        return this.MatchAndConditions(alias, conditions);
                    }
                case Microsoft.Xrm.Sdk.Query.LogicalOperator.Or:
                    {
                        return MatchOrConditions(alias, conditions);
                    }

                default: throw new ArgumentException($"Unknown operation {opr}");
            }
        }

        private bool MatchAndConditions(string alias, Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Query.ConditionExpression> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }

            var entity = this[alias];
            if (entity == null)
            {
                return false;
            }

            // will keep look for false, and return false if found, if non where false, the final fallback true will win
            foreach (var condition in conditions)
            {
                var value = this[alias][condition.AttributeName];
                switch (condition.Operator)
                {
                    case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal:
                        {
                            if (!value.Equal(condition.Values))
                            {
                                return false;
                            }
                            break;
                        }
                }
            }

            return true;
        }

        private bool MatchOrConditions(string alias, Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Query.ConditionExpression> conditions)
        {
            // will keep look for a true, and return true if found, if non where true, the final fallback false will win
            foreach (var condition in conditions)
            {
                var value = this[alias][condition.AttributeName];
                switch (condition.Operator)
                {
                    case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal:
                        {
                            if (value.Equal(condition.Values))
                            {
                                return true;
                            }
                            break;
                        }
                }
            }

            return false;
        }

    }
}
