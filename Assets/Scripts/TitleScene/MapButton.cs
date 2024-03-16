using AT_RPG.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public partial class MapButton : MonoBehaviour
    {
        // 버튼 뷰포트
        [SerializeField] private TMP_Text mapName;
        [SerializeField] private TMP_Text lastModifiedTime;

        // 맵 설정 정보
        [SerializeField] private MapSettingData mapSettingData;

        [SerializeField] private FadeCanvasAnimation fadeAnimation;
        [SerializeField] private PopupCanvasAnimation popupAnimation;
        [SerializeField] private SceneReference mainScene;

        // 맵 선택화면에서 피킹시 호출되는 이벤트
        private event Action<GameObject> onButtonClickAction;

        // 더블 클릭 인터벌 변수
        private float lastClickTime = 0f;
        private float catchTime = 0.25f;


        private void Awake()
        {
            // 마우스 이벤트 트리거 초기화
            if (gameObject.GetComponent<EventTrigger>() == null)
            {
                gameObject.AddComponent<EventTrigger>();
            }
        }

        private void Start()
        {
            fadeAnimation.StartFade();
            popupAnimation.StartPopup();
        }

        /// <summary>
        /// 버튼 클릭 시, 피킹 이벤트를 호출
        /// </summary>
        public void OnPick()
        {
            onButtonClickAction?.Invoke(gameObject);
        }
    }

    public partial class MapButton
    {
        public string MapName
        {
            get => mapName.text;
            set => mapName.text = value;
        }
        public string LastModifiedTime
        {
            get => lastModifiedTime.text;
            set => lastModifiedTime.text = value;
        }
        public Action<GameObject> OnPickAction
        {
            get => onButtonClickAction;
            set => onButtonClickAction = value;
        }
        public MapSettingData MapSettingData
        {
            get => mapSettingData;
            set
            {
                mapSettingData = value;
                mapName.text = mapSettingData.mapName;
                lastModifiedTime.text = mapSettingData.lastModifiedTime;
            }
        }
    }
}