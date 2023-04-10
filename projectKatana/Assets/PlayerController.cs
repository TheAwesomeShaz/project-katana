using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Collider capsuleCollider;
    [SerializeField] float runSpeed;
    [SerializeField] float attackMoveSpeed;
    [SerializeField] float attackCoolDown = 0.5f;
    [SerializeField] bool isRunning;
    [SerializeField] bool isAttacking;
    [SerializeField] bool canAttack;
    [SerializeField] float stopEmissionDelay;

    [SerializeField] Transform smokeFXPos;

    [SerializeField] ParticleSystem fireTrail;

    [SerializeField] GameObject fireExplosionFX;
    [SerializeField] GameObject stepFX;

    Enemy currentEnemy;

    [SerializeField] bool isDead;

    public bool canKillEnemy;

    private void Awake()
    {
        InputHandler.OnSwipe += OnSwipeInput;
        
        StopEmission();

    }
    private void Start()
    {
        canAttack = true;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        StopEmission();
        isRunning = false;
        isAttacking = false;
        anim.enabled = false;
        capsuleCollider.enabled = false;
    }

    public void CreateSmokeFX() { 
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
            GameObject fireFX = Instantiate(fireExplosionFX,currentEnemy.transform,false);
            Destroy(fireFX,2f);
        }
    }

    void OnSwipeInput(SwipeData data)
    {
        if (!isDead && canAttack)
        {

        isRunning = false;
        isAttacking = true;
        canAttack = false;

        StartCoroutine(SetAttackTrueAfterTime(attackCoolDown));

        // different swipes for different powerups
        switch (data.direction){
            case SwipeDirection.Up:
                //Debug.Log("UP Swipe Detected!");

                break;
            case SwipeDirection.Down:
                //Debug.Log("Down Swipe Detected!");

                break;
            case SwipeDirection.Left:
                //Debug.Log("Left Swipe Detected!");

                break;
            case SwipeDirection.Right:
                    //Debug.Log("Right Swipe Detected!");
                    StartEmission();

                break;
        }
        }   


    }

    public void StepFX()
    {
        GameObject smokeFX = Instantiate(stepFX, smokeFXPos.position, Quaternion.identity);
        Destroy(smokeFX, 2f);
    }

    public void DelayedStopEmission(float time)
    {
        StartCoroutine(StopEmissionAfterTime(time));
    }

    public void StopEmission()
    { 
        var fireEmission = fireTrail.emission;
        fireEmission.enabled = false;
        Debug.Log("Stop Emission");
    }

    IEnumerator StopEmissionAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StopEmission();
    }

    IEnumerator SetAttackTrueAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    public void StartEmission()
    {
        var fireEmission = fireTrail.emission;
        fireEmission.enabled = true;

        DelayedStopEmission(stopEmissionDelay);
    }


}
