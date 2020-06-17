using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Core
{
    public interface ITreeNode<out T>
    {
        T Value { get; }
        ITreeNode<T> Parent { get; }
        int Index { get; }
    }
        
    public class TreeNode<T> : ITreeNode<T>
    {
        public T Value { get; }
        public ITreeNode<T> Parent { get; }
        public int Index { get; private set; }

        public TreeNode(T value, ITreeNode<T> parent, int index = -1)
        {
            Value = value;
            Parent = parent;
            Index = index;
        }

        public TreeNode<T> UpdateIndex(int index)
        {
            Index = index;
            return this;
        }
    }
    
    public static class Utilities
    {
        public delegate IEnumerable<T> ChildrenFunc<T>(T parent);
        public delegate T SelfFunc<T>(T self);
        
        [Pure]
        public static IEnumerable<T> Yield<T>(this T self)
        {
            yield return self;
        }

        [Pure]
        public static IEnumerable<ITreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc)
        {
            return node.Flatten(childrenFunc, default(ITreeNode<T>)).Select((treeNode, i) => (ITreeNode<T>)treeNode.UpdateIndex(i));
        }

        [Pure]
        private static IEnumerable<TreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, ITreeNode<T> parent)
        {
            if (node == null) return Enumerable.Empty<TreeNode<T>>();
            var treeNode = new TreeNode<T>(node, parent);
            return treeNode.Yield().Concat(childrenFunc(node).SelectMany(child => child.Flatten(childrenFunc, treeNode)));
        }
        
        [Pure]
        public static IEnumerable<ITreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, SelfFunc<T> selfFunc)
        {
            return selfFunc(node).Flatten(childrenFunc, default, selfFunc).Select((treeNode, i) => (ITreeNode<T>)treeNode.UpdateIndex(i));
        }

        [Pure]
        private static IEnumerable<TreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, ITreeNode<T> parent, SelfFunc<T> selfFunc)
        {
            if (node == null) return Enumerable.Empty<TreeNode<T>>();
            var treeNode = new TreeNode<T>(node, parent);
            return treeNode.Yield().Concat(childrenFunc(node).SelectMany(child => selfFunc(child).Flatten(childrenFunc, treeNode, selfFunc)));
        }

        [Pure]
        public static IEnumerable<float> NormalizeUnsafe([NotNull] this IEnumerable<float> weights)
        {
            var sum = weights.Sum();
            return weights.Select(w => w / sum);
        }
        
        [Pure]
        public static IEnumerable<float> Normalize([NotNull] this IEnumerable<float> weights)
        {
            var sum = weights.Where(w => w > 0).Sum();
            if (sum <= math.FLT_MIN_NORMAL) sum = 1;
            return weights.Select(w => math.max(w, 0) / sum);
        }
        
        // https://stackoverflow.com/a/27851610
        [Pure]
        public static bool IsZeroSizeStruct([NotNull] this Type t)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return t.IsValueType
                   && !t.IsPrimitive
                   && t.GetFields(flags).All(fi => IsZeroSizeStruct(fi.FieldType))
            ;
        }

        public static Type[] GetTypesWithoutException(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Cannot get types from {assembly.FullName}: {ex.Message}\n{ex.StackTrace}");
                return Array.Empty<Type>();
            }
        }

        public static readonly Lazy<Type[]> BEHAVIOR_TREE_ASSEMBLY_TYPES = new Lazy<Type[]>(() =>
            typeof(VirtualMachine).Assembly.GetTypesIncludeReference().ToArray()
        );

        public static IEnumerable<Type> GetTypesIncludeReference(this Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(asm => asm.GetReferencedAssemblies().Any(name => name.Name == assemblyName))
                .Append(assembly)
                .SelectMany(asm => asm.GetTypesWithoutException())
                .Distinct()
            ;
        }
    }
}
