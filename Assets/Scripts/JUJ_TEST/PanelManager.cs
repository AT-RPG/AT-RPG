using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PanelManager : MonoBehaviour
{
    public GameObject BuyPOPupPanel;
    public GameObject BuyYesPanel;
    public GameObject SellPOPupPanel;
    public GameObject SellYesPanel;
    public GameObject ErrorPanel;
    public TextMeshProUGUI goldText;
    int currentGold;
    public void ButYesButton()
    {
        BuyYesPanel.SetActive(true);
        currentGold = int.Parse(goldText.text);
        currentGold -= 300;

        // 현재 골드를 다시 텍스트로 설정합니다.
        goldText.text = currentGold.ToString();
    }
    public void ErrorPanelExitButton()
    {
        ErrorPanel.SetActive(false);
    }
    public void BuyNoButton()
    {
        BuyPOPupPanel.SetActive(false);
    }
    public void BuyExitButton()
    {
        BuyPOPupPanel.SetActive(false);
        BuyYesPanel.SetActive(false);
    }
    public void SellYesButton()
    {
        SellYesPanel.SetActive(true);
        currentGold = int.Parse(goldText.text);
        currentGold += 300;

        // 현재 골드를 다시 텍스트로 설정합니다.
        goldText.text = currentGold.ToString();
    }
    public void SellNoButton()
    {
        SellPOPupPanel.SetActive(false);
    }
    public void SellExitButton()
    {
        SellPOPupPanel.SetActive(false);
        SellYesPanel.SetActive(false);
    }
}
