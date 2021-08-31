using System;
using Unity.Entities;
using UnityEngine;

namespace Nuwa.Blob
{
    [Serializable]
    public class BlobViewer
    {
        [SerializeField] public bool Writable = false;
        [SerializeField] internal string TypeName = "(Empty)";
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] internal IViewer Viewer;

        private RuntimeViewerFactoryRegister _register = new RuntimeViewerFactoryRegister();

        public unsafe void View<T>(T* dataPtr) where T : unmanaged => UnsafeView(new IntPtr(dataPtr), typeof(T));

        public void UnsafeView(IntPtr dataPtr, Type type)
        {
#if UNITY_EDITOR
            TypeName = type.ToReadableName();
            var viewerFactory = _register.FindFactory(type);
            if (Viewer == null || Viewer.GetType() != viewerFactory.Type)
                Viewer = (IViewer) viewerFactory.Create();
            Viewer.View(dataPtr, type, _register);
#endif
        }

        public void Register(IDynamicViewerFactory factory)
        {
            _register.Register(factory);
        }
    }

    public static class BlobViewerExtension
    {
        public static unsafe void View<T>(this BlobViewer viewer, BlobAssetReference<T> blobAssetReference) where T : unmanaged
        {
            fixed (T* ptr = &blobAssetReference.Value)
            {
                viewer.View(ptr);
            }
        }
    }
}