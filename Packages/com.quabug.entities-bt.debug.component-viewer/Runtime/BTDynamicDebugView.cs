using System;
using EntitiesBT.Variant;
using Nuwa.Blob;

namespace EntitiesBT.DebugView
{
    public class BTDynamicDebugView : BTDebugView
    {
        public BlobViewer Default = new BlobViewer();
        public BlobViewer Runtime = new BlobViewer();

        private IntPtr _defaultDataPtr;
        private IntPtr _runtimeDataPtr;
        private Type _nodeType;

        public override void Init()
        {
            var blob = Blob;
            _defaultDataPtr = blob.GetDefaultDataPtr(Index);
            _runtimeDataPtr = blob.GetRuntimeDataPtr(Index);

            var typeId = blob.GetTypeId(Index);
            DebugComponentLookUp.BEHAVIOR_NODE_ID_TYPE_MAP.TryGetValue(typeId, out _nodeType);

            Default.Register(new BlobVariantROViewer.Factory { Index = Index, Blob = Blob, Blackboard = Blackboard.Value });
            Runtime.Register(new BlobVariantROViewer.Factory { Index = Index, Blob = Blob, Blackboard = Blackboard.Value });
        }

        public override void Tick()
        {
            Default.UnsafeView(_defaultDataPtr, _nodeType);
            Runtime.UnsafeView(_runtimeDataPtr, _nodeType);
        }
    }
}