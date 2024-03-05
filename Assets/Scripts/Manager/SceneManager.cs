using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityObject = UnityEngine.Object;


namespace AT_RPG.Manager
{
    public delegate IEnumerator SceneChangedCoroutine();

    [Flags]
    public enum LoadMode
    {
        Instant = 1,
        LoadingResources = 2,
        LoadingResourcesAndSaveDatas = 3
    }

    public partial class SceneManager : Singleton<SceneManager>
    {
        // 씬 변경 전, 호출되는 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event Action beforeSceneChangeAction;

        // 씬 변경 전, 호출되는 단발성 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 사용되면 Clear됨
        private event Action beforeSceneChangeDisposableAction;

        // 씬 변경 후, 호출되는 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event Action afterSceneChangeAction;

        // 씬 변경 후, 호출되는 단발성 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 사용되면 Clear됨
        private event Action afterSceneChangeDisposableAction;

        // 씬 변경 전, 호출되는 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event SceneChangedCoroutine beforeSceneChangeCoroutine = null;

        // 씬 변경 전, 호출되는 단발성 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 사용되면 Clear됨
        private event SceneChangedCoroutine beforeSceneChangeDisposableCoroutine = null;

        // 씬 변경 후, 호출되는 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event SceneChangedCoroutine afterSceneChangeCoroutine = null;

        // 씬 변경 후, 호출되는 단발성 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 사용되면 Clear됨
        private event SceneChangedCoroutine afterSceneChangeDisposableCoroutine = null;


        // 씬 페이크 로딩 지속 시간
        // NOTE : 비동기 로딩에만 적용
        [SerializeField] private float fakeLoadingDuration = 0.75f;

        // 씬 로딩중
        [SerializeField] private bool isLoading = false;

        // 로딩 씬
        [SerializeField] private UnityObject loadingScene = null;

        protected override void Awake()
        {
            base.Awake();
        }


        /// <summary>
        /// 현재 씬에서 다음 씬으로 전환
        /// </summary>
        /// <param name="sceneName">다음 씬 이름</param>
        /// <param name="isLoadingSceneIncluded">다음 씬으로 로딩 전에 로딩 씬을 거칠건지</param>
        public void LoadSceneCor(string sceneName, LoadMode loadMode)
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

            isLoading = true;
            switch (loadMode)
            {
                case LoadMode.Instant:
                    StartCoroutine(InternalLoadScene(sceneName));
                    break;

                case LoadMode.LoadingResources:
                    StartCoroutine(InternalLoadSceneIncludedResources(sceneName));
                    break;

                case LoadMode.LoadingResourcesAndSaveDatas:
                    StartCoroutine(InternalLoadSceneIncludedResourcesAndSaveDatas(sceneName));
                    break;

                default:
                    break;
            }
        }
        private IEnumerator InternalLoadScene(string sceneName)
        {
            string prevSceneName = CurrentSceneName.ToString();
            string nextSceneName = sceneName;

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeDisposableCoroutine));

            // 다음 씬 리소스 로딩
            ResourceManager.Instance.LoadAllAssetsAtSceneCor(nextSceneName);
            while (ResourceManager.Instance.IsLoading)
            {
                yield return null;
            }

            // 다음 씬 로드
            // 바로 씬 변경X
            AsyncOperation loadSceneRequest = UnitySceneManager.LoadSceneAsync(nextSceneName);
            loadSceneRequest.allowSceneActivation = false;
            while (!loadSceneRequest.isDone)
            {
                // 씬 로딩
                if (loadSceneRequest.progress <= 0.9f)
                {
                    yield return null;
                }

                // 페이크 로딩
                float fakeLoadingElapsedTime = 0f;
                float fakeLoadingDuration = this.fakeLoadingDuration;
                while (fakeLoadingElapsedTime <= fakeLoadingDuration)
                {
                    fakeLoadingElapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 씬 변경O
                loadSceneRequest.allowSceneActivation = true;
                break;
            }

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeDisposableCoroutine));

            // Disposable 이벤트 초기화
            beforeSceneChangeDisposableAction = null;
            beforeSceneChangeDisposableCoroutine = null;
            afterSceneChangeCoroutine = null;
            afterSceneChangeDisposableCoroutine = null;

            // 이전 씬 리소스 언로딩
            // TODO : 이렇게 순서를 말고 ResourceManager에서 Queue로 처리하도록 변경
            while (ResourceManager.Instance.IsUnloading)
            {
                yield return null;
            }
            ResourceManager.Instance.UnloadAllAssetsAtSceneCor(prevSceneName);

            isLoading = false;
        }

        private IEnumerator InternalLoadSceneIncludedResources(string sceneName)
        {
            string prevSceneName = CurrentSceneName.ToString();
            string nextSceneName = sceneName;

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeDisposableCoroutine));

            // 로드 씬으로 이동
            yield return UnitySceneManager.LoadSceneAsync(loadingScene.name);

            // 다음 씬 리소스 로딩
            ResourceManager.Instance.LoadAllAssetsAtSceneCor(nextSceneName);
            while (ResourceManager.Instance.IsLoading)
            {
                yield return null;
            }

            // 다음 씬 로드
            // 바로 씬 변경X
            AsyncOperation loadSceneRequest = UnitySceneManager.LoadSceneAsync(nextSceneName);
            loadSceneRequest.allowSceneActivation = false;
            while (!loadSceneRequest.isDone)
            {
                // 씬 로딩
                if (loadSceneRequest.progress <= 0.9f)
                {
                    yield return null;
                }

                // 페이크 로딩
                float fakeLoadingElapsedTime = 0f;
                float fakeLoadingDuration = this.fakeLoadingDuration;
                while (fakeLoadingElapsedTime <= fakeLoadingDuration)
                {
                    fakeLoadingElapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 씬 변경O
                loadSceneRequest.allowSceneActivation = true;
                break;
            }

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeDisposableCoroutine));

            // Disposable 이벤트 초기화
            beforeSceneChangeDisposableAction = null;
            beforeSceneChangeDisposableCoroutine = null;
            afterSceneChangeCoroutine = null;
            afterSceneChangeDisposableCoroutine = null;

            // 이전 씬 리소스 언로딩
            // TODO : 이렇게 순서를 말고 ResourceManager에서 Queue로 처리하도록 변경
            while (ResourceManager.Instance.IsUnloading)
            {
                yield return null;
            }
            ResourceManager.Instance.UnloadAllAssetsAtSceneCor(prevSceneName);

            isLoading = false;
        }

        private IEnumerator InternalLoadSceneIncludedResourcesAndSaveDatas(string sceneName)
        {
            string prevSceneName = CurrentSceneName.ToString();
            string nextSceneName = sceneName;

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangeDisposableCoroutine));

            // 로드 씬으로 이동
            yield return UnitySceneManager.LoadSceneAsync(loadingScene.name);

            // 다음 씬 리소스 로딩
            ResourceManager.Instance.LoadAllAssetsAtSceneCor(nextSceneName);
            while (ResourceManager.Instance.IsLoading)
            {
                yield return null;
            }

            // 다음 씬 로드
            // 바로 씬 변경X
            AsyncOperation loadSceneRequest = UnitySceneManager.LoadSceneAsync(nextSceneName);
            loadSceneRequest.allowSceneActivation = false;
            while (!loadSceneRequest.isDone)
            {
                // 씬 로딩
                if (loadSceneRequest.progress <= 0.9f)
                {
                    yield return null;
                }

                // 페이크 로딩
                float fakeLoadingElapsedTime = 0f;
                float fakeLoadingDuration = this.fakeLoadingDuration;
                while (fakeLoadingElapsedTime <= fakeLoadingDuration)
                {
                    fakeLoadingElapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 씬 변경O
                loadSceneRequest.allowSceneActivation = true;
                break;
            }

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeAction));
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangeDisposableAction));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeCoroutine));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangeDisposableCoroutine));

            // Disposable 이벤트 초기화
            beforeSceneChangeDisposableAction = null;
            beforeSceneChangeDisposableCoroutine = null;
            afterSceneChangeCoroutine = null;
            afterSceneChangeDisposableCoroutine = null;

            // 이전 씬 리소스 언로딩
            // TODO : 이렇게 순서를 말고 ResourceManager에서 Queue로 처리하도록 변경
            while (ResourceManager.Instance.IsUnloading)
            {
                yield return null;
            }
            ResourceManager.Instance.UnloadAllAssetsAtSceneCor(prevSceneName);

            isLoading = false;
        }

        private IEnumerator WaitActionUntilIsDone(Action action)
        {
            bool isDone = false;
            Action endAction = () =>
            {
                isDone = true;
            };

            action += endAction;
            action?.Invoke();
            yield return new WaitUntil(() => isDone);
            action -= endAction;
        }

        private IEnumerator WaitSceneChangedCoroutineUntilIsDone(SceneChangedCoroutine routines)
        {
            if (routines == null)
            {
                yield break;
            }

            // 모든 코루틴을 시작하고 리스트에 추가
            List<Coroutine> runningCoroutines = new List<Coroutine>();
            foreach (SceneChangedCoroutine routine in routines.GetInvocationList())
            {
                IEnumerator coroutine = routine();
                runningCoroutines.Add(StartCoroutine(coroutine));
            }

            // 모든 코루틴이 완료될 때까지 기다림
            foreach (Coroutine coroutine in runningCoroutines)
            {
                yield return coroutine;
            }
        }
    }

    public partial class SceneManager
    {
        // 씬 페이크 로딩 지속 시간
        // NOTE : 비동기 로딩에만 적용
        public float FakeLoadingDuration
        {
            get
            {
                return fakeLoadingDuration;
            }
            set
            {
                fakeLoadingDuration = value;
            }
        }

        // 씬 변경 전, 호출되는 이벤트
        // NOTE : 이벤트가 끝나기 전까지 씬 변경X
        public Action AeforeSceneChangedEvent
        {
            get
            {
                return beforeSceneChangeAction;
            }
            set
            {
                beforeSceneChangeAction = value;
            }
        }

        // 씬 변경 후, 호출되는 이벤트
        // NOTE : 이벤트가 끝나기 전까지 씬 변경X
        public Action AfterSceneChangedEvent
        {
            get
            {
                return afterSceneChangeAction;
            }
            set
            {
                afterSceneChangeAction = value;
            }
        }

        // 씬 변경 전, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        public SceneChangedCoroutine BeforeSceneChangedCoroutine
        {
            get
            {
                return beforeSceneChangeCoroutine;
            }
            set
            {
                beforeSceneChangeCoroutine = value;
            }
        }

        // 씬 변경 후, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        public SceneChangedCoroutine AfterSceneChangedCoroutine
        {
            get
            {
                return afterSceneChangeCoroutine;
            }
            set
            {
                afterSceneChangeCoroutine = value;
            }
        }

        // 현재 씬 이름
        public string CurrentSceneName => UnitySceneManager.GetActiveScene().name;

        // 씬 로딩중
        public bool IsLoading => isLoading;

        // 로딩 씬
        public UnityObject LoadingScene => loadingScene;
    }
}