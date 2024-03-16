using UnityEngine;

namespace AT_RPG
{
    public class BackgroundCanvas : MonoBehaviour
    {
        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;

        [SerializeField] private GameObject root;

        // Start is called before the first frame update
        void Start()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
        }

        public GameObject Root => root;
    }
}