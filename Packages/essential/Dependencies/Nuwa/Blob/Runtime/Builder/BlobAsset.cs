using System;
using System.IO;
using System.Linq;
using Blob;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Nuwa.Blob
{
    [Serializable]
    public class BlobAsset<T> : IDisposable where T : unmanaged
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] internal IBuilder Builder;

        BlobAssetReference<T> _blobAssetReference;

        public BlobAssetReference<T> Reference
        {
            get
            {
                if (!_blobAssetReference.IsCreated) _blobAssetReference = Create();
                return _blobAssetReference;
            }
        }

        public ref T Value => ref Reference.Value;

        public IBuilder FindBuilderByPath(string path)
        {
            var pathList = path.Split('.');
            return pathList.Aggregate(Builder, (builder, name) => builder.GetBuilder(name));
        }

        private BlobAssetReference<T> Create()
        {
            using var stream = new BlobMemoryStream();
            Builder.Build(stream);
            stream.Length = (int)Utilities.Align<T>((int)stream.Length);
            return BlobAssetReference<T>.Create(stream.ToArray());
        }

        public void Dispose()
        {
            if (_blobAssetReference.IsCreated) _blobAssetReference.Dispose();
        }
    }
}