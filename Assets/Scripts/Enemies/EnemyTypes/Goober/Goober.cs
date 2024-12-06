using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Goober : EnemyClass
{
    //Player
    private Transform target;
    private Transform player;
    private PlayerMovement pm;
    private Rigidbody2D playerRb;
    private PlayerHealthManager playerHealth;

    private Vector2 directionToPlayer;

    private string[] states = { "Patrol", "Chase", "Attack" }; //use the index as the state
    public int state = 0;
    public bool pause = false;
    public float maxWanderTime = 6f;
    public float curWanderTime;

    [SerializeField] private int attackDamage;

    private Tilemap wall;
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
        pm = player.gameObject.GetComponent<PlayerMovement>();
        playerRb = player.gameObject.GetComponent<Rigidbody2D>();
        playerHealth = player.gameObject.GetComponent<PlayerHealthManager>();

        wall = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
        //target = player;
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
        //directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
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
        }
        else
        {
            StartCoroutine(wander());
        }
    }

    private IEnumerator wander()
    {
        if (wanderPosition == Vector2.zero || curWanderTime == 0f)
        {
            // Wait for a random interval before setting a new position
            yield return new WaitForSeconds(Random.Range(0, 4));

            // Generate a nearby random position within the wander range
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

        // Check if the enemy is close enough to the target position
        if (Vector3.Distance(transform.position, wanderPosition) < 0.5f)
        {
            wanderPosition = Vector2.zero; // Reset to allow the next random position
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
        if(Vector2.Distance((Vector2)transform.position, (Vector2)player.position) < attackingRadius)
        {
            state = 2;
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack() //MAYBE: change to regular function with timers so you can stop them at any time
    {
        agent.speed = 0f;
        agent.SetDestination(transform.position);
        directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        yield return new WaitForSeconds(0.4f);

        Collider2D selfCollider = GetComponent<Collider2D>();
        selfCollider.enabled = false;
        int playerLayerMask = 1 << 3; // Correct bitmask for layer 3
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, attackingRadius * 2, playerLayerMask);

        Debug.DrawRay(transform.position, directionToPlayer*attackingRadius*2, Color.white, 1f);
        selfCollider.enabled = true;
        //print(hit.collider.name);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            print("-HEALTH");
            playerHealth.health -= attackDamage;
            pm.canMove = false;
            playerRb.AddForce(directionToPlayer * 3000, ForceMode2D.Force);
        }
        state = 1;
        //pm.canMove = false;
        yield return new WaitForSeconds(0.2f);
        pm.canMove = true;
        //pm.updateMovement = false;
    }
}