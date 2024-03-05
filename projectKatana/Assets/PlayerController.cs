using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] CharacterController characterController;
    [SerializeField] ParticleSystem[] particleTrails;
    [SerializeField] GameObject[] particleExplosionFX;
    [SerializeField] GameObject stepFX;
    [SerializeField] Transform smokeFXPos;

    public AnimationCurve jumpCurve;

    public bool canKillEnemy;
    [SerializeField] float runSpeed;
    [SerializeField] float currentAttackMoveSpeed;
    [SerializeField] float attackMoveSpeed = 1f;
    [SerializeField] float dashMoveSpeed = 10f;

    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;

    [SerializeField] float jumpSpeed;
    [SerializeField] float attackCoolDown = 0.5f;
    [SerializeField] float stopEmissionDelay = 0.6f;

    [SerializeField] bool isRunning;
    [SerializeField] bool isAttacking;
    [SerializeField] bool isJumping;
    [SerializeField] bool canAttack;
    [SerializeField] bool isDead;

    [SerializeField] string currentAttackString = null;

    string[] attackStrings = { "Attack", "Attack2", "AttackDash" };
    int attackType = 0;
    Enemy currentEnemy;
    Transform currentJumpTransform;


     float currentJumpTime;

    private void Awake()
    {
        InputHandler.OnSwipe += OnSwipeInput;

        StopEmission();

    }
    private void Start()
    {
        canAttack = true;
        currentAttackMoveSpeed = attackMoveSpeed;
    }


    void Update()
    {
        if (!isDead && !isJumping && isRunning)
        {
            characterController.SimpleMove(Vector3.forward * runSpeed/* * Time.deltaTime*/);
            //transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        }
        if (!isDead && isAttacking)
        {
            characterController.SimpleMove(Vector3.forward * currentAttackMoveSpeed);
            //transform.Translate(Vector3.forward * attackMoveSpeed * Time.deltaTime);
        }

        if (!isDead && isJumping)
        {
            Debug.Log(currentJumpTransform);
            currentJumpTime += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(transform.position, currentJumpTransform.position, currentJumpTime);
            pos.y += jumpCurve.Evaluate(currentJumpTime);
            transform.position = pos;
        }

        HandleAnimation();

        

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
        if (other.CompareTag("Jump"))
        {
            
            currentJumpTransform = other.transform.GetChild(0).transform;
            Jump();
        }
        if (other.CompareTag("Land"))
        {
            Debug.Log("landing trigger");
            Land();
        }
    }

    public void ResetAttackSpeed()
    {
        currentAttackMoveSpeed = attackMoveSpeed;
        canAttack = false;
    }

    private void Land()
    {
        
        isJumping = false;
        isRunning = true;
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
        characterController.enabled = false;
        isDead = true;
    }

    public void Jump()
    {
        isRunning = false;
        isAttacking = false;
        isJumping = true;

        anim.SetTrigger("Jump");
    }

    private void HandleAnimation()
    {
        anim.SetBool("Run", isRunning);
        if(currentAttackString != null)
        {
            
            anim.SetBool(currentAttackString, isAttacking);
        }
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

    IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(Vector3.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void OnSwipeInput(SwipeData data)
    {
        if (!isDead && !characterController.isGrounded)
        {
            isRunning = false;
            isJumping = false;
            isAttacking=true;
            currentAttackString = "AttackDash";
            currentAttackMoveSpeed = dashMoveSpeed;
            StartCoroutine(Dash()); 
            Debug.Log("Using Dash Attack");
            Debug.Log(currentAttackString + isAttacking);
        }

        if (!isDead && canAttack)
        {
            currentAttackString = attackStrings[Random.Range(0, attackStrings.Length)];

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

        Debug.Log("Stop Emission for all of em");
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
