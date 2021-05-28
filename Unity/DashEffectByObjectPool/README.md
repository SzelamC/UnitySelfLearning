# 物件池實現

**物件池(Object Pool)**，係一個常用嘅Design Pattern，避免重覆建立同銷毀一堆常用嘅object，算係Game Optimization嘅一種。

* 呢個scipts係掛係Dash Shadow Prefab到，調整Dash Shadow Prefab嘅樣，eg.顏色同埋透明度，同埋要佢顯示完返返個Object Pool(Queue)到。
* Class 解釋
    1. OnEnable()係一個當GameObject係Active個陣就會自己用，所以我地係入面Initialize同埋Get晒所有會用嘅Component同parameter
    2. 透明度同顏色係Update()到Set，因為要佢有漸變效果，我地係每次Update都將佢透明度降低
    3. 個Dash Shadow顯示完之後，我地要佢返入去個Object Pool到，啫係我地create嘅queue或者list
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashShadow : MonoBehaviour
{
    private Transform playerTrans;

    private SpriteRenderer shadowSpriteRen;
    private SpriteRenderer playerSpriteRen;

    private Color color;

    [Header("時間參數")]
    public float _lastingTime;
    public float _showingTime;

    [Header("透明度參數")]
    [SerializeField]private float _alpha;
    public float _defaultAlpha;
    public float _alphaMultiplier;

    private void OnEnable()
    {
        //拎到相關Components
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        shadowSpriteRen = GetComponent<SpriteRenderer>();
        playerSpriteRen = playerTrans.GetComponent<SpriteRenderer>();
        
        //Initialize 透明度
        _alpha = _defaultAlpha;

        //拎到player嘅sprite
        shadowSpriteRen.sprite = playerSpriteRen.sprite;

        //Set返個transform同player一樣
        transform.position = playerTrans.position;
        transform.rotation = playerTrans.rotation;
        transform.localScale = playerTrans.localScale;

        //開始顯示嘅時間
        _showingTime = Time.time;
    }
    void Update()
    {
        //降低透明度
        _alpha *= _alphaMultiplier;
        
        //殘影顏色
        color = new Color(0.5f, 0.5f, 1, _alpha);
        shadowSpriteRen.color = color;

        if(Time.time >= _showingTime + _lastingTime)
        {
            //返回Object Pool
            DashShadowObjectPool.instance.ReturnPool(this.gameObject);
        }
    }
}
```
</br>

* 我地有Dash Shadow嘅Prefab，都要有一個Object Pool儲住佢，跟住我地要create一個Object Pool。
* Class解釋
    1. **Awake()** 我地create呢個class嘅instance，令到其他class要access呢個class嘅時候容易啲
    2. **FillPool()** 我地create一個Dash Shadow嘅prefab，佢嘅父類就係呢個Class嘅GameObject，啫係我地嘅Object Pool，最尾再用**ReturnPool()**
    3. **ReturnPool()** 我地set左個Dash Shadow唔係active先，同埋令佢入個Queue
    4. 有入就有出，當我地要佢顯示個陣，我地係個Object Pool到拎，**GetFromPool()** set個Dash Shadow係active同埋要係queue到拎佢出嚟。

```cs
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
```

* 最尾係 **PlayerController** 到加返Dash嘅code
  * **Update()** 到call **ReadyToDash()** 去check佢係咪有㩒dash key (自己set)
  * **FixedUpdate()** 到call **Dash()**
```cs
 void ReadyToDash()
    {
        _isDashing = true;
        
        _dashTimeLeft = _dashLastingTime;

        _lastDash = Time.time;
    }

    void Dash()
    {
        if(_isDashing)
        {
            if(_dashTimeLeft > 0)
            {
                playerRigidbody.velocity = new Vector2(_dashSpeed * _faceDirection, playerRigidbody.velocity.y);

                _dashTimeLeft -= Time.deltaTime;

                DashShadowObjectPool.instance.GetFromPool();
            }
            if(_dashTimeLeft <= 0)
            {
                _isDashing = false;
            }
        }
    }
```