using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static CoreData.CData;
using static CoreData.CData.Player;
using static CoreData.ToolClass;

public class CMenu_InfoBox_stateUI : MonoBehaviour
{
   public GameObject cMenu_InfoBox_stateUI_handObj;
   public  GameObject cMenu_InfoBox_stateUI_bodyObj;
    public Text PlayerName;
    public Text HP;
    public Text Money;
    public Text LOVE;
    public Text EXP;
    public Text EXP_Need;
    public Text AT;
    public Text DF;
    private void OnEnable()
    {
        cMenu_InfoBox_stateUI_handObj=gameObject.transform.Find("cMenu_InfoBox_stateUI_handObj").gameObject;
        cMenu_InfoBox_stateUI_bodyObj = gameObject.transform.Find("cMenu_InfoBox_stateUI_bodyObj").gameObject;
        if(MainItem.GetItem(MainItem.PickedLoc.hand)!=Objects.CanPickedObject.none)
        cMenu_InfoBox_stateUI_handObj.GetComponent<Text>().text= 
            GameLang.GetString(ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.hand),Objects.CPObjTagHead. langID))
                .Replace(" ", "\u00A0");
        if(MainItem.GetItem(MainItem.PickedLoc.body)!=Objects.CanPickedObject.none)
        cMenu_InfoBox_stateUI_bodyObj.GetComponent<Text>().text=
            GameLang.GetString(ObjTagFind(MainItem.GetItemTag(MainItem.PickedLoc.body), Objects.CPObjTagHead.langID))
                .Replace(" ", "\u00A0");

        PlayerName.text = Name.GetName();
        HP.text=Health.GetStringHealth();
        Money.text=Player.Money.GetMoneyNum().ToString();
        LOVE.text = Player.LOVE.GetLOVE().ToString();
        EXP.text=Player.LOVE.GetEXP().ToString();   
        EXP_Need.text=Player.LOVE.GetNextLvNeedExp().ToString();
        AT.text=Attack.GetStringAT();
        DF.text=Player.Defense.GetStringDF();
    }
    bool exitWaitLock = false;
    void Update()
    {
        if (GameKey.CancelKeyClick() && !exitWaitLock)
        {
            exitWaitLock = true;
            gameObject.SetActive(false);
            Thread thread = new(() =>
            {
                Thread.Sleep(100);
                exitWaitLock = false;
            });
            thread.Start();
        }
    }
}
