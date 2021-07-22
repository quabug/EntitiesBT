using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant.Expression
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExpressionTypeAttribute : Attribute
    {
        public Type Type { get; }
        public int Id { get; }
        internal bool HasId { get; } = true;

        public ExpressionTypeAttribute(Type type, string guid)
        {
            Type = type;
            Id = GuidHashCode(guid);
        }

        public ExpressionTypeAttribute(Type type, int id)
        {
            Type = type;
            Id = id;
        }

        public ExpressionTypeAttribute(Type type)
        {
            Type = type;
            HasId = false;
        }
    }

    public static class ExpressionReferenceTypeRegistry
    {
        private static readonly IReadOnlyDictionary<int, Type> _idTypeMap;
        private static readonly IReadOnlyDictionary<Type, int> _typeIdMap;
        private static readonly IReadOnlyList<Type> _referenceTypes;

        static ExpressionReferenceTypeRegistry()
        {
            var expressionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetCustomAttributes<ExpressionTypeAttribute>())
            ;

            var idTypeMap = new Dictionary<int, Type>();
            var typeIdMap = new Dictionary<Type, int>();
            var referenceTypes = new List<Type>();

            foreach (var attribute in expressionTypes)
            {
                if (attribute.HasId)
                {
                    idTypeMap.Add(attribute.Id, attribute.Type);
                    typeIdMap.Add(attribute.Type, attribute.Id);
                }
                referenceTypes.Add(attribute.Type);
            }

            _idTypeMap = idTypeMap;
            _typeIdMap = typeIdMap;
            _referenceTypes = referenceTypes;
        }

        public static IReadOnlyList<Type> GetReferenceTypes() => _referenceTypes;

        public static Type GetTypeById(int id)
        {
            _idTypeMap.TryGetValue(id, out var type);
            return type;
        }

        public static int GetIdByType(Type type)
        {
            return _typeIdMap.TryGetValue(type, out var id) ? id : throw new ExpressionTypeNotRegisteredException(type);
        }
    }

    [Serializable]
    public class ExpressionTypeNotRegisteredException : Exception
    {
        public Type Type { get; }

        public ExpressionTypeNotRegisteredException(Type type)
            : base($"{type.FullName} must be register into {nameof(ExpressionReferenceTypeRegistry)} by {nameof(ExpressionTypeAttribute)}")
        {
            Type = type;
        }
    }
}