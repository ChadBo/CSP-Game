using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "PulseAttack", menuName = "Enemy Attacks/Pulse Attack")]
public class PulseAttack : EnemyAttack
{
    [SerializeField] private Sprite[] pulseSprites;
    [SerializeField] private Sprite[] bodySprites;


    public override IEnumerator Attack(Goober goober)
    {
        //GETTING COMPONENTS

        //SETUP
        goober.canAttack = false;
        goober.agent.speed = 0f;

        goober.attackIndicator.enabled = true;
        goober.sr.sprite = bodySprites[1];
        goober.attackIndicator.sprite = pulseSprites[0];
        goober.IndicatorLight.enabled = true;

        goober.localKnockback *= 0.5f;

        yield return new WaitForSeconds(attackIndicatorTime);

        //ATTACK
        goober.StartCoroutine(attackSpriteHandler(goober));

        goober.directionToPlayer = ((Vector2)goober.player.position - (Vector2)goober.transform.position).normalized;

        Collider2D selfCollider = goober.gameObject.GetComponent<Collider2D>();
        selfCollider.enabled = false;
        int playerLayerMask = 1 << 3; // Correct bitmask for layer 3
        RaycastHit2D hit = Physics2D.Raycast(goober.transform.position, goober.directionToPlayer, attackingRadius, playerLayerMask);

        Debug.DrawRay(goober.transform.position, goober.directionToPlayer * attackingRadius, Color.white, 1f);
        selfCollider.enabled = true;
        if (hit.collider != null && hit.collider.CompareTag("Player") && !goober.pm.isRolling)
        {
            ScreenShakeController.instance.StartShake(0.5f, 3f);
            goober.playerHealth.health -= attackDamage;
            goober.pm.canMove = false;
            goober.playerRb.AddForce(goober.directionToPlayer * 3000, ForceMode2D.Force);
        }

        goober.localKnockback *= 2f;
        yield return new WaitForSeconds(0.2f);
        goober.pm.canMove = true;
        yield return new WaitForSeconds(0.3f);
        goober.attackIndicator.enabled = false;
        goober.IndicatorLight.enabled = false;
        goober.sr.sprite = bodySprites[0];
        goober.state = 1;

        //goober.agent.enabled = true;
        goober.agent.speed = goober.chaseSpeed;

        yield return new WaitForSeconds(0.1f);
        Debug.Log(goober.transform.position.y);
        yield return new WaitForSeconds(attackCooldown);
        goober.canAttack = true;
    }

    public IEnumerator attackSpriteHandler(Goober goober)
    {
        goober.sr.sprite = bodySprites[2];
        goober.IndicatorLight.intensity = 1f;
        goober.attackIndicator.sprite = pulseSprites[1];
        yield return new WaitForSeconds(0.1f);
        goober.sr.sprite = bodySprites[3];
        goober.attackIndicator.sprite = pulseSprites[2];
    }
}