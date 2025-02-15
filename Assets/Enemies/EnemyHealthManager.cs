using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public float life;
    public float knockback;

    private void Update()
    {
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
