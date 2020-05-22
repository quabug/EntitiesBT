using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class GameObjectBlackboard : IBlackboard
    {
        private readonly GameObject _gameObject;
        private readonly Dictionary<object, object> _dict = new Dictionary<object, object>();
        
        public GameObjectBlackboard(GameObject gameObject)
        {
            _gameObject = gameObject;
        }
        
        public object this[object key]
        {
            get
            {
                var type = key as Type;
                if (type != null && type.IsSubclassOf(typeof(Component)))
                    return _gameObject.GetComponent(type);
                return _dict[key];
            }
            set
            {
                var type = key as Type;
                if (type != null && type.IsSubclassOf(typeof(Component)))
                    _gameObject.AddComponent(type);
                _dict[key] = value;
            }
        }

        public bool Has(object key)
        {
            var type = key as Type;
            if (type != null && type.IsSubclassOf(typeof(Component)))
                return _gameObject.GetComponent(type) != null;
            return _dict.ContainsKey(key);
        }

        public unsafe void* GetPtrRW(object key)
        {
            throw new NotImplementedException();
        }

        public unsafe void* GetPtrRO(object key)
        {
            var type = (Type)key;
            var obj = Activator.CreateInstance(type);
            var ptr = new IntPtr(UnsafeUtility.PinGCObjectAndGetAddress(obj, out var handle));
            Marshal.StructureToPtr(_dict[key], ptr, false);
            UnsafeUtility.ReleaseGCObject(handle);
            return ptr.ToPointer();
        }
    }
}
