using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public class ReaderMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate TResult Delegate<out TResult, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TResult : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        public ReaderMethodAttribute() : base(typeof(Delegate<,,>)) {}
    }

    public class RefReaderMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate ref TResult Delegate<TResult, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TResult : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        public RefReaderMethodAttribute() : base(typeof(Delegate<,,>)) {}
    }

    public class WriterMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate void Delegate<in T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        public WriterMethodAttribute() : base(typeof(Delegate<,,>)) {}
    }

    public class ReadOnlyPointerMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate IntPtr Delegate<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        public ReadOnlyPointerMethodAttribute() : base(typeof(Delegate<,>)) {}
    }

    public class ReadWritePointerMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate IntPtr Delegate<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        public ReadWritePointerMethodAttribute() : base(typeof(Delegate<,>)) {}
    }

    public class AccessorMethodAttribute : RegisterDelegateMethodAttribute
    {
        public delegate IEnumerable<ComponentType> Delegate(ref BlobVariant variant);
        public AccessorMethodAttribute() : base(typeof(Delegate)) {}
    }

    public class VariantClassAttribute : RegisterDelegateClassAttribute
    {
        public VariantClassAttribute([NotNull] string guid) : base(guid) {}
    }
}
