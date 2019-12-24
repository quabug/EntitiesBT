using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT
{
    [MayOnlyLiveInBlobStorage]
    public unsafe struct SimpleBlobArray<T> where T : struct
    {
        private readonly void* _ptr;
        public int Length => *(int*)_ptr;

        public SimpleBlobArray(void* ptr)
        {
            _ptr = ptr;
        }

        public void* GetUnsafePtr()
        {
            return _ptr;
        }

        public void* ArrayDataPtr => (byte*)_ptr + sizeof(int);

        public static int Size(int Length)
        {
            return sizeof(int) + UnsafeUtility.SizeOf<T>() * Length;
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
    }
}
