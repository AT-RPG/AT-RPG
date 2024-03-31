using System;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
    [CreateAssetMenu(fileName = "ResourceManagerSettings", menuName = "ScriptableObject/ResourceManager Setting")]
    public class ResourceManagerSetting : ScriptableObject
    {
        // 어드레서블 에셋 번들의 저장 경로
        public string AssetBundlesSavePath = Application.streamingAssetsPath;

        // 리소스 페이크 로딩 지속 시간
        public float FakeLoadingDuration = 0.75f;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ResourceManagerSetting))]
    public class ResourceManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResourceManagerSetting script = (ResourceManagerSetting)target;
            serializedObject.Update();

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                
            }
            EditorGUI.EndDisabledGroup();

            // 수정 가능 필드 정의
            script.AssetBundlesSavePath = EditorGUILayout.TextField(nameof(script.AssetBundlesSavePath), script.AssetBundlesSavePath);
            script.FakeLoadingDuration = EditorGUILayout.FloatField(nameof(script.FakeLoadingDuration), script.FakeLoadingDuration);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

