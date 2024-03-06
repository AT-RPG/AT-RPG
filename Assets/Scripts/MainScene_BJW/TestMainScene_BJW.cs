using AT_RPG.Manager;
using UnityEngine;

public class TestMainScene_BJW : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var resource in ResourceManager.Instance.GetAll())
        {
            Instantiate(resource);
        }
    }
}
