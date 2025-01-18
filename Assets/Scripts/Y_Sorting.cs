using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Sorting : MonoBehaviour
{
    private Transform player;
    [SerializeField] int playerLayer;
    [SerializeField] private Collider2D checkCollider;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform AnchorPoint;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        if(player.position.y > AnchorPoint.position.y)
        {
            sr.sortingOrder = playerLayer + 3;
        }
        else
        {
            sr.sortingOrder = playerLayer - 3;
        }
    }
}
