using System;
using System.Linq;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTWeightRandomSelector : BTNode<WeightRandomSelectorNode, WeightRandomSelectorNode.Data>
    {
        [SerializeField] private float[] _weights;

        public override int Size => WeightRandomSelectorNode.Data.Size(_weights.Length);

        protected override void Build(ref WeightRandomSelectorNode.Data data)
        {
            data.Sum = _weights.Sum();
            data.Weights.FromArrayUnsafe(_weights);
        }

        private void OnTransformChildrenChanged()
        {
            Array.Resize(ref _weights, ChildCount);
        }

        private void OnValidate()
        {
            Array.Resize(ref _weights, ChildCount);
            for (var i = 0; i < _weights.Length; i++)
                if (_weights[i] < 0) _weights[i] = 0;
        }
    }
}
