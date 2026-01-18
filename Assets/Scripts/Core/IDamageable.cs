using UnityEngine;

/// <summary>
/// ダメージを受けられるオブジェクトのインターフェース
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    void TakeDamage(int damage);
    
    /// <summary>
    /// 現在のHP
    /// </summary>
    int CurrentHealth { get; }
    
    /// <summary>
    /// 最大HP
    /// </summary>
    int MaxHealth { get; }
    
    /// <summary>
    /// HPが0かどうか（死亡しているか）
    /// </summary>
    bool IsDead { get; }
}
