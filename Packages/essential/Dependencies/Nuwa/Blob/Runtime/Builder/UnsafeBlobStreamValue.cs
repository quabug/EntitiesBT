using Blob;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Nuwa.Blob
{
    public unsafe class UnsafeBlobStreamValue<T> where T : unmanaged
    {
        private readonly IBlobStream _stream;
        private readonly int _position;

        public UnsafeBlobStreamValue([NotNull] IBlobStream stream, int position)
        {
            _stream = stream;
            _position = position;
            Assert.IsTrue(position < stream.Length);
        }

        public ref T Value
        {
            get
            {
                fixed (void* ptr = &_stream.Buffer[_position])
                {
                    return ref *(T*)ptr;
                }
            }
        }
    }
}