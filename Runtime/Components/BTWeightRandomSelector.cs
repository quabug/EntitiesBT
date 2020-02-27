using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTWeightRandomSelector : BTNode<WeightRandomSelectorNode>
    {
        [SerializeField] private float[] _weights;

        protected override void Build(ref WeightRandomSelectorNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Sum = _weights.Sum();
            builder.AllocateArray(ref data.Weights, _weights);
        }

        protected override void Update()
        {
            base.Update();
            Array.Resize(ref _weights, Children.Count());
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Array.Resize(ref _weights, Children.Count());
            for (var i = 0; i < _weights.Length; i++)
                if (_weights[i] < 0) _weights[i] = 0;
        }
    }
}
