using System;
using System.Linq;
using EntitiesBT.Variant;
using Nuwa;
using UnityEngine;

namespace EntitiesBT
{
    [ExecuteAlways]
    public class GraphVariantNode : MonoBehaviour
    {
        [HideInInspector]
        public string VariantClass;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(VariantClass), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariant Variant;

        public bool IsReadOnly => Variant is IVariantReader;
        public bool IsReadWrite => Variant is IVariantReaderAndWriter;
        public bool IsWriteOnly => Variant is IVariantWriter;

#if UNITY_EDITOR
        private void Update()
        {
            var objectName = Variant == null
                ? VariantClass.Substring(0, VariantClass.IndexOf('+')).Split('.').Last()
                : Variant.GetType().Name
            ;

            var accessName = Variant switch
            {
                IVariantReader _ => "RO",
                IVariantWriter _ => "WO",
                IVariantReaderAndWriter _ => "RW",
                _ => throw new NotImplementedException()
            };

            name = $"[{accessName}] {objectName}";
        }
#endif
    }
}