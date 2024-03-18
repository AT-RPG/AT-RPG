using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                            <br/>
    /// + 팝업 UI가 가질 최상위 클래스                     <br/>
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract partial class Popup : MonoBehaviour
    {
        /// <summary>
        /// 등록된 팝업 캔버스
        /// </summary>
        protected PopupCanvas popupCanvas;

        protected PopupRenderMode popupRenderMode = PopupRenderMode.Default;
    }

    public abstract partial class Popup
    {
        public PopupCanvas PopupCanvas
        {
            get => popupCanvas;
            set => popupCanvas = value;
        }

        public PopupRenderMode PopupRenderMode
        {
            get => popupRenderMode;
            set => popupRenderMode = value;
        }
    }
}