using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Patterns;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField]
    private PlayerAnimationController animContr;
    [SerializeField]
    private PlayerInteractionController interContr;
    [SerializeField]
    private PlayerMovementController movementContr;
    [SerializeField]
    private PlayerHitController hitContr;
    [SerializeField]
    private PlayerDrillController drillContr;
    [SerializeField]
    private Inventory inventory;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private HealthController _healthController;

    private bool isDead = false;
    private Rigidbody2D rb;

    public Transform CameraTarget => _cameraTarget;
    public bool IsDead => isDead;

    public PlayerAnimationController AnimationController => animContr;
    public PlayerInteractionController InteractionController => interContr;
    public PlayerHitController HitController => hitContr;
    public Inventory Inventory => inventory;
    public PlayerMovementController MovementController => movementContr;

    protected override void Init()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    public void Kill()
    {
        isDead = true;
        rb.gravityScale = 0;
        
        movementContr.OnDead();
        
        interContr.enabled = false;

        if (drillContr != null)
        {
            drillContr.StopDrilling();
            drillContr.enabled = false;
        }

        foreach (var c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }
        
        _healthController.ChangeHealthAt(int.MinValue);
        
        (SceneManager.Instance.PauseMenuController as DivingMenuController)!.OpenDeadMenu();
        
        animContr.DBAnimationController.SetBool("IsDead", true);
    }

    public void DisableMovement()
    {
        movementContr.IsBlocked = true;
    }
    public void EnableMovement()
    {
        movementContr.IsBlocked = false;
    }

    public void DisableInteractions()
    {
        interContr.CollectorController.enabled = false;
        interContr.enabled = false;
    }

    public void EnableInteractions()
    {
        interContr.CollectorController.enabled = true;
        interContr.enabled = true;
    }
}
