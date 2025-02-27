using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHitDetection : MonoBehaviour
{
    private Goober goober;
    public bool hasHitPlayer = false;

    void Start()
    {
        //goober = transform.parent.GetComponent<DashEnemyMovementBehavior>();
        goober = transform.parent.GetComponent<Goober>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasHitPlayer && !goober.pm.isRolling)
        {
            hasHitPlayer = true;
            goober.playerHealth.health -= 1;
            ScreenShakeController.instance.StartShake(0.6f, 2);
            Invoke("resetHitPlayer", 0.2f);
            goober.ApplyHitEffect(true, collision.GetComponent<PlayerMovement>().sr);
        }
    }

    private void resetHitPlayer()
    {
        hasHitPlayer = false;
    }
}
