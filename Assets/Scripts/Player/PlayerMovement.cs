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

    [SerializeField] private Vector2 movement;
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
        if (!isRolling)
        {
            movement.Set(InputManager.Movement.x, InputManager.Movement.y);
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
    //TODO fix downward roll
    private bool checkIfCanRoll()
    {
        if(!PlayerAttack.canAttack || !canRoll) { return false; }
        RaycastHit2D wall;
        wall = Physics2D.Raycast(transform.position, movement, rollCheckDistance, wallLayer);
        if (wall.collider != null)
        {
            print(wall.collider.name);
            if(wall.collider.CompareTag("Wall"))
            {
                print("NoRoll");
                return false;
            }
        }
        return true;
    }
}