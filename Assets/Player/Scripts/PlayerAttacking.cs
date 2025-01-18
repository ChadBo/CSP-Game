using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttacking : MonoBehaviour
{
    private PlayerMovement pm;
    private Rigidbody2D rb;
    public GameObject playerSprite;
    private Animator animator;
    private SpriteRenderer playerSR;
    private float attackInput;

    [HideInInspector] public bool canAttack = true;
    [Header("Combo")]
    public int currentAttack = 0;
    public UnityEvent[] SwordAttacks;
    public float comboTime = 0.7f;
    public float comboTimer = 0f;

    [Header("Forces")]
    public float thrustForce;

    [Header("AttackModifiers")]
    public float attackDistance = 10f;
    public float attackDuration = 0.5f;
    public Vector2 attackDirection;
    public float attackRangeAngle = 67.75f;
    public int slashDamage;
    public int stabDamage;
    public int holdAttackDamage;
    public float holdAttackRange;
    public float holdAttackMoveSpeed;
    private bool isHolding = false;

    [Header("Weapon")]
    public GameObject weapon;
    public GameObject weaponHolder;
    private SpriteRenderer weaponSR;
    private Transform slashTransform;
    private SpriteRenderer slashSR;
    public Sprite[] slashSprites;
    public Transform[] SlashOrientations;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        animator = playerSprite.GetComponent<Animator>();
        playerSR = playerSprite.GetComponent<SpriteRenderer>();

        weaponSR = weapon.GetComponent<SpriteRenderer>();
        slashSR = weapon.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        slashTransform = slashSR.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //attack
        AttackHandler();
        if(isHolding)
        {
            EndHoldAttack();
        }
        //aim
        weaponFaceCursor();
        swordLayerUpdate();

        //combo
        if (comboTimer >= 0)
        {
            comboCountdown();
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        DrawAttackingGizmos();
    }

    private void DrawAttackingGizmos()
    {
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

        if (attackInput == 1 && canAttack)
        {
            Swing();
        }
    }

    private void ChooseAttack()
    {
        if (InputManager.IsTapAttack)
        {
            SwordAttacks[currentAttack].Invoke();
            //hit any enemies?
            attackCheck(attackDirection);
            currentAttack++;
        }
        else if (!InputManager.IsTapAttack)
        {
            StartHoldAttack();
        }
    }

    //FUNCTIONS to call IEnumerators: used as unityEvents
    public void CallSwing1()
    {
        StartCoroutine(Swing1());
    }
    public void CallSwing2()
    {
        StartCoroutine(Swing2());
    }
    public void CallStab()
    {
        StartCoroutine(Stab());
    }

    //SWINGING
    private void Swing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)gameObject.transform.position).normalized;
        animator.SetFloat("AimHorizontal", attackDirection.x);
        animator.SetFloat("AimVertical", attackDirection.y);
        animator.SetBool("Slash", true);
        if(playerSR.flipX == true)
        {
            pm.canFlip = false;
            playerSR.flipX = false;
        }

        //Which attack?
        comboTimer = comboTime;
        ChooseAttack();
    }

    //The feel & rotations & sprites of swing 1
    private IEnumerator Swing1()
    {
        canAttack = false;
        pm.canMove = false;
        weaponSR.enabled = true;
        //
        slashSR.enabled = true;
        slashTransform.position = SlashOrientations[0].position;
        slashTransform.rotation = SlashOrientations[0].rotation;
        //
        weapon.transform.localRotation = Quaternion.Euler(0, 0, -45f);
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash", false);
        slashSR.enabled = false;
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weaponSR.enabled = false;
        pm.canMove = true;
        canAttack = true;
        pm.canFlip = true;
    }
    private IEnumerator Swing2()
    {
        canAttack = false;
        pm.canMove = false;
        weaponSR.enabled = true;
        //
        slashSR.enabled = true;
        slashSR.flipY = true;
        slashTransform.position = SlashOrientations[1].position;
        slashTransform.rotation = SlashOrientations[1].rotation;
        //
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 45f);
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash", false);
        //
        slashSR.enabled = false;
        slashSR.flipY = false;
        //
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weaponSR.enabled = false;
        pm.canMove = true;
        canAttack = true;
        pm.canFlip = true;
    }
    private IEnumerator Stab()
    {
        print(currentAttack);
        canAttack = false;
        pm.canMove = false;
        weaponSR.enabled = true;
        attackDistance += 0.5f;
        attackRangeAngle *= 0.25f;
        //
        slashSR.sprite = slashSprites[1];
        slashTransform.position = SlashOrientations[2].position;
        slashTransform.rotation = SlashOrientations[2].rotation;
        slashSR.enabled = true;
        //
        weapon.transform.localRotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce * 5, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash", false);
        //
        slashSR.enabled = false;
        slashSR.sprite = slashSprites[0];
        //
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weaponSR.enabled = false;
        pm.canMove = true;
        //
        attackRangeAngle *= 4f;
        attackDistance -= 0.5f;
        yield return new WaitForSeconds(0.5f);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        canAttack = true;
        pm.canFlip = true;
        weaponFaceCursor();
    }
    private IEnumerator HoldStab()
    {
        canAttack = false;
        pm.canMove = false;
        weaponSR.enabled = true;
        attackDistance += 0.5f;
        attackRangeAngle *= 0.25f;
        //
        slashSR.sprite = slashSprites[1];
        slashTransform.position = SlashOrientations[2].position;
        slashTransform.rotation = SlashOrientations[2].rotation;
        weapon.transform.localPosition = new Vector3(0, 0.37f, 0);
        
        slashSR.enabled = true;
        //
        weapon.transform.localRotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce * 5, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash", false);
        //
        slashSR.enabled = false;
        slashSR.sprite = slashSprites[0];
        //
        yield return new WaitForSeconds(0.1f);
        weapon.transform.localRotation = Quaternion.identity;
        weaponSR.enabled = false;
        pm.canMove = true;
        pm.moveSpeed = pm.walkSpeed;
        pm.canRoll = true;
        //
        attackRangeAngle *= 4f;
        attackDistance -= 0.5f;
        yield return new WaitForSeconds(0.5f);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        canAttack = true;
        pm.canFlip = true;
        weaponFaceCursor();
        InputManager.IsTapAttack = true;
    }

    private void StartHoldAttack()
    {
        canAttack = false;
        pm.moveSpeed = holdAttackMoveSpeed;
        pm.canRoll = false;
        pm.canFlip = false;
        weaponSR.enabled = true;
        isHolding = true;
        weaponSR.sortingOrder = playerSR.sortingOrder - 1;
        weapon.transform.localPosition = new Vector3(0, 0.1f, 0);
    }
    private void EndHoldAttack()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)gameObject.transform.position).normalized;
        animator.SetFloat("AimHorizontal", attackDirection.x);
        animator.SetFloat("AimVertical", attackDirection.y);
        //
        if (attackInput == 0)
        {
            isHolding = false;
            StartCoroutine(HoldStab());
            //hit any enemies?
            currentAttack = 3;
            attackCheck(attackDirection);
            currentAttack = 0;
        }
        //
    }

    private void weaponFaceCursor()
    {
        if (canAttack || isHolding)
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
            if (hit.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                Goober gooberScript = hit.GetComponent<Goober>();

                Vector2 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(attackDir, toTarget);

                if (angle <= attackRangeAngle) //If it hits an enemy in a slash range
                {
                    EnemyHit(attackDir, enemyRb, gooberScript);
                }
            }
        }
    }

    private void EnemyHit(Vector2 attackDir, Rigidbody2D enemyRb, Goober gooberScript)
    {
        if (gooberScript.attackBehavior.recievedKnockback > 0)
        {
            if (currentAttack != 2)
            {
                enemyRb.AddForce(attackDir * gooberScript.attackBehavior.recievedKnockback * 100, ForceMode2D.Impulse);
                gooberScript.life -= slashDamage;
            }
            else
            {
                enemyRb.AddForce(attackDir * gooberScript.attackBehavior.recievedKnockback * 200, ForceMode2D.Impulse);
                gooberScript.life -= stabDamage;
            }
        }
        else //if they have negative knockback, they should hit enemy and fly backwards like hitting a block of stone
        {
            if (currentAttack != 3)
            {
                rb.AddForce(attackDir * gooberScript.attackBehavior.recievedKnockback * 10, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(attackDir * gooberScript.attackBehavior.recievedKnockback * 20, ForceMode2D.Impulse);
            }
        }
    }

    private void comboCountdown()
    {
        comboTimer -= Time.deltaTime;

        if (comboTimer < 0)
        {
            comboTimer = 0;
            currentAttack = 0;
        }
    }

    private void swordLayerUpdate()
    {
        if(attackDirection.y > 0 && attackDirection.x > 0)
        {
            weapon.gameObject.GetComponent<SpriteRenderer>().sortingOrder = playerSR.sortingOrder - 1;
        }
        else if (attackDirection.y > 0 && attackDirection.x < 0)
        {
            weapon.gameObject.GetComponent<SpriteRenderer>().sortingOrder = playerSR.sortingOrder + 1;
        }
        else
        {
            weapon.gameObject.GetComponent<SpriteRenderer>().sortingOrder = playerSR.sortingOrder + 1;
        }
    }
}