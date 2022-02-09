using System.Linq;
using EntitiesBT;
using EntitiesBT.Editor;
using JetBrains.Annotations;
using OneShot;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using BehaviorTreeNode = EntitiesBT.BehaviorTreeNode;

namespace GraphExt.Editor
{
    public sealed class BehaviorTreeGraphWindowExtension : IGraphWindowExtension
    {
        public BaseGraphInstaller BaseGraphInstaller;
        public NodeGraphInstaller<BehaviorTreeNode, BehaviorTreeNodeComponent> BehaviorGraphInstaller;
        public NodeGraphInstaller<VariantNode, VariantNodeComponent> VariantGraphInstaller;

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IMenuEntryInstaller[] MenuEntries;

        private Container _container;
        private MenuBuilder _menuBuilder;
        private WindowSystems _systems;
        private VisualElement _root;

        public void Recreate(VisualElement root)
        {
            _root = root;
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
            ResetGraphBackend(PrefabStageUtility.GetCurrentPrefabStage());
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
        }

        public void Tick()
        {
            _systems?.Tick();
        }

        public void Clear()
        {
            _menuBuilder?.Dispose();
            _menuBuilder = null;
            _systems?.Dispose();
            _systems = null;
            _container?.Dispose();
            _container = null;
        }

        private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
        {
            if (prefabStage == null)
            {
                RemoveGraphView();
                Clear();
            }
            else
            {
                var typeContainers = new TypeContainers();
                _container = new Container();
                _container.RegisterInstance(prefabStage);
                _container.RegisterInstance(prefabStage.prefabContentsRoot);

                BaseGraphInstaller.Install(_container, typeContainers);
                BehaviorGraphInstaller.Install(_container, typeContainers, prefabStage.prefabContentsRoot);
                VariantGraphInstaller.Install(_container, typeContainers, prefabStage.prefabContentsRoot);

                InstantiateSystems(_container);
                CreateMenuBuilder();
                ReplaceGraphView(_container.Resolve<UnityEditor.Experimental.GraphView.GraphView>());
            }
        }

        private void RemoveGraphView()
        {
            var graph = _root.Q<UnityEditor.Experimental.GraphView.GraphView>();
            if (graph != null) _root.Remove(graph);
        }

        private void ReplaceGraphView(UnityEditor.Experimental.GraphView.GraphView graphView)
        {
            RemoveGraphView();
            _root.Add(graphView);
        }

        private void InstantiateSystems(Container container)
        {
            _systems = container.Instantiate<WindowSystems>();
            _systems.Initialize();
        }

        private void CreateMenuBuilder()
        {
            foreach (var installer in MenuEntries) installer.Install(_container);
            _menuBuilder = _container.Instantiate<MenuBuilder>();
        }

        private void ClearEditorView(PrefabStage closingStage)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            // do NOT clear editor if current stage is not closing
            ResetGraphBackend(currentStage == closingStage ? null : currentStage);
        }
    }
}
