using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttacking : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Animator playerAnimator;
    private SpriteRenderer playerSR;
    private float attackInput;

    private bool canAttack = true;
    [Header("Combo")]
    public int currentAttack = 0;
    public UnityEvent[] SwordAttacks;
    public float comboTime = 0.7f;
    public float comboTimer = 0f;

    [Header("Forces")]
    public float thrustForce;
    public float knockback = 1f;

    [Header("AttackModifiers")]
    public float attackDistance = 10f;
    public float attackDuration = 0.5f;
    public Vector2 attackDirection;
    public float attackRangeAngle = 67.75f;

    [Header("Weapon")]
    public GameObject weapon;
    public GameObject weaponHolder;
    private Transform slashTransform;
    private SpriteRenderer slashSR;
    public Sprite[] slashSprites;
    public Transform[] SlashOrientations;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();

        slashSR = weapon.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        slashTransform = slashSR.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //attack
        AttackHandler();
        //aim
        weaponFaceCursor();

        //combo
        if(comboTimer >= 0)
        {
            comboCountdown();
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        Debug.DrawRay(weaponHolder.transform.position, attackDirection * attackDistance, Color.cyan);

        // Calculate the left and right cone boundaries with ±30 degrees
        Vector2 leftBoundary = Quaternion.Euler(0, 0, attackRangeAngle / 2 + 15) * attackDirection;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, -attackRangeAngle / 2 - 15) * attackDirection;

        // Draw the two boundary rays
        Debug.DrawRay(weaponHolder.transform.position, leftBoundary * attackDistance, Color.yellow);
        Debug.DrawRay(weaponHolder.transform.position, rightBoundary * attackDistance, Color.yellow);
    }

    private void AttackHandler() //CHOOSES WHICH ATTACK TO USE
    {
        attackInput = InputManager.Attack; //Set attack input value [0 or 1]
        if (currentAttack == 3)
        {
            currentAttack = 0;
        }

        if(attackInput == 1 && canAttack)
        {
            Swing();
        }
    }

    private void ChooseAttack()
    {
        //print("choose attack:" + currentAttack);
        SwordAttacks[currentAttack].Invoke();
    }

    //FUNCTIONS to call IEnumerators: used as unityEvents
    public void CallSwing1()
    {
        StartCoroutine(Swing1Feel());
    }
    public void CallSwing2()
    {
        StartCoroutine(Swing2Feel());
    }
    public void CallStab()
    {
        StartCoroutine(StabFeel());
    }


    //SWINGING
    private void Swing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)gameObject.transform.position).normalized;
        playerAnimator.SetFloat("AimHorizontal", attackDirection.x);
        playerAnimator.SetFloat("AimVertical", attackDirection.y);
        playerAnimator.SetTrigger("Slash");
        StartCoroutine(attackFlip());

        //Which attack?
        comboTimer = comboTime;
        ChooseAttack();
        currentAttack++;
        //hit any enemies?
        attackCheck(attackDirection);
    }

    //The feel & rotations & sprites of swing 1
    private IEnumerator Swing1Feel()
    {
        canAttack = false;
        playerMovement.canMove = false;
        weapon.GetComponent<SpriteRenderer>().enabled = true;
        //
        slashSR.enabled = true;
        slashTransform.position = SlashOrientations[0].position;
        slashTransform.rotation = SlashOrientations[0].rotation;
        //
        weapon.transform.localRotation = Quaternion.Euler(0, 0, -45f);
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        playerAnimator.ResetTrigger("Slash");
        slashSR.enabled = false;
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        playerMovement.canMove = true;
        canAttack = true;
    }
    private IEnumerator Swing2Feel()
    {
        canAttack = false;
        playerMovement.canMove = false;
        weapon.GetComponent<SpriteRenderer>().enabled = true;
        //slash setup
        slashSR.enabled = true;
        slashSR.flipY = true;
        slashTransform.position = SlashOrientations[1].position;
        slashTransform.rotation = SlashOrientations[1].rotation;
        //
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 45f);
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        playerAnimator.ResetTrigger("Slash");
        //
        slashSR.enabled = false;
        slashSR.flipY = false;
        //
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        playerMovement.canMove = true;
        canAttack = true;
    }
    private IEnumerator StabFeel()
    {
        canAttack = false;
        playerMovement.canMove = false;
        weapon.GetComponent<SpriteRenderer>().enabled = true;
        attackDistance += 0.5f;
        //
        slashSR.sprite = slashSprites[1];
        slashTransform.position = SlashOrientations[2].position;
        slashTransform.rotation = SlashOrientations[2].rotation;
        slashSR.enabled = true;
        //
        weapon.transform.localRotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce*5, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        playerAnimator.ResetTrigger("Slash");
        //
        slashSR.enabled = false;
        slashSR.sprite = slashSprites[0];
        //
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        playerMovement.canMove = true;
        //
        attackDistance -= 0.5f;
        yield return new WaitForSeconds(0.5f);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        canAttack = true;
        weaponFaceCursor();
    }

    private void weaponFaceCursor()
    {
        if(canAttack)
        {
            //RotateSword
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg - 90f;
            weaponHolder.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void attackCheck(Vector2 attackDir)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackDistance);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy")) {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                Goober gooberScript = hit.GetComponent<Goober>();

                Vector2 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(attackDir, toTarget);

                if (angle <= attackRangeAngle) // Check for objects within a sector
                {
                    //Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                    print("hit");
                    if(currentAttack != 3)
                    {
                        enemyRb.AddForce(attackDir * knockback * 100, ForceMode2D.Impulse);
                    }
                    else
                    {
                        enemyRb.AddForce(attackDir * knockback * 200, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private IEnumerator attackFlip()
    {
        SpriteRenderer weaponSR = weapon.GetComponent<SpriteRenderer>();
        //RenderLayer
        if (Mathf.Abs(attackDirection.y) > Mathf.Abs(attackDirection.x) && attackDirection.y > 0f) //Aiming Up
        {
            weaponSR.sortingOrder = 3;
            if (attackDirection.x < 0f) // *arm to the right is default sprite* more left, nothing happens if more right
            {
                playerMovement.canFlip = false;
                playerSR.flipX = true;
                yield return new WaitForSeconds(attackDuration);
                playerSR.flipX = false;
                playerMovement.canFlip = true;
            }
        }
        else if (Mathf.Abs(attackDirection.y) > Mathf.Abs(attackDirection.x) && attackDirection.y < 0f) //Aiming down
        {
            weaponSR.sortingOrder = 5;
            if (attackDirection.x > 0f) // *arm to the left is default sprite* more right, nothing happens if more right
            {
                playerMovement.canFlip = false;
                playerSR.flipX = true;
                yield return new WaitForSeconds(attackDuration);
                playerSR.flipX = false;
                playerMovement.canFlip = true;
            }
        }
        else
        {
            weaponSR.sortingOrder = 5;
            if (attackDirection.x > 0f) // *arm to the left is default sprite* more right, nothing happens if more right
            {
                playerMovement.canFlip = false;
                playerSR.flipX = true;
                yield return new WaitForSeconds(attackDuration);
                playerSR.flipX = false;
                playerMovement.canFlip = true;
                print("right");
            }
            else
            {
                playerMovement.canFlip = false;
                playerSR.flipX = false;
                yield return new WaitForSeconds(attackDuration);
                playerMovement.canFlip = true;
                print("left");
            }
        }
    }

    private void comboCountdown()
    {
        comboTimer -= Time.deltaTime;

        if(comboTimer < 0)
        {
            comboTimer = 0;
            currentAttack = 0;
        }
    }
}