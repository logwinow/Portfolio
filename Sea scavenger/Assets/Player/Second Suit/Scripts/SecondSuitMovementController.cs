using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondSuitMovementController : PlayerMovementController
{
    [SerializeField]
    private float flySpeed = 1f;
    [SerializeField]
    private float gravitySpeed = 1f;
    [SerializeField]
    private float maxDownSpeed = 1f;
    [SerializeField]
    private BoxCollider2D bodyCollider;
    [SerializeField]
    private Transform groundPointTr;
    [SerializeField] private ParticleSystem _leftBootFX;
    [SerializeField] private ParticleSystem _rightBootFX;
    [SerializeField] private float _flyingAccelerationScaler = 1f;
    [SerializeField] private OxygenController _oxygenController;
    [SerializeField] private float _oxygenConsumptionScaler;
    [SerializeField] private SectionID _oxygenConsumptionParameterID;
    [SerializeField] private SectionID _flyingAccelerationParameterID;
    [SerializeField] private float _stepTime = 1f;
    [ReadOnly]
    [SerializeField] private float _oxygenConsumption;
    [ReadOnly]
    [SerializeField] private float _flyingAcceleration;
    
    private bool isFlying = false;
    private Vector2 overlapingBoxSize;
    private float _lastStepTime;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(groundPointTr.position, 
            new Vector3(bodyCollider.size.x, 2 * CHECK_DISTANCE));
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        
        overlapingBoxSize = new Vector2(bodyCollider.size.x, 2 * CHECK_DISTANCE);
    }

    private void Start()
    {
        _oxygenConsumption =
            _oxygenConsumptionScaler 
            / GameManager.Instance.GetParameter(_oxygenConsumptionParameterID).Value;
        _flyingAcceleration =
            GameManager.Instance.GetParameter(_flyingAccelerationParameterID).Value
            * _flyingAccelerationScaler;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !PlayerController.Instance.IsDead 
            && !IsBlocked)
        {
            speed.y = Mathf.Lerp(speed.y, flySpeed, Time.deltaTime * _flyingAcceleration);
            
            _oxygenController.Consumpt(_oxygenConsumption);

            if (isFlying)
                return;

            isFlying = true;

            PlayFX();
            
            PlayerController.Instance.AnimationController.DBAnimationController
                .SetBool("IsFlying", true);
            AudioManager.Instance.PlayJetpack();
        }
        else if (Input.GetKeyUp(KeyCode.Space) || IsBlocked)
        {
            isFlying = false;
            
            StopFX();
            
            PlayerController.Instance.AnimationController.DBAnimationController
                .SetBool("IsFlying", false);
            AudioManager.Instance.StopJetpack();
        }
    }

    private void FixedUpdate()
    {
        if (!AtDead() && !AtAttacked())
        {
            if (InputHorizontalSpeed() && !isFlying)
            {
                if (Time.time > _lastStepTime + _stepTime)
                {
                    AudioManager.Instance.PlayOneShot(AudioManager.WALK, true);
                    _lastStepTime = Time.time;
                }
            }

            if (!isFlying)
            {
                if (Physics2D.OverlapBox(groundPointTr.position, overlapingBoxSize, 
                    0, 1))
                {
                    if (PlayerController.Instance.AnimationController.DBAnimationController
                        .GetBool("IsFalling"))
                    {
                        speed.y = 0;
                        
                        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsFalling", false);
                        AudioManager.Instance.PlayOneShot(AudioManager.LANDED);
                    }
                }
                else
                {
                    speed.y = Mathf.Clamp(speed.y - gravitySpeed * Time.fixedDeltaTime,
                        -maxDownSpeed, Mathf.Infinity);
                    
                    PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsFalling", true);
                }
            }
        }

        rb.velocity = speed;
    }

    private void PlayFX()
    {
        _leftBootFX.Play();
        _rightBootFX.Play();
    }
    
    private void StopFX()
    {
        _leftBootFX.Stop();
        _rightBootFX.Stop();
    }

    public override void OnDead()
    {
        StopFX();
    }
}
