using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public abstract class BTNode : MonoBehaviour, INodeDataBuilder
    {
        public BehaviorNodeFactory Factory { get; set; }
        public int Index { get; set; }
        
        public abstract int Type { get; }
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);

        private void Reset() => name = GetType().Name;
    }
}