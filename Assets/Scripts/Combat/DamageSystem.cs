using UnityEngine;

/// <summary>
/// ダメージシステム
/// ダメージ計算のロジックを提供（クリティカル、属性など）
/// </summary>
public static class DamageSystem
{
    /// <summary>
    /// ダメージを計算（防御力を考慮）
    /// </summary>
    /// <param name="baseDamage">基本ダメージ</param>
    /// <param name="defense">防御力</param>
    /// <returns>実際のダメージ</returns>
    public static int CalculateDamage(int baseDamage, int defense)
    {
        // 防御力を考慮したダメージ計算（最小ダメージは1）
        int actualDamage = Mathf.Max(1, baseDamage - defense);
        return actualDamage;
    }
    
    /// <summary>
    /// クリティカルダメージを計算
    /// </summary>
    /// <param name="baseDamage">基本ダメージ</param>
    /// <param name="criticalChance">クリティカル発生率（0.0～1.0）</param>
    /// <param name="criticalMultiplier">クリティカル倍率</param>
    /// <param name="isCritical">クリティカルが発生したか（出力）</param>
    /// <returns>実際のダメージ</returns>
    public static int CalculateCriticalDamage(int baseDamage, float criticalChance, float criticalMultiplier, out bool isCritical)
    {
        isCritical = Random.Range(0f, 1f) < criticalChance;
        
        if (isCritical)
        {
            return Mathf.RoundToInt(baseDamage * criticalMultiplier);
        }
        
        return baseDamage;
    }
    
    /// <summary>
    /// 完全なダメージ計算（防御力 + クリティカル）
    /// </summary>
    /// <param name="baseDamage">基本ダメージ</param>
    /// <param name="defense">防御力</param>
    /// <param name="criticalChance">クリティカル発生率（0.0～1.0）</param>
    /// <param name="criticalMultiplier">クリティカル倍率</param>
    /// <param name="isCritical">クリティカルが発生したか（出力）</param>
    /// <returns>実際のダメージ</returns>
    public static int CalculateFullDamage(int baseDamage, int defense, float criticalChance, float criticalMultiplier, out bool isCritical)
    {
        // クリティカル計算
        int damageAfterCritical = CalculateCriticalDamage(baseDamage, criticalChance, criticalMultiplier, out isCritical);
        
        // 防御力計算
        int finalDamage = CalculateDamage(damageAfterCritical, defense);
        
        return finalDamage;
    }
    
    /// <summary>
    /// 属性ダメージを計算（将来的な拡張用）
    /// </summary>
    /// <param name="baseDamage">基本ダメージ</param>
    /// <param name="attackType">攻撃属性</param>
    /// <param name="defenseType">防御属性</param>
    /// <returns>実際のダメージ</returns>
    public static int CalculateElementalDamage(int baseDamage, ElementType attackType, ElementType defenseType)
    {
        // 属性相性テーブル（将来的に実装）
        // 現在は基本ダメージを返す
        return baseDamage;
    }
}

/// <summary>
/// 属性タイプ（将来的な拡張用）
/// </summary>
public enum ElementType
{
    None,       // 無属性
    Fire,       // 炎
    Water,      // 水
    Earth,      // 土
    Wind        // 風
}
