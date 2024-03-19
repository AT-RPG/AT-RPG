using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{

    [CreateAssetMenu(fileName = "MultiplayManagerSettings", menuName = "ScriptableObject/MultiplayManager Setting")]
    public partial class MultiplayManagerSetting : ScriptableObject
    {
        [ReadOnly] public readonly string AuthenticationDataPath = Application.streamingAssetsPath;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MultiplayManagerSetting))]
    public class MultiplayManagerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MultiplayManagerSetting script = (MultiplayManagerSetting)target;

            // 읽기 전용 필드 정의
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.LabelField(nameof(script.AuthenticationDataPath), script.AuthenticationDataPath);
            }
            EditorGUI.EndDisabledGroup();

            // 수정 가능 필드 정의
            {

            }

            // 변경 내용 업데이트
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}