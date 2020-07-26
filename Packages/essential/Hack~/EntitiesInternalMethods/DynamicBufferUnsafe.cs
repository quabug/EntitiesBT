using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Entities;

namespace EntitiesBT.Core
{
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct DynamicBufferUnsafe<T> where T : struct
    {
        private BufferHeader* m_Buffer;

        private int m_InternalCapacity;
        
        public DynamicBufferUnsafe(IntPtr header, int internalCapacity)
        {
            m_Buffer = (BufferHeader*)header.ToPointer();
            m_InternalCapacity = internalCapacity;
        }

        public int Length
        {
            get
            {
                return m_Buffer->Length;
            }
        }
        public int Capacity
        {
            get
            {
                return m_Buffer->Capacity;
            }
            set
            {
                BufferHeader.SetCapacity(m_Buffer, value, UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), BufferHeader.TrashMode.RetainOldData, false, 0, m_InternalCapacity);
            }
        }

        public bool IsCreated
        {
            get
            {
                return m_Buffer != null;
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckBounds(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException($"Index {index} is out of range in DynamicBufferUnsafe of '{Length}' Length.");
#endif
        }
        
        public T this[int index]
        {
            get
            {
                CheckBounds(index);
                return UnsafeUtility.ReadArrayElement<T>(BufferHeader.GetElementPointer(m_Buffer), index);
            }
            set
            {
                CheckBounds(index);
                UnsafeUtility.WriteArrayElement<T>(BufferHeader.GetElementPointer(m_Buffer), index, value);
            }
        }

        public void ResizeUninitialized(int length)
        {
            EnsureCapacity(length);
            m_Buffer->Length = length;
        }

        public void EnsureCapacity(int length)
        {
            BufferHeader.EnsureCapacity(m_Buffer, length, UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), BufferHeader.TrashMode.RetainOldData, false, 0);
        }

        public void Clear()
        {
            m_Buffer->Length = 0;
        }

        public void TrimExcess()
        {
            byte* oldPtr = m_Buffer->Pointer;
            int length = m_Buffer->Length;

            if (length == Capacity || oldPtr == null)
                return;

            int elemSize = UnsafeUtility.SizeOf<T>();
            int elemAlign = UnsafeUtility.AlignOf<T>();

            bool isInternal;
            byte* newPtr;

            // If the size fits in the internal buffer, prefer to move the elements back there.
            if (length <= m_InternalCapacity)
            {
                newPtr = (byte*)(m_Buffer + 1);
                isInternal = true;
            }
            else
            {
                newPtr = (byte*)UnsafeUtility.Malloc((long)elemSize * length, elemAlign, Allocator.Persistent);
                isInternal = false;
            }

            UnsafeUtility.MemCpy(newPtr, oldPtr, (long)elemSize * length);

            m_Buffer->Capacity = Math.Max(length, m_InternalCapacity);
            m_Buffer->Pointer = isInternal ? null : newPtr;

            UnsafeUtility.Free(oldPtr, Allocator.Persistent);
        }

        public int Add(T elem)
        {
            int length = Length;
            ResizeUninitialized(length + 1);
            this[length] = elem;
            return length;
        }

        public void Insert(int index, T elem)
        {
            int length = Length;
            ResizeUninitialized(length + 1);
            CheckBounds(index); //CheckBounds after ResizeUninitialized since index == length is allowed
            int elemSize = UnsafeUtility.SizeOf<T>();
            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);
            UnsafeUtility.MemMove(basePtr + (index + 1) * elemSize, basePtr + index * elemSize, (long)elemSize * (length - index));
            this[index] = elem;
        }

        public void AddRange(NativeArray<T> newElems)
        {
            int elemSize = UnsafeUtility.SizeOf<T>();
            int oldLength = Length;
            ResizeUninitialized(oldLength + newElems.Length);

            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);
            UnsafeUtility.MemCpy(basePtr + (long)oldLength * elemSize, newElems.GetUnsafeReadOnlyPtr<T>(), (long)elemSize * newElems.Length);
        }

        public void RemoveRange(int index, int count)
        {
            CheckBounds(index + count - 1);

            int elemSize = UnsafeUtility.SizeOf<T>();
            byte* basePtr = BufferHeader.GetElementPointer(m_Buffer);

            UnsafeUtility.MemMove(basePtr + index * elemSize, basePtr + (index + count) * elemSize, (long)elemSize * (Length - count - index));

            m_Buffer->Length -= count;
        }

        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        public void* GetUnsafePtr()
        {
            return BufferHeader.GetElementPointer(m_Buffer);
        }

        public void* GetUnsafeReadOnlyPtr()
        {
            return BufferHeader.GetElementPointer(m_Buffer);
        }

        public DynamicBufferUnsafe<U> Reinterpret<U>() where U : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (UnsafeUtility.SizeOf<U>() != UnsafeUtility.SizeOf<T>())
                throw new InvalidOperationException($"Types {typeof(U)} and {typeof(T)} are of different sizes; cannot reinterpret");
#endif
            return new DynamicBufferUnsafe<U>(new IntPtr(m_Buffer), m_InternalCapacity);
        }

        public void CopyFrom(DynamicBufferUnsafe<T> v)
        {
            ResizeUninitialized(v.Length);

            UnsafeUtility.MemCpy(BufferHeader.GetElementPointer(m_Buffer),
                BufferHeader.GetElementPointer(v.m_Buffer), Length * UnsafeUtility.SizeOf<T>());
        }

        public void CopyFrom(T[] v)
        {
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            
#if NET_DOTS
            Clear();
            foreach (var d in v)
            {
                Add(d);
            }
#else
            ResizeUninitialized(v.Length);

            GCHandle gcHandle = GCHandle.Alloc((object)v, GCHandleType.Pinned);
            IntPtr num = gcHandle.AddrOfPinnedObject();

            UnsafeUtility.MemCpy(BufferHeader.GetElementPointer(m_Buffer),
                (void*)num, Length * UnsafeUtility.SizeOf<T>());
            gcHandle.Free();
#endif
        }
    }
}