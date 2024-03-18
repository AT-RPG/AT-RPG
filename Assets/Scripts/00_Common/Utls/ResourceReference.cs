using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;
using System;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
    /// <summary>
    /// 에셋 번들의 리소스 레퍼런스 <br/>
    /// 인스펙터에서 리소스를 바인딩하면 해당 리소스의 이름으로 리소스를 얻을 수 있습니다.
    /// </summary>
    [System.Serializable]
    public struct ResourceReference<T> where T : UnityObject
    {
        [SerializeField] private string resourceName;

        // 바인딩된 리소스를 반환합니다.
        // + 환경이 에디터이면 바인딩된 리소스를 그대로 반환합니다.
        // + 환경이 런타임이면 ResourceManager에서 GUID에 매핑된 정보를 통해 리소스를 반환합니다. 
        public T Resource
        {
            get
            {
#if UNITY_EDITOR
                return editorResource as T;
#else
                return ResourceManager.Get<T>(resourceName);
#endif
            }
        }
            
        public ResourceReference(UnityObject resource)
        {
            // 리소스의 래퍼런스로 이름을 사용해서 찾기 때문에
            // (Clone)을 제거
            if (resource.name.EndsWith("(Clone)"))
            {
                resourceName = resource.name.Replace("(Clone)", "");
            }
            else
            {
                resourceName = resource.name;
            }

#if UNITY_EDITOR
            editorResource = resource;
#endif
        }

#if UNITY_EDITOR
        [SerializeField] private UnityObject editorResource;
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
            UnityObject newResource = EditorGUI.ObjectField(position, label, resource.objectReferenceValue, typeof(UnityObject), allowSceneObjects: false);
            
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
