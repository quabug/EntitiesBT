using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Nuwa.Blob.Editor;
using UnityEditor;

namespace Nuwa.Blob
{
    public static class Extensions
    {
        public static BuilderFactory FindBuilderCreator([NotNull] this FieldInfo fieldInfo)
        {
            var customBuilder = fieldInfo.GetCustomAttribute<CustomBuilderAttribute>()?.BuilderType;
            return GetBuilderFactory(fieldInfo.FieldType, customBuilder);
        }

        public static BuilderFactory GetBuilderFactory([NotNull] this Type valueType, Type customBuilder)
        {
            var builderType = typeof(Builder<>).MakeGenericType(valueType);
            var builders = TypeCache.GetTypesDerivedFrom(builderType);
            if (customBuilder == null && builders.Count == 1) return new BuilderFactory(builders[0]);
            if (customBuilder != null && builders.Contains(customBuilder)) return new BuilderFactory(customBuilder);
            if (customBuilder != null)
                throw new InvalidCustomBuilderException(
                    $"Invalid {customBuilder.Name} of {valueType.Name}, must be one of [{string.Join(",", builders.Select(b => b.Name))}]");

            try
            {
                var defaultBuilder = builders.SingleOrDefault(b => b.GetCustomAttribute<DefaultBuilderAttribute>() != null);
                return defaultBuilder == null ? DynamicBuilder() : new BuilderFactory(defaultBuilder);
            }
            catch (Exception ex)
            {
                throw new MultipleBuildersException(
                    $"There's multiple builders [{string.Join(",", builders.Select(b => b.Name))}] for {valueType.Name}, must mark one of them as `DefaultBuilder` or use `CustomBuilder` on this field",
                    ex);
            }

            BuilderFactory DynamicBuilder()
            {
                var factory = DynamicBuilderFactoryRegister.FindFactory(valueType);
                if (factory == null) throw new ArgumentException($"cannot find any builder for {valueType}");
                return new BuilderFactory(factory.BuilderType, () => factory.Create(valueType));
            }
        }
    }

    [Serializable]
    public class InvalidCustomBuilderException : Exception
    {
        public InvalidCustomBuilderException() {}
        public InvalidCustomBuilderException(string message) : base(message) {}
        public InvalidCustomBuilderException(string message, Exception inner) : base(message, inner) {}
        protected InvalidCustomBuilderException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    [Serializable]
    public class MultipleBuildersException : Exception
    {
        public MultipleBuildersException() {}
        public MultipleBuildersException(string message) : base(message) {}
        public MultipleBuildersException(string message, Exception inner) : base(message, inner) {}
        protected MultipleBuildersException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}