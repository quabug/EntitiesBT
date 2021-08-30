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

    public interface IDynamicViewerFactory
    {
        public int Order { get; }
        public Type ViewerType { get; }
        public bool IsValid(Type dataType, FieldInfo fieldInfo);
        public object Create(Type dataType, FieldInfo fieldInfo);
    }

    public abstract class DynamicViewerFactory<T> : IDynamicViewerFactory where T : IViewer
    {
        public virtual int Order => 0;
        public Type ViewerType => typeof(T);
        public abstract bool IsValid(Type dataType, FieldInfo fieldInfo);
        public abstract object Create(Type dataType, FieldInfo fieldInfo);
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

            var viewerFactory = elementType.FindViewerType();
            var elementSize = UnsafeUtility.SizeOf(elementType);
            var arrayPtr = dataPtr + offsetPtr;

            for (var i = 0; i < length; i++)
            {
                if (Elements[i] == null || Elements[i].GetType() != viewerFactory.Type)
                    Elements[i] = (IViewer) viewerFactory.Create();
                var elementPtr = arrayPtr + elementSize * i;
                Elements[i].View(elementPtr, elementType);
            }
        }

        public class Factory : DynamicViewerFactory<DynamicArrayViewer>
        {
            public override int Order => 100;

            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobArray<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                return new DynamicArrayViewer();
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
                var viewerFactory = field.FieldType.FindViewerType(field);
                if (FieldsViewer[i] == null || FieldsViewer[i].GetType() != viewerFactory.Type)
                    FieldsViewer[i] = (IViewer) viewerFactory.Create();
                var fieldOffset = UnsafeUtility.GetFieldOffset(field);
                FieldsViewer[i].View(dataPtr + fieldOffset, field.FieldType);
            }
        }

        public class Factory : DynamicViewerFactory<DynamicBlobDataViewer>
        {
            public override int Order => 1000;

            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return !dataType.IsPrimitive && !dataType.IsEnum && dataType.IsValueType;
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                return new DynamicBlobDataViewer();
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
            var viewerFactory = dataType.FindViewerType();
            if (Value == null || Value.GetType() != viewerFactory.Type) Value = (IViewer) viewerFactory.Create();
            ref var offsetPtr = ref UnsafeUtility.AsRef<int>(dataPtr.ToPointer());
            Value.View(dataPtr + offsetPtr, dataType);
        }

        public class Factory : DynamicViewerFactory<DynamicPtrViewer>
        {
            public override int Order => 100;

            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobPtr<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                return new DynamicPtrViewer();
            }
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

        public class Factory : DynamicViewerFactory<DynamicPtrViewer>
        {
            public override int Order => 100;

            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobPtr<>);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                return new DynamicPtrViewer();
            }
        }
    }

    static class ViewerExtension
    {
        public static TypeFactory FindViewerType(this Type dataType, FieldInfo fieldInfo = null)
        {
            var viewerType = typeof(Viewer<>).MakeGenericType(dataType);
            viewerType = TypeCache.GetTypesDerivedFrom(viewerType).SingleOrDefault();
            if (viewerType != null) return new TypeFactory(viewerType);
            var dynamicFactory = DynamicViewerFactoryRegister.FindFactory(dataType, fieldInfo);
            if (dynamicFactory != null) return new TypeFactory(dataType, () => dynamicFactory.Create(dataType, fieldInfo));
            throw new ArgumentException($"cannot find proper viewer {dataType}");
        }
    }
}