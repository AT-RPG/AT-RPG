using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityObject = UnityEngine.Object;


namespace AT_RPG.Manager
{
    public delegate IEnumerator SceneChangedCoroutine();

    public partial class SceneManager : Singleton<SceneManager>
    {
        // 씬 변경 전, 호출되는 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event Action beforeSceneChangedEvent;

        // 씬 변경 후, 호출되는 이벤트
        // NOTE1 : 이벤트가 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event Action afterSceneChangedEvent;

        // 씬 변경 전, 호출되는 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event SceneChangedCoroutine beforeSceneChangedCoroutine = null;

        // 씬 변경 후, 호출되는 코루틴
        // NOTE1 : 코루틴이 끝나기 전까지 씬 변경X
        // NOTE2 : 한번 등록되면 계속 등록되어 있음
        private event SceneChangedCoroutine afterSceneChangedCoroutine = null;

        // 페이크 로딩 지속 시간
        [SerializeField] private float fakeLoadingDuration = 1.5f;

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
        public void LoadSceneCor(string sceneName, bool isLoadingSceneIncluded)
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
            if (isLoadingSceneIncluded)
            {
                StartCoroutine(InternalLoadSceneIncludedLoadingSceneCor(sceneName));
            }
            else
            {
                StartCoroutine(InternalLoadSceneCor(sceneName));
            }
        }

        private IEnumerator InternalLoadSceneIncludedLoadingSceneCor(string sceneName)
        {
            yield return StartCoroutine(InternalLoadSceneCor(loadingScene.name));
            yield return StartCoroutine(InternalLoadSceneCor(sceneName));
        }

        private IEnumerator InternalLoadSceneCor(string sceneName)
        {
            string prevSceneName = CurrentSceneName.ToString();
            string nextSceneName = sceneName;
            float currFakeLoadingDuration = this.fakeLoadingDuration;

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(beforeSceneChangedEvent));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(beforeSceneChangedCoroutine));

            // 현재 씬 리소스 로딩
            ResourceManager.Instance.LoadAllAssetsAtSceneCor(nextSceneName);
            while (ResourceManager.Instance.IsLoading)
            {
                yield return null;
            }

            // 씬 로드
            // 바로 씬 변경X
            AsyncOperation loadSceneRequest = UnitySceneManager.LoadSceneAsync(nextSceneName);
            loadSceneRequest.allowSceneActivation = false;
            while (!loadSceneRequest.isDone)
            {
                if (loadSceneRequest.progress >= 0.9f)
                {
                    // 90프로 이후 페이크 로딩
                    currFakeLoadingDuration -= Time.deltaTime;
                    if (currFakeLoadingDuration <= 0f)
                    {
                        // 씬 변경O
                        loadSceneRequest.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            // 이전 씬 리소스 언로딩
            // TODO : 이렇게 순서를 말고 ResourceManager에서 Queue로 처리하도록 변경
            while(ResourceManager.Instance.IsUnloading)
            {
                yield return null;
            }
            ResourceManager.Instance.UnloadAllAssetsAtSceneCor(prevSceneName);

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitActionUntilIsDone(afterSceneChangedEvent));
            yield return StartCoroutine(WaitSceneChangedCoroutineUntilIsDone(afterSceneChangedCoroutine));

            isLoading = false;

            yield break;
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
        // 페이크 로딩 지속 시간
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
                return beforeSceneChangedEvent;
            }
            set
            {
                beforeSceneChangedEvent = value;
            }
        }

        // 씬 변경 후, 호출되는 이벤트
        // NOTE : 이벤트가 끝나기 전까지 씬 변경X
        public Action AfterSceneChangedEvent
        {
            get
            {
                return afterSceneChangedEvent;
            }
            set
            {
                afterSceneChangedEvent = value;
            }
        }

        // 씬 변경 전, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        public SceneChangedCoroutine BeforeSceneChangedCoroutine
        {
            get
            {
                return beforeSceneChangedCoroutine;
            }
            set
            {
                beforeSceneChangedCoroutine = value;
            }
        }

        // 씬 변경 후, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        public SceneChangedCoroutine AfterSceneChangedCoroutine
        {
            get
            {
                return afterSceneChangedCoroutine;
            }
            set
            {
                afterSceneChangedCoroutine = value;
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