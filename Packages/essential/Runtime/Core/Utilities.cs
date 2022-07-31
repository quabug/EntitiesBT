using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Blob;
using JetBrains.Annotations;
using UnityEngine;

namespace EntitiesBT.Core
{
    public static class Utilities
    {
        [Pure]
        public static IEnumerable<T> Yield<T>(this T self)
        {
            yield return self;
        }

        //
        // [Pure]
        // public static IEnumerable<float> NormalizeUnsafe([NotNull] this IEnumerable<float> weights)
        // {
        //     var sum = weights.Sum();
        //     return weights.Select(w => w / sum);
        // }
        //
        // [Pure]
        // public static IEnumerable<float> Normalize([NotNull] this IEnumerable<float> weights)
        // {
        //     var sum = weights.Where(w => w > 0).Sum();
        //     if (sum <= math.FLT_MIN_NORMAL) sum = 1;
        //     return weights.Select(w => math.max(w, 0) / sum);
        // }
        
        [Pure]
        public static bool IsZeroSizeStruct([NotNull] this Type t)
        {
            // return TypeManager.IsZeroSized(TypeManager.GetTypeIndex(t));
            // https://stackoverflow.com/a/27851610
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return t.IsValueType
                   && !t.IsPrimitive
                   && t.GetFields(flags).All(fi => IsZeroSizeStruct(fi.FieldType))
            ;
        }

        internal static Type[] GetTypesWithoutException(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Cannot get types from {assembly.FullName}: {ex.Message}\n{ex.StackTrace}");
                return Array.Empty<Type>();
            }
        }

        public static readonly Lazy<IReadOnlyCollection<Assembly>> ALL_ASSEMBLIES = new Lazy<IReadOnlyCollection<Assembly>>(
            () => new ReadOnlyCollection<Assembly>(AppDomain.CurrentDomain.GetAssemblies())
        );

        public static readonly Lazy<IReadOnlyCollection<Type>> ALL_TYPES = new Lazy<IReadOnlyCollection<Type>>(
            () => new ReadOnlyCollection<Type>(ALL_ASSEMBLIES.Value.SelectMany(assembly => assembly.GetTypesWithoutException()).ToArray())
        );

        public static readonly Lazy<IReadOnlyCollection<Type>> BEHAVIOR_TREE_ASSEMBLY_TYPES = new Lazy<IReadOnlyCollection<Type>>(
            () => new ReadOnlyCollection<Type>(typeof(VirtualMachine).Assembly.GetTypesIncludeReference().ToArray())
          , LazyThreadSafetyMode.PublicationOnly
        );

        public static IEnumerable<Type> GetTypesIncludeReference(this Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            return ALL_ASSEMBLIES.Value
                .Where(asm => asm.GetReferencedAssemblies().Any(name => name.Name == assemblyName))
                .Append(assembly)
                .SelectMany(asm => asm.GetTypesWithoutException())
                .Distinct()
            ;
        }

        [Pure]
        public static ISet<Assembly> FindReferenceAssemblies([NotNull] this Assembly assembly)
        {
            var assemblies = new HashSet<Assembly>();
            FindReferences(assembly);
            return assemblies;

            void FindReferences(Assembly asm)
            {
                if (asm == null || assemblies.Contains(asm)) return;
                assemblies.Add(asm);
                foreach (var referenceAssemblyName in asm.GetReferencedAssemblies())
                {
                    var referenceAssembly = AppDomain.CurrentDomain.Load(referenceAssemblyName);
                    FindReferences(referenceAssembly);
                }
            }
        }

        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        [Pure] public static int GuidHashCode(string guid) => GuidHashCode(Guid.Parse(guid));
        [Pure] public static int GuidHashCode(Guid guid) => guid.GetHashCode();

        // https://stackoverflow.com/a/5461399
        [Pure]
        public static bool IsAssignableFromGeneric(this Type genericType, Type givenType)
        {
            while (true)
            {
                var interfaceTypes = givenType.GetInterfaces();
                if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType)) return true;
                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) return true;
                var baseType = givenType.BaseType;
                if (baseType == null) return false;
                givenType = baseType;
            }
        }

        [Pure]
        public static string GetCurrentFilePath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return fileName;
        }

        [Pure]
        public static string GetCurrentDirectoryPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return Path.GetDirectoryName(fileName);
        }

        [Pure]
        public static string GetCurrentDirectoryProjectRelativePath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
        {
            return GetProjectRelativePath(Path.GetDirectoryName(fileName));
        }

        [Pure]
        public static string GetProjectRelativePath(string path)
        {
            var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            return path.Substring(projectPath.Length);
        }

        [Pure, NotNull]
        public static string TrimEnd([NotNull] this string str, [NotNull] string trim)
        {
            return str.EndsWith(trim) ? str.Substring(0, str.Length - trim.Length) : str;
        }
    }
}
