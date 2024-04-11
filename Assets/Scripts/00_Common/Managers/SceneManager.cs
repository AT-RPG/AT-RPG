using System;
using System.Collections;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


namespace AT_RPG.Manager
{
    /// <summary>
    /// 씬에서 씬으로 이동하는데 사용되는 클래스입니다.
    /// </summary>
    public partial class SceneManager : Singleton<SceneManager>
    {
        // 매니저 기본 설정
        private static SceneManagerSettings setting;

        // 씬 로드 후 호출되는 이벤트
        private static event Action afterSceneLoadAction;

        // 씬 로딩중
        private static bool isLoading = false;

        // 씬 로딩 성공 시 콜백
        public delegate void CompletedCallback();

        // 시작 조건 콜백 true->로딩 시작, false->로딩 대기
        public delegate bool StartConditionCallback();


        protected override void Awake()
        {
            base.Awake();
            setting = Resources.Load<SceneManagerSettings>("SceneManagerSettings");
        }



        /// <summary>
        /// 현재 씬에서 다음 씬으로 비동기적 전환합니다.
        /// </summary>
        /// <param name="sceneName">다음 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        public static void LoadSceneCoroutine(
            string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            if (sceneName == CurrentSceneName)
            {
                Debug.LogError("동일한 씬으로 로딩할 수 없습니다.");
                return;
            }

            if (isLoading)
            {
                Debug.LogError("씬이 로딩중입니다.");
                return;
            }

            Instance.StartCoroutine(InternalLoadSceneCoroutine(sceneName, started, completed));
        }

        /// <summary>
        /// 현재 씬에서 다음 씬으로 동기적 전환합니다.
        /// </summary>
        /// <param name="sceneName">다음 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        public static void LoadScene(string sceneName, CompletedCallback completed = null)
        {
            if (sceneName == CurrentSceneName)
            {
                Debug.LogError("동일한 씬으로 로딩할 수 없습니다.");
                return;
            }

            if (isLoading)
            {
                Debug.LogError("씬이 로딩중입니다.");
                return;
            }

            InternalLoadScene(sceneName, completed);
        }



        /// <summary>
        /// 다음 씬을 비동기 로딩과 동시에 페이크 로딩합니다.
        /// </summary>
        /// <param name="sceneName">다음 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        private static IEnumerator InternalLoadSceneCoroutine(string sceneName, StartConditionCallback started = null, CompletedCallback completed = null)
        {
            isLoading = true;

            // 시작 조건
            if (started != null)
            {
                while (!started.Invoke())
                {
                    yield return null;
                }
            }

            // 씬 로딩.
            AsyncOperation loadSceneRequest = UnitySceneManager.LoadSceneAsync(sceneName);
            loadSceneRequest.allowSceneActivation = false;
            while (!loadSceneRequest.isDone)
            {
                // loadSceneRequest.allowSceneActivation = false; 로 인해 0.9f캡에 걸림.
                if (loadSceneRequest.progress <= 0.9f)
                {
                    yield return null;
                }

                yield return LoadFakeDuration();

                // 로딩된 씬으로 변경 가능 Flag 설정.
                loadSceneRequest.allowSceneActivation = true;
                yield return null;
            }

            isLoading = false;
            yield return null;
            completed?.Invoke();
            afterSceneLoadAction?.Invoke();
        }

        /// <summary>
        /// 페이크 로딩
        /// </summary>
        private static IEnumerator LoadFakeDuration()
        {
            float fakeLoadingElapsedTime = 0f;
            float fakeLoadingDuration = setting.FakeLoadingDuration;
            while (fakeLoadingElapsedTime <= fakeLoadingDuration)
            {
                fakeLoadingElapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 다음 씬을 동기적으로 로딩합니다.
        /// </summary>
        /// <param name="sceneName">다음 씬 이름</param>
        /// <param name="completed">로딩이 끝나고 호출되는 델리게이트</param>
        private static void InternalLoadScene(string sceneName, CompletedCallback completed)
        {
            isLoading = true;
            UnitySceneManager.LoadScene(sceneName);
            isLoading = false;

            completed?.Invoke();
            afterSceneLoadAction?.Invoke();
        }
    }

    public partial class SceneManager
    {
        // 매니저 기본 설정
        public static SceneManagerSettings Setting => setting;

        // 씬 로드 후 호출되는 이벤트
        public static Action AfterSceneLoadAction
        {
            get
            {
                return afterSceneLoadAction;
            }
            set
            {
                afterSceneLoadAction = value;
            }
        }

        // 현재 씬 이름
        public static string CurrentSceneName => UnitySceneManager.GetActiveScene().name;

        // 씬 로딩중
        public static bool IsLoading => isLoading;
    }
}