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
    [SerializeField] float stopEmissionDelay = 0.6f;

    int attackType = 0;

    [SerializeField] Transform smokeFXPos;

    [SerializeField] ParticleSystem[] particleTrails;



    [SerializeField] GameObject[] particleExplosionFX;
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
            GameObject fireFX = Instantiate(particleExplosionFX[attackType],currentEnemy.transform.position,Quaternion.identity);
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
                attackType = (int)AttackType.Water;
                break;
            case SwipeDirection.Down:
                attackType = (int)AttackType.Poison;

                break;

            case SwipeDirection.Left:

                break;

            case SwipeDirection.Right:
                    Debug.Log("Right Swipe Detected!");
                    attackType = (int)AttackType.Fire;

                break;
            }
            StartEmission();
        }   
    }

    public void StepFX()
    {
        GameObject smokeFX = Instantiate(stepFX, smokeFXPos.position, Quaternion.identity);
        Destroy(smokeFX, 2f);
    }

    public void StopEmission()
    {

        for (int i = 0; i < particleTrails.Length; i++)
        {
            var particleEmission = particleTrails[i].emission;
            particleEmission.enabled = false;

            var particleChild = particleTrails[i].transform.GetChild(0).GetComponent<ParticleSystem>();

            var particleChildEmission = particleChild.emission;
            particleChildEmission.enabled = false;
        }

        //Invoke(nameof(DisableParticleTrails), 1f);

        Debug.Log("Stop Emission for all of em");
    }

    //public void DisableParticleTrails()
    //{
    //    foreach (var particleTrail in particleTrails)
    //    {
    //        particleTrail.transform.GetChild(0).gameObject.SetActive(false);
    //    }
    //}

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
        particleTrails[attackType].gameObject.SetActive(true);

        var particleEmission = particleTrails[attackType].emission;
        particleEmission.enabled = true;

        var particleChild = particleTrails[attackType].transform.GetChild(0).GetComponent<ParticleSystem>();

        var particleChildEmission = particleChild.emission;
        particleChildEmission.enabled = true;

        StartCoroutine(StopEmissionAfterTime(stopEmissionDelay));
    }


}

public enum AttackType
{
    Fire,
    Water,
    Poison,
    None,
}
