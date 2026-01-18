using UnityEngine;
using System;

/// <summary>
/// HP管理システム
/// </summary>
public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnHealthDepleted;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        
        // 既に死亡している場合は何もしない（ただし、HPが0になった瞬間は処理する）
        bool wasDead = IsDead;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // HPが0になった時（または、0以下になった時）にイベントを発火
        // 既に死亡していなかった場合のみ発火（重複発火を防ぐ）
        if (currentHealth <= 0 && !wasDead)
        {
            Debug.Log($"[HealthSystem] OnHealthDepleted event firing. currentHealth: {currentHealth}, wasDead: {wasDead}, subscribers: {(OnHealthDepleted?.GetInvocationList().Length ?? 0)}");
            OnHealthDepleted?.Invoke();
        }
        else if (currentHealth <= 0 && wasDead)
        {
            Debug.LogWarning($"[HealthSystem] HP is 0 but wasDead was true. OnHealthDepleted event not fired. currentHealth: {currentHealth}");
        }
    }
    
    /// <summary>
    /// HPを回復する
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// HPを最大値に設定
    /// </summary>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// HPが0かどうか
    /// </summary>
    public bool IsDead => currentHealth <= 0;
}
