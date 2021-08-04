// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;

namespace EntitiesBT.Components.Odin
{
    public class OdinRepeatForever : OdinNode<EntitiesBT.Nodes.RepeatForeverNode>
    {
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatForeverNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.BreakStates = BreakStates;
        }
    }
}

#endif
