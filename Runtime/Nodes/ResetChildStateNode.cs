using System.Linq;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    // [BehaviorNode("81A539FE-FC10-4746-9AFB-D41C01E8727B", BehaviorNodeType.Decorate)]
    // public class ResetChildStateNode
    // {
    //     public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
    //     {
    //         var endIndex = blob.GetEndIndex(index);
    //         var firstChildIndex = index + 1;
    //         if (firstChildIndex >= endIndex) return 0;
    //         
    //         blob.ResetStates(firstChildIndex);
    //         return blob.TickChildren(index, bb).FirstOrDefault();
    //     }
    // }
}
