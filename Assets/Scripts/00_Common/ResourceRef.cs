using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;

namespace AT_RPG
{
    /// <summary>
    /// 에셋 번들용 리소스 레퍼런스
    /// </summary>
    [System.Serializable]
    public struct ResourceRef<T> where T : Object
    {
        [SerializeField] private string resourceName;
        [SerializeField] public T Resource => ResourceManager.Instance.Get<T>(resourceName, false);

#if UNITY_EDITOR
        [SerializeField] public Object editorResourceViewer;
#endif
    }

    /// <summary>
    /// 에셋 번들용 글로벌 리소스 레퍼런스
    /// </summary>
    [System.Serializable]
    public struct GlobalResourceRef<T> where T : Object
    {
        [SerializeField] private string resourceName;
        [SerializeField] public T Resource => ResourceManager.Instance.Get<T>(resourceName, true);

#if UNITY_EDITOR
        [SerializeField] public Object editorResourceViewer;
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GlobalResourceRef<>))]
    public class GlobalResourceRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // GlobalResource.resourceName을 리플렉션
            SerializedProperty resourceName = property.FindPropertyRelative("resourceName");
            SerializedProperty resource = property.FindPropertyRelative("editorResourceViewer");

            // 인스펙터 변경 감지
            EditorGUI.BeginChangeCheck();

            // 인스펙터에서 수정한 리소스 정보를 GET
            Object newResource = EditorGUI.ObjectField(position, label, resource.objectReferenceValue, typeof(Object), allowSceneObjects: false);

            // 인스펙터 변경 감지됨
            if (EditorGUI.EndChangeCheck())
            {
                if (newResource)
                {
                    resourceName.stringValue = newResource.name;
                    resource.objectReferenceValue = newResource;
                }
                else
                {
                    resourceName.stringValue = "";
                    resource.objectReferenceValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ResourceRef<>))]
    public class ResourceRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // ResourceRef.resourceName을 리플렉션
            SerializedProperty resourceName = property.FindPropertyRelative("resourceName");
            SerializedProperty resource = property.FindPropertyRelative("editorResourceViewer");

            // 인스펙터 변경 감지
            EditorGUI.BeginChangeCheck();

            // 인스펙터에서 수정한 리소스 정보를 GET
            Object newResource = EditorGUI.ObjectField(position, label, resource.objectReferenceValue, typeof(Object), allowSceneObjects: false);
            
            // 인스펙터 변경 감지됨
            if (EditorGUI.EndChangeCheck())
            {
                if (newResource)
                {
                    resourceName.stringValue = newResource.name;
                    resource.objectReferenceValue = newResource;
                }
                else
                {
                    resourceName.stringValue = "";
                    resource.objectReferenceValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
