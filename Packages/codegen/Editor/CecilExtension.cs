using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using ICustomAttributeProvider = Mono.Cecil.ICustomAttributeProvider;
using MethodAttributes = Mono.Cecil.MethodAttributes;
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

        [Pure, CanBeNull]
        public static MethodDefinition GetMethodNullable([NotNull] this TypeDefinition self, [NotNull] string name)
		{
			return self.Methods.FirstOrDefault(m => m.Name == name);
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

        public static string ToReadableName(this TypeReference type)
        {
            if (!type.IsGenericInstance) return type.Name;
            return $"{type.Name.Split('`')[0]}<{string.Join(",", ((GenericInstanceType)type).GenericArguments.Select(a => a.Name))}>";
        }

        public static IReadOnlyList<TypeReference> ResolveGenericArguments(this TypeDefinition self, TypeReference @base)
        {
            if (!@base.IsGenericInstance)
                return self.IsGenericInstance ? self.GenericParameters.ToArray() : Array.Empty<TypeReference>();

            var genericBase = (GenericInstanceType) @base;
            var selfBase = ParentTypes(self).Where(p => IsTypeEqual(p, @base))
                // let it throw
                .Select(p => (GenericInstanceType)p)
                .First(p => IsPartialGenericMatch(p, genericBase))
            ;
            var genericArguments = new TypeReference[self.GenericParameters.Count];
            var selfBaseGenericArguments = selfBase.GenericArguments.ToList();
            for (var i = 0; i < self.GenericParameters.Count; i++)
            {
                var genericParameter = self.GenericParameters[i];
                var index = selfBaseGenericArguments.FindIndex(type => type.Name == genericParameter.Name);
                if (index >= 0) genericArguments[i] = genericBase.GenericArguments[index];
                else genericArguments[i] = self.GenericParameters[i];
            }
            return genericArguments;
        }

        public static IEnumerable<TypeReference> ParentTypes(this TypeDefinition type)
        {
            var parents = Enumerable.Empty<TypeReference>();
            if (type.HasInterfaces) parents = type.Interfaces.Select(i => i.InterfaceType);
            if (type.BaseType != null) parents = parents.Append(type.BaseType);
            return parents;
        }

        public static bool IsPartialGenericMatch(this GenericInstanceType partial, GenericInstanceType concrete)
        {
            if (!IsTypeEqual(partial, concrete))
                throw new ArgumentException($"{partial} and {concrete} have different type");
            if (partial.GenericArguments.Count != concrete.GenericArguments.Count)
                throw new ArgumentException($"{partial} and {concrete} have different count of generic arguments"); ;
            if (concrete.GenericArguments.Any(arg => arg.IsGenericParameter))
                throw new ArgumentException($"{concrete} is not a concrete generic type"); ;

            return partial.GenericArguments
                .Zip(concrete.GenericArguments, (partialArgument, concreteArgument) => (partialArgument, concreteArgument))
                .All(t => t.partialArgument.IsGenericParameter || IsTypeEqual(t.partialArgument, t.concreteArgument))
            ;
        }

        public static bool IsTypeEqual(this TypeReference lhs, TypeReference rhs)
        {
            return IsTypeEqual(lhs.Resolve(), rhs.Resolve());
        }

        public static bool IsTypeEqual(this TypeDefinition lhs, TypeDefinition rhs)
        {
            return lhs != null && rhs != null &&
                   lhs.MetadataToken == rhs.MetadataToken &&
                   lhs.Module.Name == rhs.Module.Name
                ;
        }

        //.method public hidebysig specialname rtspecialname instance void
        //  .ctor() cil managed
        //{
        //  .maxstack 8

        //  IL_0000: ldarg.0      // this
        //  IL_0001: call         instance void class [GenericSerializeReference.Tests]GenericSerializeReference.Tests.MultipleGeneric/Object`2<int32, float32>::.ctor()
        //  IL_0006: nop
        //  IL_0007: ret

        //} // end of method Object::.ctor
        public static void AddEmptyCtor(this TypeDefinition type, MethodReference baseCtor)
        {
            var attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var ctor = new MethodDefinition(".ctor", attributes, baseCtor.ReturnType);
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseCtor));
            ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(ctor);
        }

        public static TypeReference CreateTypeReference(this TypeDefinition type, IReadOnlyList<TypeReference> genericArguments)
        {
            return type.HasGenericParameters
                ? (TypeReference) type.MakeGenericInstanceType(genericArguments.ToArray())
                : type
            ;
        }

        public static bool IsPublicOrNestedPublic(this TypeDefinition type)
        {
            foreach (var t in type.GetSelfAndAllDeclaringTypes())
            {
                if ((t.IsNested && !t.IsNestedPublic) || (!t.IsNested && !t.IsPublic)) return false;
            }
            return true;
        }

        public static string NameWithOuterClasses(this TypeDefinition type)
        {
            return type.GetSelfAndAllDeclaringTypes().Aggregate("", (name, t) => $"{t.Name}.{name}");
        }

        public static IEnumerable<TypeDefinition> GetSelfAndAllDeclaringTypes(this TypeDefinition type)
        {
            yield return type;
            while (type.DeclaringType != null)
            {
                yield return type.DeclaringType;
                type = type.DeclaringType;
            }
        }

        public static CustomAttribute AddCustomAttribute<T>(
            this ICustomAttributeProvider attributeProvider
            , ModuleDefinition module
            , params Type[] constructorTypes
        ) where T : Attribute
        {
            var attribute = new CustomAttribute(module.ImportReference(typeof(T).GetConstructor(constructorTypes)));
            attributeProvider.CustomAttributes.Add(attribute);
            return attribute;
        }

        public static TypeDefinition GenerateDerivedClass(this TypeReference baseType, IEnumerable<TypeReference> genericArguments, string className, ModuleDefinition module = null)
        {
            // .class nested public auto ansi beforefieldinit
            //   Object
            //     extends class [GenericSerializeReference.Tests]GenericSerializeReference.Tests.MultipleGeneric/Object`2<int32, float32>
            //     implements GenericSerializeReference.Tests.TestMonoBehavior/IBase
            // {

            //   .method public hidebysig specialname rtspecialname instance void
            //     .ctor() cil managed
            //   {
            //     .maxstack 8

            //     IL_0000: ldarg.0      // this
            //     IL_0001: call         instance void class [GenericSerializeReference.Tests]GenericSerializeReference.Tests.MultipleGeneric/Object`2<int32, float32>::.ctor()
            //     IL_0006: nop
            //     IL_0007: ret

            //   } // end of method Object::.ctor
            // } // end of class Object
            module ??= baseType.Module;
            var classAttributes = TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;
            var type = new TypeDefinition("", className, classAttributes);
            type.BaseType = baseType.HasGenericParameters ? baseType.MakeGenericInstanceType(genericArguments.ToArray()) : baseType;
            var ctor = module.ImportReference(baseType.Resolve().GetConstructors().First(c => !c.HasParameters)).Resolve();
            var ctorCall = new MethodReference(ctor.Name, module.ImportReference(ctor.ReturnType))
            {
                DeclaringType = type.BaseType,
                HasThis = ctor.HasThis,
                ExplicitThis = ctor.ExplicitThis,
                CallingConvention = ctor.CallingConvention,
            };
            type.AddEmptyCtor(ctorCall);
            return type;
        }

        public static TypeDefinition CreateNestedStaticPrivateClass(this TypeDefinition type, string name)
        {
            // .class nested private abstract sealed auto ansi beforefieldinit
            //   <$PropertyName>__generic_serialize_reference
            //     extends [mscorlib]System.Object
            var typeAttributes = TypeAttributes.Class |
                                 TypeAttributes.Sealed |
                                 TypeAttributes.Abstract |
                                 TypeAttributes.NestedPrivate |
                                 TypeAttributes.BeforeFieldInit;
            var nestedType = new TypeDefinition("", name, typeAttributes);
            nestedType.BaseType = type.Module.ImportReference(typeof(object));
            type.NestedTypes.Add(nestedType);
            return nestedType;
        }

        public static IEnumerable<CustomAttribute> GetAttributesOf<T>([NotNull] this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.HasCustomAttributes ?
                provider.CustomAttributes.Where(IsAttributeOf) :
                Enumerable.Empty<CustomAttribute>();

            static bool IsAttributeOf(CustomAttribute attribute) => attribute.AttributeType.FullName == typeof(T).FullName;
        }

        public static MethodReference ImportMethod(this ModuleDefinition module, Type methodDeclaringType, string methodName)
        {
            var ext = module.ImportReference(methodDeclaringType);
            var methodDefinition = ext.Resolve().Methods.First(m => m.Name == methodName);
            return module.ImportReference(methodDefinition);
        }
    }
}