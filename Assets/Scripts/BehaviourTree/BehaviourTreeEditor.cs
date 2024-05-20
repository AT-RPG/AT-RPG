#if UNITY_EDITOR

using UnityEditor;
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
        private BehaviourTreeEditorGraphView treeView;

        private BehaviourTreeEditorInspectorView inspectorView;

        private BehaviourTree tree;

        [MenuItem("AT_RPG/BehaviourTreeEditor")]
        public static void ShowExample()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            ConnectUIBuilderFiles();
        }

        /// <summary>
        /// UI Builder파일(.uxml, .uss)를 코드와 연결.
        /// </summary>
        private void ConnectUIBuilderFiles()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            uxml.CloneTree(rootVisualElement);

            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            rootVisualElement.styleSheets.Add(uss);

            rootVisualElement.schedule.Execute(() =>
            {
                treeView = rootVisualElement.Q<BehaviourTreeEditorGraphView>();

                inspectorView = rootVisualElement.Q<BehaviourTreeEditorInspectorView>();

                var treeField = rootVisualElement.Q<ObjectField>("BehaviourTree");
                treeField.RegisterValueChangedCallback(evt =>
                {
                    treeView.DeleteView();

                    tree = evt.newValue as BehaviourTree;
                    if (tree)
                    {
                        treeView.Tree = tree;
                        treeView.CreateView();
                    }
                });

            }).StartingIn(1000);
        }
    }
}

#endif