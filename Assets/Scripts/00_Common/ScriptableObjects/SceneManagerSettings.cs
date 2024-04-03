using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SceneManagerSettings", menuName = "ScriptableObject/SceneManager Setting")]
    public class SceneManagerSettings : ScriptableObject
    {
        public AssetReferenceScene IntroSceneAsset;
        public AssetReferenceScene LoadingSceneAsset;
        public AssetReferenceScene TitleSceneAsset;
        public AssetReferenceScene MainSceneAsset;

        // 리소스 페이크 로딩 지속 시간
        public float FakeLoadingDuration;

        // 씬에서 사용하는 에셋 번들 라벨을 각 씬에 매핑합니다.
        public List<string> IntroSceneAssetBundleLabelMap = new List<string>();
        public List<string> LoadingSceneAssetBundleLabelMap = new List<string>();
        public List<string> TitleSceneAssetBundleLabelMap = new List<string>();
        public List<string> MainSceneAssetBundleLabelMap = new List<string>();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SceneManagerSettings))]
    public class SceneManagerSettingEditor : Editor
    {
        public void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            SceneManagerSettings script = (SceneManagerSettings)target;
            serializedObject.Update();

            // 수정 가능 필드 정의
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.IntroSceneAsset)), new GUIContent(nameof(script.IntroSceneAsset)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.LoadingSceneAsset)), new GUIContent(nameof(script.LoadingSceneAsset)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.TitleSceneAsset)), new GUIContent(nameof(script.TitleSceneAsset)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.MainSceneAsset)), new GUIContent(nameof(script.MainSceneAsset)));

            EditorGUILayout.Space(20);

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.IntroSceneAssetBundleLabelMap)), new GUIContent(nameof(script.IntroSceneAssetBundleLabelMap)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.LoadingSceneAssetBundleLabelMap)), new GUIContent(nameof(script.LoadingSceneAssetBundleLabelMap)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.TitleSceneAssetBundleLabelMap)), new GUIContent(nameof(script.TitleSceneAssetBundleLabelMap)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.MainSceneAssetBundleLabelMap)), new GUIContent(nameof(script.MainSceneAssetBundleLabelMap)));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
