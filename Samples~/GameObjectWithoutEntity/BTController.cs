using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class BTController : MonoBehaviour
    {
        public BTNode RootNode;
        private NodeBlobRef _nodeBlobRef;
        private IBlackboard _bb;

        private void Awake()
        {
            _nodeBlobRef = new NodeBlobRef(RootNode.ToBlob());
            Destroy(RootNode.gameObject);
            _bb = new GameObjectBlackboard(gameObject);
            VirtualMachine.Reset(_nodeBlobRef, _bb);
        }

        private void Update()
        {
            _bb.SetData(new TickDeltaTime{ Value = Time.deltaTime });
            VirtualMachine.Tick(_nodeBlobRef, _bb);
        }

        private void OnDestroy()
        {
            _nodeBlobRef.BlobRef.Dispose();
        }
    }

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
