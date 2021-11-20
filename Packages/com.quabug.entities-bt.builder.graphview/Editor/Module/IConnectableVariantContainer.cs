using System.Collections.Generic;

namespace EntitiesBT.Editor
{
    public interface IConnectableVariantContainer
    {
        IEnumerable<ConnectableVariant> ConnectableVariants { get; }
    }
}