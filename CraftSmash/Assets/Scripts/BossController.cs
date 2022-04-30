using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    public float currentHealthPoints;
    public float maxHealthPoints = 100;
    public bool isAlive = true;

    [Header("Shield")]
    public GameObject shieldObject;
    public float currentShieldPoints;
    public float maxShieldPoints = 100;

    [Header("Combat")]
    public bool isInCombat = false;
    public bool isAttacking = false;
    public float aggroRange = 30f;
    float attackAnimationTimer = 0f;

    [Header("Movement")]
    public float runSpeed = 5f;
    float velocityY = 0;
    public float gravity = -12;
    public bool isGrounded = false;

    PlayerCharacterController player;
    CharacterController cController;
    Animator animator;

    float distanceGround;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        animator = GetComponentInChildren<Animator>();
        cController = GetComponent<CharacterController>();

        currentHealthPoints = maxHealthPoints;
        currentShieldPoints = maxShieldPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            CheckForGrounded();
            CheckForPlayer();
            MoveToPlayer();
            IsAttacking();
            Shield();
            Death();

            if (currentShieldPoints <= 0)
            {
                runSpeed = 7f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInCombat)
        {
            currentHealthPoints -= damage;
        }
    }

    public void TakeShieldDamage(float damage)
    {
        if (isInCombat)
        {
            currentShieldPoints -= damage;
        }
    }

    void Shield()
    {
        if (currentShieldPoints > 0)
        {
            shieldObject.SetActive(true);
        }
        else
        {
            shieldObject.SetActive(false);
        }
    }

    void Death()
    {
        if (currentHealthPoints <= 0)
        {
            isAlive = false;
            animator.SetBool("isDead", true);
            cController.enabled = false;
        }
    }

    void MoveToPlayer()
    {
        velocityY += Time.deltaTime * gravity;
        cController.Move(Vector3.up * velocityY);

        if (isInCombat)
        {

            gameObject.transform.LookAt(new Vector3(player.transform.position.x, gameObject.transform.position.y, player.transform.position.z));

            if (MeleeRange() && player.isAlive == true)
            {
                AttackPlayer();
            }
            else
            {
                Vector3 velocity = transform.forward * runSpeed + Vector3.up * velocityY;
                cController.Move(velocity * Time.deltaTime);

                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        

        if (cController.isGrounded)
        {
            velocityY = 0;
        }

        if (player.isAlive == false)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        if (isAttacking == false)
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            
        }
    }

    void IsAttacking()
    {
        if (isAttacking)
        {
            attackAnimationTimer += Time.deltaTime;

            if (attackAnimationTimer >= 1f)
            {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                attackAnimationTimer = 0f;
                
                float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);
                if (distance <= 5f)
                {
                    player.startBossAttackSound = true;
                    player.TakeDamage(5f);
                }
            }
        }
    }

    bool MeleeRange()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance <= 3f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckForPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (distance <= aggroRange)
        {
            isInCombat = true;
        }
        else
        {
            isInCombat = false;
        }
    }

    void CheckForGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (hit.distance > 0.1f)
        {
            isGrounded = false;

        }
        else
        {
            isGrounded = true;
        }
    }
}
