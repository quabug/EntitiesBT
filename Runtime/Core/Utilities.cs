using System.Collections.Generic;
using System.Linq;

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
        
        public static IEnumerable<T> Yield<T>(this T self)
        {
            yield return self;
        }

        public static IEnumerable<ITreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc)
        {
            return node.Flatten(childrenFunc, default).Select((treeNode, i) => (ITreeNode<T>)treeNode.UpdateIndex(i));
        }

        private static IEnumerable<TreeNode<T>> Flatten<T>(this T node, ChildrenFunc<T> childrenFunc, ITreeNode<T> parent)
        {
            var treeNode = new TreeNode<T>(node, parent);
            return treeNode.Yield().Concat(childrenFunc(node).SelectMany(child => child.Flatten(childrenFunc, treeNode)));
        }
    }
}
