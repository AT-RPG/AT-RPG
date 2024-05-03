using Fusion;
using System.Collections;
using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 멀티플레이와 관련된 모든 기능을 관리하는 클래스
    /// </summary>
    public partial class MultiplayManager : Singleton<MultiplayManager>
    {
        private static MultiplayManagerSettings setting;

        // 데이터를 실제로 주고받는걸 구현하는 클래스
        private static MultiplayNetworkRunner networkRunner;

        // 다른 클라이언트가 세션에 들어올 수 있도록 하는 초대코드
        // 이 코드를 생성 후, 다른 클라이언트에게 공유합니다.
        private static int inviteCode = 0;

        // 현재 클라이언트의 플레이 방식
        private static GameMode gameMode = GameMode.Single;

        // 네트워크에 연결중인지?
        private static bool isConnecting = false;

        // 플레이어의 시작지점
        private static Vector3 playerSpawnPoint = Vector3.zero;

        // 시작 조건 콜백 true->로딩 시작, false->로딩 대기
        public delegate bool StartConditionCallback();

        protected override void Awake()
        {
            base.Awake();
            setting = Resources.Load<MultiplayManagerSettings>("MultiplayManagerSettings");
        }

        /// <summary>
        /// 포톤 클라우드에 연결을 시도합니다. <br/>
        /// 클라우드에 연결되면, 다른 클라이언트가 초대코드로 접근할 수 있는 세션을 생성합니다.
        /// </summary>
        public static void Connect(
            StartConditionCallback started = null, ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            if (networkRunner)
            {
                return;
            }

            Instance.StartCoroutine(ConnectImpl(started, connected, disconnected));
        }

        /// <summary>
        /// 다른 클라이언트가 만든 세션에 연결을 시도합니다. <br/>
        /// 세션에 연결되면, 게임을 시작합니다.
        /// </summary>
        public static void Connect(
            string inviteCode, StartConditionCallback started = null, ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            if (networkRunner)
            {
                return;
            }

            Instance.StartCoroutine(ConnectImpl(inviteCode, started, connected, disconnected));
        }

        private static IEnumerator ConnectImpl(
            StartConditionCallback started = null, ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            isConnecting = true;

            while (started != null && !started.Invoke()) { yield return null;  }

            networkRunner = Instantiate(setting.MultiplayNetworkRunnerPrefab).GetComponent<MultiplayNetworkRunner>();
            var connectionTask = networkRunner.ConnectToCloud();
            yield return new WaitUntil(() => connectionTask.IsCompleted);

            if (connectionTask.Result.Ok)
            {
                connected?.Invoke();
                SetGameMode(GameMode.Host);
            }
            else
            {
                disconnected?.Invoke();
                DisconnectAsync();
            }

            isConnecting = false;
        }

        private static IEnumerator ConnectImpl(
            string inviteCode, StartConditionCallback started = null, ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            isConnecting = true;

            while (started != null && !started.Invoke()) { yield return null; }

            networkRunner = Instantiate(setting.MultiplayNetworkRunnerPrefab).GetComponent<MultiplayNetworkRunner>();
            var connectionTask = networkRunner.ConnectToPlayer(inviteCode);
            yield return new WaitUntil(() => connectionTask.IsCompleted);

            if (connectionTask.Result.Ok)
            {
                connected?.Invoke();
                SetGameMode(GameMode.Client);
            }
            else
            {
                disconnected?.Invoke();
                DisconnectAsync();
            }

            isConnecting = false;
        }

        /// <summary>
        /// 세션에 연결을 종료합니다.
        /// </summary>
        public static async void DisconnectAsync()
        {
            // 서버에 연결되어있지 않습니다.
            if (!networkRunner)
            {
                return;
            }

            await networkRunner.Disconnect();
            networkRunner = null;
            inviteCode = 0;

            SetGameMode(GameMode.Single);
        }

        /// <summary>
        /// 다른 클라이언트가 호스트의 세션에 들어올 수 있도록 초대 코드를 생성합니다.
        /// </summary>
        public static int CreateInviteCode()
        {
            inviteCode = inviteCode == 0 ? Random.Range(100000, 1000000) : inviteCode;
            return inviteCode;
        }

        /// <summary>
        /// 현재 클라이언트의 플레이 방식을 변경합니다.
        /// </summary>
        private static void SetGameMode(GameMode newGameMode)
        {
            gameMode = newGameMode;
        }
    }

    public partial class MultiplayManager
    {
        public static MultiplayManagerSettings Setting => setting;

        public static int InviteCode => inviteCode;

        public static GameMode GameMode => gameMode;

        public static bool IsConnecting => isConnecting;

        public static Vector3 PlayerSpawnPoint
        {
            get => playerSpawnPoint;
            set => playerSpawnPoint = value;
        }
    }
}
