using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using OneShot;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphView = GraphExt.Editor.GraphView;

namespace EntitiesBT.Editor
{
    [Serializable]
    public class BaseGraphInstaller
    {
        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IGraphViewFactory GraphViewFactory = new DefaultGraphViewFactory();

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IMenuEntryInstaller[] MenuEntries;

        public Container Container { get; private set; }

        public void Install(Container container, TypeContainers typeContainers)
        {
            container.RegisterInstance(GraphViewFactory);

            Container = typeContainers.CreateTypeContainer(
                container,
                typeof(IGraphViewFactory),
                typeof(GraphView),
                typeof(UnityEditor.Experimental.GraphView.GraphView)
            );

            Container.RegisterSingleton(() => GraphViewFactory.Create(FindCompatiblePorts));
            container.Register(Container.Resolve<GraphView>);
            container.Register<UnityEditor.Experimental.GraphView.GraphView>(Container.Resolve<GraphView>);

            foreach (var entryInstaller in MenuEntries) entryInstaller.Install(Container);

            IEnumerable<Port> FindCompatiblePorts(Port startPort)
            {
                var behaviorGraphContainer = typeContainers.GetTypeContainer(typeof(GraphRuntime<EntitiesBT.BehaviorTreeNode>));
                var behaviorGraphPorts = behaviorGraphContainer.Resolve<IReadOnlyDictionary<Port, PortId>>();
                var behaviorGraphFunc = behaviorGraphContainer.Resolve<GraphView.FindCompatiblePorts>();

                // var variantGraphContainer = typeContainers.GetTypeContainer(typeof(GraphRuntime<VariantNode>));
                // var variantGraphPorts = variantGraphContainer.Resolve<IReadOnlyDictionary<Port, PortId>>();
                // var variantGraphFunc = variantGraphContainer.Resolve<GraphView.FindCompatiblePorts>();

                // TODO: check compatible across graph
                if (behaviorGraphPorts.ContainsKey(startPort)) return behaviorGraphFunc(startPort);
                // if (variantGraphPorts.ContainsKey(startPort)) return variantGraphFunc(startPort);
                return Enumerable.Empty<Port>();
            }
        }
    }
}