using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSuitMovementController : PlayerMovementController
{
    
    [SerializeField]
    private float jumpSpeed = 1f;
    [SerializeField]
    private float gravitySpeed = 1f;
    [SerializeField]
    private float maxDownSpeed = 1f;
    [SerializeField]
    private BoxCollider2D bodyCollider;
    [SerializeField]
    private Transform groundPointTr;
    [SerializeField]
    private Transform ceilingPointTr;
    [SerializeField] private float _stepTime;
    [SerializeField] private string _moveSoundName;
    [SerializeField] private string _jumpSoundName;
    [SerializeField] private string _landSoundName;
    
    private bool isFlying = false;
    private Vector2 overlapingBoxSize;
    private float _lastStepTime;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(groundPointTr.position, new Vector3(bodyCollider.size.x, 2 * CHECK_DISTANCE));
        Gizmos.DrawWireCube(ceilingPointTr.position, new Vector3(bodyCollider.size.x, 2 * CHECK_DISTANCE));
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        
        overlapingBoxSize = new Vector2(bodyCollider.size.x, 2 * CHECK_DISTANCE);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFlying && !PlayerController.Instance.IsDead 
            && !IsBlocked)
        {
            speed.y = jumpSpeed;

            isFlying = true;

            PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsJumping", true);
            AudioManager.Instance.PlayOneShot(_jumpSoundName, true);
        }
    }

    private void FixedUpdate()
    {
        if (!AtAttacked() && !AtDead())
        {
            if (InputHorizontalSpeed() && !isFlying)
            {
                if (Time.time > _lastStepTime + _stepTime)
                {
                    AudioManager.Instance.PlayOneShot(_moveSoundName, true);
                    _lastStepTime = Time.time;
                }
            }

            if (Physics2D.OverlapBox(groundPointTr.position, overlapingBoxSize, 0, 1))
            {
                if (isFlying)
                {
                    if (PlayerController.Instance.AnimationController.DBAnimationController.GetBool("IsFalling"))
                    {
                        isFlying = false;
                        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsFalling", false);
                        AudioManager.Instance.PlayOneShot(_landSoundName);
                    }
                }
                else
                {
                    speed.y = -1;
                }
            }
            else
            {
                speed.y = Mathf.Clamp(
                    speed.y - gravitySpeed * Time.fixedDeltaTime,
                    -maxDownSpeed, Mathf.Infinity);

                if (speed.y > 0 && Physics2D.OverlapBox(ceilingPointTr.position, overlapingBoxSize, 0, 1))
                {
                    speed.y = 0;
                    PlayerController.Instance.AnimationController.DBAnimationController
                        .SetBool("IsJumping", false);
                }

                if (!isFlying)
                {
                    isFlying = true;
                    PlayerController.Instance.AnimationController.DBAnimationController
                        .SetBool("IsFalling", true);
                }
            }
        }

        rb.velocity = speed;
    }
}
