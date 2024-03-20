using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 멀티플레이와 관련된 모든 기능을 관리합니다.
    /// </summary>
    public partial class MultiplayManager : Singleton<MultiplayManager>, INetworkRunnerCallbacks
    {
        private static MultiplayManagerSetting setting;

        private static MultiplayAuthentication authentication;

        // 포톤 클라우드 네트워크 객체
        private static NetworkRunner runner;

        private static StartGameArgs sessionOption;

        // 포톤 클라우드 애플리케이션 서버에 연결되었는지?
        private static bool isConnected = false;

        // 초대 코드와 세션옵션을 연결
        // 다른 플레이어가 초대 코드로 세션에 입장할 때 사용됩니다.
        private static KeyValuePair<int, StartGameArgs> inviteCodeToSessionOption;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<MultiplayManagerSetting>("MultiplayManagerSettings");
        }



        /// <summary>
        /// 포톤 클라우드 서버에 연결을 시도합니다.
        /// </summary>
        public static async void ConnectToServerAsync(MapSettingData mapSettingData, OnConnectedCallback connected = null, OnDisconnectedCallback disconnected = null)
        {
            // 맵 데이터를 통해 세션 정보를 새로 만듭니다.
            var sessionStartOption = new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = mapSettingData.mapName,
                Scene = UnitySceneManager.GetActiveScene().buildIndex,
                SceneManager = Instance.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            // 설정된 세션 정보는 이제 멀티플레이 매니저가 정보를 캐싱합니다.
            sessionOption = sessionStartOption;

            // 설정된 세션 정보로 포톤 클라우드 서버에 세션 생성을 요청합니다.
            runner = Instance.gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
            StartGameResult serverConnection = await runner.StartGame(sessionStartOption);
            if (serverConnection.Ok)
            {
                connected?.Invoke();
            }
            else
            {
                disconnected?.Invoke();
            }
        }

        /// <summary>
        /// 클라이언트의 고유 식별자를 생성합니다.
        /// 호스트에서 이 식별자를 통해, 이전에 월드에 접속했는지 여부를 판단하게됩니다.
        /// </summary>
        public static void CreateAuthentication()
        {
            authentication = MultiplayAuthentication.IsExist() ? MultiplayAuthentication.Load() : MultiplayAuthentication.CreateNew();
        }

        /// <summary>
        /// 다른 클라이언트가 호스트의 세션에 들어올 수 있도록 초대 코드를 생성합니다.
        /// </summary>
        public static int CreateInviteCode()
        {
            int inviteCode = UnityEngine.Random.Range(100000, 1000000);
            inviteCodeToSessionOption = new KeyValuePair<int, StartGameArgs>(inviteCode, sessionOption);    
            return inviteCode;
        }




        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }

    public partial class MultiplayManager
    {
        public static MultiplayManagerSetting Setting => setting;

        public static MultiplayAuthentication Authentication => authentication;

        public static StartGameArgs SessionOption
        {
            get => sessionOption;
            set => sessionOption = value;
        }

        public static bool IsConnected => isConnected;
    }
}
