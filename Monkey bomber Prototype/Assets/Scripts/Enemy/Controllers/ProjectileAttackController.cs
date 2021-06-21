using System.Collections;
using System.Collections.Generic;
using Custom.Utility;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileAttackController : MonoBehaviour
{
    [SerializeField] protected Transform shootPointTransform;

    protected AttackerItemData attackerItemData;

    public UnityEvent onProjectileAttackActivated;
    public UnityEvent onProjectileAttackDeactivated;

    public virtual void InitializeAttack(AttackerItemData attackerItemData)
    {   
        this.attackerItemData = attackerItemData;

        onProjectileAttackActivated?.Invoke();
    }

    public void Throw(Vector3 speed)
    {
        if (attackerItemData == null)
            return;
        
        var attackerCollectible = ItemsManager.Instance.GetFromPool(attackerItemData); 
        var attackerItem = attackerCollectible.GetComponent<GrenadeProjectile>();
        
        attackerCollectible.Deactive();
        
        attackerItem.transform.position = shootPointTransform.position;

        attackerItem.Throw(RecalculateSpeed(speed));
        
        EndAttack();
    }

    protected Vector3 RecalculateSpeed(Vector3 speed)
    {
        return speed + (transform.position - shootPointTransform.position).SetY(0) / 4;
    }

    protected virtual void EndAttack()
    {
        attackerItemData = null;
        
        onProjectileAttackDeactivated?.Invoke();
    }
}
