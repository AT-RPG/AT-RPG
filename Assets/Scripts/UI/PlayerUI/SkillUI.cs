using System.Collections;
using System.Collections.Generic;
using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private Image skillBGImage;
    [SerializeField] private Image skillCooldownImage;

    private void Start() 
    {
        GameManager.Event.ChangeSkillSpriteEvent += ChangeSkillSprite;
        GameManager.Event.CheckSkillCooldownEvent += CheckSkillCooldown;
    }

    public void ChangeSkillSprite(Sprite _skillSprite)
    {
        if(_skillSprite == null)
        {
            skillBGImage.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            skillBGImage.transform.parent.gameObject.SetActive(true);
            skillBGImage.sprite = _skillSprite;
            skillCooldownImage.sprite = _skillSprite;
        }
    }

    public void CheckSkillCooldown(float _skillCooldown)
    {
        skillCooldownImage.fillAmount = _skillCooldown;
    }
}
