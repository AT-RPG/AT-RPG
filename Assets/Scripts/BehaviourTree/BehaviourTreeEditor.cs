#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AT_RPG
{
    /// BT 툴 설정
    public partial class BehaviourTreeEditor
    {
        public static readonly string UxmlPath = "Assets/Scripts/BehaviourTree/BehaviourTreeEditor.uxml";

        public static readonly string UssPath = "Assets/Scripts/BehaviourTree/BehaviourTreeEditor.uss";
    }


    public partial class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeEditorGraphView graphView;

        private BehaviourTreeEditorInspectorView inspectorView;

        private BehaviourTree tree;

        private static BehaviourTree cacheTree;

        private ObjectField treeField;



        [MenuItem("AT_RPG/BehaviourTreeEditor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public static void CloseWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            if (wnd != null)
            {
                wnd.Close();
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (tree != null)
            {
                CloseWindow();
                OpenWindow();
                cacheTree = tree;
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            LoadUXML();
            LoadUSS();
            Initialize();
        }



        /// <summary>
        /// UXML 파일을 로드하고 rootVisualElement에 클론.
        /// </summary>
        private void LoadUXML()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            uxml.CloneTree(rootVisualElement);
        }

        /// <summary>
        /// USS 파일을 로드하고 rootVisualElement에 추가.
        /// </summary>
        private void LoadUSS()
        {
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            rootVisualElement.styleSheets.Add(uss);
        }

        /// <summary>
        /// 초기 설정 작업을 스케줄.
        /// </summary>
        private void Initialize()
        {
            rootVisualElement.schedule.Execute(() =>
            {
                graphView = rootVisualElement.Q<BehaviourTreeEditorGraphView>();
                inspectorView = rootVisualElement.Q<BehaviourTreeEditorInspectorView>();

                treeField = rootVisualElement.Q<ObjectField>("BehaviourTree");
                treeField.RegisterValueChangedCallback(OnBehaviourTreeChanged);
                treeField.value = cacheTree;

            }).StartingIn(100);
        }

        private void DeleteGraphView()
        {
            graphView.DeleteTreeView();
            inspectorView.Update();
        }

        private void CreateGraphView(BehaviourTree tree)
        {
            if (tree != null)
            {
                this.tree = tree;
                graphView.focusable = true;
                graphView.OnNodeSelected = OnNodeSelect;
                graphView.OnNodeUnselected = OnNodeUnselect;
                graphView.OnSetAsRoot = OnSetAsRoot;

                graphView.CreateTreeView(this.tree);
            }
        }

        private void OnBehaviourTreeChanged(ChangeEvent<Object> evt)
        {
            DeleteGraphView();

            BehaviourTree tree = evt.newValue as BehaviourTree;
            if (tree != null)
            {
                CreateGraphView(tree);
            }
        }

        /// <summary>
        /// 인스펙터에 노드의 내용을 직렬화
        /// </summary>
        private void OnNodeSelect(BehaviourTreeEditorNodeView nodeView)
        {
            inspectorView.Update(nodeView);
        }

        /// <summary>
        /// 인스펙터에서 노드의 내용을 클리어.
        /// </summary>
        private void OnNodeUnselect(BehaviourTreeEditorNodeView nodeView)
        {
            inspectorView.Update(null);
        }

        /// <summary>
        /// 타겟 노드를 <see cref="tree"/>의 루트 노드로 설정.
        /// </summary>
        /// <param name="node"></param>
        private void OnSetAsRoot(BehaviourNode node)
        {
            tree.SetRoot(node);
        }
    }
}

#endif