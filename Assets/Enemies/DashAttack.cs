using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DashAttack", menuName = "Enemy Attacks/Dash Attack")]
public class DashAttack : EnemyAttack
{
    public float dashSpeed = 10f; // Adjust as needed
    public float dashDuration = 0.5f; // Adjust duration as needed
    public float overshootDistance;
    [HideInInspector] public bool isDashing;
    public bool useSlashSprite;

    public override void Attack(Goober goober)
    {
        goober.StartCoroutine(ExecuteAttack(goober));
    }

    protected override IEnumerator ExecuteAttack(Goober goober)
    {
        goober.canAttack = false;
        goober.animator.SetBool("Warn", true);
        goober.agent.speed = 0;

        yield return new WaitForSeconds(attackIndicatorTime);
        goober.DashHitPlayerDetectionColl.enabled = true;

        goober.animator.SetBool("Attack", true);
        goober.animator.SetBool("Warn", false);

        isDashing = true;
        if (useSlashSprite)
        {
            Vector2 dirToTarget = goober.player.transform.position - goober.transform.position;
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
            goober.slashSr.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            goober.slashSr.enabled = true;
        }

        goober.GetComponent<Rigidbody2D>().AddForce(goober.directionToPlayer * dashSpeed * 115, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        goober.DashHitPlayerDetectionColl.enabled = false;
        if (useSlashSprite)
        {
            goober.slashSr.enabled = false;
        }

        yield return new WaitForSeconds(dashDuration - 0.2f);
        goober.animator.SetBool("Attack", false);

        yield return new WaitForSeconds(attackCooldown);

        goober.canAttack = true;
        goober.state = 1;
    }
}
