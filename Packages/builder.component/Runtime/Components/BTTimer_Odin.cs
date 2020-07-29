// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTTimer_Odin : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
        public EntitiesBT.Variable.VariableProperty<System.Single> CountdownSeconds;
#endif

        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            
#if ODIN_INSPECTOR
            CountdownSeconds.Allocate(ref builder, ref data.CountdownSeconds, Self, tree);
#endif

            data.BreakReturnState = BreakReturnState;
        }
    }
}
