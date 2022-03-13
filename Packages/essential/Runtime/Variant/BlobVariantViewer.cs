using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Entities;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Variant
{
    [Serializable]
    public class BlobVariantROViewer : IViewer
    {
        private int _index;
        private NodeBlobRef _blob;
        private EntityBlackboard _blackboard;

        public string Value;

        public unsafe void View(IntPtr dataPtr, Type type, RuntimeViewerFactoryRegister register)
        {
            try
            {
                ref var data = ref UnsafeUtility.AsRef<BlobVariant>(dataPtr.ToPointer());
                var valuePtr = data.ReadOnlyPtrWithReadWriteFallback(_index, ref _blob, ref _blackboard);
                var valueType = type.GetGenericArguments()[0];
                Value = Marshal.PtrToStructure(valuePtr, valueType).ToString();
            }
            catch (Exception)
            {
                Value = "Unable to read";
            }
        }

        public class Factory : DynamicViewerFactory<BlobVariantROViewer>
        {
            public int Index;
            public NodeBlobRef Blob;
            public EntityBlackboard Blackboard;

            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobVariantRO<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                var valueType = dataType.GetGenericArguments()[0];
                return new BlobVariantROViewer { _index = Index, _blackboard = Blackboard, _blob = Blob };
            }
        }
    }
}