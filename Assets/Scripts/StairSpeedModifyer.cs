using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairSpeedModifyer : MonoBehaviour
{
    private PlayerMovement pm;
    private Rigidbody2D rb;
    public float diagonalSpeedMult;

    void Start()
    {
        pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        rb = pm.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if ((pm.movement.x != 0 && pm.movement.y == 0) || (pm.movement.x == 0 && pm.movement.y != 0))
            {
                pm.moveSpeed = diagonalSpeedMult * pm.walkSpeed;
            }
            else
            {
                pm.moveSpeed = pm.walkSpeed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pm.moveSpeed = pm.walkSpeed;
    }
}
