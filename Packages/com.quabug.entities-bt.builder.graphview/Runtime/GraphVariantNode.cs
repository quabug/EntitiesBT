using EntitiesBT.Variant;
using UnityEngine;

namespace EntitiesBT
{
    public class GraphVariantNode : MonoBehaviour
    {
        [SerializeReference] public IVariant Variant;

        public bool IsReadOnly => Variant is IVariantReader;
        public bool IsReadWrite => Variant is IVariantReaderAndWriter;
        public bool IsWriteOnly => Variant is IVariantWriter;
    }
}