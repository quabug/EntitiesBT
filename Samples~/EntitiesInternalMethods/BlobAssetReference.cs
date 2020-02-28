// using System;
// using Unity.Entities;
//
// namespace EntitiesBT.Core
// {
//     public struct BlobAssetReference : IDisposable
//     {
//         internal BlobAssetReferenceData _data;
//        
//         public unsafe bool IsCreated => (IntPtr) _data.m_Ptr != IntPtr.Zero;
//
//         public unsafe void* GetUnsafePtr()
//         {
//             _data.ValidateAllowNull();
//             return _data.m_Ptr;
//         }
//        
//         public void Dispose()
//         {
//             _data.Dispose();
//         }
//
//         public unsafe int Length => ValidateAndGet(data => data.Header->Length, 0);
//         public unsafe ulong Hash => ValidateAndGet(data => data.Header->Hash, 0ul);
//
//         private T ValidateAndGet<T>(Func<BlobAssetReferenceData, T> getter, T @default)
//         {
//             if (!IsCreated) return @default;
//             _data.ValidateNotNull();
//             return getter(_data);
//         }
//        
//         public static BlobAssetReference Create<T>(BlobAssetReference<T> @ref) where T : struct
//         {
//             return Create(@ref.m_data);
//         }
//        
//         internal static BlobAssetReference Create(BlobAssetReferenceData blobData)
//         {
//             return new BlobAssetReference { _data = blobData };
//         }
//        
//         public static BlobAssetReference Null => new BlobAssetReference();
//
//         public static unsafe bool operator ==(BlobAssetReference lhs, BlobAssetReference rhs)
//         {
//             return lhs._data.m_Ptr == rhs._data.m_Ptr;
//         }
//        
//         public static unsafe bool operator !=(BlobAssetReference lhs, BlobAssetReference rhs)
//         {
//             return lhs._data.m_Ptr != rhs._data.m_Ptr;
//         }
//        
//         public bool Equals(BlobAssetReference other)
//         {
//             return _data.Equals(other._data);
//         }
//        
//         public override bool Equals(object obj)
//         {
//             return this == (BlobAssetReference) obj;
//         }
//        
//         public override int GetHashCode()
//         {
//             return _data.GetHashCode();
//         }
//     }
// }
