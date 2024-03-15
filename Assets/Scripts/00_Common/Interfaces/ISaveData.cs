namespace AT_RPG
{
    public interface ISaveData
    {
        /// <summary>
        /// DataManager를 통해 저장하기 전, 호출
        /// </summary>
        public SerializableData SaveData();
    }
}