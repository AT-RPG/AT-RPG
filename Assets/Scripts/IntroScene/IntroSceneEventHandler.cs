using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {        
        public void LoadSceneTo()
        {
            SceneManager.Instance.LoadSceneCor(
                SceneManager.Instance.Setting.TitleScene.SceneName,
                LoadMode.LoadingResources
                );
        }
    }
}