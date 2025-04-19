using Godot;
using System;

public partial class EnemyActor : BaseActor, IDamageable
{


    /// <summary>
    /// Inherited by all enemy actors, contains damage properties and methods.
    /// </summary>
	protected EnemyStateEnum enemyState = EnemyStateEnum.ALIVE;

    protected int damage;

    protected int health = 100; // Default health value

    protected int maxHealth;

    public void Die()
    {
        GD.Print("Enemy is dead");
        // Emit signal for enemy death.
        enemyState = EnemyStateEnum.DEAD;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(int heal)
    {
    }

    public bool IsDead()
    {
        if(enemyState == EnemyStateEnum.DEAD) return true;
        return false; 
    }

    public void TakeDamage(int damage)
    {
        health -= damage; 
        // GD.Print("Enemy damaged: health = " + health);
        if (health <= 0)
        {
            Die();
        }
    }
}