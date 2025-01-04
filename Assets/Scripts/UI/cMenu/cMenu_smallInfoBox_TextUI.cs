using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CoreData.CData.Player;
using static CoreData.CData;

public class CMenu_smallInfoBox_TextUI : MonoBehaviour
{
    public Text PlayerName;
    public Text LV;
    public Text HP;
    public Text Money;
    private void OnEnable()
    {
        PlayerName.text=Name.GetName();
        LV.text = LOVE.GetLOVE().ToString();
        HP.text= Health.GetStringHealth();
        Money.text= Player.Money.GetMoneyNum().ToString();
    }
}
