using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using AT_RPG.Manager;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "GameManagerSettings", menuName = "ScriptableObject/GameManager Setting")]
    public class GameManagerSettings : ScriptableObject
    {
        /// '<see cref="GameManager.OnBeforeFirstSceneLoad"/>'에서 로드할 어드레서블 라벨
        public List<AssetLabelReference> PreloadAddressableLabelMap = new List<AssetLabelReference>();
    }
}