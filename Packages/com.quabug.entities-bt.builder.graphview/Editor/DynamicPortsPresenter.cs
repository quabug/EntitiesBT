using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

namespace EntitiesBT.Editor
{
    [UsedImplicitly]
    public class DynamicPortsPresenter : ITickableWindowSystem
    {
        private readonly GraphView _graphView;
        private readonly IPortViewFactory _portViewFactory;
        private readonly IReadOnlyDictionary<NodeId, Node> _currentNodeViews;
        private readonly IDictionary<PortId, Port> _currentPortViews;
        private readonly IDictionary<PortId, PortData> _currentPortDataMap;
        private readonly FindPortData _findPorts;

        public DynamicPortsPresenter(
            GraphView graphView,
            IPortViewFactory portViewFactory,
            IReadOnlyDictionary<NodeId, Node> currentNodeViews,
            IDictionary<PortId, Port> currentPortViews,
            IDictionary<PortId, PortData> currentPortDataMap,
            FindPortData findPorts
        )
        {
            _graphView = graphView;
            _portViewFactory = portViewFactory;
            _currentNodeViews = currentNodeViews;
            _currentPortViews = currentPortViews;
            _currentPortDataMap = currentPortDataMap;
            _findPorts = findPorts;
        }

        public void Tick()
        {
            var currentPorts = _currentPortViews.Keys.ToHashSet();

            foreach (var nodePair in _currentNodeViews)
            {
                var nodeId = nodePair.Key;
                var ports = _findPorts(nodeId);
                foreach (var port in ports)
                {
                    var portId = new PortId(nodeId, port.Name);
                    if (currentPorts.Contains(portId)) currentPorts.Remove(portId);
                    else AddPort(portId, port);
                }
            }

            foreach (var removed in currentPorts) DeletePort(removed);

            PortContainer FindPortContainer(in PortId portId)
            {
                var portName = portId.Name;
                return _currentNodeViews[portId.NodeId].Query<PortContainer>()
                    .Where(container => container.PortName == portName)
                    .First()
                ;
            }

            void AddPort(in PortId portId, in PortData port)
            {
                var container = FindPortContainer(portId);
                if (container == null) return; // TODO: warning?
                var portView = _portViewFactory.CreatePort(port);
                _currentPortViews.Add(portId, portView);
                _currentPortDataMap.Add(portId, port);
                container.AddPort(portView);
            }

            void DeletePort(in PortId portId)
            {
                var portView = _currentPortViews[portId];
                _graphView.DeleteElements(portView.connections);
                portView.DisconnectAll();
                _currentPortViews.Remove(portId);
                _currentPortDataMap.Remove(portId);
                var container = FindPortContainer(portId);
                container?.RemovePort();
            }
        }
    }
}