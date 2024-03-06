using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private ResourceReference<GameObject> testObj;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(testObj.Resource);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
