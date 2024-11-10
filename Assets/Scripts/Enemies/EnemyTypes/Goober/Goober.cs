using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goober : EnemyClass
{
    //Player
    private Transform target;
    private Transform player;
    private string[] states = { "Patrol", "Chase", "Attack" };
    public int state = 0;
    public bool pause = false;

    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
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
        if (wanderPosition == Vector2.zero)
        {
            yield return new WaitForSeconds(Random.Range(0, 4));
            wanderPosition = new Vector2Int((int)Random.Range(0, wanderRange), (int)Random.Range(0, wanderRange));
            //print(wanderPosition);
        }
        agent.speed = roamingSpeed;
        agent.SetDestination(((Vector3Int)wanderPosition));
        //reach destination
        if (new Vector2(transform.position.x, transform.position.y) == wanderPosition)
        {
            wanderPosition = Vector2Int.zero;
        }
    }

    private void chasePlayer()
    {
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
    }
}