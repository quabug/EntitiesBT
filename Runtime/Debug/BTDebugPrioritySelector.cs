using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [AddComponentMenu("")] // hide from component menu
    public class BTDebugPrioritySelector : BTDebugView<PrioritySelectorNode>
    {
        public int[] DefaultWeights;
        public int[] RuntimeWeights;

        public override void Init()
        {
            ref var runtime = ref Blob.GetNodeData<PrioritySelectorNode>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
            
            ref var @default = ref Blob.GetNodeDefaultData<PrioritySelectorNode>(Index).Weights;
            DefaultWeights = @default.ToArray();
        }

        public override void Tick()
        {
            ref var runtime = ref Blob.GetNodeData<PrioritySelectorNode>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
        }

        protected override void OnValidate()
        {
            if (!IsValid) return;
            
            ref var @default = ref Blob.GetNodeDefaultData<PrioritySelectorNode>(Index);
            SetData(ref @default, DefaultWeights);
            
            ref var runtime = ref Blob.GetNodeData<PrioritySelectorNode>(Index);
            SetData(ref runtime, RuntimeWeights);

            void SetData(ref PrioritySelectorNode data, int[] array)
            {
                Array.Resize(ref array, data.Weights.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    if (array[i] < 0) array[i] = 0;
                    data.Weights[i] = array[i];
                }
            }
        }
    }
}
