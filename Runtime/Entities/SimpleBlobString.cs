using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Entities
{
    public struct SimpleBlobString
    {
        private SimpleBlobArray<char> _data;
        public int Length => _data.Length;
        
        public new unsafe string ToString()
        {
            return new string((char*) _data.ArrayDataPtr, 0, _data.Length);
        }

        public unsafe void FromStringUnsafe(string str)
        {
            _data.Length = str.Length;
            for (var i = 0; i < Length; i++) UnsafeUtilityEx.ArrayElementAsRef<char>(_data.ArrayDataPtr, i) = str[i];
        }

        public static int Size(int length) => SimpleBlobArray<char>.Size(length);
    }
}
