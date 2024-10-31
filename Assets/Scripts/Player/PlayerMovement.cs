using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    public bool canMove = true;

    private Vector2 movement;
    public Vector2 facing = new Vector2(0,-1);

    private Rigidbody2D rb;
    private Animator animator;

    public bool canFlip = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdatePlayerFacing();
        if (canMove)
        {
            movePlayer();
        }
        flip();
    }

    private void movePlayer()
    {
        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        rb.velocity = movement * moveSpeed;

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    private void UpdatePlayerFacing()
    {
        if (movement != Vector2.zero)
        {
            facing = movement;
            animator.SetFloat("FaceHorizontal", facing.x);
            if (facing.y != 0)
            {
                facing = new Vector2(facing.x, facing.y / Mathf.Abs(facing.y));
                animator.SetFloat("FaceVertical", facing.y);
            }
        }
    }

    private void flip()
    {
        if (!canFlip) { return; }

        if(facing.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (facing.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}