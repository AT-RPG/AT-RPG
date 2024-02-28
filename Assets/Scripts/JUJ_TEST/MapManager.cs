using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] public GameObject treePrefab; // ���� ������
    [SerializeField] public GameObject rockPrefab; // �� ������
    public float spawnRadius = 10f; // ���� ����
    public int numberOfObjects = 20; // ���� ����

    void Start()
    {
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < numberOfObjects; i++) // ���� ������ŭ
        {
            Vector3 randomPosition = GetRandomPosition();
            GameObject selectedPrefab = Random.Range(0f, 1f) > 0.5f ? treePrefab : rockPrefab; // ������ �����Ͽ� 50���� Ȯ���� ����
            Instantiate(selectedPrefab, randomPosition, Quaternion.identity); //���� ��ġ�� ȸ���� �������� �ʰ� ����
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius; //������ ������ŭ �������� ����
        Vector3 randomPosition = new Vector3(randomCircle.x, 0f, randomCircle.y) + transform.position;
        // 2D ���͸� 3D�� ��ȯ�մϴ�. Y �� ���� 0���� �����Ǿ� �־�, ������ ��ġ�� 3D �������� ��� �� ��ġ�ϰ� �˴ϴ�. �� ��, ���� ��ũ��Ʈ�� ����� ���� ������Ʈ�� ��ġ(transform.position)�� ���Ͽ� ���� 3D ������ ��ġ�� ��ȯ�մϴ�.
        return randomPosition; // 3D�� ��ȯ
    }
}
