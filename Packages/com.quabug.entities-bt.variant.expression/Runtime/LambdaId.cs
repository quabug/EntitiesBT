using System;

namespace EntitiesBT.Variant.Expression
{
    public readonly struct LambdaId : IEquatable<LambdaId>
    {
        public readonly int BehaviorTreeId;
        public readonly int NodeId;

        public LambdaId(int behaviorTreeId, int nodeId)
        {
            BehaviorTreeId = behaviorTreeId;
            NodeId = nodeId;
        }

        public bool Equals(LambdaId other)
        {
            return BehaviorTreeId == other.BehaviorTreeId && NodeId == other.NodeId;
        }

        public override bool Equals(object obj)
        {
            return obj is LambdaId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BehaviorTreeId * 397) ^ NodeId;
            }
        }
    }

}