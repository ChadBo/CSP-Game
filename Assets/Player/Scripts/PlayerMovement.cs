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
    [Header("MovementAspects")]
    public bool canMove = true;
    public bool updateMovement = true; // only update movement when the player gives further input, then, operate like normal.
    public Vector2 movement;
    public Vector2 facing = new Vector2(0, -1);
    public bool canFlip = true;
    [Header("Components")]
    private Rigidbody2D rb;
    public GameObject playerSprite;
    public SpriteRenderer ShadowSr;
    public Sprite rollShadow;
    public Sprite standShadow;
    private SpriteRenderer sr;
    private Animator animator;
    [SerializeField] private ParticleSystem rollPS;
    [Header("Rolling")]
    private float rollInput;
    public bool isRolling = false;
    public bool canRoll = true;
    public float rollCheckDistance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = playerSprite.GetComponent<SpriteRenderer>();
        animator = playerSprite.GetComponent<Animator>();
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

        if (facing.x > 0)
        {
            sr.flipX = true;
        }
        else if (facing.x < 0)
        {
            sr.flipX = false;
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
        //
        if((movement.x < 0 && movement.y > 0) || (movement.x > 0 && movement.y < 0))
        {
            playerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }
        else if((movement.x > 0 && movement.y > 0) || (movement.x < 0 && movement.y < 0))
        {
            playerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }
        animator.SetBool("Rolling", true);
        ShadowSr.sprite = rollShadow;
        rollPS.Play();

        yield return new WaitForSeconds(0.4f);

        if (playerSprite.transform.rotation.z != 0)
        {
            playerSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        moveSpeed = walkSpeed;
        rb.drag = rbDrag;

        animator.SetBool("Rolling", false);
        ShadowSr.sprite = standShadow;

        PlayerAttack.canAttack = true;
        isRolling = false;
        yield return new WaitForSeconds(0.25f);
        canRoll = true;
    }

    private bool checkIfCanRoll()
    {
        if (!canRoll && canMove || movement == Vector2.zero) { return false; }
        RaycastHit2D wall;
        int wallLayerMask = 1 << 7; // Create a LayerMask for layer 7
        wall = Physics2D.Raycast(transform.position, movement, rollCheckDistance, wallLayerMask);
        if (wall.collider != null)
        {
            return false;
        }

        return true;
    }
}