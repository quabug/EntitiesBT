using System;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [Serializable]
    public sealed class GraphWindowExtension : IGraphWindowExtension
    {
        [SerializeField] private GraphInstaller _installer;

        [SerializeReference, Nuwa.SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
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
                try
                {
                    var typeContainers = new TypeContainers();
                    _container = new Container();
                    _container.RegisterInstance(prefabStage).AsSelf();
                    _container.RegisterInstance(prefabStage.prefabContentsRoot).AsSelf();

                    _installer.Install(_container, typeContainers, prefabStage.prefabContentsRoot);

                    InstantiateSystems(_container);
                    CreateMenuBuilder();
                    ReplaceGraphView(_container.Resolve<UnityEditor.Experimental.GraphView.GraphView>());
                    InjectComponents(prefabStage.prefabContentsRoot);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        // TODO: optimize
        private void InjectComponents(GameObject root)
        {
            foreach (var component in root.GetComponentsInChildren<Component>(includeInactive: true))
            {
                _container.InjectAll(component, component.GetType());
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
