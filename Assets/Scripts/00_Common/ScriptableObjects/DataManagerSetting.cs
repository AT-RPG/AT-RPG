using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "DataManagerSettings", menuName = "ScriptableObject/DataManager Setting")]
    public class DataManagerSetting : ScriptableObject
    {
        // 세이브 파일 기본 경로
        [ReadOnly] private string defaultSaveFolderPath;
        public string DefaultSaveFolderPath => defaultSaveFolderPath;

        // 세이브 파일 페이크 로딩 지속 시간
        // NOTE : 비동기 로딩에만 적용
        public float FakeLoadingDuration = 0.75f;

        public void OnEnable()
        {
            defaultSaveFolderPath = Application.persistentDataPath + "/Saves";
        }
    }


#if UNITY_EDITOR
    /// <summary>
    /// 파일 저장 경로를 ReadOnly로 인스펙터에서 수정을 막는 클래스
    /// </summary>
    [CustomEditor(typeof(DataManagerSetting))]
    public class DataManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DataManagerSetting script = (DataManagerSetting)target;

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.LabelField(nameof(script.DefaultSaveFolderPath), script.DefaultSaveFolderPath);
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
