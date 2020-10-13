// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;

namespace EntitiesBT.Components
{
    public class OdinTimer : OdinNode<EntitiesBT.Nodes.TimerNode>
    {
        [OdinSerialize, NonSerialized]
        public EntitiesBT.Variable.VariableProperty<System.Single> CountdownSeconds;

        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            CountdownSeconds.Allocate(ref builder, ref data.CountdownSeconds, Self, tree);
            data.BreakReturnState = BreakReturnState;
        }
    }
}

#endif
