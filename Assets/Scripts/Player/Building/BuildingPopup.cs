using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AT_RPG
{
    public class BuildingPopup : Popup
    {
        /// <summary>
        /// 팝업 생성 시, 처음에 블러의 값을 트위닝하는데 사용합니다.
        /// </summary>
        [SerializeField] private Animator animator;

        /// <summary>
        /// 팝업의 뒷면을 블러 처리하는데 사용합니다.
        /// </summary>
        [SerializeField] private Volume volume;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator.SetTrigger("Create");
        }
    }
}