using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT.Components
{
    public static class BehaviorTreeExtensions
    {
        [Pure]
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this TextAsset file)
        {
            var dataPtr = UnsafeUtility.PinGCArrayAndGetDataAddress(file.bytes, out var gcHandle);
            var reader = new MemoryBinaryReader((byte*)dataPtr);
            try
            {
                var storedVersion = reader.ReadInt();
                if (storedVersion != NodeBlob.VERSION) throw new FormatException("Version is not match.");
                return reader.Read<NodeBlob>();
            }
            finally
            {
                reader.Dispose();
                UnsafeUtility.ReleaseGCObject(gcHandle);
            }
        }

        [Pure]
        public static ComponentTypeSet GetAccessTypes<TNodeBlob>(this ref TNodeBlob blob)
            where TNodeBlob : struct, INodeBlob
        {
            var accessTypes = Enumerable.Empty<ComponentType>();
            for (var i = 0; i < blob.Count; i++)
                accessTypes = accessTypes.Concat(VirtualMachine.GetAccessTypes(i, ref blob));
            return new ComponentTypeSet(accessTypes);
        }

        public static void AddBehaviorTree(
            this Entity entity
          , EntityManager dstManager
          , NodeBlobRef blob
          , int order = 0
          , AutoCreateType autoCreateType = AutoCreateType.None
          , BehaviorTreeThread thread = BehaviorTreeThread.ForceRunOnMainThread
          , string debugName = default
        )
        {
            var bb = new EntityBlackboard { Entity = entity, EntityManager = dstManager };
            VirtualMachine.Reset(ref blob, ref bb);

            dstManager.AddBuffer<BehaviorTreeBufferElement>(entity);
            dstManager.AddComponent<CurrentBehaviorTreeComponent>(entity);

            var behaviorTreeEntity = dstManager.CreateEntity();
            
#if UNITY_EDITOR
            dstManager.SetName(behaviorTreeEntity, $"[BT]{dstManager.GetName(entity)}:{order}.{debugName}");
#endif
            var query = blob.GetAccessTypes();
            var dataQuery = new BlackboardDataQuery(query, components =>
                dstManager.CreateEntityQuery(components.ToArray()));
            dstManager.AddSharedComponentData(behaviorTreeEntity, dataQuery);
            
            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeComponent
            {
                Blob = blob, Thread = thread, AutoCreation = autoCreateType
            });
            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeTargetComponent {Value = entity});
            dstManager.AddComponentData(behaviorTreeEntity, new BehaviorTreeOrderComponent {Value = order});
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

        public static void AllocateArray<T>(ref this BlobBuilder builder, ref BlobArray<T> blobArray, IList<T> sourceArray) where T : struct
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
