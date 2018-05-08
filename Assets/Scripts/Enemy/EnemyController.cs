using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour {

    public EnemyType enemyType;
    public enum EnemyType
    {
        Elemental = 0,
        Goblin = 1,
        Monster = 2,
        Troll = 3,
        Spider = 4,
        Dragon = 5
    }
    

    private static readonly int EnemyTypeFloat = Animator.StringToHash("EnemyType");
    private static readonly int SpeedFloat = Animator.StringToHash("Speed");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int AttackTypeFloat = Animator.StringToHash("AttackType");
    private static readonly int DieTrigger = Animator.StringToHash("Die");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");
    private static readonly int KnockbackTrigger = Animator.StringToHash("Knockback");
    private static readonly int AttackState = Animator.StringToHash("Attack");
    private static readonly int KnockbackState = Animator.StringToHash("Knockback");



    private static readonly Dictionary<EnemyType, int> EnemyAttackCount = new Dictionary<EnemyType, int>()
    {
        { EnemyType.Elemental,  1},
        { EnemyType.Goblin,     6},
        { EnemyType.Monster,    2},//ima jos napad nogon, on je na 3. poziciji, ali se ne koristi
        { EnemyType.Troll,      7},
        { EnemyType.Spider,     1},
        { EnemyType.Dragon,     4}
    };


    private Animator animator;
    private bool isDead = false;

    void Start () {
        animator = GetComponent<Animator>();
        animator.SetFloat(EnemyTypeFloat, (int)enemyType);
        
    }



    public void Attack()
    {
        if (isDead) return;
        var eac = EnemyAttackCount[enemyType];
        var animIndex = Random.Range(0, eac);

        animator.SetFloat(AttackTypeFloat, animIndex);
        animator.SetTrigger(AttackTrigger);
    }
    public void Die()
    {
        if (isDead) return;
        animator.SetTrigger(DieTrigger);
        isDead = true;

    }
  
    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == AttackState;
    }
    public bool CanAttack()
    { 
        int currentState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        int nextState = animator.GetNextAnimatorStateInfo(0).shortNameHash;

        return currentState != AttackState && currentState != KnockbackState &&
               nextState != AttackState && nextState != KnockbackState;
    }
    public bool IsDead()
    {
        return isDead;
    }
    public void SetSpeed(float speed)
    {
        animator.SetFloat(SpeedFloat, speed/2);
        
    }
    public void KnockBack()
    {
        if (isDead) return;
        animator.ResetTrigger(HitTrigger);
        if(animator.GetCurrentAnimatorStateInfo(0).shortNameHash != KnockbackState)
        {
            animator.applyRootMotion = true;
            animator.SetTrigger(KnockbackTrigger);
        }
    }
   
    public void Hit()
    {
        animator.applyRootMotion = false;
        animator.SetTrigger(HitTrigger);
    }
}
