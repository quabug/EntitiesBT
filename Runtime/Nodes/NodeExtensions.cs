using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using JetBrains.Annotations;

namespace EntitiesBT.Nodes
{
    public static class NodeExtensions
    {
        public static void ResetChildren<TNodeBlob, TBlackboard>(this int parentIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            // TODO: reset children which is not NodeState.None?
            var firstChildIndex = parentIndex + 1;
            var lastChildIndex = blob.GetEndIndex(parentIndex);
            var childCount = lastChildIndex - firstChildIndex;
            VirtualMachine.Reset(firstChildIndex, ref blob, ref bb, childCount);
        }
        
        // GC free version of TickChildren().LastOrDefault()
        [LinqTunnel]
        public static NodeState TickChildrenReturnLastOrDefault<TNodeBlob, TBlackboard>(
          this int parentIndex
          , ref TNodeBlob blob
          , ref TBlackboard bb
          , Predicate<NodeState> breakCheck
        )
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return TickChildrenReturnBreakOrDefault(parentIndex, ref blob, ref bb, breakCheck, state => !state.IsCompleted());
        }
        
        // GC free version of TickChildren().FirstOrDefault()
        public static NodeState TickChildrenReturnFirstOrDefault<TNodeBlob, TBlackboard>(
          this int parentIndex
          , ref TNodeBlob blob
          , ref TBlackboard bb
        )
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return TickChildrenReturnBreakOrDefault(parentIndex, ref blob, ref bb, state => true, state => !state.IsCompleted());
        }
        
        [LinqTunnel]
        private static NodeState TickChildrenReturnBreakOrDefault<TNodeBlob, TBlackboard>(
          this int parentIndex
          , ref TNodeBlob blob
          , ref TBlackboard bb
          , Predicate<NodeState> breakCheck
          , Predicate<NodeState> tickCheck
        )
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            NodeState currentState = 0;
            var endIndex = blob.GetEndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                var prevState = blob.GetState(childIndex);
                currentState = tickCheck(prevState) ? VirtualMachine.Tick(childIndex, ref blob, ref bb) : 0;
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
    }
}
