using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [Header("Core")]

    [Header("Player NOT Seen")]
    public float sightRadius;
    public float wanderRange;
    public float roamingSpeed;

    [Header("Player Seen")]
    public bool playerSeen = false;
    public float chaseDistance;
    public float chaseSpeed;
}
