using System;
using System.Collections.Generic;
using EntitiesBT.Core;
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

        public ref T GetRef<T>(object key) where T : struct
        {
            throw new NotImplementedException();
        }

        public bool Has(object key)
        {
            var type = key as Type;
            if (type != null && type.IsSubclassOf(typeof(Component)))
                return _gameObject.GetComponent(type) != null;
            return _dict.ContainsKey(key);
        }
    }
}
