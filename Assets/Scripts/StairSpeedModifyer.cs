using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairSpeedModifyer : MonoBehaviour
{
    private PlayerMovement pm;
    private Rigidbody2D rb;
    public float stairSpeed;
    public bool stairsGoRight;

    void Start()
    {
        pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        rb = pm.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            pm.moveSpeed = stairSpeed;
            if ((pm.movement.x > 0 && stairsGoRight) || (pm.movement.x < 0 && !stairsGoRight))
            {
                pm.addedMoveY = 1f;
            }
            else if ((pm.movement.x < 0 && stairsGoRight) || (pm.movement.x > 0 && !stairsGoRight))
            {
                pm.addedMoveY = -1f;
            }
            else { pm.addedMoveY = 0; }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pm.addedMoveY = 0;
        pm.moveSpeed = pm.walkSpeed;
    }
}
