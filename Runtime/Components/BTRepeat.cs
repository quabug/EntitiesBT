using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTRepeat : BTNode
    {
        enum RepeatType
        {
            Forever,
            Times,
            Duration
        }

        [SerializeField] private RepeatType Type = RepeatType.Forever;
        public NodeState BreakStates;
        [Header("for `Times` repeater")]
        public int RepeatTimes;
        [Header("for `Duration` repeater")]
        public float RepeatDurationInSeconds;

        public override BehaviorNodeType NodeType => BehaviorNodeType.Decorate;

        public override int NodeId
        {
            get
            {
                switch (Type)
                {
                case RepeatType.Forever:
                    return typeof(RepeatForeverNode).GetBehaviorNodeAttribute().Id;
                case RepeatType.Times:
                    return typeof(RepeatTimesNode).GetBehaviorNodeAttribute().Id;
                case RepeatType.Duration:
                    return typeof(RepeatDurationNode).GetBehaviorNodeAttribute().Id;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override unsafe int Size
        {
            get
            {
                switch (Type)
                {
                case RepeatType.Forever:
                    return sizeof(RepeatForeverNode);
                case RepeatType.Times:
                    return sizeof(RepeatTimesNode);
                case RepeatType.Duration:
                    return sizeof(RepeatDurationNode);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
            switch (Type)
            {
            case RepeatType.Forever:
            {
                var ptr = (RepeatForeverNode*) dataPtr;
                ptr->BreakStates = BreakStates;
                break;
            }
            case RepeatType.Times:
            {
                var ptr = (RepeatTimesNode*) dataPtr;
                ptr->TickTimes = RepeatTimes;
                ptr->BreakStates = BreakStates;
                break;
            }
            case RepeatType.Duration:
            {
                var ptr = (RepeatDurationNode*) dataPtr;
                ptr->CountdownSeconds = RepeatDurationInSeconds;
                ptr->BreakStates = BreakStates;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
