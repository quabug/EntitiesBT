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

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
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
