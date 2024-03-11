using UnityEngine;

namespace AT_RPG
{
    public class QuitGamePopup : Popup
    {
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private BlurCanvasAnimation blurAnimation;

        private void Awake()
        {
            if (!popupCanvas)
            {
                return;
            }

            popupCanvas.Popups.Push(this);
        }

        private void Start()
        {
            fadeAnimation.StartFade();
            blurAnimation.StartFade();
        }

        private void Update()
        {
            OnEscapeKeyPressed();
        }

        private void OnEscapeKeyPressed()
        {
            // 종료 버튼이 눌리면
            if (!isEscapePressed &&
                Input.GetKeyDown(KeyCode.Escape))
            {
                // 팝업 캔버스에 등록된 경우, Stack(순차적으로 팝업 종료)이용
                if (popupCanvas)
                {
                    if (popupCanvas.Popups.Pop().GetInstanceID() == GetInstanceID())
                    {
                        EndAnimate();
                    }
                    else
                    {
                        return;
                    }
                }
                // 팝업 단일로 이용하는 경우
                else
                {
                    EndAnimate();
                }
            }
        }

        /// <summary>
        /// 종료 애니메이션과 함께, 이 팝업을 삭제
        /// </summary>
        private void EndAnimate()
        {
            isEscapePressed = true;

            fadeAnimation.EndFade(() =>
            {
                Destroy(gameObject);
            });
            blurAnimation.EndFade();
        }

        /// <summary>
        /// '종료' 버튼이 눌리면 호출
        /// </summary>
        public void OnQuitButtonPressed()
        {
            Application.Quit();
        }

        /// <summary>
        /// '계속' 버튼이 눌리면 호출
        /// </summary>
        public void OnContinueButtonPressed()
        {
            EndAnimate();
        }
    }
}
