using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private PlayerAttacking PA;
    public SpriteRenderer swordObjSr;
    public SpriteRenderer PromptSr;
    private float interactInput;
    private bool isInZone;

    private void Start()
    {
        PA = GameObject.FindWithTag("Player").GetComponent<PlayerAttacking>();
    }

    private void Update()
    {
        interactInput = InputManager.Interact;
        if(isInZone)
        {
            if (interactInput > 0)
            {
                PA.enabled = true;
                PA.pointer.enabled = true;
                swordObjSr.enabled = false;
                Destroy(PromptSr.gameObject);
                gameObject.GetComponent<InteractPrompt>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) { return; }
        PromptSr.enabled = false;
        isInZone = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        PromptSr.enabled = true;
        isInZone = true;
    }
}
