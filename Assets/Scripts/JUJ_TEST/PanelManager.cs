using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject BuyPOPupPanel;
    public GameObject BuyYesPanel;
    public GameObject SellPOPupPanel;
    public GameObject SellYesPanel;

    public void ButYesButton()
    {
        BuyYesPanel.SetActive(true);
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
