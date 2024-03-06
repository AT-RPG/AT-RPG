using AT_RPG.Manager;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    // 로딩 상태를 표시할 텍스트
    [SerializeField] private TMP_Text loadingStatusText;

    // 다음 씬으로 

    private void Update()
    {
        if (ResourceManager.Instance.IsLoading)
        {
            loadingStatusText.text = "Resource pre-caching";
        }
        else
        if (SceneManager.Instance.IsLoading)
        {
            loadingStatusText.text = "Loading Scene";
        }
        else
        {
            loadingStatusText.text = "Wait for a second";
        }
    }
}
