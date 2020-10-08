using System;
using System.Collections.Generic;
using System.Linq;
using DotsStencil;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual.Editor
{
    [Serializable, ComponentNodeSearcher("EntitiesBT/Variable/")]
    class VisualVariableComponentModel : DotsNodeModel<VisualVariableComponent>, IReferenceComponentTypes
    {
        Type ComponentType => TypedNode.Type.TypeIndex == -1 ? null : TypeManager.GetType(TypedNode.Type.TypeIndex);
        public override string Title => "Visual Variable " + (TypedNode.Type.TypeIndex == -1 ? "<Unknown Component>" : ComponentType.Name);
        public override IReadOnlyDictionary<string, List<PortMetaData>> PortCustomData =>
            new Dictionary<string, List<PortMetaData>>
        {
            {
                nameof(VisualVariableComponent.ComponentData),
                TypedNode.Type.TypeIndex == -1 ? new List<PortMetaData>() : ComponentType.ToPortMetaData().ToList()
            }
        };

        public IEnumerable<TypeReference> ReferencedTypes => Enumerable.Repeat(TypedNode.Type, 1);
    }
}
