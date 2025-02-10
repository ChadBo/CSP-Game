using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Sorting : MonoBehaviour
{
    private Transform player;
    public int playerLayer;
    [SerializeField] private Collider2D checkCollider;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform AnchorPoint;
    public bool canSort = true;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canSort) { return; }
        if (!collision.CompareTag("Player")) { return; }

        if(player.position.y > AnchorPoint.position.y)
        {
            sr.sortingOrder = playerLayer + 2;
        }
        else
        {   
            sr.sortingOrder = playerLayer - 2;
        }
    }
}
