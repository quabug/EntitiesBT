using System;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using Unity.Collections;
using Unity.Entities;
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

        protected override Type NodeType
        {
            get
            {
                switch (Type)
                {
                case RepeatType.Forever:
                    return typeof(RepeatForeverNode);
                case RepeatType.Times:
                    return typeof(RepeatTimesNode);
                case RepeatType.Duration:
                    return typeof(RepeatDurationNode);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            switch (Type)
            {
            case RepeatType.Forever:
            {
                var ptr = (RepeatForeverNode*) dataPtr;
                ptr->BreakStates = BreakStates;
                return;
            }
            case RepeatType.Times:
            {
                var ptr = (RepeatTimesNode*) dataPtr;
                ptr->TickTimes = RepeatTimes;
                ptr->BreakStates = BreakStates;
                return;
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
