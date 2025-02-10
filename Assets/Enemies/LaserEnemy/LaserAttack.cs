using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "LaserAttack", menuName = "Enemy Attacks/Laser Attack")]
public class LaserAttack : EnemyAttack
{
    private LineRenderer lineRenderer;
    private ParticleSystem hitPS;
    private Animator animator;

    public override IEnumerator Attack(Goober goober)
    {
        lineRenderer = goober.GetComponent<LineRenderer>();
        hitPS = goober.transform.parent.Find("HitParticleEffect").GetComponent<ParticleSystem>();
        animator = goober.GetComponent<Animator>();
        //SETUP
        goober.canAttack = false;
        goober.agent.speed = 0f;
        goober.agent.SetDestination(goober.transform.position);

        goober.attackIndicator.enabled = true;
        animator.SetBool("Warn", true);
        goober.IndicatorLight.enabled = true;

        yield return new WaitForSeconds(attackIndicatorTime);

        //ATTACK
        goober.GetComponent<Rigidbody2D>().AddForce(goober.directionToPlayer * -recievedKnockback*5000);

        //yield return new WaitForSeconds(1);

        goober.attackIndicator.enabled = false;
        animator.SetBool("Warn", false);
        goober.IndicatorLight.enabled = false;

        lineRenderer.enabled = true;
        goober.StartCoroutine(FadeOutLine(lineRenderer, 0.2f));

        goober.directionToPlayer = ((Vector2)goober.player.position - (Vector2)goober.transform.position).normalized;
        
        Collider2D selfCollider = goober.gameObject.GetComponent<Collider2D>();
        selfCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(goober.transform.position, goober.directionToPlayer, attackingRadius);
        Debug.DrawRay(goober.transform.position, goober.directionToPlayer * attackingRadius * 4, Color.white, 1.5f);
        selfCollider.enabled = true;

        if(hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if(!goober.pm.isRolling)
            {
                goober.playerHealth.health -= attackDamage;
                goober.pm.canMove = false;
                goober.playerRb.AddForce(goober.directionToPlayer * 3000, ForceMode2D.Force);

                lineRenderer.SetPosition(0, goober.transform.position);
                lineRenderer.SetPosition(1, hit.point);
                hitPS.gameObject.transform.position = hit.point;
                hitPS.Play();
            }
            else
            {
                selfCollider.enabled = false;
                RaycastHit2D pastPlayerHit = Physics2D.Raycast(hit.point + goober.directionToPlayer * 0.1f, goober.directionToPlayer, 200, ~LayerMask.GetMask("Player"));
                selfCollider.enabled = true;

                if (pastPlayerHit.collider != null)
                {
                    lineRenderer.SetPosition(0, goober.transform.position);
                    lineRenderer.SetPosition(1, pastPlayerHit.point);
                    hitPS.gameObject.transform.position = pastPlayerHit.point;
                    hitPS.Play();
                }
                else
                {
                    // Set a default far endpoint if nothing is hit
                    Vector2 endPoint = (Vector2)hit.point + goober.directionToPlayer * attackingRadius * 4;
                    lineRenderer.SetPosition(0, goober.transform.position);
                    lineRenderer.SetPosition(1, endPoint);
                    hitPS.gameObject.transform.position = endPoint;
                    hitPS.Play();
                }
            }

        }
        else
        {
            lineRenderer.SetPosition(0, goober.transform.position);
            lineRenderer.SetPosition(1, hit.point);
            hitPS.gameObject.transform.position = hit.point;
            hitPS.Play();
        }
        goober.state = 1;
        yield return new WaitForSeconds(0.2f);
        goober.pm.canMove = true;
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(attackCooldown);
        goober.canAttack = true;
    }

    private IEnumerator FadeOutLine(LineRenderer lineRenderer, float fadeDuration)
    {
        Material lineMaterial = lineRenderer.material;
        Color startColor = lineMaterial.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            lineMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lineMaterial.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        lineRenderer.enabled = false;
    }

}