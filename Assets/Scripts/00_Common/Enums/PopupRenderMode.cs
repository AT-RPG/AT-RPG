namespace AT_RPG
{
    // 다른 팝업이 새로 추가될 때, 현재 팝업의 랜더링 옵션을 설정합니다.
    public enum PopupRenderMode
    {
        // 다른 팝업이 새로 추가될 때, 이 팝업이 그대로 레이어에 노출됩니다.
        Default,
         
        // 다른 팝업이 새로 추가될 때, 이 팝업을 숨깁니다.
        Hide,
    }
}