using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Assertions;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTDecorator<T> : BTNode<T> where T : unmanaged, INodeData
    {
        [SerializeField] private bool _shouldRerunOnceOnError = true;

        protected override void OnValidate()
        {
            Assert.AreEqual(BehaviorNodeType.Decorate, BehaviorNodeType);
        }

        public override IEnumerable<INodeDataBuilder> Children => _shouldRerunOnceOnError
            ? new BTVirtualDecorator<ErrorRerunOnceNode>(base.Children).Yield()
            : base.Children
        ;
    }
}
