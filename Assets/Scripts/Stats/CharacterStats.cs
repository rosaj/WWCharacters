
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour {

    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }


    public Stat damage;
    public Stat armor;

    public virtual void Awake()
    {
        CurrentHealth = MaxHealth;

    }
    public virtual void TakeDamage(int damage)
    {
        Debug.Log(gameObject.name + " takes " + damage + " damage, health: "+CurrentHealth);
        damage -= armor.Value;
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            TakeHit();
        }
    }
    public virtual void Die()
    {
        Debug.Log(gameObject.name + " Died");
       
    }
    public virtual void TakeHit()
    {
       // animator.SetTrigger(HitTrigger);
    }
    public abstract bool IsAttacking();

    public virtual int GetDamageValue()
    {
        return damage.Value;
    }
}
