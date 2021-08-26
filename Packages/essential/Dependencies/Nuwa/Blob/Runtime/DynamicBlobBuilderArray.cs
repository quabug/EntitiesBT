using System;
using System.Diagnostics;
using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Nuwa.Blob;

namespace Unity.Entities
{
    /// <summary>
    /// Used by the <see cref="BlobBuilder"/> methods to reference the arrays within a blob asset.
    /// </summary>
    /// <remarks>Use this reference to initialize the data of a newly created <see cref="BlobArray{T}"/>.</remarks>
    public struct BlobBuilderDynamicArray
    {
        private IntPtr m_data;
        private int m_length;
        private Type m_elementType;

        /// <summary>
        /// For internal, <see cref="BlobBuilder"/>, use only.
        /// </summary>
        public BlobBuilderDynamicArray(IntPtr data, int length, Type elementType)
        {
            m_data = data;
            m_length = length;
            m_elementType = elementType;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckIndexOutOfRange(int index)
        {
            if (0 > index || index >= m_length)
                throw new IndexOutOfRangeException($"Index {index} is out of range of '{m_length}' Length.");
        }

        /// <summary>
        /// Array index accessor for the elements in the array.
        /// </summary>
        /// <param name="index">The sequential index of an array item.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when index is less than zero or greater than the length of the array (minus one).</exception>
        public IntPtr this[int index]
        {
            get
            {
                CheckIndexOutOfRange(index);
                return Utility.ArrayElementAsPtr(m_data, index, UnsafeUtility.SizeOf(m_elementType));
            }
        }

        public unsafe void SetElementAt<T>(int index, in T value) where T : unmanaged
        {
            Assert.AreEqual(m_elementType, typeof(T));
            UnsafeUtility.AsRef<T>(this[index].ToPointer()) = value;
        }

        /// <summary>
        /// Reports the number of elements in the array.
        /// </summary>
        public int Length => m_length;

        /// <summary>
        /// Provides a pointer to the data stored in the array.
        /// </summary>
        /// <remarks>You can only call this function in an [unsafe context].
        /// [unsafe context]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/unsafe-code
        /// </remarks>
        /// <returns>A pointer to the first element in the array.</returns>
        public IntPtr GetUnsafePtr() => m_data;
    }
}
