using AT_RPG.Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    public class TitleSceneEventHandler : MonoBehaviour
    {
        [Tooltip("클라우드 서버 연결 정보")]
        [SerializeField] private TMP_Text connectedInfo;

        // Start is called before the first frame update
        void Start()
        {
            RequestConnectToServer();
        }

        /// <summary>
        /// 포톤 클라우드 애플리케이션 인스턴스에 연결을 요청합니다.
        /// </summary>
        private void RequestConnectToServer()
        {
            MultiplayManager.ConnectToServer(SetOnline, SetOffline);
        }

        private void SetOnline()
        {
            connectedInfo.text = "Online";
        }

        private void SetOffline()
        {
            connectedInfo.text = "Offline";
        }
    }

}