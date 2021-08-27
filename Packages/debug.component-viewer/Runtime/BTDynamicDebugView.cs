using System;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    public class BTDynamicDebugView : BTDebugView
    {
        // [SerializeReference] public ISerializableNodeData Default;
        // [SerializeReference] public ISerializableNodeData Runtime;

        public IntPtr DefaultData;
        public IntPtr RuntimeData;

        public override void Init()
        {
            var blob = Blob;
            RuntimeData = blob.GetDefaultDataPtr(Index);
            DefaultData = blob.GetRuntimeDataPtr(Index);
            // Default = CreateSerializableNodeData();
            // Runtime = CreateSerializableNodeData();

            // ISerializableNodeData CreateSerializableNodeData()
            // {
            //     var type = SerializableNodeDataRegistry.FindSerializableType(blob.GetTypeId(Index));
            //     return type == null ? null : (ISerializableNodeData) Activator.CreateInstance(type);
            // }
        }

        public override unsafe void Tick()
        {
            // Default?.Load(DefaultData.ToPointer());
            // Runtime?.Load(RuntimeData.ToPointer());
        }
    }
}