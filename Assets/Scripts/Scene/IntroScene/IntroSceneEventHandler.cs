using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {        
        public void LoadTitileScene()
        {
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.TitleSceneAsset.SceneName;
            string loadingScene = SceneManager.Setting.LoadingSceneAsset.SceneName;
            SceneManager.LoadScene(loadingScene, () =>
            {
                // ResourceManager.LoadAllResourcesCoroutine(SceneManager.Setting.TitleSceneAsset);

                // ResourceManager.UnloadAllResourcesCoroutine(SceneManager.CurrentSceneName);

                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
            });
        }
    }
}