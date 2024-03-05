using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 씬 에셋 레퍼런스
    /// </summary>
    [System.Serializable]
    public struct SceneReference
    {
        [SerializeField] private string sceneName;

#if UNITY_EDITOR
        [SerializeField] public Object editorSceneAsset;
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // SceneReference을 리플렉션
            SerializedProperty sceneName = property.FindPropertyRelative("sceneName");
            SerializedProperty sceneAsset = property.FindPropertyRelative("editorSceneAsset");

            // 인스펙터 변경 감지
            EditorGUI.BeginChangeCheck();

            // 인스펙터에서 수정한 리소스 정보를 GET
            Object newSceneAsset = EditorGUI.ObjectField(position, label, sceneAsset.objectReferenceValue, typeof(Object), allowSceneObjects: false);

            // 인스펙터 변경 감지됨
            if (EditorGUI.EndChangeCheck())
            {
                if (newSceneAsset)
                {
                    sceneName.stringValue = newSceneAsset.name;
                    sceneAsset.objectReferenceValue = newSceneAsset;
                }
                else
                {
                    sceneName.stringValue = "";
                    sceneAsset.objectReferenceValue = null;
                }

                // 변경 사항 저장
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}