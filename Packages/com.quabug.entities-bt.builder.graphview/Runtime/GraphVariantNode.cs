using EntitiesBT.Variant;
using Nuwa;
using UnityEngine;

namespace EntitiesBT
{
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
    }
}