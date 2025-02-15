using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class Goober : EnemyClass
{
    //Player
    private Transform target;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement pm;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public PlayerHealthManager playerHealth;
    [HideInInspector] public SpriteRenderer sr;
    public Light2D IndicatorLight;

    [HideInInspector] public Vector2 directionToPlayer;
    [HideInInspector] public Vector2 attackDirection;

    [Header("Wandering")]
    private string[] states = { "Patrol", "Chase", "Attack" }; //use the index as the state
    public int state = 0;
    public bool pause = false;
    public float maxWanderTime = 6f;
    public float curWanderTime;

    [Header("Attacking")]
    public EnemyAttack attackBehavior;
    public bool canAttack = true;
    [HideInInspector] public SpriteRenderer attackIndicator;
    private EnemyHealthManager healthScript;
    [HideInInspector] public float localKnockback;

    private Tilemap wall;
    [HideInInspector] public NavMeshAgent agent;

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
    }

    private void Update()
    {
        if(pause) { return; }
        if(state == 0)
        {
            patrol();
        }
        else if (state == 1)
        {
            chasePlayer();
        }
        wanderCountdown();
    }

    //PATROLLING / WANDERING
    private void patrol()
    {
        Collider2D circlehit = Physics2D.OverlapCircle(transform.position, sightRadius, LayerMask.GetMask("Player")); //TODO only detect if they are within the frame and if the player 
        if (circlehit != null && circlehit.CompareTag("Player"))
        {
            target = player;
            state = 1;
            playerSeen = true;
            Invoke("SetCanAttack", 0.75f); 
        }
        else
        {
            StartCoroutine(wander());
        }
    }
    private void SetCanAttack()
    {
        canAttack = true;
    }

    private IEnumerator wander()
    {
        if (wanderPosition == Vector2.zero || curWanderTime == 0f)
        {
            yield return new WaitForSeconds(Random.Range(0, 4));

            float randomX = Random.Range(-wanderRange, wanderRange);
            float randomY = Random.Range(-wanderRange, wanderRange);
            wanderPosition = (Vector2)transform.position + new Vector2(randomX, randomY);
            if (wall.GetTile(wall.WorldToCell(wanderPosition)) != null)
            {
                wanderPosition = Vector2.zero;
            }
            else
            {
                curWanderTime = maxWanderTime;
            }
        }

        agent.speed = roamingSpeed;
        if (agent.enabled)
        {
            agent.SetDestination(wanderPosition);
        }

        if (Vector3.Distance(transform.position, wanderPosition) < 0.5f)
        {
            wanderPosition = Vector2.zero;
        }
    }
    private void wanderCountdown()
    {
        if(curWanderTime == 0f) { return; }
        else if(curWanderTime > 0f)
        {
            curWanderTime -= Time.deltaTime;
        }
        else if(curWanderTime < 0f)
        {
            curWanderTime = 0f;
        }
    }

    //CHASING
    private void chasePlayer()
    {
        if (!agent.enabled) { return; }
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
        checkIfCanAttack();
    }

    //ATTACKING
    private void checkIfCanAttack()
    {
        directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        if (Vector2.Distance((Vector2)transform.position, (Vector2)player.position) < attackBehavior.attackingRadius && canAttack) //if within attack range;
        {
            Collider2D selfColl = gameObject.GetComponent<Collider2D>();
            selfColl.enabled = false;
            RaycastHit2D toPlayer = Physics2D.Raycast(transform.position, directionToPlayer);
            selfColl.enabled = true;
            if(toPlayer.collider != null && toPlayer.collider.CompareTag("Player"))
            {
                //Debug.DrawRay(transform.position, directionToPlayer * attackBehavior.attackingRadius, Color.white, 2f);
                state = 2;
                StartCoroutine(attackBehavior.Attack(this));
            }
        }
    }
}