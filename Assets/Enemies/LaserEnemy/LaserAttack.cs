using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "LaserAttack", menuName = "Enemy Attacks/Laser Attack")]
public class LaserAttack : EnemyAttack
{
    private LineRenderer lineRenderer;
    public override IEnumerator Attack(Goober goober)
    {
        lineRenderer = goober.GetComponent<LineRenderer>();
        //SETUP
        goober.canAttack = false;
        goober.agent.speed = 0f;
        goober.agent.SetDestination(goober.transform.position);

        goober.attackIndicator.enabled = true;

        yield return new WaitForSeconds(attackIndicatorTime);

        //ATTACK
        goober.attackIndicator.enabled = false;
        lineRenderer.enabled = true;

        goober.directionToPlayer = ((Vector2)goober.player.position - (Vector2)goober.transform.position).normalized;
        //attackDirection = directionToPlayer + pm.movement * 0.75f; //adjusts to where player will be rather than just where they are
        lineRenderer.SetPosition(1, goober.directionToPlayer * attackingRadius * 2);

        Collider2D selfCollider = goober.gameObject.GetComponent<Collider2D>();
        selfCollider.enabled = false;
        int playerLayerMask = 1 << 3; // Correct bitmask for layer 3
        RaycastHit2D hit = Physics2D.Raycast(goober.transform.position, goober.directionToPlayer, attackingRadius * 2, playerLayerMask);

        Debug.DrawRay(goober.transform.position, goober.directionToPlayer * attackingRadius * 2, Color.white, 1f);
        selfCollider.enabled = true;

        if (hit.collider != null && hit.collider.CompareTag("Player") && !goober.pm.isRolling)
        {
            goober.playerHealth.health -= attackDamage;
            goober.pm.canMove = false;
            goober.playerRb.AddForce(goober.directionToPlayer * 3000, ForceMode2D.Force);
        }
        goober.state = 1;
        yield return new WaitForSeconds(0.2f);
        goober.pm.canMove = true;
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(attackCooldown);
        goober.canAttack = true;
    }
}
