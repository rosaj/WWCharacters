using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CombatWeapon : MonoBehaviour {

    CharacterStats stats;
 //   MoveBehaviour moveBehaviour;
 //   EnemyController enemyController;

    public delegate void GameObjectAction(GameObject source, GameObject enemy);
    public GameObjectAction OnDamage;



    public void Start()
    {
        if(!stats)
            stats = GetComponentInParent<CharacterStats>();
        /*
        if (!moveBehaviour)
            moveBehaviour = GetComponentInParent<MoveBehaviour>();
        if (!enemyController)
            enemyController = GetComponentInParent<EnemyController>();*/
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
      //  Debug.Log("Collision with " + other.gameObject.name);
        var characterStats = other.gameObject.GetComponentInParent<CharacterStats>();
        if (characterStats)
        {
        //    bool isAttacking = false;
            if(characterStats.GetType() != stats.GetType())
            {
                if (stats.IsAttacking())
                {
                    characterStats.TakeDamage(stats.GetDamageValue());
                    if (OnDamage != null) OnDamage.Invoke(gameObject, other.gameObject);
                }
                //    if (moveBehaviour) isAttacking = moveBehaviour.GetAttacks;
                //   else isAttacking = enemyController.IsAttacking();

            }
            
          /*  if (isAttacking)
            {
                characterStats.TakeDamage(stats.damage.Value);
                if (OnDamage != null) OnDamage.Invoke(gameObject, other.gameObject);
            }*/
        }
    }
    public CharacterStats Stats
    {
        get
        {
            return stats;
        }

        set
        {
            stats = value;
        }
    }
    /*
    public MoveBehaviour MoveBehaviour
    {
        get
        {
            return moveBehaviour;
        }

        set
        {
            moveBehaviour = value;
        }
    }

    public EnemyController EnemyController
    {
        get
        {
            return enemyController;
        }

        set
        {
            enemyController = value;
        }
    }
*/
}
