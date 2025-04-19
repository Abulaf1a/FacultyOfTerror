public interface IDamageable{
    void TakeDamage(int damage); 
    void Heal(int heal);
    int GetHealth();
    int GetMaxHealth();
    bool IsDead();
    void Die();
}