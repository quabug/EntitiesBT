using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        
        public static IEnumerable<T> Yield<T>(this T self)
        {
            yield return self;
        }

        public static IEnumerable<ITreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc)
        {
            return node.Flatten(childrenFunc, default(ITreeNode<T>)).Select((treeNode, i) => (ITreeNode<T>)treeNode.UpdateIndex(i));
        }

        private static IEnumerable<TreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, ITreeNode<T> parent)
        {
            if (node == null) return Enumerable.Empty<TreeNode<T>>();
            var treeNode = new TreeNode<T>(node, parent);
            return treeNode.Yield().Concat(childrenFunc(node).SelectMany(child => child.Flatten(childrenFunc, treeNode)));
        }
        
        public static IEnumerable<ITreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, SelfFunc<T> selfFunc)
        {
            return selfFunc(node).Flatten(childrenFunc, default, selfFunc).Select((treeNode, i) => (ITreeNode<T>)treeNode.UpdateIndex(i));
        }

        private static IEnumerable<TreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, ITreeNode<T> parent, SelfFunc<T> selfFunc)
        {
            if (node == null) return Enumerable.Empty<TreeNode<T>>();
            var treeNode = new TreeNode<T>(node, parent);
            return treeNode.Yield().Concat(childrenFunc(node).SelectMany(child => selfFunc(child).Flatten(childrenFunc, treeNode, selfFunc)));
        }

        public static IEnumerable<float> NormalizeUnsafe(this IEnumerable<float> weights)
        {
            var sum = weights.Sum();
            return weights.Select(w => w / sum);
        }
        
        public static IEnumerable<float> Normalize(this IEnumerable<float> weights)
        {
            var sum = weights.Where(w => w > 0).Sum();
            if (sum <= math.FLT_MIN_NORMAL) sum = 1;
            return weights.Select(w => math.max(w, 0) / sum);
        }
        
        // https://stackoverflow.com/a/27851610
        public static bool IsZeroSizeStruct(this Type t)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return t.IsValueType
                   && !t.IsPrimitive
                   && t.GetFields(flags).All(fi => IsZeroSizeStruct(fi.FieldType))
            ;
        }
    }
}
