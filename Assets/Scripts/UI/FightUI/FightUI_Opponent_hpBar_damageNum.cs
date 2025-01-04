using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightUI_Opponent_hpBar_damageNum : MonoBehaviour
{
    private void OnEnable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(1, 1);
    }
    private void Update()
    {
        if (Time.timeSinceLevelLoad % 5 < Time.deltaTime)//5ÃëºóÐ¶ÔØ×Ô¼º
            GameObject.Destroy(gameObject);
    }
}
