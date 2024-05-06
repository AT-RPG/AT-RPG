using System;
using System.Collections;
using System.Collections.Generic;
using AT_RPG;
using UnityEngine;

public class EventManager
{
    #region PlayerUI
    public Action ChangePotionEvent;
    #endregion

    #region InventoryUI
    #endregion

    #region SkillUI
    public Action<Sprite> ChangeSkillSpriteEvent;
    public Action<float> CheckSkillCooldownEvent;
    #endregion
}
