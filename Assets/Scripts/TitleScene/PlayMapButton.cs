using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public class PlayMapButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private SceneReference mainScene;

        // 더블 클릭 인터벌 변수
        private float lastClickTime = 0f;
        private float catchTime = 0.25f;


        private void Awake()
        {
            // 마우스 이벤트 트리거 초기화
            if (gameObject.GetComponent<EventTrigger>() == null)
            {
                gameObject.AddComponent<EventTrigger>();
            }
        }

        private void Start()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
        }

        /// <summary>
        /// 더블 클릭 시, 씬으로 이동
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 더블 클릭시 씬 로딩
                if (Time.time - lastClickTime < catchTime)
                {
                    string currSceneName = SceneManager.CurrentSceneName;
                    string nextSceneName = mainScene;
                    SceneManager.LoadScene(SceneManager.Setting.LoadingScene, () =>
                    {
                        ResourceManager.LoadAllResourcesCoroutine(nextSceneName);
                        ResourceManager.UnloadAllResourcesCoroutine(currSceneName);
                        SceneManager.LoadSceneCoroutine(nextSceneName, () =>
                        {
                            return !ResourceManager.IsLoading;
                        });
                    });
                }

                lastClickTime = Time.time;
            }
        }
    }

}