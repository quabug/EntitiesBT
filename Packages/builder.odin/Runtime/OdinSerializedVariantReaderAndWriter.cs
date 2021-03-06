using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EntitiesBT.Components.Odin
{
    [Serializable]
    public class OdinSerializedVariantReaderAndWriter<T> : ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        [field: SerializeField]
        public bool IsLinked { get; private set; }

        [field: OdinSerialize, NonSerialized, ShowIf(nameof(IsLinked))]
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; private set; }

        [field: OdinSerialize, NonSerialized, HideIf(nameof(IsLinked))]
        public IVariantReader<T> Reader { get; private set; }

        [field: OdinSerialize, NonSerialized, HideIf(nameof(IsLinked))]
        public IVariantWriter<T> Writer { get; private set; }
    }
}