using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalEnemiesHandler : MonoBehaviour
{
    public GameObject[] enemiesGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
    private Dictionary<Goober, Transform> Enemies = new Dictionary<Goober, Transform>();

    void Start()
    {
        foreach (GameObject enemy in enemiesGameObjects) {
            Enemies.Add(enemy.GetComponent<Goober>(), enemy.transform);
        }
    }

    public void RespawnEnemy(GameObject enemy)
    {
        enemy.SetActive(true);
    }
}
