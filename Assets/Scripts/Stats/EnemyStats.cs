using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyStats : CharacterStats {

    private Enemy enemy;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void Die()
    {
        base.Die();
        enemy.Died();
       
    }
    public override void TakeHit()
    {
        base.TakeHit();
        enemy.TakeHit();
    }

    public override bool IsAttacking()
    {
        return enemy.IsAttacking();
    }
}
