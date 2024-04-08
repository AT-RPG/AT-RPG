using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 드랍된 아이템의 정보를 가지고 있는 클래스
    /// </summary>
    public class DropItemInfo : MonoBehaviour
    {
        [SerializeField] private int itemCount = 0;
        [SerializeField] private DropItem itemType;
        [SerializeField] private DropItemData dropItemData;
        private int itemIndex;
        private string itemName;
        private int itemPriceBuy;
        private int itemPriceSell;

        private void OnEnable() 
        {
            RandomItemCount();
            SetItemInfo();
        }

        // 랜덤으로 아이템의 개수를 정하여 정해둠
        // 몬스터 CSV테이블에서 정해둔 정보로 결정해야할것 같음
        private void RandomItemCount()
        {
            System.Random random = new();
            int rnd = random.Next(1, 4);
            itemCount = rnd;
        }

        /// <summary>
        /// 아이템의 Index, 이름, 구매판매가격을 결정해줌
        /// 추후 아이템의 UI에 사용될 내용들을 변수에 넣어두었음
        /// </summary>
        private void SetItemInfo()
        {
            switch (itemType)
            {
                case DropItem.Gold:
                itemIndex = dropItemData.dropItemStat[(int)DropItem.Gold].index;
                itemName = dropItemData.dropItemStat[(int)DropItem.Gold].itemName;
                itemPriceBuy = dropItemData.dropItemStat[(int)DropItem.Gold].priceBuy;
                itemPriceSell = dropItemData.dropItemStat[(int)DropItem.Gold].priceSell;
                break;
                case DropItem.HealPotion:
                itemIndex = dropItemData.dropItemStat[(int)DropItem.HealPotion].index;
                itemName = dropItemData.dropItemStat[(int)DropItem.HealPotion].itemName;
                itemPriceBuy = dropItemData.dropItemStat[(int)DropItem.HealPotion].priceBuy;
                itemPriceSell = dropItemData.dropItemStat[(int)DropItem.HealPotion].priceSell;
                break;
                case DropItem.MonsterPiece:
                itemIndex = dropItemData.dropItemStat[(int)DropItem.MonsterPiece].index;
                itemName = dropItemData.dropItemStat[(int)DropItem.MonsterPiece].itemName;
                itemPriceBuy = dropItemData.dropItemStat[(int)DropItem.MonsterPiece].priceBuy;
                itemPriceSell = dropItemData.dropItemStat[(int)DropItem.MonsterPiece].priceSell;
                break;
                case DropItem.Rock:
                itemIndex = dropItemData.dropItemStat[(int)DropItem.Rock].index;
                itemName = dropItemData.dropItemStat[(int)DropItem.Rock].itemName;
                itemPriceBuy = dropItemData.dropItemStat[(int)DropItem.Rock].priceBuy;
                itemPriceSell = dropItemData.dropItemStat[(int)DropItem.Rock].priceSell;
                break;
                case DropItem.Wood:
                itemIndex = dropItemData.dropItemStat[(int)DropItem.Wood].index;
                itemName = dropItemData.dropItemStat[(int)DropItem.Wood].itemName;
                itemPriceBuy = dropItemData.dropItemStat[(int)DropItem.Wood].priceBuy;
                itemPriceSell = dropItemData.dropItemStat[(int)DropItem.Wood].priceSell;
                break;
            }
        }

        /// <summary>
        /// DropItem에 Player가 닿으면 Player의 보유 아이템에 더해줌
        /// </summary>
        /// <param name="other">DropItem에 닿는 오브젝트</param>
        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                gameObject.SetActive(false);
                // playerData에 item정보 및 count 전달
                switch(itemType)
                {
                    case DropItem.Gold:
                    GameManager.Instance.Player.AddPlayerCoin(itemCount);
                    break;
                    case DropItem.HealPotion:
                    GameManager.Instance.Player.AddPlayerHealPotion(itemCount);
                    break;
                    case DropItem.MonsterPiece:
                    GameManager.Instance.Player.AddPlayerMonsterPiece(itemCount);
                    break;
                }
            }
        }
    }
}

