using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class Boss : MonoBehaviour
{
    public bool isEnabled = true;
    public bool playerSeen;
    // Player
    private Transform target;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement pm;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public PlayerHealthManager playerHealth;
    [HideInInspector] public SpriteRenderer sr;
    public Light2D IndicatorLight;
    public ParticleSystem gotHitPS;

    [HideInInspector] public Vector2 directionToPlayer;
    [HideInInspector] public Vector2 attackDirection;

    [Header("Wandering")]
    private string[] states = { "Patrol", "Chase", "Attack" }; // Use the index as the state
    public int state = 0;
    public bool pause = false;
    public int nativeFacingDirectionForFlipX = -1;
    public bool canFlip = true;

    [Header("Attacking")]
    public bool canAttack = true;
    public float currentAttackRadius;
    [HideInInspector] public SpriteRenderer attackIndicator;
    private EnemyHealthManager healthScript;
    [HideInInspector] public float localKnockback;

    private Tilemap wall;
    [HideInInspector] public NavMeshAgent agent;

    // 🔹 Empty Variables (Only Used by Dash Attack, But Always Exist)
    public Animator animator;
    [HideInInspector] public SpriteRenderer slashSr;
    [HideInInspector] public Collider2D DashHitPlayerDetectionColl;

    public MaterialPropertyBlock hitMaterialPropertyBlock;
    public bool hitEffectEnabled = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
        pm = player.gameObject.GetComponent<PlayerMovement>();
        playerRb = player.gameObject.GetComponent<Rigidbody2D>();
        playerHealth = player.gameObject.GetComponent<PlayerHealthManager>();
        healthScript = GetComponent<EnemyHealthManager>();
        sr = GetComponent<SpriteRenderer>();
        attackIndicator = transform.GetChild(0).GetComponent<SpriteRenderer>();

        wall = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
        localKnockback = healthScript.knockback;
        ResetMaterial();

        // 🔹 Initialize Animator & Slash Variables (If They Exist)
        animator = gameObject.GetComponent<Animator>();
        if (transform.childCount > 0)
        {
            slashSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        target = player;
    }

    private void Update()
    {
        if (pause) return;

        else if (state == 1 && isEnabled) chasePlayer();

        directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        flipSideways();
        animator.SetFloat("Speed", agent.speed);
    }


    private void SetCanAttack() => canAttack = true;

    public void StartChasingPlayer()
    {
        target = player;
        state = 1;
        playerSeen = true;
        Invoke("SetCanAttack", 0.75f);
    }

    // CHASING
    private void chasePlayer()
    {
        if (!agent.enabled) return;

        agent.SetDestination(target.position);
        checkIfCanAttack();
    }

    // ATTACKING
    private void checkIfCanAttack()
    {
        if (Vector2.Distance(transform.position, player.position) < currentAttackRadius && canAttack)
        {
            Collider2D selfColl = gameObject.GetComponent<Collider2D>();
            selfColl.enabled = false;
            RaycastHit2D toPlayer = Physics2D.Raycast(transform.position, directionToPlayer);
            selfColl.enabled = true;

            if (toPlayer.collider != null && toPlayer.collider.CompareTag("Player"))
            {
                state = 2;
                StartCoroutine(SlashAttack());
            }
        }
    }
    //FOR SELF
    private void ResetMaterial()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat("_UseMainTexColor", 1.0f);
        sr.SetPropertyBlock(mpb);
    }
    //FOR PLAYER
    public void ApplyHitEffect(bool enableEffect, SpriteRenderer playerSR)
    {
        if (playerSR == null) return;

        if (hitMaterialPropertyBlock == null)
            hitMaterialPropertyBlock = new MaterialPropertyBlock();

        // Get the current property block
        playerSR.GetPropertyBlock(hitMaterialPropertyBlock);

        // Set the boolean property for the shader
        hitMaterialPropertyBlock.SetFloat("_UseMainTexColor", enableEffect ? 0.0f : 1.0f);

        // Apply it back to the material
        playerSR.SetPropertyBlock(hitMaterialPropertyBlock);
        StartCoroutine(ResetHitEffect(playerSR));
    }

    private IEnumerator ResetHitEffect(SpriteRenderer playerSR) //TODO DONT LET THIS COROUTINE STOP ON DEATH!!! ALLOW OTHERS, BUT MAKE SURE THE COLOR GETS RESET
    {
        yield return new WaitForSeconds(0.3f);
        ApplyHitEffect(false, playerSR);
    }

    private void flipSideways()
    {
        if (!canFlip) return;
        if (nativeFacingDirectionForFlipX == -1 && directionToPlayer.x > 0)
        {
            sr.flipX = true;
        }
        else if (nativeFacingDirectionForFlipX == 1 && directionToPlayer.x < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    private IEnumerator SlashAttack()
    {
        canAttack = false;
        animator.SetBool("Slash", true);
        agent.speed = 0;
        canFlip = false;

        yield return new WaitForSeconds(1f);

        animator.SetBool("Slash", false);
        canFlip = true;

        yield return new WaitForSeconds(1f);
        agent.speed = 3.5f;
        canAttack = true;
        state = 1;
    }
}