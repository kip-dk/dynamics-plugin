﻿namespace Kipon.Xrm
{
    using System;
    public abstract class TargetReference<T> where T: Microsoft.Xrm.Sdk.Entity
    {
        private Microsoft.Xrm.Sdk.EntityReference target;

        public TargetReference(Microsoft.Xrm.Sdk.EntityReference target)
        {
            if (target.LogicalName != this._logicalName)
            {
                throw new ArgumentException($"Target reference does not match this type, expected {_logicalName} got {target.LogicalName }");
            }
            this.target = target;
        }

        public Microsoft.Xrm.Sdk.EntityReference Value
        {
            get
            {
                return this.target;
            }
        }

        protected abstract string _logicalName { get; }
    }
}
