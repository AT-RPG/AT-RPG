using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    public partial class FadeOut : MonoBehaviour
    {
        [Header("효과 설정값")]
        [Space(10)]

        // 효과 지속시간
        [SerializeField] private float duration = 1.0f;

        // 페이드 대상 이미지
        [SerializeField] private Image targetImage = null;

        public IEnumerator StartFade()
        { 
            // 시작/종료 시간 기록
            float startTime = Time.time;
            float endTime = startTime + duration;

            // 페이드 아웃
            while (Time.time < endTime)
            {
                // 페이드 아웃 비율
                float ratio = (Time.time - startTime) / duration;

                if (targetImage != null)
                {
                    targetImage.color = new Color(
                        targetImage.color.r,
                        targetImage.color.g,
                        targetImage.color.b,
                        Mathf.Lerp(targetImage.color.a, 0f, ratio)
                    );
                }

                yield return null;
            }

            // 페이드 아웃이 완료된 후, 알파 값 0으로 설정
            if (targetImage != null)
            {
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0f);
            }
        }
    }

    public partial class FadeOut
    {
        // 효과 지속시간
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        // 페이드 대상 이미지
        public Image TargetImage
        {
            get
            {
                return targetImage;
            }
            set
            {
                targetImage = value;
            }
        }
    }
}