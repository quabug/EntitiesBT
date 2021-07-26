#if UNITY_EDITOR

using System.Linq;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntitiesBT.DebugView
{
    [UpdateAfter(typeof(VirtualMachineSystem))]
    public class BehaviorTreeDebugViewerSystem : SystemBase
    {
        private const string _NAME = "__bt_debug_view__";

        private class ReferenceObject : ISystemStateComponentData
        {
            public BTDebugViewTreesManager Value;
        }

        protected override void OnUpdate()
        {
            Entities.WithoutBurst()
                .WithStructuralChanges()
                .WithAll<BehaviorTreeDebug>()
                .WithNone<ReferenceObject>()
                .ForEach((Entity entity, Transform transform) =>
            {
                var debugView = new GameObject();
                var root = debugView.AddComponent<BTDebugViewTreesManager>();
                root.EntityManager = EntityManager;
                root.Entity = entity;
                debugView.name = _NAME;
                debugView.transform.SetParent(transform);
                EntityManager.AddComponentObject(entity, new ReferenceObject { Value = root });
            }).Run();

            Entities.WithoutBurst()
                .WithStructuralChanges()
                .WithAll<BehaviorTreeDebug>()
                .WithNone<ReferenceObject, Transform>()
                .ForEach((Entity entity) =>
            {
                var debugView = new GameObject();
                var root = debugView.AddComponent<BTDebugViewTreesManager>();
                root.EntityManager = EntityManager;
                root.Entity = entity;
                var parent = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(obj => obj.name == _NAME)
                             ?? new GameObject(_NAME);
                debugView.transform.SetParent(parent.transform);
                debugView.name = EntityManager.GetName(entity);
                EntityManager.AddComponentObject(entity, new ReferenceObject { Value = root });
            }).Run();

            Entities.WithoutBurst()
                .WithStructuralChanges()
                .WithNone<BehaviorTreeDebug>()
                .ForEach((Entity entity, ReferenceObject @ref) =>
                {
                    GameObject.Destroy(@ref.Value.gameObject);
                    EntityManager.RemoveComponent<ReferenceObject>(entity);
                })
                .Run();

            Entities.WithoutBurst()
                .WithAll<BehaviorTreeDebug>()
                .ForEach((ReferenceObject @ref) => @ref.Value.Tick())
                .Run();
        }
    }
}

#endif