using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public int health;
    public Image healthUIBar;
    public Sprite[] healthBarImages;

    // Update is called once per frame
    void Update()
    {
        Die();
        setHealthBarLook();
    }

    private void Die()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void setHealthBarLook()
    {
        healthUIBar.sprite = healthBarImages[health-1];
    }
}