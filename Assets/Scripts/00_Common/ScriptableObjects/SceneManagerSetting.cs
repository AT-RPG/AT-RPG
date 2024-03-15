using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SceneManagerSettings", menuName = "ScriptableObject/SceneManager Setting")]
    public class SceneManagerSetting : ScriptableObject
    {
        public SceneReference IntroScene;
        public SceneReference LoadingScene;
        public SceneReference TitleScene;
        public SceneReference MainScene;

        public float FakeLoadingDuration = 0f;
    }
}
