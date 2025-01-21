using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public float health;
    public Image healthUIImage;

    // Update is called once per frame
    void Update()
    {
        Die();

        healthUIImage.fillAmount = health / 100;
    }

    private void Die()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}