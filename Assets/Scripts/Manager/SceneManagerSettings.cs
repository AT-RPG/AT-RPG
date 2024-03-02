using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SceneManagerSettings", menuName = "Scriptable Object/SceneManager Setting")]
    public class SceneManagerSettings : ScriptableObject
    {
        [Tooltip("로딩에 사용할 씬의 이름, 빌드 설정에 씬 등록 필요")]
        [SerializeField] public string LoadingSceneName;

    }

}