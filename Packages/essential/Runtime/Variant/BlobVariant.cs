using System.Runtime.InteropServices;

namespace EntitiesBT.Variant
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariant
    {
        public int VariantId;
        public int MetaDataOffsetPtr;
    }
}
