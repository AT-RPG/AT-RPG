using System;

namespace AT_RPG
{
    /// <summary>
    /// 맵 설정값을 저장하는 클래스
    /// </summary>
    [Serializable]
    public class WorldSettingData
    {
        // 생성되는 맵의 이름
        public string worldName;

        // 멀티플레이 설정
        public bool   isMultiplayEnabled;

        // 마지막으로 플레이한 시간
        public string lastModifiedTime;

        public WorldSettingData() { }
    }

}