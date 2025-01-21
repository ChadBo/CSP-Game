using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Goober : EnemyClass
{
    //Player
    private Transform target;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement pm;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public PlayerHealthManager playerHealth;
    [HideInInspector] public NavMeshAgent _navMeshAgent;

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
    public SpriteRenderer attackIndicator;

    private Tilemap wall;
    public NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
        pm = player.gameObject.GetComponent<PlayerMovement>();
        playerRb = player.gameObject.GetComponent<Rigidbody2D>();
        playerHealth = player.gameObject.GetComponent<PlayerHealthManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        wall = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
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
        Die();
    }

    private void Die()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    //PATROLLING / WANDERING
    private void patrol()
    {
        Collider2D circlehit = Physics2D.OverlapCircle(transform.position, sightRadius, LayerMask.GetMask("Player"));
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
        agent.SetDestination(wanderPosition);

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
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
        checkIfCanAttack();
    }

    //ATTACKING
    private void checkIfCanAttack()
    {
        if(Vector2.Distance((Vector2)transform.position, (Vector2)player.position) < attackBehavior.attackingRadius && canAttack)
        {
            state = 2;
            StartCoroutine(attackBehavior.Attack(this));
        }
    }
}