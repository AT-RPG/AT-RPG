using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 멀티플레이와 관련된 모든 기능을 관리합니다.
    /// </summary>
    public partial class MultiplayManager : Singleton<MultiplayManager>
    {
        private static MultiplayManagerSetting setting;

        private static MultiplayAuthentication authentication;

        // 포톤 클라우드 애플리케이션 서버에 연결되었는지?
        private static bool isConnected = false;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<MultiplayManagerSetting>("MultiplayManagerSettings");
        }

        /// <summary>
        /// 포톤 PUN2 서버에 연결을 시도합니다.
        /// </summary>
        public static void ConnectToServer(OnConnectedCallback connected, OnDisconnectedCallback disconnected)
        {
            GameObject launcherInstance = Instantiate(setting.multiplayLauncherPrefab.Resource);
            MultiplayLauncher launcher = launcherInstance.GetComponent<MultiplayLauncher>();
            launcher.OnConnectedCallback += CreateAuthentication;
            launcher.OnConnectedCallback += connected;
            launcher.OnDisconnectedCallback += disconnected;
        }

        public static void CreateAuthentication()
        {
            authentication = MultiplayAuthentication.IsExist() ? MultiplayAuthentication.Load() : MultiplayAuthentication.CreateNew();
        }
    }

    public partial class MultiplayManager
    {
        public static MultiplayManagerSetting Setting => setting;

        public static MultiplayAuthentication Authentication => authentication;

        public static bool IsConnected => isConnected;
    }
}
