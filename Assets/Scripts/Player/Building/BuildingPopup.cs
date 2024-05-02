using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace AT_RPG
{
    public class BuildingPopup : Popup
    {
        /// <summary>
        /// 팝업 생성 시, 버튼들을 트위닝하는데 사용합니다.
        /// </summary>
        [SerializeField] private Animator animator;

        /// <summary>
        /// 팝업의 뒷면을 블러 처리하는데 사용합니다.
        /// </summary>
        [SerializeField] private Volume volume;

        /// <summary>
        /// 팝업 상단 버튼에 바인딩된 건설 오브젝트
        /// </summary>
        [SerializeField] private AssetReferenceResource<GameObject> upperButtonBuildingPrefab;

        /// <summary>
        /// 팝업 하단 버튼에 바인딩된 건설 오브젝트
        /// </summary>
        [SerializeField] private AssetReferenceResource<GameObject> lowerButtonBuildingPrefab;

        /// <summary>
        /// 팝업 오른쪽 버튼에 바인딩된 건설 오브젝트
        /// </summary>
        [SerializeField] private AssetReferenceResource<GameObject> rightButtonBuildingPrefab;

        /// <summary>
        /// 팝업 왼쪽 버튼에 바인딩된 건설 오브젝트
        /// </summary>
        [SerializeField] private AssetReferenceResource<GameObject> leftButtonBuildingPrefab;

        /// <summary>
        /// 팝업의 상단 버튼
        /// </summary>
        public event Action<AssetReferenceResource<GameObject>> UpperButtonAction;

        /// <summary>
        /// 팝업의 오른쪽 버튼
        /// </summary>
        public event Action<AssetReferenceResource<GameObject>> RightButtonAction;

        /// <summary>
        /// 팝업의 하단 버튼
        /// </summary>
        public event Action<AssetReferenceResource<GameObject>> LowerButtonAction;

        /// <summary>
        /// 팝업의 왼쪽 버튼
        /// </summary>
        public event Action<AssetReferenceResource<GameObject>> LeftButtonAction;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator.SetTrigger("Create");
        }

        /// <summary>
        /// UI의 Upper_Button에서 트리거됩니다.
        /// </summary>
        public void OnUpperButtonClick()
        {
            UpperButtonAction?.Invoke(upperButtonBuildingPrefab);
        }

        /// <summary>
        /// UI의 Lower_Button에서 트리거됩니다.
        /// </summary>
        public void OnLowerButtonClick()
        {
            LowerButtonAction?.Invoke(lowerButtonBuildingPrefab);
        }

        /// <summary>
        /// UI의 Right_Button에서 트리거됩니다.
        /// </summary>
        public void OnRightButtonClick()
        {
            RightButtonAction?.Invoke(rightButtonBuildingPrefab);
        }

        /// <summary>
        /// UI의 Left_Button에서 트리거됩니다.
        /// </summary>
        public void OnLeftButtonClick()
        {
            LeftButtonAction?.Invoke(leftButtonBuildingPrefab);
        }
    }
}