using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SaveLoadManagerSettings", menuName = "ScriptableObject/SaveLoadManager Setting")]
    public partial class SaveLoadManagerSetting : ScriptableObject
    {
        // 세이브 파일 기본 경로
        public string   defaultSaveFolderPath;

        // 세이브 파일 페이크 로딩 지속 시간
        public float    fakeLoadingDuration = 0.75f;

        // 직렬화된 게임 오브젝트 데이터의 확장명
        public readonly string gameObjectDataFileExtension = "god";

        // 직렬화된 맵 설정 데이터의 확장명
        public readonly string worldSettingDataFileExtension = "msd";

        public void OnEnable()
        {
            defaultSaveFolderPath = Application.persistentDataPath + "/Saves";
        }
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(SaveLoadManagerSetting))]
    public partial class SaveLoadManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SaveLoadManagerSetting script = (SaveLoadManagerSetting)target;

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.LabelField(
                    nameof(script.defaultSaveFolderPath), script.defaultSaveFolderPath);
                EditorGUILayout.LabelField(
                    nameof(script.gameObjectDataFileExtension), script.gameObjectDataFileExtension);
                EditorGUILayout.LabelField(
                    nameof(script.worldSettingDataFileExtension), script.worldSettingDataFileExtension);
            }
            EditorGUI.EndDisabledGroup();

            // 수정 가능 필드 정의
            {
                script.fakeLoadingDuration
                    = EditorGUILayout.FloatField(nameof(script.fakeLoadingDuration), script.fakeLoadingDuration);
            }

            // 변경 내용 업데이트
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
