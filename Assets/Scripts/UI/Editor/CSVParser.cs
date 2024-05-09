using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;
using System.Text;
using System.Linq;

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


        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

            // 툴 사용 설명서
            GUILayout.Label("툴 사용법", EditorStyles.boldLabel);
            GUILayout.Label("1. CSV파일들을 드롭박스에 드래그-앤-드롭합니다.");
            GUILayout.Label("2. 코드가 저장될 경로를 설정합니다.");
            GUILayout.Label("3. ScriptableObject가 저장될 경로를 설정합니다.");
            GUILayout.Label("4. 'Create'버튼으로 코드 생성을 시작합니다.");
            GUILayout.Space(20);

            // 주의 사항
            GUILayout.Label("주의 사항", EditorStyles.boldLabel);
            GUILayout.Label("1. CSV파일명이 ScriptableObject코드의 클래스명이 됩니다.");
            GUILayout.Label("2. CSV데이터의 가장 상단(헤더)이 ScriptableObject의 맴버 변수명이 됩니다.");
            GUILayout.Label("3. 지원하는 맴버 변수 자료형 : bool, int, float");
            GUILayout.Label("4. CSV데이터에는 최소 하나는 요소(ex. Monster_001)가 있어야 합니다.");
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
                GUI.Box(dropArea, GetCSVList(), EditorStyles.helpBox);
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
                            csvPaths.Add(path);
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
                        Debug.Log("csv파일 로드 성공.");
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
        /// 드롭박스에 들어온 모든 CSV파일을 불러옵니다.
        /// </summary>
        private void OnCreateButtonClick()
        {
            foreach (var path in csvPaths)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    GenerateScriptableObjectCode(Path.GetFileNameWithoutExtension(path), reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// CSV파일을 ScriptableObject class로 코드 생성/ ScriptableObject 인스턴스를 생성합니다.
        /// </summary>
        private void GenerateScriptableObjectCode(string fileName, string content)
        {
            // .csv 파일의 데이터를 분석해 컬럼 이름과 데이터 타입 파악
            string[] lines = content.Split('\n');
            if (lines.Length < 2)
            {
                return;
            }

            // ScriptableObject의 이름
            string[] names = lines.Skip(1)
                                  .Select(line => line.Split(',').FirstOrDefault())
                                  .Where(item => !string.IsNullOrEmpty(item))
                                  .ToArray();

            // 첫 번째 줄은 헤더로 간주
            string[] candidates = lines[0].Split(',');

            // 데이터 타입 추론을 위한 첫 번째 데이터 줄 파싱
            string[] typedefDatas = lines[1].Split(',');

            // 지원하는 자료형 추론
            Dictionary<string, string> members = new();
            for (uint i = 0; i < candidates.Length; i++)
            {
                string candidate = String.MakeValidIdentifier(candidates[i].Trim());
                string typedefData = String.MakeValidIdentifier(typedefDatas[i].Trim());

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
            string className = String.MakeValidIdentifier(fileName);
            StringBuilder sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("using UnityEngine;");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"[CreateAssetMenu(menuName = \"ScriptableObject/{className}\", order = int.MaxValue)]");
            sourceBuilder.AppendLine($"public class {className} : ScriptableObject");
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
            string scriptPath = Path.Combine(codePath, $"{className}.cs");
            File.WriteAllText(scriptPath, sourceBuilder.ToString());
            AssetDatabase.Refresh();

            // 경로에 스크립터블 인스턴스를 생성
            if (!Directory.Exists(scriptableObjectPath))
            {
                Directory.CreateDirectory(scriptableObjectPath);
            }
            foreach (var name in names)
            {
                var instance = ScriptableObject.CreateInstance(className);
                string path = Path.Combine(scriptableObjectPath, String.MakeValidIdentifier(name) + ".asset");
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = instance;
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일들을 지웁니다.
        /// </summary>
        private void OnClearButtonClick()
        {
            csvPaths.Clear();
        }

        /// <summary>
        /// <see cref="csvPaths"/>의 모든 경로를 string으로 변환합니다.
        /// </summary>
        private string GetCSVList()
        {
            string list = "";

            foreach (var path in csvPaths)
            {
                list += path + "\n";
            }

            return list;
        }
    }
}
