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
