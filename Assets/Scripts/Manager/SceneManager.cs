using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace AT_RPG.Manager
{
    public partial class SceneManager : Singleton<SceneManager>
    {
        private Coroutine       loadCoroutine = null;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {

        }

        /// <summary>
        /// 씬을 전환. SceneManager.IsLoading으로 씬이 전부 로딩이 되었는지 확인해주세요.
        /// </summary>
        public void LoadCor(string sceneName)
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
                    asyncSceneLoading.allowSceneActivation = true;
                }

                yield return null;
            }

            // 리소스를 비동기로 로드
            Task asyncResourcesUnloading = ResourceManager.Instance.UnLoadAllAsync(prevSceneName);

            loadCoroutine = null;
            yield break;
        }
    }

    public partial class SceneManager
    {
        public string CurrentSceneName => UnitySceneManager.GetActiveScene().name;
        public bool IsLoading => loadCoroutine != null ? true : false;
    }
}