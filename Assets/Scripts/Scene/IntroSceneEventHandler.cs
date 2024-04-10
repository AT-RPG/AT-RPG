using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                foreach (var label in SceneManager.Setting.TitleSceneAddressableLabelMap) { ResourceManager.LoadAssetsAsync(label.labelString, null, true); }
                foreach (var label in SceneManager.Setting.IntroSceneAddressableLabelMap) { ResourceManager.UnloadAssetsAsync(label.labelString); }
                SceneManager.LoadSceneCoroutine(toScene, () => !ResourceManager.IsLoading);
            });
        }
    }
}