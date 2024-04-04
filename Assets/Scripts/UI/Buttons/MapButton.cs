using AT_RPG.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public partial class MapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // 버튼 뷰포트
        [SerializeField] private TMP_Text mapName;
        [SerializeField] private TMP_Text lastModifiedTime;

        // 맵 설정 정보
        [SerializeField] private WorldSettingData worldSettingData;

        [Header("버튼 상호작용")]
        [Space(5)]
        [SerializeField] private UnityEvent onButtonClick;
        [SerializeField] private UnityEvent onButtonEnter;
        [SerializeField] private UnityEvent onButtonExit;

        // 맵 선택화면에서 피킹시 호출되는 이벤트
        private event Action<GameObject> onButtonClickAction;

        private void Awake()
        {
            // 마우스 이벤트 트리거 초기화
            if (gameObject.GetComponent<EventTrigger>() == null)
            {
                gameObject.AddComponent<EventTrigger>();
            }
        }

        /// <summary>
        /// 버튼 클릭 시, 피킹 이벤트를 호출
        /// </summary>
        public void OnPick()
        {
            onButtonClickAction?.Invoke(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData) => onButtonEnter?.Invoke();

        public void OnPointerExit(PointerEventData eventData) => onButtonExit?.Invoke();

        public void OnPointerClick(PointerEventData eventData) => onButtonClick?.Invoke();
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
        public WorldSettingData WorldSettingData
        {
            get => worldSettingData;
            set
            {
                worldSettingData = value;
                mapName.text = worldSettingData.worldName;
                lastModifiedTime.text = worldSettingData.lastModifiedTime;
            }
        }
    }
}