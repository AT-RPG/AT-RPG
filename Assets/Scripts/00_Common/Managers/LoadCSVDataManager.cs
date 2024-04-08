// using AT_RPG;

// public class LoadCSVDataManager : Singleton<LoadCSVDataManager>
// {
//     public DropItemData dropItemData;

//     /// <summary>
//     /// 초기화 처리(리소스 로드) 매서드
//     /// 게임 매니저를 제외한 다른 매니저들보다 먼저 인스턴스화 되어야 함
//     /// Init 함수 호출 역시 가장 빨라야 함
//     /// </summary>
//     public void Initialize()
//     {
//     }

//     /// <summary>
//     /// 
//     /// </summary>
//     public DropItemStat GetDropItemDatas(DropItem _dropItem)
//     {
//         if(dropItemData == null) return null;

//         return dropItemData.dropItemStat[(int)_dropItem];
//     }
// }
