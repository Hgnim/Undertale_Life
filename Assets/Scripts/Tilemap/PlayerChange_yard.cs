using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerChange_yard : MonoBehaviour
{
    private static PlayerChange_yard instance;
    public static PlayerChange_yard Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public enum TileList
    {
        none,
        dirt,
        wheat
    }
    public TileBase tile_dirt; 
    public TileBase[] tile_wheat;
    public Tilemap thisTilemap;

    public void TilemapDraw(TileList tile,float x,float y,float z=0)
    {
        TileBase tb;
        Vector3Int pos;
        float[] localScale=new float[2] { 
            gameObject.transform.localScale.x, 
            gameObject.transform.localScale.y
        };//瓦片地图缩放数据
        if (x < 0)
            x -= 0.2f;
            y -= 0.2f;
        x /= localScale[0];
        y /= localScale[1];

        pos=new((int)x, Convert.ToInt32(y));
        if (
            (pos.y >= -20 && pos.y <= 12 && pos.x >= -12 && pos.x <= 22) &&//地图边缘不可绘制
            !(pos.y<=2 && pos.y>=-3 && pos.x>=-5 && pos.x<=4) //房子周边不可绘制
            )
        {        
            /*Debug.Log(localScale);
            Debug.Log(pos);*/

            switch (tile)
            {
                case TileList.dirt: tb = tile_dirt; break;
                case TileList.wheat:
                    if (thisTilemap.GetTile(pos) == tile_dirt)
                    {
                        pos.z = 1;//小麦作物等所在的Z坐标为1
                        tb = tile_wheat[0];
                    }
                    else
                        tb = null;
                    break;
                default: tb = null; break;
            }
            if (tb != null)
            {
                thisTilemap.SetTile(pos, tb);
            }
        }
    }
}
