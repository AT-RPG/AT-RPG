using AT_RPG;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SceneManagerSetting", menuName = "ScriptableObject/SceneManager Setting")]
    public class SceneManagerSetting : ScriptableObject
    {
        public SceneReference IntroScene;
        public SceneReference LoadingScene;
        public SceneReference TitleScene;
        public SceneReference MainScene;
    }
}
