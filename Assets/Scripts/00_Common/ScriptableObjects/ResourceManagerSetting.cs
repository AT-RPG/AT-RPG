using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "ResourceManagerSettings", menuName = "ScriptableObject/ResourceManager Setting")]
    public class ResourceManagerSetting : ScriptableObject
    {
        // 에셋 번들 저장 경로
        [ReadOnly] public readonly string AssetBundlesSavePath = Application.streamingAssetsPath;

        // 리소스 GUID 파일 저장 경로
        [ReadOnly] public readonly string ResourceGUIDMapSavePath = Application.streamingAssetsPath;

        // 리소스 GUID 파일 이름
        [ReadOnly] public readonly string ResourceGUIDMapFileName = "ResourceGUIDMap";

        // 전역 에셋 번들 네이밍
        [ReadOnly] public readonly string GlobalAssetBundleName = "Global";

        // 리소스 페이크 로딩 지속 시간
        // NOTE : 비동기 로딩에만 적용
        public float FakeLoadingDuration = 0.75f;
    }


#if UNITY_EDITOR
    /// <summary>
    /// 파일 저장 경로를 ReadOnly로 인스펙터에서 수정을 막는 클래스
    /// </summary>
    [CustomEditor(typeof(ResourceManagerSetting))]
    public class ResourceManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResourceManagerSetting script = (ResourceManagerSetting)target;

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.LabelField(nameof(script.AssetBundlesSavePath), script.AssetBundlesSavePath);
                EditorGUILayout.LabelField(nameof(script.ResourceGUIDMapSavePath), script.ResourceGUIDMapSavePath);
                EditorGUILayout.LabelField(nameof(script.ResourceGUIDMapFileName), script.ResourceGUIDMapFileName);
                EditorGUILayout.LabelField(nameof(script.GlobalAssetBundleName), script.GlobalAssetBundleName);
            }
            EditorGUI.EndDisabledGroup();

            // 수정 가능 필드 정의
            {
                script.FakeLoadingDuration 
                    = EditorGUILayout.FloatField(nameof(script.FakeLoadingDuration), script.FakeLoadingDuration);
            }

            // 변경 내용 업데이트
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

