using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CoreData.CData;
using static CoreData.CData.People;
using static CoreData.ToolClass;

public class FightUI_FightBox_OpponentWeapon : MonoBehaviour
{
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();

        myID =int.Parse( name.Split('_')[0]);
        moveID = FightUI.OpponentAttackData.weaponMoveID[myID];        
    }
    int myID;
    int[] moveID;

    Rigidbody2D rb;
    private void Update()
    {
        if (FightUI.opponentID.Length == 1)
        {
            switch (FightUI.opponentID[0])
            {
                case ID.debugDummy:
                    switch (moveID[0])
                    {
                        case 0:
                            switch (moveID[1])
                            {
                                case 0:
                                    if (rb.velocity == Vector2.zero)
                                        rb.velocity = new Vector2(0, -1.5f);
                                    if(transform.localPosition.y<-3)
                                        KillMe();
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
    bool hurt = false;//每个武器只能伤害对手一遍
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "FightUI_Heart" && hurt == false && FightUI.friskINVState==false)
        {
            hurt = true;
            FightUI.ChangeFriskHealth(int.Parse( ObjTagFind(FightUI.opponentTag[0],TagHead.attack))*-1);
            KillMe();
        }
    }
    /// <summary>
    /// 自我销毁，自杀
    /// </summary>
    void KillMe()
    {
        GameObject.Destroy(FightUI.OpponentAttackData.weapon[myID]);
        FightUI.OpponentAttackData.weapon[myID] = null;
        FightUI.OpponentAttackData.weaponMoveID[myID] = null;      
    }
}
