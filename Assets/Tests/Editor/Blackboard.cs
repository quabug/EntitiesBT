using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Test
{
    public struct Blackboard : IBlackboard
    {
        interface IBoxedValue
        {
            unsafe void* ValuePtr { get; }
        }
        
        class BoxedValue<T> : IBoxedValue where T : struct
        {
            public T Value;
            public unsafe void* ValuePtr => UnsafeUtility.AddressOf(ref Value);
            public BoxedValue(T value) => Value = value;
        }
        
        private Dictionary<Type, object> _map;

        public void SetData<T>(T value) where T : struct
        {
            if (_map == null) _map = new Dictionary<Type, object>();
            _map[typeof(T)] = new BoxedValue<T>(value);
        }

        public void SetObject<T>(T value) where T : class
        {
            if (_map == null) _map = new Dictionary<Type, object>();
            _map[typeof(T)] = value;
        }

        public bool HasData<T>() where T : struct
        {
            return HasData(typeof(T));
        }

        public T GetData<T>() where T : struct
        {
            return GetDataRef<T>();
        }

        public ref T GetDataRef<T>() where T : struct
        {
            return ref ((BoxedValue<T>)_map[typeof(T)]).Value;
        }

        public bool HasData(Type type)
        {
            return _map != null && _map.ContainsKey(type);
        }

        public unsafe IntPtr GetDataPtrRO(Type type)
        {
            return new IntPtr(((IBoxedValue)_map[type]).ValuePtr);
        }

        public IntPtr GetDataPtrRW(Type type)
        {
            return GetDataPtrRO(type);
        }

        public T GetObject<T>() where T : class
        {
            return (T)_map[typeof(T)];
        }
    }
}