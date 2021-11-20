using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public interface INodePropertyContainer
    {
        [CanBeNull] NodePropertyView FindByPort([NotNull] Port port);
    }

    public static class NodePropertyContainerExtension
    {
        [CanBeNull] public static NodePropertyView FindByEdge([NotNull] this INodePropertyContainer container, [NotNull] Edge edge)
        {
            return container.FindByPort(edge.input) ?? container.FindByPort(edge.output);
        }
    }
}