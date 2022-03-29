using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class VariantValueTypeAttribute : Attribute
    {
        public Type Type { get; }
        public int Id { get; }

        public VariantValueTypeAttribute(Type type, string guid)
        {
            Type = type;
            Id = GuidHashCode(guid);
        }

        public VariantValueTypeAttribute(Type type, int id)
        {
            Type = type;
            Id = id;
        }
    }

    public static class VariantValueTypeRegistry
    {
        private static readonly IReadOnlyDictionary<int, Type> _idTypeMap;
        private static readonly IReadOnlyDictionary<Type, int> _typeIdMap;

        static VariantValueTypeRegistry()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetCustomAttributes<VariantValueTypeAttribute>())
            ;

            var idTypeMap = new Dictionary<int, Type>();
            var typeIdMap = new Dictionary<Type, int>();

            foreach (var attribute in types)
            {
                idTypeMap.Add(attribute.Id, attribute.Type);
                typeIdMap.Add(attribute.Type, attribute.Id);
            }

            _idTypeMap = idTypeMap;
            _typeIdMap = typeIdMap;
        }

        public static IEnumerable<Type> GetAllTypes() => _typeIdMap.Keys;

        public static Type GetTypeById(int id)
        {
            _idTypeMap.TryGetValue(id, out var type);
            return type;
        }

        public static int GetIdByType(Type type)
        {
            return _typeIdMap.TryGetValue(type, out var id) ? id : throw new VariantValueTypeNotRegisteredException(type);
        }
    }

    [Serializable]
    public class VariantValueTypeNotRegisteredException : Exception
    {
        public Type Type { get; }

        public VariantValueTypeNotRegisteredException(Type type)
            : base($"{type.FullName} must be register into {nameof(VariantValueTypeRegistry)} by {nameof(VariantValueTypeAttribute)}")
        {
            Type = type;
        }
    }
}