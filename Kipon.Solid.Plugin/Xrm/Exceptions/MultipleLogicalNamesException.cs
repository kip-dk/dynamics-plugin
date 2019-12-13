﻿namespace Kipon.Xrm.Exceptions
{
    using System;
    public class MultipleLogicalNamesException : BaseException
    {
        public MultipleLogicalNamesException(Type type, System.Reflection.MethodInfo method) : base($"{ type.FullName }, method { method.Name } is requesting entities of different types. That is not allowed.")
        {
        }
    }
}