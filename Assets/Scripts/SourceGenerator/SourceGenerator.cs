#if UNITY_EDITOR

using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
    [InitializeOnLoad]
    public class SourceGenerator : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            string assemblyName = Path.GetFileNameWithoutExtension(path);
            if (assemblyName != Assembly.GetExecutingAssembly().GetName().Name) { return content; }

            return content;
        }
    }

    [CustomEditor(typeof(SourceGenerator))]
    public class SourceGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 수정 가능 필드 정의
            EditorGUILayout.PropertyField(serializedObject.FindProperty("assembly"), new GUIContent("assembly"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
