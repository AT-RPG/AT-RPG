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


        private void Start()
        {
            animator.SetTrigger("Create");
        }


        private void OnFadeInBlur(float duration)
        {
            StartCoroutine(OnFadeInBlurImpl(duration));
        }

        private IEnumerator OnFadeInBlurImpl(float duration)
        {
            DepthOfField dof = null;
            volume.profile.TryGet(out dof);
            if (!dof)
            {
                yield break;
            }

            float time = 0f;
            while (time <= duration)
            {
                dof.focalLength = new ClampedFloatParameter(Mathf.Lerp(0f, 300f, time /duration), 0f, 300f);
                time += Time.deltaTime;
                yield return null;
            }
        }

        private void OnFadeOutBlur(float duration)
        {
            StartCoroutine(OnFadeOutBlurImpl(duration));
        }

        private IEnumerator OnFadeOutBlurImpl(float duration)
        {
            DepthOfField dof = null;
            volume.profile.TryGet(out dof);
            if (!dof)
            {
                yield break;
            }

            float time = 0f;
            while (time <= duration)
            {
                dof.focalLength = new ClampedFloatParameter(Mathf.Lerp(300f, 0f, time / duration), 0f, 300f);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}