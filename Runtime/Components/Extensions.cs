using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT.Components
{
    public enum BehaviorTreeThread
    {
        ForceRunOnMainThread
      , ForceRunOnJob
      , ControlledByBehaviorTree
    }
    
    public static class BehaviorTreeExtensions
    {
        public static BlobAssetReference<NodeBlob> ToBlob(this TextAsset file)
        {
            var result = BlobAssetReference<NodeBlob>.TryRead(file.bytes, NodeBlob.VERSION, out var blobRef);
            if (!result) throw new FormatException("Version is not match.");
            return blobRef;
        }

        public static ISet<ComponentType> GetAccessTypes(this BlobAssetReference<NodeBlob> blob)
        {
            return new HashSet<ComponentType>(Enumerable
                .Range(0, blob.Value.Types.Length)
                .Select(i => blob.Value.Types[i])
                .SelectMany(VirtualMachine.GetAccessTypes)
            );
        }

        public static void AddBehaviorTree(
            this Entity entity
          , EntityManager dstManager
          , BlobAssetReference<NodeBlob> blobRef
          , BehaviorTreeThread thread = BehaviorTreeThread.ForceRunOnMainThread
        )
        {
            var blob = new NodeBlobRef { BlobRef = blobRef };
            
            switch (thread)
            {
            case BehaviorTreeThread.ForceRunOnMainThread:
            {
                var bb = new EntityBlackboard { EntityManager = dstManager, Entity = entity };
                VirtualMachine.Reset(blob, bb);
                dstManager.AddComponentData(entity, new ForceRunOnMainThreadTag());
                break;
            }
            case BehaviorTreeThread.ForceRunOnJob:
            {
                AddJobComponents();
                dstManager.AddComponentData(entity, new ForceRunOnJobTag());
                break;
            }
            case BehaviorTreeThread.ControlledByBehaviorTree:
            {
                AddJobComponents();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(thread), thread, null);
            }
            
            dstManager.AddComponentData(entity, blob);

            void AddJobComponents()
            {
                var dataQuery = new BlackboardDataQuery {Value = blobRef.GetAccessTypes()};
                var bb = new EntityBlackboard();
                VirtualMachine.Reset(blob, bb);
                dstManager.AddComponentData(entity, new IsRunOnMainThread { Value = false });
                dstManager.AddSharedComponentData(entity, dataQuery);
            }
        }
        
        // https://stackoverflow.com/a/27851610
        public static bool IsZeroSizeStruct(this Type t)
        {
            return t.IsValueType && !t.IsPrimitive && 
                   t.GetFields((BindingFlags)0x34).All(fi => IsZeroSizeStruct(fi.FieldType));
        }
    }

    public static class BlobBuilderExtensions
    {
        private static Func<BlobBuilder, Type, object> _CONSTRUCT_ROOT;
        private static Func<BlobBuilder, Type, Allocator, object> _CREATE_REFERENCE;
        
        static BlobBuilderExtensions()
        {
            var methodFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
            var constructRoot = typeof(BlobBuilderExtensions).GetMethod("ConstructRootPtr", methodFlags);
            _CONSTRUCT_ROOT = (builder, type) => constructRoot.MakeGenericMethod(type).Invoke(null, new object[] {builder});
            
            var createReference = typeof(BlobBuilderExtensions).GetMethod("CreateReference", methodFlags);
            _CREATE_REFERENCE = (builder, type, allocator) => createReference.MakeGenericMethod(type).Invoke(null, new object[] {builder, allocator});
        }
        
        public static unsafe void* ConstructRootPtr<T>(this BlobBuilder builder) where T : struct
        {
            return UnsafeUtility.AddressOf(ref builder.ConstructRoot<T>());
        }

        public static unsafe void* ConstructRootPtrByType(this BlobBuilder builder, Type dataType)
        {
            Assert.IsTrue(dataType.IsValueType);
            var ptr = _CONSTRUCT_ROOT(builder, dataType);
            return Pointer.Unbox(ptr);
        }

        public static void AllocateArray<T>(this BlobBuilder builder, ref BlobArray<T> blobArray, IList<T> sourceArray) where T : struct
        {
            var array = builder.Allocate(ref blobArray, sourceArray.Count);
            for (var i = 0; i < sourceArray.Count; i++) array[i] = sourceArray[i];
        }

        public static BlobAssetReference CreateReference<T>(this BlobBuilder builder, Allocator allocator = Allocator.Temp) where T : struct
        {
            var @ref = builder.CreateBlobAssetReference<T>(allocator);
            return BlobAssetReference.Create(@ref);
        }
        
        public static BlobAssetReference CreateReferenceByType(this BlobBuilder builder, Type type, Allocator allocator = Allocator.Temp)
        {
            var @ref = _CREATE_REFERENCE(builder, type, allocator);
            return (BlobAssetReference)@ref;
        }
    }
}
