using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variable;
using Runtime;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Builder.Visual
{
    public class GraphVariablePropertyReader<T> : IVariablePropertyReader<T> where T : unmanaged
    {
        private readonly InputDataPort _port;

        public GraphVariablePropertyReader(InputDataPort port)
        {
            _port = port;
        }

        public void Allocate(ref BlobBuilder builder, ref BlobVariableReader<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = ID;
            builder.Allocate(ref blobVariable, _port);
        }

        static GraphVariablePropertyReader()
        {
            var type = typeof(GraphVariablePropertyReader<T>);
            VariableReaderRegisters<T>.Register(ID, type.Getter("GetData"), GetComponentAccess);

            IEnumerable<ComponentType> GetComponentAccess(ref BlobVariableReader<T> variable)
            {
                return ComponentType.ReadOnly<CurrentBehaviorTreeComponent>().Yield();
            }
        }

        public static readonly int ID = new Guid("A3B60190-98AF-42DC-A723-35AD725172BB").GetHashCode();

        [Preserve]
        private static unsafe T GetData<TNodeBlob, TBlackboard>(ref BlobVariableReader<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var port = ref blobVariable.Value<InputDataPort>();
            var behaviorTree = bb.GetData<CurrentBehaviorTreeComponent>().RefValue.BehaviorTree;
            // HACK: how to support multiple worlds?
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var graphInstance = entityManager.GetComponentObject<GraphInstanceComponent>(behaviorTree).Value;

            T data;
            void* ptr = &data;
            var value = graphInstance.ReadValue(port);
            Value.SetPtrToValue(ptr, value.Type, value);
            return data;
        }
    }
}
