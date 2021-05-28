using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Component
    private Rigidbody2D playerRigidbody;
    private BoxCollider2D playerBoxCollider;
    public Image cdImage;

    //Move Parameter
    public float _runSpeed; 
    
    //Jump Parameter
    public float _jumpForce; 
    public int _maxJumpNum;

    //Dash Parameter
    public float _dashLastingTime;
    public float _dashSpeed;
    public float _dashCoolDown;

    //Move
    private float _faceDirection;
    private float _moveDirection;

    //Jump
    [SerializeField] private int _jumpCount;
    private bool _isJumpPress;
    
    //Dash
    private float _lastDash = -10f;
    private bool _isDashing;
    private float _dashTimeLeft;
    
    
    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
    }


    void Update()
    {   
        GroundCheck();
        _faceDirection = Input.GetAxisRaw("Horizontal");
        _moveDirection = Input.GetAxis("Horizontal");
        if(_jumpCount > 0 &&Input.GetKeyDown(KeyCode.Space))
        {
            _isJumpPress = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if(Time.time >= (_lastDash + _dashCoolDown))
            {
                ReadyToDash();
            }
        }
    }

    void FixedUpdate() 
    {
        Dash();
        if(_isDashing) return;
        Move();
        Jump();
        
    }

    void Move()
    {
        playerRigidbody.velocity = new Vector2(_runSpeed * _moveDirection, playerRigidbody.velocity.y);
    }

    void Jump()
    {
        if(_isJumpPress)
        {
            playerRigidbody.velocity = Vector2.up * _jumpForce;
            _isJumpPress = false;
            _jumpCount--;
        }
    }

    void GroundCheck()
    {
        if(playerBoxCollider.IsTouchingLayers(LayerMask.GetMask("Floor")))
        {
            _jumpCount = _maxJumpNum;
        }
    }
    
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
}
