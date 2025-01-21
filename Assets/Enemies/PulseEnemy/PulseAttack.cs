using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "PulseAttack", menuName = "Enemy Attacks/Pulse Attack")]
public class PulseAttack : EnemyAttack
{
    private LineRenderer lineRenderer;
    private Animator animator;
    private Animator pulseAnimator;

    public override IEnumerator Attack(Goober goober)
    {
        //GETTING COMPONENTS
        animator = goober.GetComponent<Animator>();
        pulseAnimator = goober.attackIndicator.GetComponent<Animator>();
        pulseAnimator.enabled = true;

        //SETUP
        goober.canAttack = false;
        goober.agent.speed = 0f;
        goober._navMeshAgent.enabled = false;

        goober.attackIndicator.enabled = true;
        animator.SetTrigger("Warn");
        pulseAnimator.SetTrigger("Indicate");
        recievedKnockback *= 0.5f;

        yield return new WaitForSeconds(attackIndicatorTime);

        //ATTACK
        animator.SetTrigger("Attack");
        pulseAnimator.SetTrigger("Expand");

        goober.directionToPlayer = ((Vector2)goober.player.position - (Vector2)goober.transform.position).normalized;

        Collider2D selfCollider = goober.gameObject.GetComponent<Collider2D>();
        selfCollider.enabled = false;
        int playerLayerMask = 1 << 3; // Correct bitmask for layer 3
        RaycastHit2D hit = Physics2D.Raycast(goober.transform.position, goober.directionToPlayer, attackingRadius, playerLayerMask);

        Debug.DrawRay(goober.transform.position, goober.directionToPlayer * attackingRadius, Color.white, 1f);
        selfCollider.enabled = true;
        Debug.Log("attacked");
        if (hit.collider != null && hit.collider.CompareTag("Player") && !goober.pm.isRolling)
        {
            goober.playerHealth.health -= attackDamage;
            goober.pm.canMove = false;
            goober.playerRb.AddForce(goober.directionToPlayer * 3000, ForceMode2D.Force);
        }
        else if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            Debug.Log(hit.collider.name);
        }
        recievedKnockback *= 2f;
        yield return new WaitForSeconds(0.2f);
        goober.attackIndicator.enabled = false;
        pulseAnimator.SetTrigger("Indicate");
        pulseAnimator.enabled = false;
        goober.pm.canMove = true;
        yield return new WaitForSeconds(0.3f);
        animator.SetTrigger("Idle");
        goober.state = 1;
        goober._navMeshAgent.enabled = true;
        yield return new WaitForSeconds(attackCooldown);
        goober.canAttack = true;
    }
}