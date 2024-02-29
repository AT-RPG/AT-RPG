using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace AT_RPG.Manager
{
    public partial class SceneManager : Singleton<SceneManager>
    {
        [SerializeField] private Coroutine loadCoroutine = null;

        // TODO - 하드코딩 제거
        [SerializeField] private float fakeLoadingDuration = 1.5f;

        private event Action sceneChangedEvent;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 씬을 전환. SceneManager.IsLoading으로 씬이 전부 로딩이 되었는지 확인해주세요.
        /// </summary>
        public void Load(string sceneName)
        {
            if (loadCoroutine == null)
            {
                loadCoroutine =
                    StartCoroutine(InternalLoadCor(sceneName));
            }
        }

        private IEnumerator InternalLoadCor(string sceneName)
        {
            string prevSceneName = CurrentSceneName;
            string nextSceneName = sceneName;
            float fakeLoadingDuration = this.fakeLoadingDuration;

            // 씬을 비동기 로드
            // 바로 씬을 전환X
            AsyncOperation asyncSceneLoading = UnitySceneManager.LoadSceneAsync(nextSceneName);
            asyncSceneLoading.allowSceneActivation = false;

            // 리소스를 비동기로 로드
            Task asyncResourcesLoading = ResourceManager.Instance.LoadAllAsync(nextSceneName);

            while (!asyncSceneLoading.isDone)
            {
                // TODO - 로드전에 해야할 것들은 여기에
                if (asyncSceneLoading.progress >= 0.9f && 
                    asyncResourcesLoading.IsCompleted)
                {
                    // 페이크 로딩
                    fakeLoadingDuration -= Time.deltaTime;
                    if (fakeLoadingDuration <= 0f)
                    {
                        asyncSceneLoading.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            // 리소스를 비동기로 언로드
            Task asyncResourcesUnloading = ResourceManager.Instance.UnLoadAllAsync(prevSceneName);

            // 씬 변경 이벤트 실행
            sceneChangedEvent?.Invoke();

            loadCoroutine = null;
            yield break;
        }

    }

    public partial class SceneManager
    {
        // 현재 화면에 보이는 씬 이름
        public string CurrentSceneName => UnitySceneManager.GetActiveScene().name;

        // 씬이 현재 로딩중인지?
        public bool IsLoading => loadCoroutine != null ? true : false;

        // 씬이 바뀌고 난 후 트리거되는 이벤트
        public Action SceneChangedEvent
        {
            get
            {
                return sceneChangedEvent;
            }
            set
            {
                sceneChangedEvent = value;
            }
        }
    }
}