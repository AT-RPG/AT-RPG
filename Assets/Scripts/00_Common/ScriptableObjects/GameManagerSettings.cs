using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "GameManagerSettings", menuName = "ScriptableObject/GameManager Setting")]
    public class GameManagerSettings : ScriptableObject
    {
        // GameManager.OnBeforeFirstSceneLoad()에서 실행되는 게임 시작전에 로드할 어드레서블 라벨
        public List<AssetLabelReference> PreloadAddressableLabelMap = new List<AssetLabelReference>();
    }
}