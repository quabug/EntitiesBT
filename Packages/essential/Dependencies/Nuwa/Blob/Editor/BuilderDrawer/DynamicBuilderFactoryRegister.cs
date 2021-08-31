using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;

namespace Nuwa.Blob.Editor
{
    public static class DynamicBuilderFactoryRegister
    {
        private static IReadOnlyList<IDynamicBuilderFactory> _factories;

        static DynamicBuilderFactoryRegister()
        {
            _factories = TypeCache.GetTypesDerivedFrom<IDynamicBuilderFactory>()
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Select(Activator.CreateInstance).Cast<IDynamicBuilderFactory>()
                .OrderBy(factory => factory.Order)
                .ToArray()
            ;
        }

        [CanBeNull] public static object Create([NotNull] Type dataType, [CanBeNull] FieldInfo fieldInfo)
        {
            return _factories.FirstOrDefault(f => f.IsValid(dataType, fieldInfo))?.Create(dataType, fieldInfo);
        }

        [CanBeNull] public static IDynamicBuilderFactory FindFactory([NotNull] Type dataType, [CanBeNull] FieldInfo fieldInfo)
        {
            return _factories.FirstOrDefault(f => f.IsValid(dataType, fieldInfo));
        }
    }
}