using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public interface IConnectableVariantViewContainer
    {
        [CanBeNull] ConnectableVariantView FindByPort([NotNull] Port port);
        [NotNull] IEnumerable<ConnectableVariantView> Views { get; }
    }

    public static class ConnectableVariantViewContainerExtension
    {
        [CanBeNull] public static ConnectableVariantView FindByEdge([NotNull] this IConnectableVariantViewContainer container, [NotNull] Edge edge)
        {
            return container.FindByPort(edge.input) ?? container.FindByPort(edge.output);
        }
    }
}