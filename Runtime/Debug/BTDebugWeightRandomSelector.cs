using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [AddComponentMenu("")] // hide from component menu
    public class BTDebugWeightRandomSelector : BTDebugView<WeightRandomSelectorNode>
    {
        public float[] DefaultWeights;
        public float[] RuntimeWeights;

        public override void Init()
        {
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
            
            ref var @default = ref Blob.GetNodeDefaultData<WeightRandomSelectorNode>(Index).Weights;
            DefaultWeights = @default.ToArray();
        }

        public override void Tick()
        {
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
        }

        protected override void OnValidate()
        {
            if (!IsValid) return;
            
            ref var @default = ref Blob.GetNodeDefaultData<WeightRandomSelectorNode>(Index);
            SetData(ref @default, DefaultWeights);
            
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode>(Index);
            SetData(ref runtime, RuntimeWeights);

            void SetData(ref WeightRandomSelectorNode data, float[] array)
            {
                Array.Resize(ref array, data.Weights.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    if (array[i] < 0) array[i] = 0;
                    data.Weights[i] = array[i];
                }
                data.Sum = array.Sum();
            }
        }
    }
}
