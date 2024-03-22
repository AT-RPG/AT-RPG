using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;
using System;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
    /// <summary>
    /// 에셋 번들의 리소스 레퍼런스 <br/>
    /// 인스펙터에서 리소스를 바인딩하면 해당 리소스의 이름으로 리소스를 얻을 수 있습니다.  <br/>
    /// </summary>
    [System.Serializable]
    public struct ResourceReference<T> where T : UnityObject
    {
        [SerializeField] private string resourceName;

        public T Resource => ResourceManager.Get<T>(resourceName);

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
            resourceGUID = Guid.Empty.ToString();        
#endif
        }

#if UNITY_EDITOR
        [SerializeField] private string resourceGUID;
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
            SerializedProperty resourceGUID = property.FindPropertyRelative("resourceGUID");

            // 인스펙터 변경 감지
            EditorGUI.BeginChangeCheck();

            // 현재 GUID를 바탕으로 리소스를 찾습니다.
            Guid currentGUID = Guid.Empty;
            Guid.TryParse(resourceGUID.stringValue, out currentGUID);
            UnityObject currentResource = null;
            if (currentGUID != Guid.Empty)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(resourceGUID.stringValue);
                currentResource = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
            }

            // 인스펙터에서 수정한 리소스 정보를 GET
            UnityObject updatedResource = EditorGUI.ObjectField(position, label, currentResource, typeof(UnityObject), allowSceneObjects: false);
            
            // 인스펙터 변경 감지됨
            if (EditorGUI.EndChangeCheck())
            {
                if (updatedResource)
                {
                    string newPath = AssetDatabase.GetAssetPath(updatedResource);
                    string newGUIDString = AssetDatabase.AssetPathToGUID(newPath);
                    resourceName.stringValue = updatedResource.name;
                    resourceGUID.stringValue = newGUIDString;
                }
                else
                {
                    resourceName.stringValue = "";
                    resourceGUID.stringValue = Guid.Empty.ToString();
                }

                // 변경 사항 저장
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
