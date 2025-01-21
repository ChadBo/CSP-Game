using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [Header("Core")]
    public float life;
    public bool knockback;

    [Header("Player NOT Seen")]
    public float sightRadius;
    public Vector2 wanderPosition;
    public float wanderRange;
    public float roamingSpeed;

    [Header("Player Seen")]
    public bool playerSeen = false;
    public float chaseDistance;
    public float chaseSpeed;
}
