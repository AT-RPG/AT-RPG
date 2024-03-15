using System;

namespace AT_RPG
{
    /// <summary>
    /// 맵 설정값을 저장하는 클래스
    /// </summary>
    [Serializable]
    public class MapSettingData
    {
        // 생성되는 맵의 이름
        public string mapName;

        public MapSettingData() { }
    }

}