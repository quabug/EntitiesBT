using System;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT.Entities
{
    [Serializable]
    public enum VariableValueSource
    {
        CustomValue,
        ComponentValue
    }
        
    [Serializable]
    public struct Variable<T> where T : struct
    {
        static Variable()
        {
            Assert.AreNotEqual(DataSize, 0);
        }
        
        public static int DataSize => UnsafeUtility.SizeOf<T>();
        
        public VariableValueSource ValueSource;
        public T CustomValue;
        public string ComponentValue;
        public ulong ComponentStableHash;
        public int ComponentDataOffset;

        public int BlobSize => ValueSource == VariableValueSource.ComponentValue ? 16 : 4 + UnsafeUtility.SizeOf<T>();
        public Type Type => typeof(T);
    }
    
    [StructLayout(LayoutKind.Explicit), MayOnlyLiveInBlobStorage, Serializable]
    public unsafe struct BlobVariable
    {
        [FieldOffset(0), SerializeField] private bool _isCustomVariable;
        [FieldOffset(4)] private int _componentDataOffset;
        [FieldOffset(8)] private ulong _componentStableHash;
        
        [FieldOffset(0), SerializeField] private int _dataSize;
        private void* _dataPtr => UnsafeUtility.AddressOf(ref _componentDataOffset);

        public void FromVariableUnsafe<T>(Variable<T> variable) where T : struct
        {
            _isCustomVariable = variable.ValueSource == VariableValueSource.CustomValue;
            if (_isCustomVariable)
            {
                _dataSize = Variable<T>.DataSize;
                UnsafeUtilityEx.AsRef<T>(_dataPtr) = variable.CustomValue;
            }
            else
            {
                _componentStableHash = variable.ComponentStableHash;
                _componentDataOffset = variable.ComponentDataOffset;
            }
        }

        public Variable<T> ToVariable<T>() where T : struct
        {
            if (_isCustomVariable)
            {
                return new Variable<T>
                {
                    ValueSource = VariableValueSource.CustomValue
                  , CustomValue = UnsafeUtilityEx.AsRef<T>(_dataPtr)
                };
            }

            return new Variable<T>
            {
                ValueSource = VariableValueSource.ComponentValue
              , ComponentStableHash = _componentStableHash
              , ComponentDataOffset = _componentDataOffset
            };
        }

        public ref T GetData<T>(IBlackboard bb) where T : struct
        {
            if (_isCustomVariable) return ref UnsafeUtilityEx.AsRef<T>(_dataPtr); // TODO: check size
            return ref bb.GetDataRef<T>(_componentStableHash, _componentDataOffset);
        }
        
        public void SetData<T>(IBlackboard bb, T value) where T : struct
        {
            if (_isCustomVariable) UnsafeUtilityEx.AsRef<T>(_dataPtr) = value; // TODO: check size
            else bb.GetDataRef<T>(_componentStableHash, _componentDataOffset) = value;
        }
    }
}
