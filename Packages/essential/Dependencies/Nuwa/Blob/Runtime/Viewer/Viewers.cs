using System;
using System.Linq;
using System.Reflection;
using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob
{
    public interface IViewer
    {
        void View(IntPtr dataPtr, Type type);
    }

    public abstract class Viewer<T> : IViewer where T : unmanaged
    {
        public unsafe void View(IntPtr dataPtr, Type type)
        {
            Assert.AreEqual(type, typeof(T));
            View(ref UnsafeUtility.AsRef<T>(dataPtr.ToPointer()));
        }

        public abstract void View(ref T data);
    }

    [Serializable]
    public class PlainDataViewer<T> : Viewer<T> where T : unmanaged
    {
        public T Value;
        public override void View(ref T data) => Value = data;
    }

    [Serializable]
    public class StringViewer : Viewer<BlobString>
    {
        public string Value;
        public override void View(ref BlobString data) => Value = data.ToString();
    }

    [Serializable]
    public class DynamicArrayViewer : IViewer
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IViewer[] Elements;

        public unsafe void View(IntPtr dataPtr, Type type)
        {
            Assert.IsTrue(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BlobArray<>));
            ref var offsetPtr = ref UnsafeUtility.AsRef<int>(dataPtr.ToPointer());
            ref var length = ref UnsafeUtility.AsRef<int>((dataPtr + sizeof(int)).ToPointer());
            var elementType = type.GenericTypeArguments[0];
            Array.Resize(ref Elements, length);

            var viewerType = elementType.FindViewerType();
            var elementSize = UnsafeUtility.SizeOf(elementType);
            var arrayPtr = dataPtr + offsetPtr;

            for (var i = 0; i < length; i++)
            {
                if (Elements[i] == null || Elements[i].GetType() != viewerType)
                    Elements[i] = (IViewer) Activator.CreateInstance(viewerType);
                var elementPtr = arrayPtr + elementSize * i;
                Elements[i].View(elementPtr, elementType);
            }
        }
    }

    [Serializable]
    public class DynamicBlobDataViewer : IViewer
    {
        public string TypeName;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IViewer[] FieldsViewer;

        public void View(IntPtr dataPtr, Type type)
        {
            Assert.IsTrue(!type.IsPrimitive && !type.IsEnum && type.IsValueType);
            TypeName = type.AssemblyQualifiedName;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Array.Resize(ref FieldsViewer, fields.Length);
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var viewerType = field.FieldType.FindViewerType();
                if (FieldsViewer[i] == null || FieldsViewer[i].GetType() != viewerType)
                    FieldsViewer[i] = (IViewer) Activator.CreateInstance(viewerType);
                var fieldOffset = UnsafeUtility.GetFieldOffset(field);
                FieldsViewer[i].View(dataPtr + fieldOffset, field.FieldType);
            }
        }
    }

    [Serializable]
    public class DynamicPtrViewer : IViewer
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IViewer Value;

        public unsafe void View(IntPtr dataPtr, Type type)
        {
            Assert.IsTrue(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BlobPtr<>));
            var dataType = type.GenericTypeArguments[0];
            var viewerType = dataType.FindViewerType();
            if (Value == null || Value.GetType() != viewerType) Value = (IViewer) Activator.CreateInstance(viewerType);
            ref var offsetPtr = ref UnsafeUtility.AsRef<int>(dataPtr.ToPointer());
            Value.View(dataPtr + offsetPtr, dataType);
        }
    }

    [Serializable]
    public class DynamicEnumViewer : IViewer
    {
        public long Ptr;
        public string EnumType;

        public void View(IntPtr dataPtr, Type type)
        {
            Assert.IsTrue(type.IsEnum);
            Ptr = dataPtr.ToInt64();
            EnumType = type.AssemblyQualifiedName;
        }
    }

    static class ViewerExtension
    {
        public static Type FindViewerType(this Type dataType)
        {
            var viewerType = typeof(Viewer<>).MakeGenericType(dataType);
            viewerType = TypeCache.GetTypesDerivedFrom(viewerType).SingleOrDefault();
            if (viewerType != null) return viewerType;
            if (dataType.IsEnum) return typeof(DynamicEnumViewer);
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobPtr<>))
                return typeof(DynamicPtrViewer);
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobArray<>))
                return typeof(DynamicArrayViewer);
            if (!dataType.IsPrimitive) return typeof(DynamicBlobDataViewer);
            throw new ArgumentException($"cannot find proper viewer {dataType}");
        }
    }
}