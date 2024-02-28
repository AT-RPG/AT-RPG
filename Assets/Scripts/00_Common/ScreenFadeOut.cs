using AT_RPG.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    /// <summary>
    /// 화면이 점점 어두워지는 효과 생성
    /// </summary>
    public partial class ScreenFadeOut : MonoBehaviour
    {
        // 출력할 캔버스
        [SerializeField] private Canvas canvas = UIManager.Instance.Canvas;

        // 효과 지속시간
        [SerializeField] private float duration = 1.0f;

        private void Awake()
        {
            gameObject.AddComponent<Image>();
        }

        private void Start()
        {
            StartCoroutine(FadeOutCor());
        }

        public IEnumerator FadeOutCor()
        {
            while (duration <= 0f)
            {
                duration -= Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
    }

    public partial class ScreenFadeOut
    {
        // 출력할 캔버스
        public Canvas Canvas
        {
            get
            {
                return canvas;
            }
            set
            {
                canvas = value;
            }
        }

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
    }

}