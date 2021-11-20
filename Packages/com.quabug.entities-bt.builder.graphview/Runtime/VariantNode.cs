using System;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class VariantNode : MonoBehaviour
    {
        protected abstract string Name { get; }
        public abstract Type VariantType { get; }
        public Type ValueType { get; set; }

        private GraphNodeVariant.Any _graphNodeVariant = null;
        internal GraphNodeVariant.Any GraphNodeVariant
        {
            get => _graphNodeVariant;
            set
            {
                _graphNodeVariant = value;
                if (_graphNodeVariant != null && !IsValid()) GraphNodeVariant.Node = null;
            }
        }

        public abstract bool IsValid();
        public abstract IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant);

        private void Update()
        {
            name = Name;
            // if (_graphNodeVariant != null)
            // {
            //     if (_graphNodeVariant.Node != this)
            //     {
            //         Debug.LogWarning("Reset invalid reference of GraphNodeVariant");
            //         _graphNodeVariant = null;
            //     }
            //     if (!IsValid()) _graphNodeVariant.Node = null;
            // }
        }
    }
}