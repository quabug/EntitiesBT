using System.Collections.Generic;

namespace EntitiesBT.Editor
{
    public interface IConnectableVariantContainer
    {
        IEnumerable<GraphNodeVariant.Any> ConnectableVariants { get; }
    }
}