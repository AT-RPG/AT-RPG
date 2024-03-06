using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;

namespace AT_RPG
{
    /// <summary>
    /// 에셋 번들의 리소스 레퍼런스
    /// </summary>
    [System.Serializable]
    public struct ResourceReference<T> where T : Object
    {
        [SerializeField] private string resourceName;
        [SerializeField] public T Resource => ResourceManager.Instance.Get<T>(resourceName);
            
        public ResourceReference(Object resource)
        {
            resourceName = resource.name;

#if UNITY_EDITOR
            editorResource = resource;
#endif
        }

#if UNITY_EDITOR
        [SerializeField] public Object editorResource;
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourceReference<>))]
    public class ResourceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // ResourceReference 리플렉션
            SerializedProperty resourceName = property.FindPropertyRelative("resourceName");
            SerializedProperty resource = property.FindPropertyRelative("editorResource");

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

                // 변경 사항 저장
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
