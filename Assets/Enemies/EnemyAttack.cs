using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : ScriptableObject
{
    [Header("Attack")]
    public float attackingRadius;
    public float attackDamage;
    public float attackIndicatorTime;
    public float attackCooldown;
    public float recievedKnockback; //MAKE NEGATIVE TO INFLICT ON PLAYER

    public virtual void Attack(Goober goober)
    {
        goober.StartCoroutine(ExecuteAttack(goober));
    }

    // Subclasses implement this instead
    protected abstract IEnumerator ExecuteAttack(Goober goober);
}
