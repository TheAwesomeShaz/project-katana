using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator anim;

    Collider otherCollider;

    public void Die()
    {
        anim.enabled = false;
    }

    public void KillPlayer()
    {
        if (otherCollider.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.Die();
        }
        anim.SetBool("Attack", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            anim.SetBool("Attack", true);
            otherCollider = other;
        }
    }
}
