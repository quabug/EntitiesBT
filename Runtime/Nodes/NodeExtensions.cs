using System;
using System.Collections.Generic;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class NodeExtensions
    {
        public static void ResetChildren(this INodeBlob blob, int index, IBlackboard bb)
        {
            // TODO: reset children which is not NodeState.None?
            VirtualMachine.Reset(index, blob, bb, blob.GetEndIndex(index) - index);
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int index
          , IBlackboard bb
        )
        {
            return TickChildren(blob, index, bb, state => false, state => !state.IsCompleted());
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int index
          , IBlackboard bb
          , Predicate<NodeState> breakCheck
        )
        {
            return TickChildren(blob, index, bb, breakCheck, state => !state.IsCompleted());
        }
        
        public static IEnumerable<NodeState> TickChildren(
            this INodeBlob blob
          , int index
          , IBlackboard bb
          , Predicate<NodeState> breakCheck
          , Predicate<NodeState> tickCheck
        )
        {
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                var prevState = blob.GetState(childIndex);
                var currentState = tickCheck(prevState) ? VirtualMachine.Tick(childIndex, blob, bb) : 0;
                yield return currentState;
                if (breakCheck(currentState == 0 ? prevState : currentState)) yield break;
            }
        }
    }
}
