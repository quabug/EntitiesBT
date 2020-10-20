using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public interface IVariablePropertyReader<T> where T : unmanaged
    {
        void Allocate(
            ref BlobBuilder builder
          , ref BlobVariableReader<T> blobVariable
          , INodeDataBuilder self
          , ITreeNode<INodeDataBuilder>[] tree
        );
    }

    public static class VariablePropertyExtensions
    {
        public static unsafe void Allocate<T>(
            this IVariablePropertyReader<T> variable
          , ref BlobBuilder builder
          , void* blobVariablePtr
          , [NotNull] INodeDataBuilder self
          , [NotNull] ITreeNode<INodeDataBuilder>[] tree
        ) where T : unmanaged
        {
            variable.Allocate(ref builder, ref UnsafeUtility.AsRef<BlobVariableReader<T>>(blobVariablePtr), self, tree);
        }

        public static MethodInfo Getter(this Type type, string name)
        {
            var methodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsTrue(methodInfo.IsGenericMethod);
            Assert.AreEqual(2, methodInfo.GetGenericArguments().Length);
            return methodInfo;
        }
        
        public static readonly Lazy<Type[]> VARIABLE_PROPERTY_TYPES = new Lazy<Type[]>(() =>
        {
            var variableAssembly = typeof(IVariablePropertyReader<>).Assembly;
            var variableAssemblyName = variableAssembly.GetName().Name;
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.GetReferencedAssemblies().Any(name => name.Name == variableAssemblyName))
                .Append(variableAssembly)
                .SelectMany(assembly => assembly.GetTypesWithoutException())
                .Where(type => !type.IsAbstract
                               && type.IsGenericType
                               && type.GetGenericArguments().Length == 1
                               && typeof(IVariablePropertyReader<>).IsAssignableFrom(type)
                ).ToArray()
            ;
        });
    }
}
