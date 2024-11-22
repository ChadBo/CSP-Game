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
    private string[] states = { "Patrol", "Chase", "Attack" };
    public int state = 0;
    public bool pause = false;
    public float maxWanderTime = 6f;
    public float curWanderTime;

    private Tilemap wall;

    public bool printdistance = false;

    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
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
    }

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

    private void chasePlayer()
    {
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
    }
}