using System;
using Nuwa;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    // [Serializable]
    // public class DynamicPtrViewer : IViewer
    // {
    //     [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IViewer Value;
    //
    //     public unsafe void View(IntPtr dataPtr, Type type)
    //     {
    //         Assert.IsTrue(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BlobPtr<>));
    //         var dataType = type.GenericTypeArguments[0];
    //         var viewerType = dataType.FindViewerType();
    //         if (Value == null || Value.GetType() != viewerType) Value = (IViewer) Activator.CreateInstance(viewerType);
    //         ref var offsetPtr = ref UnsafeUtility.AsRef<int>(dataPtr.ToPointer());
    //         Value.View(dataPtr + offsetPtr, dataType);
    //     }
    // }
}