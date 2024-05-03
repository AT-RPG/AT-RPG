using AT_RPG.Manager;
using System;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    public class LoadingPopup : Popup
    {
        [Header("로딩 상태 메세지")]
        [SerializeField] public TMP_Text StateMsg;

        [Header("UI 애니메이션")]
        [SerializeField] private FadeCanvasAnimation fadeAnimation;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 시작 애니메이션을 실행합니다.
        /// </summary>
        public void AnimateStartSequence(SceneManager.CompletedCallback completed = null)
        {
            fadeAnimation.StartFade(() =>
            {
                SceneManager.LoadSceneCoroutine(SceneManager.Setting.LoadingScene, null, completed);
            });
        }

        /// <summary>
        /// 종료 애니메이션과 함께, 현재 팝업을 삭제합니다.
        /// </summary>
        public void AnimateEscapeSequence(SceneManager.CompletedCallback completed = null)
        {
            fadeAnimation.EndFade(() =>
            {
                Destroy(gameObject);
                completed?.Invoke();
            });
        }
    }
}