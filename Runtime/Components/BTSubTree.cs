using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTSubTree : BTNode, INodeDataBuilder
    {
        [SerializeField] private BTNode _tree = default;
        public override INodeDataBuilder Self => _tree;
    }
}
