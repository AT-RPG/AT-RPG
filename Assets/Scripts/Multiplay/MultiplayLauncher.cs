using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 포톤 클라우드 서버에 연결을 시도하는 클래스입니다.
    /// </summary>
    public class MultiplayLauncher : MonoBehaviour
    {
        public event OnConnectedCallback OnConnectedCallback;

        public event OnDisconnectedCallback OnDisconnectedCallback;

        void Start()
        {
        }
    }
}