using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogAttackController : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Instance.HitController.Hit(damage, transform.position);
            AudioManager.Instance.PlayOneShot(AudioManager.HEDGEHOG_ATTACK);
        }
    }
}
