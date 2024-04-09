using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {        
        public void LoadTitileScene()
        {
            string fromScene = SceneManager.CurrentSceneName;
            string toScene = SceneManager.Setting.TitleScene;
            string loadingScene = SceneManager.Setting.LoadingScene;
            SceneManager.LoadScene(loadingScene, () =>
            {
                foreach (var label in SceneManager.Setting.TitleSceneAddressableLabelMap) { ResourceManager.LoadAssetsAsync(label.labelString, objects => Debug.Log($"{label.labelString} 로드 완료"), true); }
                foreach (var label in SceneManager.Setting.IntroSceneAddressableLabelMap) { ResourceManager.UnloadAssetsAsync(label.labelString, () => Debug.Log($"{label.labelString} 언로드 완료")); }

                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
            });
        }
    }
}