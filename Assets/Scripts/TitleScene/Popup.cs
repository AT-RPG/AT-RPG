using System;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                            <br/>
    /// + 팝업 UI가 가질 최상위 클래스                     <br/>
    /// </summary>
    public partial class Popup : MonoBehaviour
    {
        // 팝업UI관리 캔버스
        protected PopupCanvas popupCanvas;

        /// <summary>
        /// 팝업 종료를 요청합니다.
        /// </summary>
        public virtual void InvokeDestroy() { }
    }

    public partial class Popup
    {
        // 팝업UI관리 캔버스
        public PopupCanvas PopupCanvas
        {
            get
            {
                return popupCanvas;
            }
            set
            {
                popupCanvas = value;
                popupCanvas.Popups.Push(this);
            }
        }
    }
}