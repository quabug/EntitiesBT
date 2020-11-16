using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        // [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleProperty CountdownSecondsReader;

        // public EntitiesBT.Variant.BlobVariantWriter`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]] CountdownSecondsWriter;
        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            // CountdownSecondsReader.Allocate(ref builder, ref data.CountdownSecondsReader, Self, tree);
            // data.CountdownSecondsWriter = CountdownSecondsWriter;
            // data.BreakReturnState = BreakReturnState;
        }
    }
}
