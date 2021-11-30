using System;
using EntitiesBT.Core;
using Nuwa.Blob;
using UnityEngine;

namespace EntitiesBT.Variant
{
    public abstract class ScopeValues : MonoBehaviour, IScopeValues
    {
        public int Offset { get; set; }
        public abstract IntPtr ValuePtr { get; }
        public abstract int Size { get; }
        public abstract Type BlobType { get; }
    }

    public class ScopeValues<T> : ScopeValues where T : unmanaged, IScopeValues
    {
        public BlobAsset<T> Value;

        public override IntPtr ValuePtr => Value.Value.ValuePtr;
        public override int Size => Value.Value.Size;
        public override Type BlobType => typeof(T);

        private void OnDestroy()
        {
            Value?.Dispose();
        }
    }
}