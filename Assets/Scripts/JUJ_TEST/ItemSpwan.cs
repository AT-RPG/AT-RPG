using System.Collections;
using UnityEngine;

namespace AT_RPG
{
    public class ItemSpwan : MonoBehaviour, ICharacterDamage
    {
        public Animator itemAnimator;
        public GameObject itemBoxPrefab;
        public GameObject itemBox;
        public float needTime = 3.0f;
        public LayerMask layer;
        public float inTime;
        public GameObject box;
        public GameObject[] dropItemPrefabs;
        public float[] dropItemProbabilities;
        public int minDropCount = 1;
        public int maxDropCount = 10;
        public Bounds bounds;

        void Start()
        {
            itemAnimator.speed = 0f;
            bounds = GetComponent<Collider>().bounds;
        }

        public void TakeDamage(float dmg)
        {
            // 일정 데미지 이상을 받았을 때 아이템 스폰
            if (dmg >= 10f) // 이 값은 적절히 조정해야 합니다.
            {
                StartCoroutine(SpawnItemBox(5.0f));
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (((1 << other.gameObject.layer) & layer) != 0)
            {
                Debug.Log("들어왔ㅇㅇ");
                itemAnimator.speed = 1f;
                inTime += Time.deltaTime;
                StartCoroutine(DisableBoxAfterDelay(2.0f));
                if (inTime >= needTime)
                {
                    itemAnimator.SetBool("Bang", true);
                    if (itemBox != null)
                    {
                        Destroy(itemBox);
                    }
                    BreakBox();
                }
            }
            else
            {
                itemAnimator.speed = 0f;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & layer) != 0)
            {
                StartCoroutine(SpawnItemBox(5.0f));
                Debug.Log("재생성중");
            }
        }

        IEnumerator DisableBoxAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            box.SetActive(false);
        }

        IEnumerator SpawnItemBox(float delay)
        {
            yield return new WaitForSeconds(delay);
            itemBox = Instantiate(itemBoxPrefab, transform.position, Quaternion.identity);
        }

        public void BreakBox()
        {
            int dropCount = Random.Range(minDropCount, maxDropCount + 1);

            for (int i = 0; i < dropCount; i++)
            {
                float totalProbability = 0f;
                foreach (float probability in dropItemProbabilities)
                {
                    totalProbability += probability;
                }

                float randomValue = Random.value * totalProbability;
                float cumulativeProbability = 0f;
                GameObject chosenItemPrefab = null;
                for (int j = 0; j < dropItemProbabilities.Length; j++)
                {
                    cumulativeProbability += dropItemProbabilities[j];
                    if (randomValue <= cumulativeProbability)
                    {
                        chosenItemPrefab = dropItemPrefabs[j];
                        break;
                    }
                }

                if (chosenItemPrefab == null)
                {
                    Debug.LogWarning("No item selected.");
                    continue;
                }

                Vector3 randomPosition = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                GameObject dropItem = Instantiate(chosenItemPrefab, randomPosition, Quaternion.identity);
                dropItem.SetActive(true);
            }
        }
    }
}
