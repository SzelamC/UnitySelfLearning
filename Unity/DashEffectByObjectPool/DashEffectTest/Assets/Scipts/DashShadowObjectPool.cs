using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashShadowObjectPool : MonoBehaviour
{
    public static DashShadowObjectPool instance;
    
    public GameObject shadowPrefab;

    public int shadowNum;

    private Queue<GameObject> availableObjects = new Queue<GameObject>(); 

    void Awake()
    {
        //Create Instance
        instance = this;

        //Initialize Object Pool
        FillPool();
    }

    public void FillPool()
    {
        //用for loop生成一定數目嘅shadowPrefab，再入Object Pool
        for(int i = 0; i < shadowNum; i++)
        {
            var dashShadow = Instantiate(shadowPrefab);
            dashShadow.transform.SetParent(transform);

            ReturnPool(dashShadow);
        }
    }

    public void ReturnPool(GameObject gameObject)
    {
        //Set shadowPrefab做唔active，再入queue(隊列)(queue.Enqueue方法)
        gameObject.SetActive(false);
        availableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        //防止Object Pool物件數量唔夠
        if(availableObjects.Count == 0)
        {
            FillPool();
        }
        //係queue拎一個出嚟
        var outDashShadow = availableObjects.Dequeue();
        outDashShadow.SetActive(true);

        return outDashShadow;
    }
}
