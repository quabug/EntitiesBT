using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Collections.Generic;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace EntitiesBT.CodeGen.Editor
{
    internal static class CecilExtension
    {
        [Pure]
        public static bool HasAttribute<T>([NotNull] this TypeDefinition typeDefinition) where T : Attribute
        {
            return HasAttribute(typeDefinition, typeof(T));
        }

        [Pure]
        public static bool HasAttribute([NotNull] this TypeDefinition typeDefinition, [NotNull] Type attributeType)
        {
	        return typeDefinition.CustomAttributes.HasAttribute(attributeType);
        }

        [Pure]
        public static bool HasAttribute([NotNull] this IEnumerable<CustomAttribute> attributes, [NotNull] Type attributeType)
        {
            return attributes.Any(attribute => attributeType.IsAssignableFrom(attribute.AttributeType));
        }

        [Pure]
        public static bool HasAttribute<T>([NotNull] this IEnumerable<CustomAttribute> attributes) where T : Attribute
        {
	        return HasAttribute(attributes, typeof(T));
        }

        [Pure]
        public static bool IsAssignableFrom([NotNull] this Type type, TypeReference typeReference)
        {
	        return type.IsAssignableFrom(typeReference.Resolve().ToReflectionType());
        }

        [Pure]
        public static T GetAttribute<T>([NotNull] this TypeDefinition typeDefinition) where T : Attribute
        {
	        var customAttribute = typeDefinition
		        .CustomAttributes
		        .Single(attribute => attribute.AttributeType.Resolve().ToReflectionType() == typeof(T))
			;
	        return customAttribute.ToAttribute<T>();
        }

        [Pure]
        public static T ToAttribute<T>([NotNull] this CustomAttribute attribute) where T : Attribute
        {
	        var attributeType = typeof(T);
	        var constructorParameters = attributeType.GetConstructors().Single().GetParameters();
	        if (constructorParameters.Length != attribute.ConstructorArguments.Count)
		        throw new ArgumentException($"Invalid constructor of type {attributeType}");
	        var arguments = new object[constructorParameters.Length];
	        for (var i = 0; i < constructorParameters.Length; i++)
	        {
		        var argumentType = constructorParameters[i].ParameterType;
		        var value = attribute.ConstructorArguments[i].Value;
		        arguments[i] = argumentType.IsEnum ? Enum.ToObject(argumentType, value) : value;
	        }
	        return (T) Activator.CreateInstance(attributeType, arguments);
        }

        [Pure]
        public static Type ToReflectionType([NotNull] this TypeDefinition typeDefinition)
        {
            var fullName = Assembly.CreateQualifiedName(typeDefinition.Module.Assembly.FullName, typeDefinition.FullName);
            var type = Type.GetType(fullName);
            if (type == null) throw new ArgumentException($"Cannot get type of {fullName}");
            return type;
        }

        // [Pure]
        // public static Type ToReflectionType([NotNull] this TypeReference typeReference)
        // {
	       //  // force resolve the reference before `GetType`
	       //  typeReference.Resolve();
        //     var fullName = Assembly.CreateQualifiedName(typeReference.Module.Assembly.FullName, typeReference.FullName);
        //     return Type.GetType(fullName);
        // }

        [Pure]
        public static TypeDefinition FindTypeDefinition<T>([NotNull] this AssemblyDefinition assembly)
        {
            return FindTypeDefinition(assembly, typeof(T));
        }

        [Pure]
        public static TypeDefinition FindTypeDefinition([NotNull] this AssemblyDefinition assemblyDefinition, [NotNull] Type reflectionType)
        {
            var stack = new Stack<string>();
            var currentType = reflectionType;
            while (currentType != null) {
                stack.Push((currentType.DeclaringType == null ? currentType.Namespace + "." : "") + currentType.Name);
                currentType = currentType.DeclaringType;
            }

            var typeDefinition = assemblyDefinition.MainModule.GetType(stack.Pop());
            if  (typeDefinition == null)
                return null;

            while  (stack.Count > 0) {
                var name = stack.Pop();
                typeDefinition = typeDefinition.NestedTypes.Single(t => t.Name == name);
            }

            return typeDefinition;
        }

        [Pure]
        public static MethodDefinition GetMethod([NotNull] this TypeDefinition self, [NotNull] string name)
		{
			return self.Methods.First(m => m.Name == name);
		}

        [Pure]
        public static void AddRange<T>([NotNull] this Collection<T> self, [NotNull, ItemNotNull] IEnumerable<T> items)
		{
			foreach (var item in items) self.Add(item);
		}

        [Pure]
		public static FieldDefinition GetField([NotNull] this TypeDefinition self, [NotNull] string name)
		{
			return self.Fields.First(f => f.Name == name);
		}

		public static TypeDefinition CreateStructTypeDefinition(
			[NotNull] this ModuleDefinition moduleDefinition
			, [NotNull] string @namespace
			, [NotNull] string name
			, TypeAttributes additionalAttributes = TypeAttributes.Public
		)
		{
            //.class public sealed sequential ansi beforefieldinit
            //  StructName
            //    extends [netstandard]System.ValueType
            //{
            //} // end of class StructName
            var typeAttributes = TypeAttributes.Class
                                 | TypeAttributes.Sealed
                                 | TypeAttributes.AnsiClass
                                 | TypeAttributes.BeforeFieldInit
                                 | additionalAttributes
			;
            var isExplicitLayout = (typeAttributes & TypeAttributes.ExplicitLayout) == TypeAttributes.ExplicitLayout;
            if (!isExplicitLayout) typeAttributes |= TypeAttributes.SequentialLayout;
            return new TypeDefinition(@namespace, name, typeAttributes)
            {
	            BaseType = moduleDefinition.ImportReference(typeof(ValueType))
            };
		}

        private const string _AUTO_PROPERTY_SUFFIX = ">k__BackingField";
        private const string _AUTO_PROPERTY_PREFIX = "<";

        public static bool IsAutoPropertyField(this FieldDefinition fieldDefinition)
        {
            return fieldDefinition.Name.StartsWith(_AUTO_PROPERTY_PREFIX);
        }

        public static string GetCodeName(this FieldDefinition fieldDefinition)
        {
	        var name = fieldDefinition.Name;
	        if (fieldDefinition.IsAutoPropertyField())
	        {
		        var prefixLength = _AUTO_PROPERTY_PREFIX.Length;
		        var suffixLength = _AUTO_PROPERTY_SUFFIX.Length;
		        name = name.Substring(prefixLength, name.Length - prefixLength - suffixLength);
	        }
	        return name;
        }
        public static bool IsINodeDataStruct(this TypeDefinition typeDefinition) =>
            typeDefinition.TypeImplements(typeof(INodeData)) && typeDefinition.IsValueType();
    }
}