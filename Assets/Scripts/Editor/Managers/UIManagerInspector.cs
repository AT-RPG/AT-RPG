using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;

[CustomEditor(typeof(UIManager))]
public class UIManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // UIManager 스크립트를 대상으로 합니다.
        UIManager uiManager = (UIManager)target;
    }
}
