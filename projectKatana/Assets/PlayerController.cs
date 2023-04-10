using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float runSpeed;
    [SerializeField] float attackMoveSpeed;
    [SerializeField] bool isRunning;
    [SerializeField] bool isAttacking;

    Enemy currentEnemy;

    [SerializeField] bool isDead;

    public bool canKillEnemy;

    private void Awake()
    {
        InputHandler.OnSwipe += OnSwipeInput;
    }

    void Update()
    {
        if (isRunning && !isDead)
        {
            transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        }
        if (isAttacking)
        {
            transform.Translate(Vector3.forward * attackMoveSpeed* Time.deltaTime);
        }
        HandleAnimation();

        // Debug input for PC
        if (Input.GetKeyDown(KeyCode.A))
        {
            isRunning = false;
            isAttacking = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out currentEnemy))
        {
            canKillEnemy = true;
        }
    }

    public void StartRunningAgain()
    {
        isAttacking = false;
        isRunning = true;
    }

    public void Die()
    {
        isRunning = false;
        isAttacking = false;
        anim.enabled = false;
    }

    private void HandleAnimation()
    {
        anim.SetBool("Run", isRunning);
        anim.SetBool("Attack", isAttacking);
        anim.SetBool("Die", isDead);
    }

    public void KillEnemy()
    {
        if (canKillEnemy)
        {
            currentEnemy?.Die();
        }
    }

    void OnSwipeInput(SwipeData data)
    {
        isRunning = false;
        isAttacking = true;
        

        // different swipes for different powerups
        switch (data.direction){
            case SwipeDirection.Up:
                Debug.Log("UP Swipe Detected!");

                break;
            case SwipeDirection.Down:
                Debug.Log("Down Swipe Detected!");

                break;
            case SwipeDirection.Left:
                Debug.Log("Left Swipe Detected!");

                break;
            case SwipeDirection.Right:
                Debug.Log("Right Swipe Detected!");

                break;
        }


    }

   
}
