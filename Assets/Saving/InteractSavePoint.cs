using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InteractSavePoint : MonoBehaviour
{
    public SpriteRenderer PromptSr;
    private float interactInput;
    private bool isInZone;
    public bool canActivate;
    public bool hasStarted;
    private PlayerHealthManager phm;

    public GameObject[] enemies;
    public Vector2[] enemiesStartPositions;

    private void Start()
    {
        phm = GameObject.FindWithTag("Player").GetComponent<PlayerHealthManager>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesStartPositions = new Vector2[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            enemiesStartPositions[i] = enemies[i].transform.position;
        }
    }


    private void Update()
    {
        interactInput = InputManager.Interact;
        if (isInZone)
        {
            if (interactInput > 0 && canActivate)
            {
                canActivate = false;
                PromptSr.enabled = false;

                phm.checkpointPos = new Vector2(transform.parent.position.x, transform.parent.position.y - 3);
                phm.UseHeal();
                StartCoroutine(phm.fadeBlack());
                TriggerRespawnEnemies();
            }
            if(phm.health <= 0)
            {
                TriggerRespawnEnemies();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        PromptSr.enabled = false;
        isInZone = false;
        canActivate = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        PromptSr.enabled = true;
        isInZone = true;
        canActivate = true;
    }

    private void Respawn(EnemyHealthManager ehm, Vector2 startPos)
    {
        hasStarted = true;
        ehm.enabled = true;
        ehm.life = ehm.maxLife;
        ehm.hasStarted = false;
        //
        Goober goober = ehm.GetComponent<Goober>();
        goober.enabled = true;
        goober.state = 0;
        goober.animator.enabled = true;

        ehm.StopAllCoroutines();
        goober.animator.SetBool("Die", false);
        goober.animator.SetBool("Attack", false);
        goober.animator.SetBool("Warn", false);
        goober.ApplyHitEffect(false, goober.pm.sr);

        goober.agent.enabled = true;

        foreach (Component thing in ehm.thingsToDisableOnDeath)
        {
            if (thing is Behaviour behaviourComponent)
            {
                behaviourComponent.enabled = true;
            }
        }

        goober.animator.SetBool("Die", false);

        if (ehm.severOnDeath)
        {
            ehm.chunk.GetComponent<SpriteRenderer>().enabled = false;
            ehm.chunk.transform.position = Vector2.zero;
        }
    }

    public void TriggerRespawnEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Respawn(enemies[i].GetComponent<EnemyHealthManager>(), enemiesStartPositions[i]);
        }
    }
}
