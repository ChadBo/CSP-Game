using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "ProjectileAttack", menuName = "Enemy Attacks/Projectile Attack")]
public class ProjectileAttack : EnemyAttack
{
    public GameObject ProjectilePrefab;

    public override void Attack(Goober goober)
    {
        goober.StartCoroutine(ExecuteAttack(goober));
    }

    protected override IEnumerator ExecuteAttack(Goober goober)
    {
        goober.canAttack = false;
        goober.animator.SetBool("Attack", true);
        goober.agent.speed = 0;

        yield return new WaitForSeconds(attackIndicatorTime);
        
        goober.animator.SetBool("Attack", false);
        GameObject projectile = Instantiate(ProjectilePrefab, goober.transform.position, Quaternion.identity);
        
        float angle = Mathf.Atan2(goober.directionToPlayer.y, goober.directionToPlayer.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+180));

        projectile.GetComponent<Rigidbody2D>().AddForce((goober.directionToPlayer + goober.pm.movement*0.5f).normalized * 300);
        projectile.GetComponent<Projectile>().controllingGoober = goober;

        yield return new WaitForSeconds(attackCooldown);

        goober.canAttack = true;
        goober.state = 1;
    }
}
