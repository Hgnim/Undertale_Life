using UnityEngine;
using static CoreData.CData.Objects;
using static CoreData.ToolClass;

public class Bedroom_dynamicObjLoad : MonoBehaviour
{
    void Start()
    {
        GameObject tmp, tmp2;
        string alias=null;
        Vector2 loc=new();
        tmp = Resources.Load("Prefabs/Object/dynamicCode_canPickedObject") as GameObject;
        for (int i = 0; i < 2; i++)
       {
            switch (i)
            {
                case 0:
                    if (CPObj_hoe.Exist)
                    {
                        alias=ObjTagFind(CPObj_hoe.defTag,CPObjTagHead.alias);
                        loc= CPObj_hoe.ObjectLocation;
                        break;
                    }
                    else goto end;
                case 1:
                    if (CPObj_wheatSeedBag.Exist)
                    {
                        alias= ObjTagFind(CPObj_wheatSeedBag.defTag,CPObjTagHead.alias);
                        loc = CPObj_wheatSeedBag.ObjectLocation;
                        break;
                    }
                    else goto end;
            }            
                tmp2 = GameObject.Instantiate(tmp);
                tmp2.name = alias;
                tmp2.transform.SetParent(gameObject.transform, false);
            tmp2.transform.localPosition = loc;
                tmp2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Object/" + alias);
            foreach(BoxCollider2D bc in tmp2.GetComponents<BoxCollider2D>())
                bc.size = tmp2.GetComponent<SpriteRenderer>().bounds.size;
            tmp2.SetActive(true);
//GameObject.Destroy(GameObject.Find("obj_hoe(Clone)"));
end:;
        }
    }
}
