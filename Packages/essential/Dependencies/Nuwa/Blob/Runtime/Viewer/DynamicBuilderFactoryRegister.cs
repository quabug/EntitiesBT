using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Nuwa.Blob
{
    public static class DynamicViewerFactoryRegister
    {
        private static readonly IReadOnlyList<IDynamicViewerFactory> _factories;

#if UNITY_EDITOR
        static DynamicViewerFactoryRegister()
        {
            _factories = UnityEditor.TypeCache.GetTypesDerivedFrom<IDynamicViewerFactory>()
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Select(Activator.CreateInstance).Cast<IDynamicViewerFactory>()
                .OrderBy(factory => factory.Order)
                .ToArray()
            ;
        }
#endif

        [CanBeNull] public static object Create([NotNull] Type dataType, [CanBeNull] FieldInfo fieldInfo)
        {
            return _factories.FirstOrDefault(f => f.IsValid(dataType, fieldInfo))?.Create(dataType, fieldInfo);
        }

        [CanBeNull] public static IDynamicViewerFactory FindFactory([NotNull] Type dataType, [CanBeNull] FieldInfo fieldInfo)
        {
            return _factories.FirstOrDefault(f => f.IsValid(dataType, fieldInfo));
        }
    }
}