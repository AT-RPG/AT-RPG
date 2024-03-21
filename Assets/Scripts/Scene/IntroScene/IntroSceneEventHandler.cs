using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {        
        public void LoadTitileScene()
        {
            SceneManager.LoadScene(SceneManager.Setting.LoadingScene, () =>
            {
                ResourceManager.LoadAllResourcesCoroutine(SceneManager.Setting.TitleScene);

                ResourceManager.UnloadAllResourcesCoroutine(SceneManager.CurrentSceneName);

                SceneManager.LoadSceneCoroutine(SceneManager.Setting.TitleScene, () => !ResourceManager.IsLoading);
            });
        }
    }
}