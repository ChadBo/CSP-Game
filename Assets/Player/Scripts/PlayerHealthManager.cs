using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public float health;

    // Update is called once per frame
    void Update()
    {
        Die();
    }

    private void Die()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}