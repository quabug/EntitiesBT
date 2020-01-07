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
            _bb.SetData(new BehaviorTreeTickDeltaTime{ Value = Time.deltaTime });
            VirtualMachine.Tick(_nodeBlobRef, _bb);
        }

        private void OnDestroy()
        {
            _nodeBlobRef.BlobRef.Dispose();
        }
    }
}
