using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {
        [SerializeField] private SceneReference titleScene;
        
        public void LoadSceneTo()
        {
            SceneManager.Instance.LoadSceneCor(titleScene.SceneName, LoadMode.LoadingResources);
        }
    }
}