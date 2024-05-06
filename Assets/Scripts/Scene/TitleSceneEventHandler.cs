using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    public class TitleSceneEventHandler : MonoBehaviour
    {
        [Header("하위 팝업")]
        [SerializeField] private AssetReferenceResource<GameObject> startGamePopupPrefab;
        [SerializeField] private AssetReferenceResource<GameObject> multiplayPopupPrefab;
        [SerializeField] private AssetReferenceResource<GameObject> optionPopupPrefab;
        [SerializeField] private AssetReferenceResource<GameObject> quitPopupPrefab;

        public void OnInstantiateStartGamePopup()
        {
            UIManager.InstantiatePopup(startGamePopupPrefab, PopupRenderMode.Hide);
        }

        public void OnInstantiateMultiplayPopup()
        {
            UIManager.InstantiatePopup(multiplayPopupPrefab, PopupRenderMode.Default);

        }

        public void OnInstantiateOptionPopup()
        {
            UIManager.InstantiatePopup(optionPopupPrefab, PopupRenderMode.Hide);
        }

        public void OnInstantiateQuitPopup()
        {
            UIManager.InstantiatePopup(quitPopupPrefab, PopupRenderMode.Default);
        }
    }
}