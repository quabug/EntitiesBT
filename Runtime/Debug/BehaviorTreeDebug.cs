#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [DisallowMultipleComponent]
    public class BehaviorTreeDebug : MonoBehaviour, IConvertGameObjectToEntity
    {
        private void OnEnable() {}

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled) return;
            
            var debugView = new GameObject();
            var root = debugView.AddComponent<BTDebugViewRoot>();
            root.EntityManager = dstManager;
            root.Entity = entity;
            if (GetComponentInParent<ConvertToEntity>().ConversionMode == ConvertToEntity.Mode.ConvertAndInjectGameObject)
            {
                debugView.name = "__bt_debug_view__";
                debugView.transform.SetParent(transform);
            }
            else
            {
                debugView.name = name;
                var parent = FindOrCreateGameObject("__bt_debug_views__");
                debugView.transform.SetParent(parent.transform);
            }
        }

        public GameObject FindOrCreateGameObject(string name)
        {
            var obj = GameObject.Find(name);
            return obj ? obj : new GameObject(name);
        }
    }
    
    public class BTDebugViewRoot : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> Views;
        [NonSerialized] public Entity Entity;
        [NonSerialized] public EntityManager EntityManager;
        private EntityBlackboard _blackboard;

        private void OnEnable() {}

        private void Update()
        {
            if (EntityManager == null) return;
            if (!EntityManager.HasComponent<NodeBlobRef>(Entity)) return;
            if (_blackboard == null) _blackboard = new EntityBlackboard {EntityManager = EntityManager, Entity = Entity};
            
            var blob = EntityManager.GetComponentData<NodeBlobRef>(Entity);
            if (Views == null) CreateViews(blob);

            foreach (var view in Views.SelectMany(t => t.GetComponents<BTDebugView>())) view.Tick();
        }

        private void CreateViews(NodeBlobRef blob)
        {
            Views = new List<GameObject>(blob.Count);
            for (var i = 0; i < blob.Count; i++)
            {
                var nodeTypeId = blob.GetTypeId(i);
                var debugViewTypeList = DebugComponentLookUp.NODE_COMPONENTS_MAP[nodeTypeId];
                
                var nodeGameObject = new GameObject();
                Views.Add(nodeGameObject);
                nodeGameObject.name = $"{DebugComponentLookUp.BEHAVIOR_NODE_ID_TYPE_MAP[nodeTypeId].Name}";
                if (debugViewTypeList.Any())
                {
                    foreach (var viewType in debugViewTypeList)
                        nodeGameObject.AddComponent(viewType);
                }
                else
                {
                    nodeGameObject.name = $"?? {nodeGameObject.name}";
                    nodeGameObject.AddComponent<BTDebugView>();
                }
                
                var parentIndex = blob.ParentIndex(i);
                var parent = parentIndex >= 0 ? Views[parentIndex].transform : transform;
                nodeGameObject.transform.SetParent(parent);
            }
            
            for (var i = 0; i < Views.Count; i++)
            {
                foreach (var view in Views[i].GetComponents<BTDebugView>())
                {
                    view.Index = i;
                    view.Blackboard = _blackboard;
                    view.Blob = blob;
                    view.Entity = Entity;
                    view.EntityManager = EntityManager;
                    view.Init();
                }
            }
        }
    }
}

#endif
