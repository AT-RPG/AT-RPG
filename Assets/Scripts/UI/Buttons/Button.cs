using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    [RequireComponent(typeof(Animator))]
    public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("버튼 클릭 시 이벤트")]
        [Space(5)]
        [SerializeField] private UnityEvent onButtonClick;

        [Header("마우스 포인터 버튼에 들어올 시 이벤트")]
        [Space(5)]
        [SerializeField] private UnityEvent onButtonPointerEnter;

        [Header("마우스 포인터 버튼에 나갈 시 이벤트")]
        [Space(5)]
        [SerializeField] private UnityEvent onButtonPointerExit;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnPointerClick(PointerEventData eventData) => onButtonClick?.Invoke();

        public void OnPointerEnter(PointerEventData eventData) => onButtonPointerEnter?.Invoke();

        public void OnPointerExit(PointerEventData eventData) => onButtonPointerExit?.Invoke();
    }
}
