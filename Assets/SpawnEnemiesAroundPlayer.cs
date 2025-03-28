using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesAroundPlayer : MonoBehaviour
{
    private Transform playerPos;

    public GameObject runner;
    public GameObject samuri;
    public GameObject ballman;
    public GameObject littleguy;

    private void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        spawnWave(0, 2, 0, 0);
    }

    private void Update()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
    }

    public void spawnWave(int numRunners, int numSamuri, int numBallmen, int numlittleguys)
    {
        for (int i = 0; i < numRunners; i++)
        {
            Instantiate(runner, new Vector2(playerPos.position.x + 8, playerPos.position.y), Quaternion.identity);
        }
        for (int i = 0; i < numSamuri; i++)
        {
            Instantiate(samuri, new Vector2(playerPos.position.x + 8, playerPos.position.y), Quaternion.identity);
        }
        for (int i = 0; i < numBallmen; i++)
        {
            Instantiate(ballman, new Vector2(playerPos.position.x + 8, playerPos.position.y), Quaternion.identity);
        }
        for (int i = 0; i < numlittleguys; i++)
        {
            Instantiate(littleguy, new Vector2(playerPos.position.x + 8, playerPos.position.y), Quaternion.identity);
        }
    }
}