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
    }
}