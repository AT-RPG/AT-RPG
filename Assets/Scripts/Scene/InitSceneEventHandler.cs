using UnityEngine;
using AT_RPG.Manager;

namespace AT_RPG
{
    public class InitSceneEventHandler : MonoBehaviour
    {
        private void Start()
        {
            foreach (var label in SceneManager.Setting.InitAddressableLabelMap) { ResourceManager.LoadAssetsAsync(label.labelString); }
            SceneManager.LoadSceneCoroutine(SceneManager.Setting.IntroScene, () => !ResourceManager.IsLoading);
        }
    }
}