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
    public interface IVariableProperty<T> where T : unmanaged
    {
        void Allocate(
            ref Unity.Entities.BlobBuilder builder
          , ref EntitiesBT.Variable.BlobVariable<T> blobVariable
          , EntitiesBT.Core.INodeDataBuilder self
          , EntitiesBT.Core.ITreeNode<EntitiesBT.Core.INodeDataBuilder>[] tree
        );
    }

    public abstract class VariableProperty<T> : IVariableProperty<T> where T : unmanaged
    {
        public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public;
        
        public virtual void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, [NotNull] INodeDataBuilder self, [NotNull] ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = VariablePropertyTypeId;
            AllocateData(ref builder, ref blobVariable, self, tree);
        }
        protected virtual void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
        public virtual int VariablePropertyTypeId => 0;
    }

    public static class VariablePropertyExtensions
    {
        public static unsafe void Allocate<T>(
            this VariableProperty<T> variable
          , ref BlobBuilder builder
          , void* blobVariablePtr
          , [NotNull] INodeDataBuilder self
          , [NotNull] ITreeNode<INodeDataBuilder>[] tree
        ) where T : unmanaged
        {
            variable.Allocate(ref builder, ref UnsafeUtility.AsRef<BlobVariable<T>>(blobVariablePtr), self, tree);
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
            var variableAssembly = typeof(VariableProperty<>).Assembly;
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
                               && type.IsSubclassOfRawGeneric(typeof(VariableProperty<>))
                ).ToArray()
            ;
        });
        
        private static bool IsSubclassOfRawGeneric(this Type type, Type generic) {
            while (type != null && type != typeof(object)) {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == cur) return true;
                type = type.BaseType;
            }
            return false;
        }
    }
}
