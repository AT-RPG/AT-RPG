using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class IntroSceneEventHandler : MonoBehaviour
    {
        public void LoadSceneTo(Object nextScene)
        {
            SceneManager.Instance.LoadSceneCor(nextScene.name, true);
        }
    }
}