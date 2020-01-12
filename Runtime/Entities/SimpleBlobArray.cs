using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    [MayOnlyLiveInBlobStorage]
    public unsafe struct SimpleBlobArray<T> where T : struct
    {
        public int Length;
        public void* ArrayDataPtr => (byte*)UnsafeUtility.AddressOf(ref Length) + sizeof(int);

        public static int Size(int length)
        {
            return sizeof(int) + UnsafeUtility.SizeOf<T>() * length;
        }

        public ref T this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint) index >= (uint) Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range Length {Length}");
#endif
                return ref UnsafeUtilityEx.ArrayElementAsRef<T>(ArrayDataPtr, index);
            }
        }

        public T[] ToArray()
        {
            var result = new T[Length];
            if (Length > 0)
            {
                var src = ArrayDataPtr;
                
                var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
                var addr = handle.AddrOfPinnedObject();

                UnsafeUtility.MemCpy((void*)addr, src, Length * UnsafeUtility.SizeOf<T>());

                handle.Free();
            }
            return result;
        }

        public void FromArrayUnsafe(T[] array)
        {
            Length = array.Length;
            for (var i = 0; i < Length; i++) UnsafeUtilityEx.ArrayElementAsRef<T>(ArrayDataPtr, i) = array[i];
        }
    }
}
