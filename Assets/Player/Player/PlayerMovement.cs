using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttacking PlayerAttack;

    [Header("Speed")]
    public float walkSpeed = 5f;
    public float rollSpeed = 7f;
    public float sprintSpeed = 6.5f;
    public float moveSpeed;

    public bool canMove = true;
    public bool updateMovement = true; // only update movement when the player gives further input, then, operate like normal.

    public Vector2 movement;
    public Vector2 facing = new Vector2(0,-1);
    private Rigidbody2D rb;
    private Animator animator;

    public bool canFlip = true;

    private float rollInput;
    private bool isRolling = false;
    public bool canRoll = true;
    public float rollCheckDistance;
    int wallLayer = 7;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        PlayerAttack = GetComponent<PlayerAttacking>();
    }

    private void Update()
    {
        //RollInput value
        rollInput = InputManager.Roll;
        //move
        UpdatePlayerFacing();
        if (canMove)
        {
            movePlayer();
        }
        flip();
        Debug.DrawRay(transform.position, movement * rollCheckDistance, Color.white);
    }

    private void movePlayer()
    {
        if (updateMovement && !isRolling)
        {
            movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        }
        if(!updateMovement)
        {
            MovePlayerNormallyWhenTheyInput();
        }
        if (rollInput == 1 && checkIfCanRoll())
        {
            StartCoroutine(roll());
        }
        
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

    private IEnumerator roll()
    {
        isRolling = true;
        canRoll = false;
        PlayerAttack.canAttack = false;
        moveSpeed = rollSpeed;
        float rbDrag = rb.drag;
        rb.drag = 0f;

        animator.SetBool("Rolling", true);

        yield return new WaitForSeconds(0.4f);
        moveSpeed = walkSpeed;
        rb.drag = rbDrag;

        animator.SetBool("Rolling", false);

        PlayerAttack.canAttack = true;
        isRolling = false;
        yield return new WaitForSeconds(0.25f);
        canRoll = true;
    }

    private bool checkIfCanRoll()
    {
        if(!canRoll && canMove) { return false; }
        RaycastHit2D wall;
        int wallLayerMask = 1 << 7; // Create a LayerMask for layer 7
        wall = Physics2D.Raycast(transform.position, movement, rollCheckDistance, wallLayerMask);
        if (wall.collider != null)
        {
            return false;
        }

        return true;
    }

    private void MovePlayerNormallyWhenTheyInput()
    {
        //print(InputManager.Movement.x);
        if(InputManager.Movement.x > 0 || InputManager.Movement.y > 0)
        {
            updateMovement = true;
        }
    }
}