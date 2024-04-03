using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "ResourceManagerSettings", menuName = "ScriptableObject/ResourceManager Setting")]
    public class ResourceManagerSettings : ScriptableObject
    {
        // 어드레서블 에셋 번들의 폴더 저장 경로
        public string AssetBundlesSavePath = Application.streamingAssetsPath;

        // 어드레서블 에셋 매핑 파일의 폴더 저장 경로
        public string AssetGuidMapSavePath = Application.streamingAssetsPath;

        // 어드레서블 에셋 매핑 파일의 이름
        public string AssetGuidMapFileName = "AssetGuidMap";

        // 리소스 페이크 로딩 지속 시간
        public float FakeLoadingDuration;


        /// <summary>
        /// 어드레서블 에셋 매핑 파일 경로
        /// </summary>
        public string GetAssetGuidMapFilePath() => Path.Combine(AssetGuidMapSavePath, AssetGuidMapFileName);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ResourceManagerSettings))]
    public class ResourceManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResourceManagerSettings script = (ResourceManagerSettings)target;
            serializedObject.Update();

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                
            }
            EditorGUI.EndDisabledGroup();

            // 수정 가능 필드 정의
            {
                EditorGUILayout.LabelField("어드레서블 설정", EditorStyles.boldLabel);
                script.AssetBundlesSavePath = EditorGUILayout.TextField(nameof(script.AssetBundlesSavePath), script.AssetBundlesSavePath);
                script.AssetGuidMapSavePath = EditorGUILayout.TextField(nameof(script.AssetGuidMapSavePath), script.AssetGuidMapSavePath);
                script.AssetGuidMapFileName = EditorGUILayout.TextField(nameof(script.AssetGuidMapFileName), script.AssetGuidMapFileName);

                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("리소스 매니저 설정", EditorStyles.boldLabel);
                script.FakeLoadingDuration = EditorGUILayout.FloatField(nameof(script.FakeLoadingDuration), script.FakeLoadingDuration);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

