using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityObject = UnityEngine.Object;
using System;
using System.Reflection;


namespace AT_RPG
{
    public class CSVParser : EditorWindow
    {
        /// <summary>
        /// 코드 저장 경로
        /// </summary>
        private static string codePath = Path.Combine(Application.dataPath, "Scripts/00_Common/ScriptableObjects");

        /// <summary>
        /// ScriptableObject 저장 경로
        /// </summary>
        private static string scriptableObjectPath = Path.Combine("Assets", "Resources");

        /// <summary>
        /// 드롭박스에 들어온 CSV파일들 입니다.
        /// </summary>
        private static List<string> csvPaths = new();

        /// <summary>
        /// 스크롤바의 위치를 기억합니다.
        /// </summary>
        private static Vector2 scrollPos = Vector2.zero;

        /// <summary>
        /// readonly 스크립트를 누르면 이 스크립트로 이동하도록 하는데 사용합니다.
        /// </summary>
        private static MonoScript currentScript;

        private void OnEnable()
        {
            currentScript = MonoScript.FromScriptableObject(this);
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

            // 읽기 전용 필드에 클릭 이벤트 연결
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", currentScript, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(20);

            // 툴 사용 설명서
            GUILayout.Label("툴 사용법", EditorStyles.boldLabel);
            GUILayout.Label("1. 코드가 저장될 경로를 설정합니다.");
            GUILayout.Label("2. ScriptableObject가 저장될 경로를 설정합니다.");
            GUILayout.Label("3. CSV파일들을 드롭박스에 드래그-앤-드롭합니다. Scriptable Object 스크립트가 생성됩니다.");
            GUILayout.Label("4. 다시 CSV파일들을 드롭박스에 드래그-앤-드롭합니다. 'Create'버튼으로 스크립터블 인스턴스를 생성합니다.");
            GUILayout.Space(20);

            // 주의 사항
            GUILayout.Label("주의 사항", EditorStyles.boldLabel);
            GUILayout.Label("1. CSV파일명이 ScriptableObject코드의 클래스명이 됩니다.");
            GUILayout.Label("2. CSV데이터의 가장 상단(헤더)이 ScriptableObject의 맴버 변수명이 됩니다.");
            GUILayout.Label("3. 지원하는 맴버 변수 자료형 : string, int, float");
            GUILayout.Label("4. CSV데이터에는 최소 하나는 요소(ex. Monster_001)가 있어야 합니다.");
            GUILayout.Space(20);

            // 예시 CSV 파일
            GUILayout.Label("예시 CSV 파일 탬플릿", EditorStyles.boldLabel);

            // 버튼을 하이퍼링크 스타일로 보여주기 위한 GUIStyle 생성
            GUIStyle hyperlinkStyle = new GUIStyle(EditorStyles.label);
            hyperlinkStyle.normal.textColor = Color.blue; // 하이퍼링크 색상
            hyperlinkStyle.hover.textColor = Color.cyan;
            hyperlinkStyle.fontStyle = FontStyle.Bold;
            hyperlinkStyle.wordWrap = true;

            // 버튼 대신 라벨로 하이퍼링크 텍스트처럼 만들기
            string URL = "https://docs.unity3d.com/Manual/index.html";
            if (GUILayout.Button(URL, hyperlinkStyle))
            {
                // 버튼 클릭 시 URL 열기
                Application.OpenURL(URL);
            }
            GUILayout.Space(20);

            // 코드 저장 경로.
            GUILayout.Label("코드 저장 경로", EditorStyles.boldLabel);
            codePath = GUILayout.TextField(codePath);
            GUILayout.Space(20);

            // ScriptableObject 저장 경로
            GUILayout.Label("ScriptableObject 저장 경로", EditorStyles.boldLabel);
            scriptableObjectPath = GUILayout.TextField(scriptableObjectPath);
            GUILayout.Space(20);

            // 드래그 앤 드롭 대상 영역 표시.
            GUILayout.Label("드래그 앤 드롭 영역", EditorStyles.boldLabel);
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 200.0f, GUILayout.ExpandWidth(true));
            if (csvPaths.Count > 0)
            {
                string list = "";

                foreach (var path in csvPaths)
                {
                    list += path + "\n";
                }

                GUI.Box(dropArea, list, EditorStyles.helpBox);
            }
            else
            {
                GUI.Box(dropArea, "여기에 파일을 드래그...", EditorStyles.helpBox);
            }

            // 드래그 앤 드롭 이벤트 핸들
            Event currentEvent = Event.current;
            switch (currentEvent.type)
            {
                // 마우스 포인터를 복사 가능으로 표시
                case EventType.DragUpdated:
                    if (dropArea.Contains(currentEvent.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    currentEvent.Use();
                    break;

                // 드래그한 오브젝트가 CSV파일인지?
                case EventType.DragPerform:
                    UnityObject[] dragObjs = DragAndDrop.objectReferences;
                    bool isParsable = true;
                    foreach (var obj in dragObjs)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        string ex = Path.GetExtension(path);

                        if (ex == ".csv")
                        {
                            if (!csvPaths.Contains(path))
                            {
                                csvPaths.Add(path);
                            }
                        }
                        else
                        {
                            isParsable = false;
                            csvPaths.Clear();
                            break;
                        }
                    }

                    if (isParsable)
                    {
                        DragAndDrop.AcceptDrag();
                        OnDrag();
                    }
                    else
                    {
                        Debug.LogError("드래그한 파일중에 .csv가 아닌 파일이 있습니다.");
                    }

                    currentEvent.Use();
                    break;
            }
            GUILayout.Space(20);

            // 드롭박스에 대한 액션 버튼을 생성.
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                OnCreateButtonClick();
            }
            if (GUILayout.Button("Clear"))
            {
                OnClearButtonClick();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 에디터창을 화면에 띄웁니다.
        /// </summary>
        [MenuItem("AT_RPG/CSVParser")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CSVParser), false, nameof(CSVParser));
        }

        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일들을 스크립터블 오브젝트 클래스로 변환합니다.
        /// </summary>
        private void OnDrag()
        {
            foreach (var path in csvPaths)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string[][] elements = ParseCSV(reader.ReadToEnd());
                    string fileName = String.MakeValidIdentifier(Path.GetFileNameWithoutExtension(path));
                    GenerateScriptableObjectCode(fileName, elements);
                }
            }
        }

        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일들을 변환합니다.
        /// </summary>
        private void OnCreateButtonClick()
        {
            foreach (var path in csvPaths)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string[][] elements = ParseCSV(reader.ReadToEnd());
                    string fileName = String.MakeValidIdentifier(Path.GetFileNameWithoutExtension(path));
                    GenerateScriptableObjectInstance(fileName, elements);
                }
            }
        }

        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일들을 지웁니다.
        /// </summary>
        private void OnClearButtonClick()
        {
            csvPaths.Clear();
        }

        /// <summary>
        /// CSV파일을 ScriptableObject class 코드로 생성합니다.
        /// </summary>
        private void GenerateScriptableObjectCode(string fileName, string[][] elements)
        {
            // 지원하는 자료형 추론
            Dictionary<string, string> members = new();
            for (uint i = 0; i < elements[1].Length; i++)
            {
                string candidate = String.MakeValidIdentifier(elements[0][i].Trim());
                string typedefData = elements[1][i].Trim();

                if (int.TryParse(typedefData, out _))
                {
                    members[candidate] = "int";
                }
                else 
                if (float.TryParse(typedefData, out _))
                {
                    members[candidate] = "float";
                }
                else 
                {
                    members[candidate] = "string";
                }
            }

            // ScriptableObject 클래스 코드 생성
            StringBuilder sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("using UnityEngine;");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"[CreateAssetMenu(menuName = \"ScriptableObject/{fileName}\", order = int.MaxValue)]");
            sourceBuilder.AppendLine($"public class {fileName} : ScriptableObject");
            sourceBuilder.AppendLine("{");
            foreach (var member in members)
            {
                sourceBuilder.AppendLine($"    public {member.Value} {member.Key};");
            }
            sourceBuilder.AppendLine("}");

            // 경로에 코드 파일을 생성
            if (!Directory.Exists(codePath))
            {
                Directory.CreateDirectory(codePath);
            }
            string scriptPath = Path.Combine(codePath, $"{fileName}.cs");
            File.WriteAllText(scriptPath, sourceBuilder.ToString());

            // 변경사항 저장 및 Project 탭의 포커스를 이동합니다.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        /// <summary>
        /// CSV파일을 ScriptableObject 인스턴스로 생성합니다.
        /// </summary>
        private void GenerateScriptableObjectInstance(string fileName, string[][] elements)
        {
            // 경로에 스크립터블 인스턴스를 생성
            if (!Directory.Exists(scriptableObjectPath))
            {
                Directory.CreateDirectory(scriptableObjectPath);
            }
            for (uint i = 1; i < elements.Length; i++)
            {
                // 스크립터블 오브젝트 인스턴스를 생성합니다.
                var typename = $"{fileName}, Assembly-CSharp";
                var instanceType = Type.GetType(typename);
                var instance = CreateInstance(instanceType);

                // 리플렉션으로 스크립터블 오브젝트의 값을 CSV파일의 데이터와 동일하게 초기화합니다.
                for (uint j = 0; j < elements[0].Length; j++)
                {
                    FieldInfo field = instanceType.GetField(elements[0][j]);
                    object parseValue = ParseValue(field.FieldType, elements[i][j]);
                    field.SetValue(instance, parseValue);
                }

                // 스크립터블 오브젝트를 에셋으로 만듭니다.
                string path = Path.Combine(scriptableObjectPath, String.MakeValidIdentifier(elements[i][0]) + ".asset");
                AssetDatabase.CreateAsset(instance, path);
                Selection.activeObject = instance;
            }

            // 변경사항 저장 및 Project 탭의 포커스를 이동합니다.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        /// <summary>
        /// <paramref name="content"/>를 쉼표에 따라서 2차원 배열 데이터로 변환합니다.
        /// </summary>
        private string[][] ParseCSV(string content)
        {
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[][] elements = new string[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(',');
                elements[i] = columns;
            }

            return elements;
        }

        /// <summary>
        /// 문자열을 주어진 타입으로 변환하는 헬퍼 함수입니다.
        /// </summary>
        private object ParseValue(Type targetType, string value)
        {
            if (targetType == typeof(int)) return int.Parse(value);
            if (targetType == typeof(float)) return float.Parse(value);
            if (targetType == typeof(string)) return value;
            return null; // 원하는 타입의 기본값 반환
        }
    }
}
