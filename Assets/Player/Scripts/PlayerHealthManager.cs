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

    public Vector2 checkpointPos;
    public Animator blackScreenAnimator;

    // Update is called once per frame
    void Update()
    {
        healInput = InputManager.Heal;
        Die();
        setHealthBarLook();
        if (healInput > 0 && healMeterAmount.fillAmount == 1)
        {
            UseHeal();
        }
    }

    private void Die()
    {
        if (health <= 0)
        {
            health = 5;
            StartCoroutine(fadeBlack());
            Invoke("dieStuff", 0.6f);
        }
    }

    private void dieStuff()
    {
        transform.position = checkpointPos;
        GameObject.FindWithTag("Checkpoint").GetComponent<InteractSavePoint>().TriggerRespawnEnemies();
        health = 5;
        gameObject.GetComponent<PlayerAttacking>().ApplyHitEffect(false, gameObject.GetComponent<PlayerMovement>().sr);
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
        health = maxHealth;
        healMeterAmount.fillAmount = 0f;
    }

    public IEnumerator fadeBlack()
    {
        blackScreenAnimator.SetBool("FadeIn", true);
        yield return new WaitForSeconds (1f);
        blackScreenAnimator.SetBool("FadeIn", false);
    }
}