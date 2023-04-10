using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void Die()
    {
        anim.enabled = false;
    }

    void Attack()
    {

    }
}
