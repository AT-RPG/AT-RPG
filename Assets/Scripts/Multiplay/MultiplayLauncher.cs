using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

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

            Debug.Log("server connected");

            OnConnectedCallback?.Invoke();

            Destroy(gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            Debug.Log("server disconnected");

            OnDisconnectedCallback?.Invoke();

            Destroy(gameObject);
        }
    }
}