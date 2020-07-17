using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTModifyPriority : BTNode<ModifyPriorityNode>
    {
        [SerializeField] private BTPrioritySelector _prioritySelector = default;
        [SerializeField] private int _weightIndex = default;
        [SerializeField] private int _addWeight = default;

        protected override void Reset()
        {
            base.Reset();
            _prioritySelector = GetComponentInParent<BTPrioritySelector>();
        }

        protected override void Build(ref ModifyPriorityNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] builders)
        {
            var prioritySelectorIndex = Array.FindIndex(builders, b => b.Value == (INodeDataBuilder)_prioritySelector);
            data.PrioritySelectorIndex = prioritySelectorIndex;
            data.WeightIndex = _weightIndex;
            data.AddWeight = _addWeight;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_prioritySelector == null)
            {
                Debug.LogWarning("Refer to invalid `PrioritySelector`", gameObject);
                return;
            }

            if (_weightIndex < 0 || _weightIndex >= _prioritySelector.Weights.Length)
            {
                Debug.LogWarning($"WeightIndex {_weightIndex} out of range [0, {_prioritySelector.Weights.Length})", gameObject);
                _weightIndex = 0;
            }
        }
    }
}
