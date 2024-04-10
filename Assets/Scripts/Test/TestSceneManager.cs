using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// SceneManager의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// </summary>
    public class TestSceneManager : MonoBehaviour
    {
        // 직렬화 가능한 씬 에셋
        [SerializeField] AssetReferenceScene testSceneReferenceAsset;

        private void Start()
        {
            // SceneManagerSetting(스크립터블 오브젝트)에서 씬을 등록하시면 SceneManager.Setting에서 전역 접근이 가능합니다.
            string LoadingSceneName = SceneManager.Setting.LoadingScene;
        }

        void Update()
        {
            // testSceneReference씬으로 넘어가는 코드
            if (Input.GetKeyDown(KeyCode.F10))
            {
                // 동기적으로 로딩 씬을 로딩합니다
                // 람다식에서 LoadScene() 종료시 실행할 콜백을 설정합니다.
                string fromScene = SceneManager.CurrentSceneName;
                string toScene = testSceneReferenceAsset.SceneName;
                string loadingScene = SceneManager.Setting.LoadingScene;
                SceneManager.LoadScene(loadingScene, () =>
                {
                    // LoadCompleted 콜백이므로, 현재 씬은 로딩씬.

                    // 로딩 씬동안 리소스 로딩
                    // ResourceManager.LoadAllResourcesCoroutine(toScene);

                    // 로딩 씬동안 이전 씬 리소스 로딩
                    // ResourceManager.UnloadAllResourcesCoroutine(fromScene);

                    // 비동기적으로 로딩 씬을 로딩합니다
                    // 람다식에서 LoadSceneCoroutine()의 시작조건 콜백을 설정합니다. TRUE -> LoadSceneCoroutine() 시작
                    SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
                });

            }
        }
    }
}
