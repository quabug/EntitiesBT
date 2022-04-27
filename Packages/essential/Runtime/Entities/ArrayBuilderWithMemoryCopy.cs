using System;

namespace Blob
{
    public class ArrayBuilderWithMemoryCopy<TValue, TArray> : Builder<TArray>
        where TArray : unmanaged
        where TValue : unmanaged
    {
        private readonly Func<(int position, int length)> _getDataFunc;
        public int Alignment { get; set; } = 0;

        public ArrayBuilderWithMemoryCopy(Func<(int position, int length)> getDataFunc)
        {
            _getDataFunc = getDataFunc;
        }

        protected override unsafe void BuildImpl(IBlobStream stream)
        {
            if (Alignment <= 0) Alignment = Utilities.AlignOf<TValue>();
            var (position, length) = _getDataFunc();
            var size = (length + sizeof(TValue) - 1) / sizeof(TValue);
            stream.EnsureDataSize<TArray>().WriteArrayMeta(size).ToPatchPosition();
            fixed (void* ptr = &stream.Buffer[position])
            {
                stream.Write((byte*)ptr, size, Alignment);
            }
        }
    }
}