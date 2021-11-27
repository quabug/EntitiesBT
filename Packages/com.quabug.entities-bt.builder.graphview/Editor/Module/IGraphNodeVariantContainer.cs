using System.Collections.Generic;

namespace EntitiesBT.Editor
{
    public interface IGraphNodeVariantContainer
    {
        IEnumerable<GraphNodeVariant.Any> GraphNodeVariants { get; }
    }
}