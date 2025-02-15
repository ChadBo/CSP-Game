using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHitDetection : MonoBehaviour
{
    private DashEnemyMovementBehavior dashScript;
    void Start()
    {
        dashScript = transform.parent.GetComponent<DashEnemyMovementBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(dashScript.isDashing && collision.CompareTag("Player"))
        {
            dashScript.playerHealth.health -= dashScript.attackDamage;
            ScreenShakeController.instance.StartShake(0.6f, 2);
        }
    }
}
