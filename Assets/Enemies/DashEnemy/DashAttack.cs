using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DashAttack", menuName = "Enemy Attacks/Dash Attack")]
public class DashAttack : EnemyAttack
{
    private ParticleSystem hitPS;
    private Animator animator;
    private SpriteRenderer slashSr;
    private FaceGameObject slashFacePlayerScript;
    public float indicateTime;
    public float dashSpeed = 10f; // Adjust as needed
    public float dashDuration = 0.5f; // Adjust duration as needed
    float elapsedTime = 0f;
    public float overshootDistance;

    public override IEnumerator Attack(Goober goober)
    {
        animator = goober.GetComponent<Animator>();
        slashSr = goober.transform.GetChild(0).GetComponent<SpriteRenderer>();
        slashFacePlayerScript = slashSr.gameObject.GetComponent<FaceGameObject>();

        goober.canAttack = false;
        animator.SetBool("Attack", true);
        slashFacePlayerScript.enabled = false;
        goober.agent.speed = 0;

        yield return new WaitForSeconds(indicateTime);

        Vector2 dashDestination = SetDashDestination(goober) + goober.pm.movement*goober.pm.moveSpeed;

        slashSr.enabled = true;
        //
        Vector2 dirToTarget = goober.player.transform.position - goober.transform.position;
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        slashSr.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //
        goober.agent.acceleration = 100;
        goober.agent.speed = dashSpeed;
        goober.agent.SetDestination(dashDestination);
        Debug.Log("DASHING NOW");

        yield return new WaitForSeconds(dashDuration); // Dash for a fixed time

        slashSr.enabled = false;
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(attackCooldown);

        goober.canAttack = true;
        goober.state = 1;
        elapsedTime = 0;
    }

    private Vector2 SetDashDestination(Goober goober)
    {
        Debug.Log("SETTING DESTINATION");
        Vector2 directionToPlayer = ((Vector2)goober.player.position - (Vector2)goober.transform.position).normalized;

        goober.GetComponent<Collider2D>().enabled = false;
        RaycastHit2D playerIncluded = Physics2D.Raycast(goober.transform.position, directionToPlayer, attackingRadius + overshootDistance);
        goober.GetComponent<Collider2D>().enabled = true;

        if(playerIncluded.collider != null && playerIncluded.collider.CompareTag("Player")) // GO STRAIGHT TOWARDS PLAYER AND A LITTLE PLAST
        {
            Vector2 dashDestination = (Vector2)goober.transform.position + (goober.directionToPlayer.normalized * (attackingRadius + overshootDistance));
            return dashDestination;
        }

        goober.GetComponent<Collider2D>().enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(goober.transform.position, directionToPlayer, attackingRadius + overshootDistance, ~LayerMask.GetMask("Player"));
        goober.GetComponent<Collider2D>().enabled = true;

        if (hit.collider != null) 
        {
            float gooberToPlayerDist = Vector2.Distance(goober.transform.position, goober.player.position);
            float gooberToHitDist = Vector2.Distance(goober.transform.position, hit.point);

            if (gooberToHitDist < gooberToPlayerDist) // If the hit point is before the player, there's an obstacle
            {
                return hit.point; // Dash should not proceed
            }
            return hit.point;
        }
        else // If the ray hits nothing, dash to the tip of the ray
        {
            Vector2 dashDestination = (Vector2)goober.transform.position + (goober.directionToPlayer.normalized * (attackingRadius + overshootDistance));
            return dashDestination;
        }
    }
}
