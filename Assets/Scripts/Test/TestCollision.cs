using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private Collider testCollider;

    // Start is called before the first frame update
    void Start()
    {
        testCollider = GetComponent<Collider>();

        Debug.Log(testCollider.bounds.extents);

        transform.Rotate(Vector3.forward, 40f);

        Debug.Log(testCollider.bounds.extents);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(testCollider.bounds.extents);
    }
}
