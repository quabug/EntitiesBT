using System;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Core
{
    public struct BlobAssetReference : IDisposable
    {
        internal BlobAssetReferenceData _data;
       
        public unsafe bool IsCreated => (IntPtr) _data.m_Ptr != IntPtr.Zero;

        public unsafe void* GetUnsafePtr()
        {
            _data.ValidateAllowNull();
            return _data.m_Ptr;
        }
       
        public void Dispose()
        {
            _data.Dispose();
        }

        public unsafe int Length => ValidateAndGet(data => data.Header->Length, 0);
        public unsafe ulong Hash => ValidateAndGet(data => data.Header->Hash, 0ul);

        private T ValidateAndGet<T>(Func<BlobAssetReferenceData, T> getter, T @default)
        {
            if (!IsCreated) return @default;
            _data.ValidateNotNull();
            return getter(_data);
        }
       
        public static BlobAssetReference Create<T>(BlobAssetReference<T> @ref) where T : struct
        {
            return Create(@ref.m_data);
        }
       
        internal static BlobAssetReference Create(BlobAssetReferenceData blobData)
        {
            return new BlobAssetReference { _data = blobData };
        }

        public static unsafe BlobAssetReference Create(void* ptr, int length)
        {
            byte* buffer = (byte*) Memory.Unmanaged.Allocate(sizeof(BlobAssetHeader) + length, 16, Allocator.Persistent);
            UnsafeUtility.MemCpy(buffer + sizeof(BlobAssetHeader), ptr, length);

            BlobAssetHeader* header = (BlobAssetHeader*) buffer;
            *header = new BlobAssetHeader();

            header->Length = length;
            header->Allocator = Allocator.Persistent;

            // @TODO use 64bit hash
            header->Hash = math.hash(ptr, length);

            BlobAssetReference blobAssetReference;
            blobAssetReference._data.m_Align8Union = 0;
            header->ValidationPtr = blobAssetReference._data.m_Ptr = buffer + sizeof(BlobAssetHeader);
            return blobAssetReference;
        }

        public static unsafe BlobAssetReference Create([NotNull] byte[] data)
        {
            fixed (byte* ptr = &data[0])
            {
                return Create(ptr, data.Length);
            }
        }

        public static BlobAssetReference Null => new BlobAssetReference();

        public static unsafe bool operator ==(BlobAssetReference lhs, BlobAssetReference rhs)
        {
            return lhs._data.m_Ptr == rhs._data.m_Ptr;
        }
       
        public static unsafe bool operator !=(BlobAssetReference lhs, BlobAssetReference rhs)
        {
            return lhs._data.m_Ptr != rhs._data.m_Ptr;
        }
       
        public bool Equals(BlobAssetReference other)
        {
            return _data.Equals(other._data);
        }
       
        public override bool Equals(object obj)
        {
            return this == (BlobAssetReference) obj;
        }
       
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
    }
}
