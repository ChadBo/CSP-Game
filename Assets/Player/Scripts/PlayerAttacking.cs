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

    public bool canAttack = true;
    [Header("Combo")]
    public int currentAttack = 0;
    public UnityEvent[] SwordAttacks;
    public float comboTime = 0.45f;
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

    [Header("HoldModifiers")]
    public float maxHoldTime;
    public float currentHoldTime;
    public float requiredHoldTime;
    public float maxHoldScreenShakeStrength;
    public float currentHoldScreenShakeStrength = 0f;

    [Header("Weapon")]
    public GameObject weapon;
    public GameObject weaponHolder;
    public SpriteRenderer pointer;
    [HideInInspector] public SpriteRenderer weaponSR;
    private Transform slashTransform;
    public Sprite[] slashSprites;

    [Header("Hit Material")]
    public Shader hitShader;
    private Material hitMaterial;
    public MaterialPropertyBlock hitMaterialPropertyBlock;
    public bool hitEffectEnabled = false;

    private SwordKeyHandler currSwordKeyHandler;
    public bool shouldUpdateSwordLayer;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        animator = playerSprite.GetComponent<Animator>();
        playerSR = playerSprite.GetComponent<SpriteRenderer>();

        weaponSR = weapon.GetComponent<SpriteRenderer>();

        hitMaterial = new Material(hitShader);
        hitMaterialPropertyBlock = new MaterialPropertyBlock();

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
        flipAttacking();

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
        if(playerSR.flipX == true)
        {
            pm.canFlip = false;
            playerSR.flipX = false; //TODO MIGHT BE CAUSE OF POTENTIAL MIRROR BAD STUFF WIHT NEW ART
        }

        //Which attack?
        comboTimer = comboTime;
        ChooseAttack();
    }

    //The feel & rotations & sprites of swing 1
    private IEnumerator Swing1()
    {
        animator.SetBool("Slash", true);
        SetupSwing();
        //
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash", false);
        yield return new WaitForSeconds(0.1f);
        ResetSwingVariables();
    }
    private IEnumerator Swing2()
    {
        animator.SetBool("Slash2", true);
        SetupSwing();
        //
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Slash2", false);

        yield return new WaitForSeconds(0.1f);
        ResetSwingVariables();
    }
    private void SetupSwing()
    {
        canAttack = false;
        pm.canMove = false;
        //
        pointer.enabled = false;
    }
    private void ResetSwingVariables()
    {
        pm.canMove = true;
        canAttack = true;
        pm.canFlip = true;
        pointer.enabled = true;
    }

    private IEnumerator Stab()
    {
        animator.SetBool("Stab", true);
        SetupSwing();
        attackDistance *= 2;
        attackRangeAngle *= 0.25f;
        //
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg - 90f;
        weaponHolder.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce * 3, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        weaponSR.enabled = true;
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Stab", false);
        weaponSR.enabled = false;
        //
        yield return new WaitForSeconds(0.2f);
        ResetSwingVariables();
        canAttack = false; // set false to counter reseSwingVar() for full attack cooldown
        yield return new WaitForSeconds(0.4f);
        canAttack = true;
        //
        attackRangeAngle *= 4f;
        attackDistance /= 2;
        yield return new WaitForSeconds(0.5f);
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)weaponHolder.transform.position).normalized;
        
        weaponFaceCursor();
    }
    private IEnumerator HoldStab()
    {
        canAttack = false;
        pm.canMove = false;
        weaponSR.enabled = true;
        attackDistance += 0.5f;
        attackRangeAngle *= 0.25f;
        animator.SetBool("Tease", false);
        animator.SetBool("Stab", true);
        //
        weapon.transform.localPosition = new Vector3(0, 0.37f, 0);
        //
        weapon.transform.localRotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.AddForce(attackDirection * thrustForce * 7, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        //
        if (KeyHoleCheck(attackDirection) == false)
        {
            //resetting values
            yield return new WaitForSeconds(0.1f);
            weapon.transform.localRotation = Quaternion.identity;
            weaponSR.enabled = false;
            pm.canMove = true;
            pm.moveSpeed = pm.walkSpeed;
            pm.canRoll = true;
            animator.SetBool("Stab", false);
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
        else
        {
            StartCoroutine(OnEnterKeyHole());
        }
        InputManager.IsTapAttack = true;
    }

    private void StartHoldAttack()
    {
        canAttack = false;
        pm.moveSpeed = holdAttackMoveSpeed;
        pm.canRoll = false;
        pm.canFlip = false;
        pointer.enabled = true;
        isHolding = true;
        animator.SetBool("Tease", true);
        weaponSR.sortingOrder = playerSR.sortingOrder - 1;
        //weapon.transform.localPosition = new Vector3(0, 0.1f, 0);
    }

    private void EndHoldAttack() 
    {
        Debug.Log("HOLDING");
        currentHoldTime += Time.deltaTime;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        attackDirection = (mousePos - (Vector2)gameObject.transform.position).normalized;
        animator.SetFloat("AimHorizontal", attackDirection.x);
        animator.SetFloat("AimVertical", attackDirection.y);
        flipAttacking();

        currentHoldScreenShakeStrength = Mathf.Lerp(currentHoldScreenShakeStrength, maxHoldScreenShakeStrength, Time.deltaTime);
        ScreenShakeController.instance.StartShake(0.1f, currentHoldScreenShakeStrength);
        //
        if (currentHoldTime >= maxHoldTime || (attackInput == 0 && currentHoldTime >= requiredHoldTime))
        {
            currentHoldTime = 0;
            currentHoldScreenShakeStrength = 0;
            isHolding = false;
            StartCoroutine(HoldStab());
            currentAttack = 3;
            weaponFaceCursor();
            attackCheck(attackDirection);
            currentAttack = 0;
        }
        else if (attackInput == 0 && currentHoldTime < requiredHoldTime)
        {
            canAttack = true;
            pm.moveSpeed = pm.walkSpeed;
            pm.canRoll = true;
            pm.canFlip = true;
            pointer.enabled = true;
            isHolding = false;
            animator.SetBool("Tease", false);
            weaponSR.sortingOrder = playerSR.sortingOrder + 1;
            //DFGHKJLKLKJHGHFCCHKJLK:LFJASJNLKFLSAJL<MJNKFHBASHJFBKHJASL:JHHVGJBLKJJVHB:LASKLFBJHASJJLKSABFKAJLSNFHJABSJFHBALSFNKJBSAFHKBS
        }
    }

    private void flipAttacking()
    {
        if(attackDirection.x > 0 && attackDirection.x > Mathf.Abs(attackDirection.y))
        {
            playerSR.flipX = true;
        }
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
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, ~LayerMask.GetMask("IgnoreRaycast"));

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log(hit.name);
                Vector2 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(attackDir, toTarget);
                if (angle <= attackRangeAngle) // If it hits an enemy in a slash range
                {
                    Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                    EnemyHealthManager enemyHealth = hit.GetComponent<EnemyHealthManager>();
                    EnemyHit(attackDir, enemyRb, enemyHealth);

                    ApplyHitEffect(true, hit.GetComponent<SpriteRenderer>());

                    if (hit.gameObject.layer == LayerMask.NameToLayer("Dasher"))
                    {
                        Goober gooberScript = hit.GetComponent<Goober>();
                        if (gooberScript != null && gooberScript.attackBehavior is DashAttack dashAttack)
                        {
                            if (dashAttack.isDashing)
                            {
                                Debug.Log("Perfect Hit");
                                enemyHealth.life = 0;
                            }
                        }
                    }
                }
            }
        }
    }


    private void EnemyHit(Vector2 attackDir, Rigidbody2D enemyRb, EnemyHealthManager enemyHealth)
    {
        ScreenShakeController.instance.StartShake(0.3f, 1f);
        if (enemyHealth.knockback > 0)
        {
            enemyRb.GetComponent<Goober>().gotHitPS.Play();
            if (currentAttack != 2)
            {
                enemyRb.AddForce(attackDir * enemyHealth.knockback * 10, ForceMode2D.Impulse);
                enemyHealth.life -= slashDamage;
            }
            else
            {
                enemyRb.AddForce(attackDir * enemyHealth.knockback * 20, ForceMode2D.Impulse);
                enemyHealth.life -= stabDamage;
            }
        }
        else //if they have negative knockback, they should hit enemy and fly backwards like hitting a block of stone
        {
            if (currentAttack != 3)
            {
                rb.AddForce(attackDir * enemyHealth.knockback * 10, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(attackDir * enemyHealth.knockback * 20, ForceMode2D.Impulse);
            }
        }
    }

    private bool KeyHoleCheck(Vector2 attackDir)
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        RaycastHit2D keyHit = Physics2D.Raycast(transform.position, attackDir, attackDistance, 9);
        gameObject.GetComponent<Collider2D>().enabled = true;

        if (keyHit.collider != null && keyHit.collider.CompareTag("KeyHole"))
        {
            currSwordKeyHandler = keyHit.collider.GetComponent<SwordKeyHandler>();
            print("KEY");
            return true;
        }
        return false;
    }

    private IEnumerator OnEnterKeyHole() {
        pm.canMove = false;
        canAttack = false;
        rb.velocity = new Vector2(0, 0);
        shouldUpdateSwordLayer = false;
        weaponSR.sortingOrder = playerSR.sortingOrder - 1;
        gameObject.transform.position = new Vector2(currSwordKeyHandler.transform.position.x, currSwordKeyHandler.transform.position.y - 0.5f);
        weaponHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        currSwordKeyHandler.ShowTopPillarOnTopOfSword(true);

        yield return new WaitForSeconds(3f);
        //
        currSwordKeyHandler.ShowTopPillarOnTopOfSword(false);

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
        if(!shouldUpdateSwordLayer) { return; }

        if(attackDirection.y > 0 && attackDirection.x > 0)
        {
            weaponSR.sortingOrder = playerSR.sortingOrder - 1;
        }
        else if (attackDirection.y > 0 && attackDirection.x < 0)
        {
            weaponSR.sortingOrder = playerSR.sortingOrder - 1;
        }
        else
        {
            weaponSR.sortingOrder = playerSR.sortingOrder + 1;
        }
    }

    private void ApplyHitEffect(bool enableEffect, SpriteRenderer enemySR)
    {
        if (enemySR == null) return;

        if (hitMaterialPropertyBlock == null)
            hitMaterialPropertyBlock = new MaterialPropertyBlock();

        // Get the current property block
        enemySR.GetPropertyBlock(hitMaterialPropertyBlock);

        // Set the boolean property for the shader
        hitMaterialPropertyBlock.SetFloat("_UseMainTexColor", enableEffect ? 0.0f : 1.0f);

        // Apply it back to the material
        enemySR.SetPropertyBlock(hitMaterialPropertyBlock);
        StartCoroutine(ResetHitEffect(enemySR));
    }


    // Coroutine to reset effect after delay
    private IEnumerator ResetHitEffect(SpriteRenderer enemyRenderer)
    {
        yield return new WaitForSeconds(0.3f);
        ApplyHitEffect(false, enemyRenderer);
    }
}