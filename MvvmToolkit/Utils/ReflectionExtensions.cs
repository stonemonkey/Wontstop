// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmToolkit.Utils
{
    /// <summary>
    /// Helper methods for Type reflection
    /// </summary>
    /// <remarks>
    /// Try to minimize the use of this class. When compiling in Release, reflection is tricky due 
    /// to .NET Native toolchain see https://msdn.microsoft.com/en-us/library/dn600640(v=vs.110).aspx
    /// </remarks>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Returns a value that indicates whether current type can be assigned to specified type
        /// </summary>
        /// <param name="type">Current type</param>
        /// <param name="baseType">The type that supports the assignment</param>
        /// <returns>True if the assignment is possible, otherwise false</returns>
        public static bool IsAssignableFrom(this Type type, Type baseType)
        {
            return type.GetTypeInfo().IsAssignableFrom(baseType.GetTypeInfo());
        }

        /// <summary>
        /// Returns a value that indicates wheather current type is generic
        /// </summary>
        /// <param name="type">Current type</param>
        /// <returns>True if current type is generic, otherwise false</returns>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        /// <summary>
        /// Returns the assembly where the current type is defined
        /// </summary>
        /// <param name="type">Current type</param>
        /// <returns>The assembly in wich the type is defined</returns>
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Returns method info of the current type matching the name and the type of parameters
        /// </summary>
        /// <param name="type">Current type</param>
        /// <param name="name">The name of the method</param>
        /// <param name="parametersTypes">Method parameter types</param>
        /// <returns>Method info or null in case no match</returns>
        public static MethodInfo GetMethod(this Type type, string name, Type[] parametersTypes)
        {
            var methods = type.GetMethods(name);
            foreach (var method in methods)
            {
                var match = true;
                var parameters = method.GetParameters();
                foreach (var param in parameters)
                {
                    var valid = true;
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

        /// <summary>
        /// Returns the public methods exposed by the current type matching specified name
        /// </summary>
        /// <param name="type">Current type</param>
        /// <param name="name">The name of the methods</param>
        /// <returns>Matching method names</returns>
        public static IEnumerable<MethodInfo> GetMethods(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethods(name);
        }

        /// <summary>
        /// Returns interfaces implemented by the current type
        /// </summary>
        /// <param name="type">Current type</param>
        /// <returns>The types of the interfaces</returns>
        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        /// <summary>
        /// Returns generic argument types of the current type
        /// </summary>
        /// <param name="type">Current type</param>
        /// <returns>An array containing the types of generic arguments</returns>
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
    }
}