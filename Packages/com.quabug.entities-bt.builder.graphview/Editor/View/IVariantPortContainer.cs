using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public interface IVariantPortContainer
    {
        IReadOnlyList<Port> Ports { get; }
    }
}