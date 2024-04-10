using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Store : MonoBehaviour
{
    public GameObject buyPanel;
    public GameObject sellPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleBuyPanel()
    {
        // 패널이 활성화되어 있는지 확인하고, 상태에 따라 반대로 설정
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
    }
    public void ToggleSellPanel()
    {
        // 패널이 활성화되어 있는지 확인하고, 상태에 따라 반대로 설정
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
    }
    public void ExitButton()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
    }
}
