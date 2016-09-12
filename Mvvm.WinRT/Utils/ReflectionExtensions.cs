// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mvvm.WinRT.Utils
{
    // When compiling in Release, reflection is tricky due to .NET Native toolchain, 
    // check this out https://msdn.microsoft.com/en-us/library/dn600640(v=vs.110).aspx
    [Obsolete("Try to minimize the use of Reflection due to .NET Native toolchain usage in Release compilation")]
    public static class ReflectionExtensions
    {
        public static bool IsAssignableFrom(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsAssignableFrom(parentType.GetTypeInfo());
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] parametersTypes)
        {
            var methods = type.GetMethods(name);
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                bool match = true;
                foreach (var param in parameters)
                {
                    bool valid = true;
                    if (parametersTypes != null)
                    {
                        foreach (var ptype in parametersTypes)
                        {
                            valid &= (ptype == param.ParameterType);
                        }
                    }
                    match &= valid;
                }
                if (match)
                {
                    return method;
                }
            }

            return null;
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethods(name);
        }

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
    }
}