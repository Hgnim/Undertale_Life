using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FightUI_OpponentHeart : MonoBehaviour
{
    /*
    public float forceMagnitude = 50.0f; // �ƶ����Ĵ�С
    public float pushFrequency = 0.5f;   // ÿ����������һ��

    private Rigidbody2D rb; // �洢Rigidbody2Dʵ��

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // ��ȡ���
    }

    private void Update()
    {
        // ÿ��pushFrequency�봥��һ������
        if (Time.timeSinceLevelLoad % pushFrequency < Time.deltaTime)
        {
            // ʩ��һ����x���������������ֻ����X�ᣩ
            rb.AddForce(Vector2.right * forceMagnitude);
        }
    }
    */
    



    private Rigidbody2D rb;
    static public float speed = 1f;//�ٶ�

    private void Awake()
    {
        rb =gameObject. GetComponent<Rigidbody2D>(); // ��ȡ���
        {
            float cache = ((float)new System.Random().Next(0, (int)(speed * 10))/10);//����10��Ϊ�˼���һλС��
            rb.velocity = (new Vector2(cache, speed-cache));
        }
    }

    private void LateUpdate()
    {
        if (rb.velocity == Vector2.zero)
        {
            Vector2 cache= -lastDir;
            float cachef= ((float)new System.Random().Next(0, (int)(speed * 10)) / 10);//����10��Ϊ�˼���һλС��
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
