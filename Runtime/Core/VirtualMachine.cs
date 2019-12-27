using System.Linq;

namespace EntitiesBT.Core
{
    public class VirtualMachine
    {
        private readonly Registries<IBehaviorNode> _registries;

        public VirtualMachine(Registries<IBehaviorNode> registries)
        {
            _registries = registries;
        }

        public NodeState Tick(INodeBlob blob, IBlackboard bb)
        {
            return Tick(0, blob, bb);
        }
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            var state = _registries[typeId].Tick(index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
            return state;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            var typeId = blob.GetTypeId(index);
            _registries[typeId].Reset(index, blob, bb);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
        }

        public void Reset(int fromIndex, int count, INodeBlob blob, IBlackboard bb)
        {
            for (var i = fromIndex; i < fromIndex + count; i++)
                Reset(i, blob, bb);
        }

        public void Reset(INodeBlob blob, IBlackboard bb)
        {
            var count = blob.GetEndIndex(0);
            Reset(0, count, blob, bb);
        }
    }
}