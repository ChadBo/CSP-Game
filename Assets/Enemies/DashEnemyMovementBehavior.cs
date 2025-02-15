using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
public class DashEnemyMovementBehavior : EnemyClass
{
    //Player
    private Transform target;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement pm;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public PlayerHealthManager playerHealth;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
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
    public bool canAttack = true;
    [HideInInspector] public SpriteRenderer attackIndicator;
    [HideInInspector] public float localKnockback;
    private EnemyHealthManager healthScript;
    public float attackingRadius;
    public float attackIndicatorTime;
    public float attackCooldown;
    public float attackDamage;

    private Tilemap wall;
    [HideInInspector] public NavMeshAgent agent;

    private Animator animator;
    private SpriteRenderer slashSr;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    bool dash = false;
    public bool isDashing = false;
    public float overshootDistance;
    private Vector2 dashDestination;
    private Vector2 dashDirection;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
        pm = player.gameObject.GetComponent<PlayerMovement>();
        playerRb = player.gameObject.GetComponent<Rigidbody2D>();
        playerHealth = player.gameObject.GetComponent<PlayerHealthManager>();
        sr = GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        attackIndicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
        print(transform.GetChild(0).name);
        healthScript = GetComponent<EnemyHealthManager>();
        wall = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
        localKnockback = healthScript.knockback;

        animator = GetComponent<Animator>();
        slashSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (pause) { return; }
        if (state == 0)
        {
            patrol();
        }
        else if (state == 1)
        {
            chasePlayer();
        }
        wanderCountdown();

        if(dash)
        {
            moveTowardsDashDestination(dashDirection);
        }
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
        if (curWanderTime == 0f) { return; }
        else if (curWanderTime > 0f)
        {
            curWanderTime -= Time.deltaTime;
        }
        else if (curWanderTime < 0f)
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
        if (Vector2.Distance((Vector2)transform.position, (Vector2)player.position) < attackingRadius && canAttack) //if within attack range;
        {
            Collider2D selfColl = gameObject.GetComponent<Collider2D>();
            selfColl.enabled = false;
            RaycastHit2D toPlayer = Physics2D.Raycast(transform.position, directionToPlayer, attackingRadius+10, ~LayerMask.GetMask("TriggerColliderIgnore"));
            selfColl.enabled = true;
            //if(toPlayer.collider != null)
            //{
            //    Debug.Log(toPlayer.collider.name);
            //}
            
            if (toPlayer.collider != null && toPlayer.collider.CompareTag("Player"))
            {
                state = 2;
                if(!isDashing)
                {
                    StartCoroutine(DashAttack());
                }
            }
        }
    }

    private IEnumerator DashAttack()
    {
        
        slashSr = transform.GetChild(0).GetComponent<SpriteRenderer>();

        canAttack = false;
        animator.SetBool("Attack", true);
        agent.enabled = false;

        yield return new WaitForSeconds(attackIndicatorTime);
        isDashing = true;
        dashDestination = SetDashDestination();

        dash = true;

        slashSr.enabled = true;
        //
        Vector2 dirToTarget = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        slashSr.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //
        yield return new WaitForSeconds(dashDuration); // Dash for a fixed time
        dash = false;
        rb.velocity = Vector2.zero;
        slashSr.enabled = false;
        animator.SetBool("Attack", false);
        isDashing = false;
        yield return new WaitForSeconds(attackCooldown);
        agent.enabled = true;
        canAttack = true;
        state = 1;
        playerRb.drag = 1;
    }

    private Vector2 SetDashDestination()
    {
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        GetComponent<Collider2D>().enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, attackingRadius + overshootDistance, ~LayerMask.GetMask("Player") | ~LayerMask.GetMask("TriggerColliderIgnore"));
        GetComponent<Collider2D>().enabled = true;

        Debug.DrawRay(transform.position, directionToPlayer * (attackingRadius + overshootDistance), Color.white, 2f);
        dashDirection = (dashDestination - (Vector2)transform.position).normalized;
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            float gooberToPlayerDist = Vector2.Distance(transform.position, player.position);
            float gooberToHitDist = Vector2.Distance(transform.position, hit.point);

            if (gooberToHitDist < gooberToPlayerDist) // If the hit point is before the player, there's an obstacle
            {
                return hit.point;
                //return transform.position; // Dash should not proceed
            }
            return hit.point;
        }
        else // If the ray hits nothing, dash to the tip of the ray
        {
            dashDestination = (Vector2)transform.position + (directionToPlayer.normalized * (attackingRadius + overshootDistance));
            Debug.Log("Going to dash point" + dashDestination);
            return dashDestination;
        }
    }

    private void moveTowardsDashDestination(Vector2 dashDirection)
    {
        rb.velocity = directionToPlayer * dashSpeed; // Apply dash velocity
        Debug.Log(dashDirection);
        rb.drag = Mathf.Lerp(rb.drag, 100, dashDuration * Time.deltaTime);
    }
}