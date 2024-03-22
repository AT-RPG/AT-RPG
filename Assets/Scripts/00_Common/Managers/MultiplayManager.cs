using Fusion;
using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 멀티플레이와 관련된 모든 기능을 관리합니다.
    /// </summary>
    public partial class MultiplayManager : Singleton<MultiplayManager>
    {
        private static MultiplayManagerSetting setting = null;

        // 데이터를 실제로 주고받는걸 구현하는 클래스
        private static MultiplayNetworkRunner networkRunner = null;

        // 클라이언트의 고유 식별자
        // 이 식별자를 통해, 이전 월드에 기록이 있는 경우, 데이터를 불러옵니다.
        private static MultiplayAuthentication authentication = null;

        // 다른 클라이언트가 세션에 들어올 수 있도록 하는 초대코드
        // 이 코드를 생성 후, 다른 클라이언트에게 공유합니다.
        private static int inviteCode = 0;

        // 현재 클라이언트의 플레이 방식
        private static PlayMode playMode = PlayMode.Single;


        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<MultiplayManagerSetting>("MultiplayManagerSettings");
        }



        /// <summary>
        /// 포톤 클라우드에 연결을 시도합니다. <br/>
        /// 클라우드에 연결되면, 다른 클라이언트가 초대코드로 접근할 수 있는 세션을 생성합니다.
        /// </summary>
        public static async void ConnectToCloud(ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            if (networkRunner) { return; }

            networkRunner = Instantiate(setting.MultiplayNetworkRunnerPrefab.Resource).GetComponent<MultiplayNetworkRunner>();
            StartGameResult connectionResult = await networkRunner.ConnectToCloud();
            if (connectionResult.Ok)
            {
                connected?.Invoke();
                SetPlayMode(PlayMode.Host);
            }
            else
            {
                disconnected?.Invoke();
                SetPlayMode(PlayMode.Single);
                Disconnect();
            }
        }
        

        /// <summary>
        /// 다른 클라이언트가 만든 세션에 연결을 시도합니다. <br/>
        /// 세션에 연결되면, 게임을 시작합니다.
        /// </summary>
        public static async void ConnectToPlayer(string inviteCode, ConnectedCallback connected = null, DisconnectedCallback disconnected = null)
        {
            if (networkRunner) { return; }

            networkRunner = Instantiate(setting.MultiplayNetworkRunnerPrefab.Resource).GetComponent<MultiplayNetworkRunner>();
            StartGameResult connectionResult = await networkRunner.ConnectToPlayer(inviteCode);
            if (connectionResult.Ok)
            {
                connected?.Invoke();
                SetPlayMode(PlayMode.Client);
            }
            else
            {
                disconnected?.Invoke();
                SetPlayMode(PlayMode.Single);
                Disconnect();
            }
        }


        /// <summary>
        /// 세션에 연결을 종료합니다.
        /// </summary>
        public static void Disconnect()
        {
            if (networkRunner) { return; }

            networkRunner.Disconnect();
            SetPlayMode(PlayMode.Single);
            networkRunner = null;
        }


        /// <summary>
        /// 클라이언트의 고유 식별자를 생성합니다. <br/>
        /// 호스트에서 이 식별자를 통해, 이전에 월드에 접속했는지 여부를 판단하게됩니다.
        /// </summary>
        public static MultiplayAuthentication CreateAuthentication()
        {
            authentication = MultiplayAuthentication.IsExist() ? MultiplayAuthentication.Load() : MultiplayAuthentication.CreateNew();
            return authentication;
        }


        /// <summary>
        /// 다른 클라이언트가 호스트의 세션에 들어올 수 있도록 초대 코드를 생성합니다.
        /// </summary>
        public static int CreateInviteCode()
        {
            inviteCode = inviteCode == 0 ? UnityEngine.Random.Range(100000, 1000000) : inviteCode;
            return inviteCode;
        }


        /// <summary>
        /// 현재 클라이언트의 플레이 방식을 변경합니다.
        /// </summary>
        private static void SetPlayMode(PlayMode newPlayMode)
        {
            playMode = newPlayMode;
        }
    }

    public partial class MultiplayManager
    {
        public static MultiplayManagerSetting Setting => setting;

        public static MultiplayAuthentication Authentication => authentication;

        public static int InviteCode => inviteCode;

        public static PlayMode PlayMode => playMode;
    }
}
