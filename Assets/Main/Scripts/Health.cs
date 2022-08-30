using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable, InlineProperty]
public class Health
{
    [ReadOnly]
    public int health;
    [Min(-1), InfoBox("Remember that this doesn't need to be a huge number. Setting it to -1 makes them invulnerable.")]
    public int maxHealth = -1;
    [FoldoutGroup("Events")]
    public UnityEvent initializeEvent, hurtEvent, dieEvent;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        initializeEvent.Invoke();
    }
    
    public void SetHealthToMax()
    {
        health = maxHealth;
    }

    public void Damage(int amount)
    {
        if (health < 0)
            return;
        
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            Die();          
        }
        else
            Hurt();
    }

    public void Heal(int amount)
    {
        if (health < 0)
            return;
        
        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    public void Hurt()
    {
        // This method is called when the object loses health.
        // This is where things like animations and sounds should be played.

        hurtEvent.Invoke();
    }

    public virtual void Die()
    {
        // This method is called when the object runs out of health.
        dieEvent.Invoke();
    }
}
