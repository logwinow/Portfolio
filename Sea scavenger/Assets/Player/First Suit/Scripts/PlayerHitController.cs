using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitController : MonoBehaviour
{
    [SerializeField]
    private float pushHorForce;
    [SerializeField]
    private float pushVertForce;
    [SerializeField] private PlayerMovementController playerMovement;
    [SerializeField] private HealthController _healthController;

    public void Hit(int damage, Vector3 origin)
    {
        Push(origin);

        _healthController.ChangeHealthAt(-damage);

        AudioManager.Instance.PlayOneShot(AudioManager.ATTACKED);
        
        if (_healthController.Health <= 0)
        {
            PlayerController.Instance.Kill();
            AudioManager.Instance.PlayOneShot(AudioManager.DEATH_BY_GET_HURT);
        }
    }

    private void Push(Vector3 origin)
    {
        Vector3 _push = transform.position - origin;
        _push.x = pushHorForce * Mathf.Sign(_push.x);
        _push.y = pushVertForce;

        playerMovement.Attack(_push);
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsAttacked", true);
    }
}
