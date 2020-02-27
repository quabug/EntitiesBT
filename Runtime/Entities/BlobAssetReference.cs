namespace EntitiesBT.Entities
{
    public unsafe readonly struct BlobAssetReference
    {
        public readonly void* Ptr;
        public readonly int Length;

        public BlobAssetReference(void* ptr, int length)
        {
            Ptr = ptr;
            Length = length;
        }

        public static BlobAssetReference Empty => new BlobAssetReference(ptr: null, length: 0);
    }
}
