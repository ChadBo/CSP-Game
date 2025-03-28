using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    public float projectileDamage = 10f;
    public Goober controllingGoober;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy"))
        {
            if(collision.CompareTag("Player") && !controllingGoober.pm.isRolling)
            {
                collision.GetComponent<PlayerHealthManager>().health -= 1;
                ScreenShakeController.instance.StartShake(0.4f, 3f);
                controllingGoober.ApplyHitEffect(true, collision.GetComponent<PlayerMovement>().sr);
                Destroy(gameObject);
            }
            else if (collision.CompareTag("Player") && controllingGoober.pm.isRolling)
            {
                return;
            }
            Destroy(gameObject);
        }
    }
}
