using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthManager : MonoBehaviour
{
    public float life;
    public float knockback;
    public Coroutine dashAttackCoroutine;
    public Component[] thingsToDisableOnDeath;
    public bool severOnDeath = false;
    public GameObject chunk;

    private bool hasStarted = false;
    private void Update()
    {
        if(life <= 0 && !hasStarted)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        hasStarted = true;
        Goober goober = gameObject.GetComponent<Goober>();
        goober.enabled = false;

        goober.StopAllCoroutines();
        goober.ApplyHitEffect(false, goober.pm.sr);

        gameObject.GetComponent<NavMeshAgent>().enabled = false;

        foreach (Component thing in thingsToDisableOnDeath)
        {
            if (thing is Behaviour behaviourComponent)
            {
                behaviourComponent.enabled = false;
            }
        }

        goober.animator.SetBool("Die", true);

        if (severOnDeath)
        {
            Rigidbody2D chunkRb = chunk.GetComponent<Rigidbody2D>();

            chunk.GetComponent<SpriteRenderer>().enabled = true;
            chunkRb.AddForce(goober.directionToPlayer * -5, ForceMode2D.Impulse);

            // Add a random spin for a more dynamic effect
            float randomTorque = Random.Range(-20f, 20f); // Adjust range as needed
            chunkRb.AddTorque(randomTorque, ForceMode2D.Impulse);
        }


        yield return new WaitForSeconds(4f);

        goober.animator.enabled = false;
        this.enabled = false;
    }
}
