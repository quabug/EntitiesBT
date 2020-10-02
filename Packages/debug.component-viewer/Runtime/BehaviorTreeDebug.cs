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
    [AddComponentMenu("")] // hide in component menu
    public class BTDebugViewTreesManager : MonoBehaviour
    {
        [NonSerialized] public Entity Entity;
        [NonSerialized] public EntityManager EntityManager;

        private IDictionary<NodeBlobRef, GameObject> _trees = new Dictionary<NodeBlobRef, GameObject>();

        public void Tick()
        {
            if (EntityManager == default
                || !EntityManager.Exists(Entity)
                || !EntityManager.HasComponent<BehaviorTreeBufferElement>(Entity))
            {
                for (var i = transform.childCount - 1; i >= 0; i--)
                    Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                var buffer = EntityManager.GetBuffer<BehaviorTreeBufferElement>(Entity);
                var trees = new Dictionary<NodeBlobRef, GameObject>(buffer.Length * 2);
                for (var i = 0; i < buffer.Length; i++)
                {
                    _trees.TryGetValue(buffer[i].NodeBlob, out var tree);
                    if (tree == null)
                    {
                        tree = new GameObject(EntityManager.GetName(buffer[i].BehaviorTree));
                        tree.transform.SetParent(transform);
                        var root = tree.AddComponent<BTDebugViewRoot>();
                        root.Blackboard = new EntityBlackboard { EntityManager = EntityManager, Entity = Entity };
                        root.Blob = buffer[i].NodeBlob;
                    }
                    tree.transform.SetAsLastSibling();
                    trees[buffer[i].NodeBlob] = tree;
                }

                for (var i = transform.childCount - buffer.Length - 1; i >= 0; i--)
                    Destroy(transform.GetChild(i).gameObject);

                _trees = trees;
            }
        }
    }
    
    [AddComponentMenu("")] // hide in component menu
    public class BTDebugViewRoot : MonoBehaviour
    {
        [NonSerialized] public EntityBlackboard Blackboard;
        [NonSerialized] public NodeBlobRef Blob;
        private readonly List<BTDebugView> _views = new List<BTDebugView>();

        private void OnEnable() {}

        private void Start()
        {
            CreateViews(Blob);
        }

        public void Update()
        {
            if (!enabled) return;
            GetComponentsInChildren(_views);
            foreach (var view in _views) view.Tick();
        }

        private void CreateViews(NodeBlobRef blob)
        {
            var views = new List<GameObject>(blob.Count);
            for (var i = 0; i < blob.Count; i++)
            {
                var nodeTypeId = blob.GetTypeId(i);
                var debugViewTypeList = DebugComponentLookUp.NODE_COMPONENTS_MAP[nodeTypeId];
                
                var nodeGameObject = new GameObject();
                views.Add(nodeGameObject);
                nodeGameObject.name = $"{DebugComponentLookUp.BEHAVIOR_NODE_ID_TYPE_MAP[nodeTypeId].Name}";
                if (debugViewTypeList.Any())
                {
                    foreach (var viewType in debugViewTypeList)
                    {
                        var view = nodeGameObject.AddComponent(viewType) as BTDebugView;
                        if (view != null) view.Blob = blob;
                    }
                }
                else
                {
                    nodeGameObject.name = $"?? {nodeGameObject.name}";
                    nodeGameObject.AddComponent<BTDebugView>().Blob = blob;
                }
                
                var parentIndex = blob.ParentIndex(i);
                var parent = parentIndex >= 0 ? views[parentIndex].transform : transform;
                nodeGameObject.transform.SetParent(parent);
            }
            
            for (var i = 0; i < views.Count; i++)
            {
                foreach (var view in views[i].GetComponents<BTDebugView>())
                {
                    view.Index = i;
                    view.Blackboard = Blackboard;
                    view.Init();
                }
            }
        }
    }
}

#endif
