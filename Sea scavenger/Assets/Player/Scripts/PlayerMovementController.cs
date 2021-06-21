using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementController : MonoBehaviour
{
    protected const float CHECK_DISTANCE = 0.1f;
    
    [SerializeField] private float attackDurationTime = 0.3f;
    [SerializeField] private float moveSpeed = 1.5f;
    
    protected Rigidbody2D rb;
    protected Vector2 speed = Vector2.zero;
    private bool isAttacked;
    private float attackStartTime;
    
    public bool IsBlocked { get; set; }
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected bool AtDead()
    {
        if (!PlayerController.Instance.IsDead)
            return false;
        
        rb.velocity = speed;
        speed = Vector3.Lerp(speed, Vector3.zero, 2 * Time.fixedDeltaTime);
        
        return true;
    }

    protected bool AtAttacked()
    {
        if (!isAttacked)
            return false;
        
        if (Time.time - attackStartTime >= attackDurationTime)
        {
            isAttacked = false;
            speed.y = 0;
        }

        return true;
    }
    
    public void Attack(Vector3 force)
    {
        isAttacked = true;
        attackStartTime = Time.time;

        speed = force;
    }

    protected bool InputHorizontalSpeed()
    {
        if (!IsBlocked)
        {
            speed.x = Input.GetAxisRaw("Horizontal") * moveSpeed;

            if (speed.x > 0)
                PlayerController.Instance.AnimationController.DBAnimationController.FlipX(false);
            else if (speed.x < 0)
                PlayerController.Instance.AnimationController.DBAnimationController.FlipX(true);            
        }
        else
        {
            speed.x = 0;
        }

        PlayerController.Instance.AnimationController.DBAnimationController
            .SetBool("IsMoving", speed.x != 0);
        
        return speed.x != 0;
    }

    public virtual void OnDead() { }
}
