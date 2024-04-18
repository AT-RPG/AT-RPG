using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpwan : MonoBehaviour
{
    public Animator itemAnimator;
    public GameObject itemBox;
    public float needTime=3.0f;
    public LayerMask layer;
    public float inTime;
    public GameObject[] dropItemPrefabs; // 드롭될 아이템 프리팹 배열
    public float[] dropItemProbabilities; // 각 아이템의 드롭 확률 배열
    public int minDropCount = 1; // 최소 드롭 개수
    public int maxDropCount = 10; // 최대 드롭 개수
    // Start is called before the first frame update
    void Start()
    {
        itemAnimator.speed = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & layer) != 0)
        {
            itemAnimator.speed = 1f;
            inTime += Time.deltaTime;
            if(inTime>=needTime)
            {
                itemAnimator.SetBool("Bang", true);
                Destroy(itemBox);
                BreakBox();
            }
        }
        else
        {
            itemAnimator.speed = 0f;
        }
    }
    public void BreakBox()
    {
        // 드롭될 아이템 개수를 랜덤으로 설정합니다.
        int dropCount = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            // 랜덤으로 아이템을 선택하기 위해 확률 합을 구합니다.
            float totalProbability = 0f;
            foreach (float probability in dropItemProbabilities)
            {
                totalProbability += probability;
            }

            // 랜덤으로 아이템을 선택합니다.
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

            // 선택된 아이템이 없다면 continue하여 다음 아이템을 선택합니다.
            if (chosenItemPrefab == null)
            {
                Debug.LogWarning("No item selected.");
                continue;
            }

            // 선택된 아이템을 생성하고 활성화시킵니다.
            GameObject dropItem = Instantiate(chosenItemPrefab, transform.position, Quaternion.identity);
            dropItem.SetActive(true);
        }
    }
}
