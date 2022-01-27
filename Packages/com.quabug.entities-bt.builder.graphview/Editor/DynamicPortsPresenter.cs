using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class DynamicPortsPresenter : ITickableWindowSystem
    {
        private readonly IPortViewFactory _portViewFactory;
        private readonly FindPortData _findPorts;
        private readonly IBiDictionary<NodeId, Node> _currentNodeViews;
        private readonly IBiDictionary<PortId, Port> _currentPortViews;
        private readonly IDictionary<PortId, PortData> _currentPortDataMap;

        public DynamicPortsPresenter(
            IPortViewFactory portViewFactory,
            FindPortData findPorts,
            IBiDictionary<NodeId, Node> currentNodeViews,
            IBiDictionary<PortId, Port> currentPortViews,
            IDictionary<PortId, PortData> currentPortDataMap
        )
        {
            _portViewFactory = portViewFactory;
            _findPorts = findPorts;
            _currentNodeViews = currentNodeViews;
            _currentPortViews = currentPortViews;
            _currentPortDataMap = currentPortDataMap;
        }

        public void Tick()
        {
            var currentPorts = _currentPortViews.Keys.ToHashSet();

            foreach (var nodePair in _currentNodeViews)
            {
                var nodeId = nodePair.Key;
                var node = nodePair.Value;
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
                portView.DisconnectAll();
                _currentPortViews.Remove(portId);
                _currentPortDataMap.Remove(portId);
                var container = FindPortContainer(portId);
                container?.RemovePort();
            }
        }
    }
}