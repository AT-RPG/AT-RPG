using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Store : MonoBehaviour
{
    public GameObject StoreUI;
    public GameObject buyPanel; // 구매 패널
    public GameObject sellPanel; // 판매 패널

    public void Start()
    {
        sellPanel.SetActive(false);
    }
    public void ToggleBuyPanel() // 구매 버튼 클릭 시
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
    }
    public void ToggleSellPanel() // 판매 버튼 클릭시 
    {
        
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
    }
    public void ExitButton() // 닫기 버튼 함수
    {
        StoreUI.SetActive(false);  
    }
}
