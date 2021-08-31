using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public struct GameObjectBlackboard : IBlackboard
    {
        private readonly GameObject _gameObject;
        private readonly Dictionary<object, object> _dict;
        
        public GameObjectBlackboard(GameObject gameObject)
        {
            _gameObject = gameObject;
            _dict = new Dictionary<object, object>();
        }

        public void SetData<T>(T value) where T : struct
        {
            _dict[typeof(T)] = value;
        }

        public bool HasData<T>() where T : struct
        {
            return _dict.ContainsKey(typeof(T));
        }

        public T GetData<T>() where T : struct
        {
            return (T)_dict[typeof(T)];
        }

        public ref T GetDataRef<T>() where T : struct
        {
            throw new NotImplementedException();
        }

        public bool HasData(Type type)
        {
            return _dict.ContainsKey(type);
        }

        public IntPtr GetDataPtrRO(Type type)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetDataPtrRW(Type type)
        {
            throw new NotImplementedException();
        }

        public T GetObject<T>() where T : class
        {
            if (typeof(T).IsSubclassOf(typeof(UnityEngine.Component)))
                return _gameObject.GetComponent<T>();
            _dict.TryGetValue(typeof(T), out var obj);
            return (T)obj;
        }
    }
}
