using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [Header("Core")]
    public float health;
    public bool knockback;

    [Header("Player NOT Seen")]
    public float sightRadius;
    public float roamingSpeed;

    [Header("Player Seen")]
    public float attackingRadius;
    public float chaseDistance;
    public float chaseSpeed;


    private void Update()
    {
        
    }
}
