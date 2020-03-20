using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTSubTree : BTNode, INodeDataBuilder
    {
        [SerializeField] private BTNode _tree = default;
        protected override INodeDataBuilder SelfImpl => _tree;
    }
}
