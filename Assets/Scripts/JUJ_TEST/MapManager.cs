using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] public GameObject treePrefab; // 나무 프리펩
    [SerializeField] public GameObject rockPrefab; // 돌 프리펩
    public float spawnRadius = 10f; // 스폰 범위
    public int numberOfObjects = 20; // 스폰 갯수

    void Start()
    {
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < numberOfObjects; i++) // 스폰 갯수만큼
        {
            Vector3 randomPosition = GetRandomPosition();
            GameObject selectedPrefab = Random.Range(0f, 1f) > 0.5f ? treePrefab : rockPrefab; // 난수를 생성하여 50퍼의 확률로 생성
            Instantiate(selectedPrefab, randomPosition, Quaternion.identity); //랜덤 위치에 회전을 적용하지 않고 생성
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius; //지정한 범위만큼 무작위로 생성
        Vector3 randomPosition = new Vector3(randomCircle.x, 0f, randomCircle.y) + transform.position;
        // 2D 벡터를 3D로 변환합니다. Y 축 값은 0으로 설정되어 있어, 생성된 위치가 3D 공간에서 평면 상에 위치하게 됩니다. 그 후, 현재 스크립트가 연결된 게임 오브젝트의 위치(transform.position)를 더하여 원래 3D 공간의 위치로 변환합니다.
        return randomPosition; // 3D로 반환
    }
}
