using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CoreData.CData;

public class FightUI_FightBox_FirskWeapon : MonoBehaviour
{
    bool hurt = false;//每个武器只能伤害对手一遍
    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.name== "FightUI_OpponentHeart" && hurt==false)
        {
            hurt = true;
            FightUI.ChangeOpponentHealth(-(Player.Attack.GetAllAT()));
        }
    }
}
