using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
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
            var blob = Blob;
            ref var runtime = ref blob.GetNodeData<WeightRandomSelectorNode, NodeBlobRef>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
            
            ref var @default = ref blob.GetNodeDefaultData<WeightRandomSelectorNode, NodeBlobRef>(Index).Weights;
            DefaultWeights = @default.ToArray();
        }

        public override void Tick()
        {
            var blob = Blob;
            ref var runtime = ref blob.GetNodeData<WeightRandomSelectorNode, NodeBlobRef>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
        }

        protected override void OnValidate()
        {
            if (!IsValid) return;

            var blob = Blob;
            ref var @default = ref blob.GetNodeDefaultData<WeightRandomSelectorNode, NodeBlobRef>(Index);
            SetData(ref @default, DefaultWeights);
            
            ref var runtime = ref blob.GetNodeData<WeightRandomSelectorNode, NodeBlobRef>(Index);
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
