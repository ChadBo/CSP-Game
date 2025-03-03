using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public Image LifeUIBar;
    public Sprite[] LifeBarImages;

    public Image healMeterAmount;
    private float healInput;


    // Update is called once per frame
    void Update()
    {
        healInput = InputManager.Heal;
        Die();
        setHealthBarLook();
        UseHeal();
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
        LifeUIBar.sprite = LifeBarImages[health-1];
    }


    public void addToHealMeter(float amount)
    {
        healMeterAmount.fillAmount += amount / 10;
    }

    public void UseHeal()
    {
        if(healInput > 0 && healMeterAmount.fillAmount == 1)
        {
            health = maxHealth;
            healMeterAmount.fillAmount = 0f;
        }
    }
}