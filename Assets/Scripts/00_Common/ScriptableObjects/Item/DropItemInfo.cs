using AT_RPG.Manager;
using UnityEngine;
using System.Collections;

namespace AT_RPG
{
    /// <summary>
    /// 드랍된 아이템의 정보를 가지고 있는 클래스
    /// </summary>
    public class DropItemInfo : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private int dropAmount;
        private void OnEnable() 
        {
            dropAmount = RandomItemCount();
            // StartCoroutine(ItemDestroy());
        }

        // 랜덤으로 아이템의 개수를 정하여 정해둠
        // 몬스터 CSV테이블에서 정해둔 정보로 결정해야할것 같음
        private int RandomItemCount()
        {
            System.Random random = new();
            int rnd = random.Next(1, 4);
            return rnd;
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
                switch(item.myType)
                {
                    case DropItem.Gold:
                    GameManager.Player.AddGold(dropAmount);
                    // 인벤토리에서 gold 추가, 동시에 PlayerData에도 추가, UI변경
                    break;
                    case DropItem.HealPotion:
                    Inventory.Instance.AddInventory(item, dropAmount);
                    // GameManager.Player.AddHealPotion(dropAmount);
                    break;
                    case DropItem.MonsterPiece:
                    Inventory.Instance.AddInventory(item, dropAmount);
                    // GameManager.Player.AddMonsterPiece(dropAmount);
                    break;
                    // 골드와 장비를 제외한 모든 아이템은 인벤토리에 동일한 아이템이 있는지 확인 후 있으면 개수 추가, 
                    // 없으면 PlayerInventorySlot 8개 중 null이 있는지 확인하고 null이 있으면 ItemSlot에 해당 아이템 정보를 담아서 프리펩 Instanciate해서 위치 잡아주고,
                    // null이 없으면 인벤토리에 자리가 없으므로 아이템은 못먹고 알람을 띄워주어야 한다.

                    // 장비 아이템일경우 PlayerInventorySlot 8개중 null이 있는지 확인하고 
                    // null이 있으면 ItemSlot에 해당 아이템 정보를 담아서 프리펩 Instanciate해서 위치 잡아주고,
                    // null이 없으면 인벤토리에 자리가 없으므로 아이템은 못먹고 알람을 띄워주어야 한다.
                }

                // bool check = GameManager.Event.CheckInventoryEvent?.Invoke(this);
                // if(GameManager.Event.CheckInventoryEvent.Invoke(this))
                // {
                //     gameObject.SetActive(false);
                // }
                // else
                // {
                //     Debug.Log("인벤토리가 가득 찼습니다.");
                // }
            }
        }

        IEnumerator ItemDestroy()
        {
            float timer = 5.0f;
            while(timer > 0.0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}

