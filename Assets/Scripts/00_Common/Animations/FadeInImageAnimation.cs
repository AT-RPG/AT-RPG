using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    public partial class FadeInImageAnimation : MonoBehaviour
    {
        [Header("효과 설정값")]
        [Space(10)]

        // 효과 지속시간
        [SerializeField] private float duration = 1.0f;

        // 시작할 때 이미지를 완전 투명하게 설정?
        [SerializeField] private bool isStartFullyTransparent = false;

        // 코루틴 종료 후, 이 컴포넌트를 삭제할지 여부
        [SerializeField] private bool isDestroyThisAtDone = false;

        // 페이드 대상 이미지
        [SerializeField] private Image targetImage = null;

        /// <summary>
        /// 바인딩된 이미지에 페이드 인 효과를 적용하는 코루틴을 실행
        /// </summary>
        public void StartFadeCor()
        {
            StartCoroutine(StartFade());
        }

        /// <summary>
        /// 바인딩된 이미지에 페이드 인 효과를 적용 코루틴
        /// </summary>
        public IEnumerator StartFade()
        {
            // 처음 타겟 이미지를 완전히 투명하게 설정한다면 알파값을 0로 설정
            if (isStartFullyTransparent && targetImage != null)
            {
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0f);
            }

            // 페이드 아웃
            float elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                // 페이드 아웃 비율
                float ratio = elapsedTime / duration;

                if (targetImage != null)
                {
                    targetImage.color = new Color(
                        targetImage.color.r,
                        targetImage.color.g,
                        targetImage.color.b,
                        Mathf.Lerp(isStartFullyTransparent ? 0f : targetImage.color.a, 1f, ratio)
                    );
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 페이드 인이 완료된 후, 알파 값 1로 설정
            if (targetImage != null)
            {
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 1f);
            }

            if (isDestroyThisAtDone)
            {
                Destroy(this);
            }
        }
    }

    public partial class FadeInImageAnimation
    {
        // 효과 지속시간
        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        // 페이드 대상 이미지
        public Image TargetImage
        {
            get { return targetImage; }
            set { targetImage = value; }
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
