using System;
using System.Collections.Generic;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class NodeExtensions
    {
        public static void ResetChildren(this INodeBlob blob, int parentIndex, IBlackboard bb)
        {
            // TODO: reset children which is not NodeState.None?
            var firstChildIndex = parentIndex + 1;
            var lastChildIndex = blob.GetEndIndex(parentIndex);
            var childCount = lastChildIndex - firstChildIndex;
            VirtualMachine.Reset(firstChildIndex, blob, bb, childCount);
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int parentIndex
          , IBlackboard bb
        )
        {
            return TickChildren(blob, parentIndex, bb, state => false, state => !state.IsCompleted());
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int parentIndex
          , IBlackboard bb
          , Predicate<NodeState> breakCheck
        )
        {
            return TickChildren(blob, parentIndex, bb, breakCheck, state => !state.IsCompleted());
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int parentIndex
          , IBlackboard bb
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
    }
}
