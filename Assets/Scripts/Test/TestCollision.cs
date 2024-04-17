using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private Collider testCollider;

    // Start is called before the first frame update
    void Start()
    {
        testCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
