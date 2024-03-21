namespace AT_RPG
{
    /// <summary>
    /// 인게임 플레이 시, 플레이 방식을 명시하는데 사용합니다.
    /// </summary>
    public enum PlayMode
    {
        // 스탠드 얼론 싱글 플레이입니다.
        Single,

        // 멀티 플레이임과 동시에 세션의 호스트로 플레이합니다.
        Host,

        // 멀티 플레이임과 동시에 세션의 클라이언트로 플레이합니다.
        Client
    }
}