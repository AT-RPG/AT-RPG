using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    public partial class FadeInText : MonoBehaviour
    {
        [Header("효과 설정값")]
        [Space(10)]

        // 효과 지속시간
        [SerializeField] private float duration = 1.0f;

        // 시작할 때 텍스트를 완전 투명하게 할지 여부
        [SerializeField] private bool isStartFullyTransparent = false;

        // 코루틴 종료 후, 이 컴포넌트를 삭제할지 여부
        [SerializeField] private bool isDestroyThisAtDone = false;

        // 페이드 대상 텍스트
        [SerializeField] private TMP_Text targetText = null;

        /// <summary>
        /// 바인딩된 텍스트에 페이드 인 효과를 적용하는 코루틴을 실행합니다.
        /// </summary>
        public void StartFadeCor()
        {
            StartCoroutine(StartFade());
        }

        /// <summary>
        /// 바인딩된 텍스트에 페이드 인 효과를 적용하는 코루틴입니다.
        /// </summary>
        public IEnumerator StartFade()
        {
            if (isStartFullyTransparent && targetText != null)
            {
                // 페이드 인 시작 전 텍스트를 완전히 투명하게 설정합니다.
                targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, 0f);
            }

            // 페이드 아웃
            float elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                // 페이드 아웃 비율
                float ratio = elapsedTime / duration;

                if (targetText != null)
                {
                    targetText.color = new Color(
                        targetText.color.r,
                        targetText.color.g,
                        targetText.color.b,
                        Mathf.Lerp(isStartFullyTransparent ? 0f : targetText.color.a, 1f, ratio)
                    );
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 페이드 인이 완료된 후, 알파 값 1로 설정
            if (targetText != null)
            {
                targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, 1f);
            }

            if (isDestroyThisAtDone)
            {
                Destroy(this);
            }
        }
    }

    public partial class FadeInText
    {
        // 효과 지속시간
        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        // 페이드 대상 텍스트
        public TMP_Text TargetText
        {
            get { return targetText; }
            set { targetText = value; }
        }

        // 시작할 때 텍스트를 완전 투명하게 할지 여부
        public bool IsStartFullyTransparent
        {
            get { return isStartFullyTransparent; }
            set { isStartFullyTransparent = value; }
        }

        // 코루틴 종료 후, 이 컴포넌트를 삭제할지 여부
        public bool IsDestroyThisAtDone
        {
            get { return isDestroyThisAtDone; }
            set { isDestroyThisAtDone = value; }
        }

    }
}
