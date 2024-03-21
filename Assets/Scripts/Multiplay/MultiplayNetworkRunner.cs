using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using AT_RPG.Manager;
using System.Threading.Tasks;

namespace AT_RPG
{
    /// <summary>
    /// <see cref="MultiplayManager"/>의 네트워크 요청을 Photon Fusion 1으로 구현하는 일회용 클래스입니다.  <br/>
    /// 세션에 연결이 해제 또는 실패하면, 객체가 <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/>됩니다.
    /// </summary>
    public class MultiplayNetworkRunner : MonoBehaviour, INetworkRunnerCallbacks
    {
        private static NetworkRunner runner;



        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy()
        {
            runner.Shutdown();
        }



        /// <summary>
        /// 포톤 클라우드에 연결을 시도합니다. <br/>
        /// 클라우드에 연결되면, 다른 클라이언트가 초대코드로 접근할 수 있는 세션을 생성합니다.
        /// </summary>
        public async Task<StartGameResult> ConnectToCloud()
        {
            // 세션 정보를 새로 만듭니다.
            var sessionStartOption = new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = MultiplayManager.CreateInviteCode().ToString(),
                Scene = UnitySceneManager.GetSceneByName(SceneManager.Setting.MainScene).buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                IsVisible = false,

            };

            // 설정된 세션 정보로 포톤 클라우드 서버에 세션 생성을 요청합니다.
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
            StartGameResult connectionResult = await runner.StartGame(sessionStartOption);

            return connectionResult;
        }


        /// <summary>
        /// 다른 클라이언트가 만든 세션에 연결을 시도합니다. <br/>
        /// 세션에 연결되면, 게임을 시작합니다.
        /// </summary>
        public async Task<StartGameResult> ConnectToPlayer(string inviteCode)
        {
            // 세션 정보를 새로 만듭니다.
            var sessionStartOption = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = inviteCode,
                Scene = UnitySceneManager.GetSceneByName(SceneManager.Setting.MainScene).buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                DisableClientSessionCreation = true
            };

            // 설정된 세션 정보로 다른 클라이언트의 세션 연결을 요청합니다.
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
            StartGameResult connectionResult = await runner.StartGame(sessionStartOption);

            return connectionResult;
        }


        /// <summary>
        /// 세션에 연결을 종료합니다.
        /// </summary>
        public void Disconnect()
        {
            runner.Shutdown();
            Destroy(gameObject);
        }



        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    }
}