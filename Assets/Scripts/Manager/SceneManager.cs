using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


namespace AT_RPG.Manager
{
    public delegate IEnumerator SceneChangedCoroutine();

    public partial class SceneManager : Singleton<SceneManager>
    {
        // 페이크 로딩 지속 시간
        private float fakeLoadingDuration = 0.75f;

        // 씬 변경 전, 호출되는 이벤트
        // NOTE : 이벤트가 끝나기 전까지 씬 변경X
        private UnityEvent beforeSceneChangedEvent = new UnityEvent();

        // 씬 변경 후, 호출되는 이벤트
        // NOTE : 이벤트가 끝나기 전까지 씬 변경X
        private UnityEvent afterSceneChangedEvent = new UnityEvent();

        // 씬 변경 전, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        private SceneChangedCoroutine beforeSceneChangedCoroutine = null;

        // 씬 변경 후, 호출되는 코루틴
        // NOTE : 코루틴이 끝나기 전까지 씬 변경X
        private SceneChangedCoroutine afterSceneChangedCoroutine = null;

        // 씬 로딩중 코루틴
        private Coroutine loadCoroutine = null;


        protected override void Awake()
        {
            base.Awake();
        }



        /// <summary>
        /// 현재 씬에서 다음 씬(sceneName)으로 전환
        /// </summary>
        public void LoadSceneCor(string sceneName)
        {
            if (sceneName == CurrentSceneName)
            {
                Debug.LogError("동일한 씬으로 로딩할 수 없습니다.");
                return;
            }

            if (loadCoroutine != null)
            {
                Debug.LogError("씬이 로딩중입니다.");
                return;
            }

            loadCoroutine = StartCoroutine(InternalLoadScene(sceneName));
        }



        private IEnumerator InternalLoadScene(string sceneName)
        {
            string prevSceneName = CurrentSceneName.ToString();
            string nextSceneName = sceneName;
            float currFakeLoadingDuration = this.fakeLoadingDuration;

            // 씬 변경 이벤트, 코루틴을 실행
            // 이벤트, 코루틴이 완료될때까지 대기
            yield return StartCoroutine(WaitUnityEventUntilIsDone(beforeSceneChangedEvent));
            yield return StartCoroutine(WaitCoroutinesUntilIsDone(beforeSceneChangedCoroutine));

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
                        // 씬 변경 이벤트, 코루틴을 실행
                        // 이벤트, 코루틴이 완료될때까지 대기
                        yield return StartCoroutine(WaitUnityEventUntilIsDone(afterSceneChangedEvent));
                        yield return StartCoroutine(WaitCoroutinesUntilIsDone(afterSceneChangedCoroutine));

                        // 씬 변경O
                        loadSceneRequest.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            // 이전 씬 리소스 언로딩
            while(ResourceManager.Instance.IsUnloading)
            {
                yield return null;
            }
            ResourceManager.Instance.UnloadAllAssetsAtSceneCor(prevSceneName);

            loadCoroutine = null;

            yield break;
        }

        private IEnumerator WaitUnityEventUntilIsDone(UnityEvent unityEvent)
        {
            bool isDone = false;
            UnityAction endAction = () =>
            {
                isDone = true;
            };

            unityEvent.AddListener(endAction);
            unityEvent?.Invoke();
            yield return new WaitUntil(() => isDone);
            unityEvent.RemoveListener(endAction);
        }

        private IEnumerator WaitCoroutinesUntilIsDone(SceneChangedCoroutine routines)
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
        public UnityEvent BeforeSceneChangedEvent
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
        public UnityEvent AfterSceneChangedEvent
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
        public bool IsLoading => loadCoroutine != null ? true : false;
    }
}