using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    public List<Goober> EnemiesWithin;

    public void notifyEnemiesOfPlayer()
    {
        foreach (Goober enemy in EnemiesWithin)
        {
            if(!enemy.playerSeen)
            {
                enemy.StartChasingPlayer();
            }
        }
    }
}
