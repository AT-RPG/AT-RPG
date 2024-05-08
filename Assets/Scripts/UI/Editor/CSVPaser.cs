using UnityEngine;
using UnityEditor;
using System.IO;

namespace AT_RPG
{
    public class CSVPaser : EditorWindow
    {
        /// <summary>
        /// 드롭박스에 마우스 포인터가 들어오면 피드백할 텍스쳐
        /// </summary>
        [SerializeField] private Texture2D hoverTexture;



        /// <summary>
        /// 드롭박스에 마우스 포인터가 위치해 있는지?
        /// </summary>
        private static bool isMouseDragging = false;

        /// <summary>
        /// Scriptable Object로 변환된 스크립트가 저장되는 폴더 경로입니다.
        /// </summary>
        private static string path = Path.Combine(Application.dataPath, "Scripts/ScriptableObjects");

        /// <summary>
        /// 드롭박스에 들어온 CSV파일들 입니다.
        /// </summary>
        private static Object[] droppedObjects;

        /// <summary>
        /// 드롭박스의 기본 컬러
        /// </summary>
        private Color defaultColor = Color.white;

        /// <summary>
        /// 드롭박스에 마우스 포인터가 들어오면 피드백할 색상
        /// </summary>
        private Color hoverColor = new Color(0.8f, 0.8f, 0.8f);



        private void OnGUI()
        {
            // Scriptable Object로 변환된 스크립트가 저장되는 폴더 경로.
            GUILayout.Label("변환된 Scriptable Object 스크립트가 저장되는 폴더 경로", EditorStyles.boldLabel);
            GUILayout.TextField(path);
            GUILayout.Space(10);


            // 드래그 앤 드롭 대상 영역 표시.
            GUILayout.Label("드래그 앤 드롭 영역", EditorStyles.boldLabel);
            Event currentEvent = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 200.0f, GUILayout.ExpandWidth(true));
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    isMouseDragging = true;

                    // 드래그 가능한 오브젝트로 표시
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    // 드롭박스에 드래그 놓기?
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        droppedObjects = DragAndDrop.objectReferences;
                    }

                    currentEvent.Use();
                    break;

                case EventType.DragExited:
                    isMouseDragging = false;
                    break;

            }
            if (isMouseDragging)
            {
                GUI.Box(dropArea, hoverTexture, EditorStyles.helpBox);
            }
            else
            {
                GUI.Box(dropArea, "여기에 파일을 드래그...", EditorStyles.helpBox);
            }

            // 드롭박스에 대한 액션 버튼을 생성.
            bool isCreate = false;
            bool isClear = false;
            GUILayout.BeginHorizontal();
            isClear = GUILayout.Button("Clear");
            isCreate = GUILayout.Button("Create");
            GUILayout.EndHorizontal();

            if (isCreate)
            {
                OnCreateButtonClick();
            }

            if (isClear)
            {
                OnClearButtonClick();
            }
        }

        /// <summary>
        /// 에디터창을 화면에 띄웁니다.
        /// </summary>
        [MenuItem("AT_RPG/CSVPaser")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CSVPaser), false, nameof(CSVPaser));
        }

        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일을 Scriptable Objects로 변환합니다.
        /// </summary>
        private void OnCreateButtonClick()
        {

        }
        
        /// <summary>
        /// 드롭박스에 들어온 모든 CSV파일들을 지웁니다.
        /// </summary>
        private void OnClearButtonClick()
        {

        }

        /// <summary>
        /// 드롭박스 드래그 이벤트를 트리거 합니다.
        /// </summary>
        private void OnDragging()
        {

        }
    }
}
