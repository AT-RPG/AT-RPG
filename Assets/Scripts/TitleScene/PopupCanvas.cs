using System;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                                                    <br/>
    /// + UI 매니저에서 인스턴싱하는 팝업 UI 배치용 캔버스에 사용되는 클래스           <br/>
    /// </summary>
    public partial class PopupCanvas : MonoBehaviour
    {
        [Tooltip("팝업 UI들이 배치될 위치 레퍼런스용 인스턴스")]
        [SerializeField] private GameObject root;

        [Tooltip("ESC키를 누르면 동작하는 이벤트")]
        [SerializeField] private Action onEscapeKeyPressed;

        // 팝업 UI가 띄워진 순서를 저장
        private Stack<Popup> popups = new Stack<Popup>();



        private void Awake()
        {
            onEscapeKeyPressed += OnRemovePopupInstance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onEscapeKeyPressed?.Invoke();
            }
        }

        /// <summary>
        /// 가장 최근 팝업 인스턴스를 삭제합니다.
        /// </summary>
        private void OnRemovePopupInstance()
        {
            Popup popup = null;
            while ((popups.Count >= 1) && !(popup = popups.Pop())) { }
            popup?.InvokeDestroy();
        }
    }

    public partial class PopupCanvas
    {
        // 팝업 UI들이 배치될 위치 레퍼런스용 인스턴스
        public GameObject Root => root;

        // 팝업 UI가 띄워진 순서를 저장
        public Stack<Popup> Popups
        {
            get
            {
                return popups;
            }
            set
            {
                popups = value;
            }
        }

        // ESC키를 누르면 동작하는 이벤트
        public Action OnEscapeKeyPressed
        {
            get
            {
                return onEscapeKeyPressed;
            }
            set
            {
                onEscapeKeyPressed = value;
            }
        }
    }
}