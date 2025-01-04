using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FightUI_OpponentHeart : MonoBehaviour
{
    /*
    public float forceMagnitude = 50.0f; // 推动力的大小
    public float pushFrequency = 0.5f;   // 每隔多少秒推一次

    private Rigidbody2D rb; // 存储Rigidbody2D实例

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取组件
    }

    private void Update()
    {
        // 每隔pushFrequency秒触发一次推送
        if (Time.timeSinceLevelLoad % pushFrequency < Time.deltaTime)
        {
            // 施加一个沿x轴的力（假设我们只关心X轴）
            rb.AddForce(Vector2.right * forceMagnitude);
        }
    }
    */
    



    private Rigidbody2D rb;
    static public float speed = 1f;//速度

    private void Awake()
    {
        rb =gameObject. GetComponent<Rigidbody2D>(); // 获取组件
        {
            float cache = ((float)new System.Random().Next(0, (int)(speed * 10))/10);//乘以10是为了兼容一位小数
            rb.velocity = (new Vector2(cache, speed-cache));
        }
    }

    private void LateUpdate()
    {
        if (rb.velocity == Vector2.zero)
        {
            Vector2 cache= -lastDir;
            float cachef= ((float)new System.Random().Next(0, (int)(speed * 10)) / 10);//乘以10是为了兼容一位小数
            if (cache.x != 0)
            {
                if (cache.x < 0)
                {
                    cache.x = -cachef;
                    cache.y = -(speed - cachef);
                }
                else
                {
                    cache.x = cachef;
                    cache.y = (speed - cachef);
                }
            }
            else if (cache.y != 0)
            {
                if (cache.y < 0)
                {
                    cache.y = -cachef;
                    cache.x = -(speed - cachef);
                }
                else
                {
                    cache.y = cachef;
                    cache.x = (speed - cachef);
                }               
            }
            rb.velocity = cache;
        }            
        else
        lastDir = rb.velocity;
    }
    Vector2 lastDir;
    void OnCollisionEnter2D(Collision2D collision)        
    {
        if (collision.transform.name=="FightUI_FightBox")
        {
            rb.velocity = Vector2.Reflect(lastDir, collision.contacts[0].normal).normalized * lastDir.magnitude;
            //Debug.Log(rb.velocity);
        }
    }
}
