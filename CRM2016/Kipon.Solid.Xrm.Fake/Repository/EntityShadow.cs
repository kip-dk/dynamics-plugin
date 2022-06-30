using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Fake.Extensions.Query;

namespace Kipon.Xrm.Fake.Repository
{
    internal class EntityShadow
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        internal Guid Id { get; private set; }
        internal string LogicalName { get; private set; }

        internal string Key
        {
            get
            {
                return this.LogicalName + this.Id.ToString();
            }
        }


        internal string[] Keys
        {
            get
            {
                return values.Keys.ToArray();
            }
        }

        internal EntityShadow(string logicalName, Guid id)
        {
            this.LogicalName = logicalName;
            this.Id = id;
        }

        internal EntityShadow(EntityShadow cloneFrom)
        {
            this.Id = cloneFrom.Id;
            this.LogicalName = cloneFrom.LogicalName;
            foreach (var key in cloneFrom.values.Keys)
            {
                this[key] = cloneFrom[key];
            }
        }

        internal EntityShadow(Microsoft.Xrm.Sdk.Entity from)
        {
            this.Id = from.Id;
            this.LogicalName = from.LogicalName;
            foreach (var key in from.Attributes.Keys)
            {
                this[key] = from[key];
            }
        }

        internal object this[string logicalAttributeName]
        {
            get
            {
                if (values.ContainsKey(logicalAttributeName))
                {
                    return values[logicalAttributeName];
                }
                return null;
            }
            set
            {
                values[logicalAttributeName] = EntityShadow.ValueCloning(value);
            }
        }

        internal string KeyOf(string logicalAttributeName)
        {
            var value = this[logicalAttributeName];
            if (value == null)
            {
                return null;
            }

            #region join on a field that is already an entity reference
            var already = value as Microsoft.Xrm.Sdk.EntityReference;
            if (already != null)
            {
                return $"{already.Id.ToString()}{already.LogicalName}";
            }
            #endregion

            #region join on the primary key on this entity
            if (logicalAttributeName.ToLower() == $"{this.LogicalName}id".ToLower())
            {
                return $"{this.Id.ToString()}{this.LogicalName}";
            }
            #endregion

            #region activity types (activity types does not support M:M relations, so we will never have a guid field named activityid)
            if (logicalAttributeName.ToLower() == "activityid")
            {
                return $"{this.Id.ToString()}{this.LogicalName}";
            }
            #endregion

            #region m:m fields are represented as guid's, and always named {entitylogicalname}id
            Guid? g = value as Guid?;
            if (g != null)
            {
                return $"{g.Value.ToString()}{logicalAttributeName.Substring(0, logicalAttributeName.Length  - 2)}";
            }
            #endregion

            throw new Exceptions.UnsupportedTypeException($"{this.LogicalName}.{logicalAttributeName} cannot be resolved to a key type.", value.GetType());

        }

        internal bool Match(Microsoft.Xrm.Sdk.Query.ConditionExpression expression)
        {
            var value = this[expression.AttributeName];

            switch (expression.Operator)
            {
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Above:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.AboveOrEqual:
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.ChildOf:
                    throw new Kipon.Xrm.Fake.Exceptions.UnsupportedConditionOperatorException(expression.Operator);
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith:
                    {
                        return value.BeginsWith(expression.Values);
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Between:
                    {
                        return value.Between(expression.Values);
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains:
                    {
                        return value.Contains(expression.Values);
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotBeginWith:
                    {
                        return !value.BeginsWith(expression.Values);
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotContain:
                    {
                        return !value.Contains(expression.Values);
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotEndWith:
                    {
                        throw new NotSupportedException();
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith:
                    {
                        throw new NotSupportedException();
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal:
                    {
                        return true;
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull:
                    {
                        return value != null;
                    }
                case Microsoft.Xrm.Sdk.Query.ConditionOperator.Null:
                    {
                        return value == null;
                    }
            }

            throw new Kipon.Xrm.Fake.Exceptions.UnsupportedConditionOperatorException(expression.Operator);
        }

        internal EntityShadow Clone()
        {
            return new EntityShadow(this);
        }

        internal Microsoft.Xrm.Sdk.Entity ToEntity()
        {
            var result = new Microsoft.Xrm.Sdk.Entity { Id = this.Id, LogicalName = this.LogicalName };
            foreach (var f in this.values.Keys)
            {
                result[f] = EntityShadow.ValueCloning(this[f]);
            }
            return result;
        }

        internal void UpdateFrom(Microsoft.Xrm.Sdk.Entity entity)
        {
            foreach (var key in entity.Attributes.Keys)
            {
                this[key] = entity[key];
            }
        }

        internal object ValueOf(string attrLogicalName)
        {
            if (this.values.ContainsKey(attrLogicalName))
            {
                return EntityShadow.ValueCloning(this[attrLogicalName]);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityShadow;
            if (other != null)
            {
                return this.Key == other.Key;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        private static object ValueCloning(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType() == typeof(Microsoft.Xrm.Sdk.EntityReference))
            {
                var v = (Microsoft.Xrm.Sdk.EntityReference)value;
                return new Microsoft.Xrm.Sdk.EntityReference { Id = v.Id, LogicalName = v.LogicalName, Name = v.Name };
            }


            if (value.GetType() == typeof(Microsoft.Xrm.Sdk.OptionSetValue))
            {
                var v = (Microsoft.Xrm.Sdk.OptionSetValue)value;
                return new Microsoft.Xrm.Sdk.OptionSetValue { Value = v.Value };
            }

            if (value.GetType() == typeof(Microsoft.Xrm.Sdk.Money))
            {
                var v = (Microsoft.Xrm.Sdk.Money)value;
                return new Microsoft.Xrm.Sdk.Money { Value = v.Value };
            }

            return value;

        }
    }
}
