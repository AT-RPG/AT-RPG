#if UNITY_EDITOR

using UnityEditor;
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
        [MenuItem("AT_RPG/BehaviourTreeEditor")]
        public static void ShowExample()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            /// 글로벌 UXML 임포트
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            uxml.CloneTree(root);

            /// 글로벌 uss 임포트
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            root.styleSheets.Add(uss);
        }
    }
}

#endif