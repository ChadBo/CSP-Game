using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private PlayerHealthManager phm;

    void Start()
    {
        phm = GameObject.FindWithTag("Player").GetComponent<PlayerHealthManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        phm.checkpointPos = transform.position;
    }
}
