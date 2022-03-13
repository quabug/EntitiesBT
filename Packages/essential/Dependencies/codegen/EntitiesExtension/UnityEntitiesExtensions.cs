using System;
using Mono.Cecil;
using Unity.Entities.CodeGen;

namespace EntitiesBT.CodeGen.Editor
{
    public static class UnityEntitiesExtensions
    {
        public static bool IsValueType(this TypeReference typeReference) =>
            TypeReferenceExtensions.IsValueType(typeReference);

        public static bool TypeImplements(this TypeReference typeReference, Type interfaceType) =>
            TypeReferenceExtensions.TypeImplements(typeReference, interfaceType);

        /// <summary>
        /// Generates a closed/specialized MethodReference for the given method and types[]
        /// e.g.
        /// struct Foo { T Bar<T>(T val) { return default(T); }
        ///
        /// In this case, if one would like a reference to "Foo::int Bar(int val)" this method will construct such a method
        /// reference when provided the open "T Bar(T val)" method reference and the TypeReferences to the types you'd like
        /// specified as generic arguments (in this case a TypeReference to "int" would be passed in).
        /// </summary>
        /// <param name="method"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static MethodReference MakeGenericInstanceMethod(this MethodReference method,
            params TypeReference[] types)
        {
            var result = new GenericInstanceMethod(method);
            foreach (var type in types)
                result.GenericArguments.Add(type);
            return result;
        }

        /// <summary>
        /// Allows one to generate reference to a method contained in a generic type which has been closed/specialized.
        /// e.g.
        /// struct Foo<T> { T Bar(T val) { return default(T); }
        ///
        /// In this case, if one would like a reference to "Foo<int>::int Bar(int val)" this method will construct such a method
        /// reference when provided the open "T Bar(T val)" method reference and the closed declaring TypeReference, "Foo<int>".
        /// </summary>
        /// <param name="self"></param>
        /// <param name="closedDeclaringType">See summary above for example. Typically construct this type using `MakeGenericInstanceMethod`</param>
        /// <returns></returns>
        public static MethodReference MakeGenericHostMethod(this MethodReference self, TypeReference closedDeclaringType)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, closedDeclaringType)
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (var genericParam in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
            }

            return reference;
        }
    }
}