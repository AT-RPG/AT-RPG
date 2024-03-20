using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                                        <br/>
    /// + 팝업 캔버스에 로그 메시지를 출력해주는 팝업에 사용되는 클래스        <br/>
    /// </summary>
    public partial class LogPopup : Popup, IPopupDestroy
    {
        [SerializeField] private FadeCanvasAnimation    fadeAnimation;
        [SerializeField] private PopupCanvasAnimation   popupAnimation;

        [Tooltip("출력할 로그 이미지")]
        [SerializeField] private Image                  logImage;

        [Tooltip("출력할 로그")]
        [SerializeField] private TMP_InputField         log;

        [Tooltip("팝업의 지속시간")]
        [SerializeField] private float                  duration;



        private void Start()
        {
            AnimateStartSequence();
            StartCoroutine(DestroyUntil());
        }

        /// <summary>
        /// 시작 애니메이션을 실행합니다.
        /// </summary>
        private void AnimateStartSequence()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
        }

        /// <summary>
        /// 지속시간 후, 팝업을 종료합니다.
        /// </summary>
        private IEnumerator DestroyUntil()
        {
            yield return new WaitForSeconds(duration);
            DestroyPopup();
        }

        public void DestroyPopup()
        {
            AnimateEscapeSequence();
        }

        /// <summary>
        /// 종료 애니메이션과 함께, 현재 팝업을 삭제합니다.
        /// </summary>
        private void AnimateEscapeSequence()
        {
            popupAnimation.EndPopup();
            fadeAnimation.EndFade(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    /// <summary>                                                   
    /// 프로퍼티 선언 분리 클래스
    /// </summary>
    public partial class LogPopup
    {
        // 출력할 로그
        public string Log
        {
            get => log.text;
            set => log.text = value;
        }

        // 팝업의 지속시간
        public float Duration
        {
            get => duration;
            set => duration = value;
        }
    }
}