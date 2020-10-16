using System;
using EntitiesBT.Variable;
using Runtime;
using Unity.Assertions;

namespace EntitiesBT.Builder.Visual
{
    [Serializable]
    public struct VisualVariableComponent : IVisualVariablePropertyNode
    {
        public TypeReference Type;
        public OutputDataMultiPort ComponentData;
        public bool UseRef;

        public IVariablePropertyReader<T> GetVariableProperty<T>(int dataIndex, GraphInstance instance, GraphDefinition definition) where T : unmanaged
        {
            for (uint i = 0; i < ComponentData.GetDataCount(); i++)
            {
                var port = ComponentData.SelectPort(i);
                var portIndex = (int)port.Port.Index;
                Assert.IsTrue(portIndex < definition.PortInfoTable.Count);
                if (definition.PortInfoTable[portIndex].DataIndex == dataIndex)
                {
                    var field = definition.GetComponentFieldDescription(Type, (int)i);
                    // TODO: check data type
                    if (field.HasValue)
                        return new DynamicComponentVariablePropertyReader<T>(field.Value.DeclaringTypeHash, field.Value.Offset, UseRef);
                    throw new IndexOutOfRangeException();
                }
            }
            throw new IndexOutOfRangeException();
        }
    }
}
