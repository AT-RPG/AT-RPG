using Photon.Pun;
using Photon.Realtime;

namespace AT_RPG
{
    /// <summary>
    /// 포톤 클라우드 서버에 연결을 시도하는 클래스입니다.
    /// </summary>
    public class MultiplayLauncher : MonoBehaviourPunCallbacks
    {
        public event OnConnectedCallback OnConnectedCallback;

        public event OnDisconnectedCallback OnDisconnectedCallback;

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            OnConnectedCallback?.Invoke();

            Destroy(gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            OnDisconnectedCallback?.Invoke();

            Destroy(gameObject);
        }
    }
}