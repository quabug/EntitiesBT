using System;
using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    [Flags]
    public enum NodeState
    {
        Success = 1 << 0,
        Failure = 1 << 1,
        Running = 1 << 2
    }

    public static class NodeStateExtensions
    {
        [Pure]
        public static bool HasFlagFast(this NodeState flags, NodeState flag)
        {
            return (flags & flag) == flag;
        }

        [Pure]
        public static bool IsCompleted(this NodeState state)
        {
            return state == NodeState.Success || state == NodeState.Failure;
        }

        [Pure]
        public static bool IsRunningOrFailure(this NodeState state)
        {
            return state == NodeState.Failure || state == NodeState.Running;
        }

        [Pure]
        public static bool IsRunningOrSuccess(this NodeState state)
        {
            return state == NodeState.Success || state == NodeState.Running;
        }

        [Pure]
        public static NodeState ToNodeState(this bool isSuccess)
        {
            return isSuccess ? NodeState.Success : NodeState.Failure;
        }
    }
}
