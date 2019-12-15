using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class BTSequence : MonoBehaviour, INodeDataBuilder
    {
        public int Size => 0;
        public int Type => 0;
        public unsafe void Build(void* dataPtr) {}
    }
}
