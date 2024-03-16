namespace AT_RPG
{
    public interface IPopupDestroy
    {
        // 팝업이 삭제될 때, 팝업 관리 스택에서 Destroy()가 호출되는 대신에 DestroyPopup()를 호출합니다.
        public void DestroyPopup();
    }
}

