using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public abstract class BTNode<T> : MonoBehaviour, INodeDataBuilder where T : IBehaviorNode
    {
        public BehaviorNodeFactory Factory;
        public int Type => Factory.GetTypeId<T>();
        
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);
    }
}