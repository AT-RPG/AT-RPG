using AT_RPG.Manager;
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

        // 팝업 UI가 띄워진 순서를 저장
        private Stack<Popup> popups = new Stack<Popup>();



        private void Awake()
        {
            InputManager.AddKeyAction("Setting/Undo", RemovePopupInstance);
        }

        private void OnDestroy()
        {
            InputManager.RemoveKeyAction("Setting/Undo", RemovePopupInstance);
        }

        /// <summary>
        /// 가장 최근 팝업 인스턴스를 삭제합니다.
        /// </summary>
        public void RemovePopupInstance(InputValue inputValue)
        {
            Popup popup = null;

            if (popups.Count <= 0) { return; }

            while (!(popup = popups.Pop())) { }


            // 팝업 인스턴스 삭제
            IPopupDestroy iPopupDestroy = popup as IPopupDestroy;
            if (iPopupDestroy == null)
            {
                Destroy(popup.gameObject);
            }
            else
            {
                iPopupDestroy.DestroyPopup();
            }

            ProcessPopupRenderModeAtPeek();
        }


        
        public void RemoveAllPopupInstance()
        {
            while (popups.Count >= 1)
            {
                Popup popup = popups.Pop();

                IPopupDestroy iPopupDestroy = popup as IPopupDestroy;
                if (iPopupDestroy == null && popup != null)
                {
                    Destroy(popup.gameObject);
                }
                else if (popup != null)
                {
                    iPopupDestroy.DestroyPopup();
                }
            }
        }



        /// <summary>
        /// 팝업을 등록합니다.
        /// </summary>
        public void Push(Popup popup)
        {
            ProcessPopupRenderModeAtPeek();

            popups.Push(popup);
        }

        /// <summary>
        /// 스택의 최상단 팝업에 PopupRenderMode를 적용합니다.
        /// </summary>
        private void ProcessPopupRenderModeAtPeek()
        {
            if (popups.Count <= 0)
            {
                return;
            }

            var peek = popups.Peek();
            switch (peek.PopupRenderMode)
            {
                case PopupRenderMode.Hide:
                    if (peek.gameObject.activeSelf)
                    {
                        peek.gameObject.SetActive(false);
                    }
                    else
                    {
                        peek.gameObject.SetActive(true);
                    }
                    break;

                default:
                    break;
            }
        }

        

        public int GetPopupCount()
        {
            return popups.Count;
        }
    }

    public partial class PopupCanvas
    {
        // 팝업 UI들이 배치될 위치 레퍼런스용 인스턴스
        public GameObject Root => root;
    }
}