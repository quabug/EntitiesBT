using JetBrains.Annotations;

namespace Blob
{
    public class ArrayBuilderWithMemoryCopy<TArray> : Builder<TArray> where TArray : unmanaged
    {
        [NotNull] private readonly IBuilder _builder;

        public ArrayBuilderWithMemoryCopy([NotNull] IBuilder builder)
        {
            _builder = builder;
        }

        protected override unsafe void BuildImpl(IBlobStream stream, ref TArray data)
        {
            stream.WriteArrayMeta(_builder.PatchSize).ToPatchPosition();
            fixed (void* ptr = &stream.Buffer[_builder.PatchPosition])
            {
                stream.Write((byte*)ptr, _builder.PatchSize);
            }
        }
    }
}