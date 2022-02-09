using System;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public class EdgeToCreateObserver : IWindowSystem, IDisposable
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly IEdgeViewFactory _edgeViewFactory;
        [NotNull] private readonly IBiDictionary<EdgeId, Edge> _currentEdgeViews;
        [NotNull] private readonly IReadOnlyBiDictionary<PortId, Port> _currentPortViews;
        [NotNull] private readonly EdgeConnectFunc _connectFunc;

        public EdgeToCreateObserver(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] IEdgeViewFactory edgeViewFactory,
            [NotNull] IBiDictionary<EdgeId, Edge> currentEdgeViews,
            [NotNull] IReadOnlyBiDictionary<PortId, Port> currentPortViews,
            [NotNull] EdgeConnectFunc connectFunc
        )
        {
            _view = view;
            _edgeViewFactory = edgeViewFactory;
            _currentEdgeViews = currentEdgeViews;
            _currentPortViews = currentPortViews;
            _connectFunc = connectFunc;
            _view.graphViewChanged += OnGraphChanged;
        }

        public void Dispose()
        {
            _view.graphViewChanged -= OnGraphChanged;
        }

        private GraphViewChange OnGraphChanged(GraphViewChange @event)
        {
            if (@event.edgesToCreate != null)
            {
                foreach (var edge in @event.edgesToCreate)
                {
                    if (!_currentEdgeViews.ContainsValue(edge))
                    {
                        var input = _currentPortViews.GetKey(edge.input);
                        var output = _currentPortViews.GetKey(edge.output);
                        var edgeId = new EdgeId(input: input, output: output);
                        _currentEdgeViews.Add(edgeId, edge);
                        _connectFunc(input: input, output: output);
                        _edgeViewFactory.AfterCreated(edge);
                    }
                }
            }

            return @event;
        }
    }
}
