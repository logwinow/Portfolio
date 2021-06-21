using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyJellyfishAttackController : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        PlayerController.Instance.HitController.Hit(damage, transform.position);
        AudioManager.Instance.PlayOneShot(AudioManager.JELLYFISH_ATTACK);
    }
}
