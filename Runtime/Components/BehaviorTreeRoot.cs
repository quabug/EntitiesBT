using EntitiesBT.Components.DebugView;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [DisallowMultipleComponent]
    public class BehaviorTreeRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Order: File > RootNode")]
        [SerializeField] private TextAsset _file = default;
        [SerializeField] private BTNode _rootNode;
        [SerializeField] private BehaviorTreeThread _thread = BehaviorTreeThread.ForceRunOnMainThread;
        [SerializeField] private bool _generateDebugView;
        
        private void Reset()
        {
            _rootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (_rootNode != null && _rootNode.GetComponent<StopConvertToEntity>() == null)
                _rootNode.gameObject.AddComponent<StopConvertToEntity>();
            
            var blob = _file != null ? _file.ToBlob() : _rootNode.ToBlob();
            entity.AddBehaviorTree(dstManager, blob, _thread);
            
#if UNITY_EDITOR
            if (_generateDebugView) GenerateDebugView();
#endif

            void GenerateDebugView()
            {
                var debugView = new GameObject();
                var root = debugView.AddComponent<BTDebugViewRoot>();
                if (GetComponentInParent<ConvertToEntity>().ConversionMode == ConvertToEntity.Mode.ConvertAndInjectGameObject)
                {
                    debugView.name = "__bt_debug_view__";
                    debugView.transform.SetParent(transform);
                }
                else
                {
                    debugView.name = name;
                    var parent = "__bt_debug_views__".FindOrCreateGameObject();
                    debugView.transform.SetParent(parent.transform);
                }
                root.Init(new NodeBlobRef {BlobRef = blob}, new EntityBlackboard {Entity = entity, EntityManager = dstManager});
            }
        }
    }
}
