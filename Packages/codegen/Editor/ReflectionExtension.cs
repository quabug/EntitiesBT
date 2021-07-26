using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace EntitiesBT.CodeGen.Editor
{
    public static class ReflectionExtension
    {
        [Pure]
        public static IEnumerable<Type> GetTypesWithoutException([NotNull] this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception)
            {
                // TODO: log
                return Enumerable.Empty<Type>();
            }
        }

        [Pure]
        public static IEnumerable<T> CreateInstances<T>([NotNull] this AppDomain appDomain)
        {
            var assemblyName = typeof(T).Assembly.GetName().Name;
            return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(assembly => assembly.GetReferencedAssemblies()
                        .Append(assembly.GetName())
                        .Any(name => name.Name == assemblyName))
                    .SelectMany(assembly => assembly.GetTypesWithoutException())
                    .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    .Select(type => (T) Activator.CreateInstance(type))
                ;
        }

        [Pure]
        public static string ToReadableName(this Type type)
        {
            return type.IsGenericType ? Regex.Replace(type.ToString(), @"(\w+)`\d+\[(.*)\]", "$1<$2>") : type.ToString();
        }
    }
}