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

        public static void AllocateVariable<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, Variable<T> variable) where T : struct
        {
            blobVariable.Source = variable.ValueSource;
            switch (variable.ValueSource)
            {
            case VariableValueSource.Constant:
            {
                Allocate(ref blobVariable, variable.ConstantValue);
                break;
            }
            case VariableValueSource.ConstantScriptableObject:
            {
                var value = variable.ConstantValue;
                if (variable.ScriptableObject != null)
                {
                    var field = variable.ScriptableObject.GetType().GetField(
                        variable.ScriptableObjectValueName, BindingFlags.Public | BindingFlags.NonPublic
                    );
                    if (field != null && field.FieldType == typeof(T))
                        value = (T)field.GetValue(variable.ScriptableObjectValueName);
                    else
                        Debug.LogError($"{variable.ScriptableObject.name}.{variable.ScriptableObjectValueName} is not valid, fallback to ConstantValue");
                }
                Allocate(ref blobVariable, value);
                break;
            }
            case VariableValueSource.DynamicComponent:
            {
                var (hash, offset, valueType) = Variable.GetTypeHashAndFieldOffset(variable.ComponentValue);
                if (valueType != typeof(T) || hash == 0)
                {
                    Debug.LogError($"ComponentVariable({variable.ComponentValue}) is not valid, fallback to ConstantValue");
                    Allocate(ref blobVariable, variable.ConstantValue);
                    break;
                }
                Allocate(ref blobVariable, new DynamicComponentData{StableHash = hash, Offset = offset});
                break;
            }
            case VariableValueSource.ConstantComponent:
            case VariableValueSource.ConstantNode:
            case VariableValueSource.DynamicScriptableObject:
            case VariableValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }

            void Allocate<TValue>(ref BlobVariable<T> blob, TValue value) where TValue : struct
            {
                ref var blobPtr = ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref blob.OffsetPtr);
                ref var blobValue = ref builder.Allocate(ref blobPtr);
                blobValue = value;
            }
        }
    }
}
