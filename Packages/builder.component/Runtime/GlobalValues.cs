using System;
using EntitiesBT.Core;
using Nuwa.Blob;
using UnityEngine;

namespace EntitiesBT.Variant
{
    public abstract class GlobalValues : MonoBehaviour, IGlobalValuesBuilder
    {
        public int Offset { get; set; }
        public abstract IntPtr ValuePtr { get; }
        public abstract int Size { get; }
        public abstract Type BlobType { get; }
        public virtual object GetPreviewValue(string path) => throw new NotImplementedException();
        public virtual void SetPreviewValue(string path, object value) => throw new NotImplementedException();
    }

    public class GlobalValues<T> : GlobalValues where T : unmanaged, IGlobalValuesBlob
    {
        public BlobAsset<T> Value;

        public override unsafe IntPtr ValuePtr => new IntPtr(Value.Reference.GetUnsafePtr());
        public override int Size => Value.Value.Size;
        public override Type BlobType => typeof(T);

        private void OnDestroy()
        {
            Value?.Dispose();
        }

        public override object GetPreviewValue(string path)
        {
            var builder = Value.FindBuilderByPath(path);
            return builder.PreviewValue;
        }

        public override void SetPreviewValue(string path, object value)
        {
            var builder = Value.FindBuilderByPath(path);
            builder.PreviewValue = value;
        }
    }
}