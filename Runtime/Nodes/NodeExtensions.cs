using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using JetBrains.Annotations;

namespace EntitiesBT.Nodes
{
    public static class NodeExtensions
    {
        public static void ResetChildren([NotNull] this INodeBlob blob, int parentIndex, IBlackboard bb)
        {
            // TODO: reset children which is not NodeState.None?
            var firstChildIndex = parentIndex + 1;
            var lastChildIndex = blob.GetEndIndex(parentIndex);
            var childCount = lastChildIndex - firstChildIndex;
            VirtualMachine.Reset(firstChildIndex, blob, bb, childCount);
        }
        
        public static IEnumerable<NodeState> TickChildren(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
        )
        {
            return TickChildren(blob, parentIndex, bb, state => false, state => !state.IsCompleted());
        }
        
        [LinqTunnel]
        public static IEnumerable<NodeState> TickChildren(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
          , Predicate<NodeState> breakCheck
        )
        {
            return TickChildren(blob, parentIndex, bb, breakCheck, state => !state.IsCompleted());
        }
        
        [LinqTunnel]
        public static IEnumerable<NodeState> TickChildren(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
          , Predicate<NodeState> breakCheck
          , Predicate<NodeState> tickCheck
        )
        {
            foreach (var childIndex in blob.GetChildrenIndices(parentIndex))
            {
                var prevState = blob.GetState(childIndex);
                var currentState = tickCheck(prevState) ? VirtualMachine.Tick(childIndex, blob, bb) : 0;
                yield return currentState;
                if (breakCheck(currentState == 0 ? prevState : currentState)) yield break;
            }
        }

        #region GC free

        // GC free version of TickChildren().LastOrDefault()
        [LinqTunnel]
        public static NodeState TickChildrenReturnLastOrDefault(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
          , Predicate<NodeState> breakCheck
        )
        {
            return TickChildrenReturnBreakOrDefault(blob, parentIndex, bb, breakCheck, state => !state.IsCompleted());
        }
        
        // GC free version of TickChildren().FirstOrDefault()
        public static NodeState TickChildrenReturnFirstOrDefault(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
        )
        {
            return TickChildrenReturnBreakOrDefault(blob, parentIndex, bb, state => true, state => !state.IsCompleted());
        }
        
        [LinqTunnel]
        private static NodeState TickChildrenReturnBreakOrDefault(
            [NotNull] this INodeBlob blob
          , int parentIndex
          , [NotNull] IBlackboard bb
          , Predicate<NodeState> breakCheck
          , Predicate<NodeState> tickCheck
        )
        {
            NodeState currentState = 0;
            var endIndex = blob.GetEndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                var prevState = blob.GetState(childIndex);
                currentState = tickCheck(prevState) ? VirtualMachine.Tick(childIndex, blob, bb) : 0;
                if (breakCheck(currentState == 0 ? prevState : currentState)) break;
                childIndex = blob.GetEndIndex(childIndex);
            }
            return currentState;
        }

        [Pure]
        public static T FirstOrDefault<T>([NoEnumeration, NotNull] this IEnumerable<T> enumerable)
        {
            return enumerable.SingleOrDefault();
        }

        [Pure]
        public static T LastOrDefault<T>([NotNull] this IEnumerable<T> enumerable)
        {
            var result = default(T);
            foreach (var item in enumerable) result = item;
            return result;
        }
        

        #endregion
    }
}
