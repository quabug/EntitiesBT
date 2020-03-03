using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.DebugView;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityTranslate : BTNode<EntityTranslateNode>
    {
        public Vector3 Position;

        protected override void Build(ref EntityTranslateNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Position = Position;
        }
    }
    
    [Serializable]
    [BehaviorNode("29A30E27-7A3C-42F4-A0A4-49EFBD890279")]
    public struct EntityTranslateNode : INodeData
    {
        public float3 Position;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<Translation>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<EntityTranslateNode>(index);
            ref var translation = ref bb.GetDataRef<Translation>();
            translation.Value = data.Position;
            return NodeState.Success;
        }
    }

    public class EntityTranslateDebugView : BTDebugView<EntityTranslateNode> {}
}
