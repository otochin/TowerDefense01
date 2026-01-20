using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// キャラクター強化管理システム
/// 各キャラクターの強化レベルを管理し、強化されたステータスを計算する
/// </summary>
public class CharacterUpgradeManager : MonoBehaviour
{
    private static CharacterUpgradeManager instance;
    
    public static CharacterUpgradeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CharacterUpgradeManager>();
            }
            return instance;
        }
    }
    
    /// <summary>
    /// 強化タイプ
    /// </summary>
    public enum UpgradeType
    {
        Health,        // HP強化
        AttackPower,   // 攻撃力強化
        AttackSpeed,   // 攻撃速度強化
        MoveSpeed      // 移動速度強化
    }
    
    /// <summary>
    /// キャラクターの強化データ
    /// </summary>
    [System.Serializable]
    public class CharacterUpgradeData
    {
        public CharacterType characterType;
        public int healthUpgrade = 0;        // HP強化レベル
        public int attackPowerUpgrade = 0;  // 攻撃力強化レベル
        public int attackSpeedUpgrade = 0;  // 攻撃速度強化レベル
        public int moveSpeedUpgrade = 0;    // 移動速度強化レベル
        
        public CharacterUpgradeData(CharacterType type)
        {
            characterType = type;
        }
    }
    
    [Header("強化設定")]
    [Tooltip("HP強化時の増加量")]
    [SerializeField] private int healthUpgradeAmount = 10;
    
    [Tooltip("攻撃力強化時の増加量")]
    [SerializeField] private int attackPowerUpgradeAmount = 5;
    
    [Tooltip("攻撃速度強化時の増加量（秒）")]
    [SerializeField] private float attackSpeedUpgradeAmount = 0.1f;
    
    [Tooltip("移動速度強化時の増加量")]
    [SerializeField] private float moveSpeedUpgradeAmount = 0.5f;
    
    // 各キャラクターの強化データ
    private Dictionary<CharacterType, CharacterUpgradeData> upgradeData = new Dictionary<CharacterType, CharacterUpgradeData>();
    
    /// <summary>
    /// 強化変更イベント
    /// </summary>
    public event System.Action<CharacterType, UpgradeType> OnUpgradeChanged;
    
    private void Awake()
    {
        // シングルトンパターン
        if (instance == null)
        {
            instance = this;
            InitializeUpgradeData();
        }
        else if (instance != this)
        {
            Debug.LogWarning("[CharacterUpgradeManager] Duplicate CharacterUpgradeManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// 強化データを初期化
    /// </summary>
    private void InitializeUpgradeData()
    {
        upgradeData[CharacterType.Warrior] = new CharacterUpgradeData(CharacterType.Warrior);
        upgradeData[CharacterType.Archer] = new CharacterUpgradeData(CharacterType.Archer);
        upgradeData[CharacterType.Mage] = new CharacterUpgradeData(CharacterType.Mage);
        
        Debug.Log("[CharacterUpgradeManager] Upgrade data initialized for all character types.");
    }
    
    /// <summary>
    /// キャラクターを強化する
    /// </summary>
    public void UpgradeCharacter(CharacterType characterType, UpgradeType upgradeType)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            upgradeData[characterType] = new CharacterUpgradeData(characterType);
        }
        
        CharacterUpgradeData data = upgradeData[characterType];
        
        switch (upgradeType)
        {
            case UpgradeType.Health:
                data.healthUpgrade++;
                Debug.Log($"[CharacterUpgradeManager] {characterType} HP upgraded to level {data.healthUpgrade} (+{healthUpgradeAmount * data.healthUpgrade} HP)");
                break;
            case UpgradeType.AttackPower:
                data.attackPowerUpgrade++;
                Debug.Log($"[CharacterUpgradeManager] {characterType} Attack Power upgraded to level {data.attackPowerUpgrade} (+{attackPowerUpgradeAmount * data.attackPowerUpgrade} Attack)");
                break;
            case UpgradeType.AttackSpeed:
                data.attackSpeedUpgrade++;
                Debug.Log($"[CharacterUpgradeManager] {characterType} Attack Speed upgraded to level {data.attackSpeedUpgrade} (+{attackSpeedUpgradeAmount * data.attackSpeedUpgrade} Attack Speed)");
                break;
            case UpgradeType.MoveSpeed:
                data.moveSpeedUpgrade++;
                Debug.Log($"[CharacterUpgradeManager] {characterType} Move Speed upgraded to level {data.moveSpeedUpgrade} (+{moveSpeedUpgradeAmount * data.moveSpeedUpgrade} Move Speed)");
                break;
        }
        
        OnUpgradeChanged?.Invoke(characterType, upgradeType);
    }
    
    /// <summary>
    /// 強化されたHPを取得
    /// </summary>
    public int GetUpgradedHealth(CharacterType characterType, int baseHealth)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            return baseHealth;
        }
        
        return baseHealth + (upgradeData[characterType].healthUpgrade * healthUpgradeAmount);
    }
    
    /// <summary>
    /// 強化された攻撃力を取得
    /// </summary>
    public int GetUpgradedAttackPower(CharacterType characterType, int baseAttackPower)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            return baseAttackPower;
        }
        
        return baseAttackPower + (upgradeData[characterType].attackPowerUpgrade * attackPowerUpgradeAmount);
    }
    
    /// <summary>
    /// 強化された攻撃速度を取得
    /// </summary>
    public float GetUpgradedAttackSpeed(CharacterType characterType, float baseAttackSpeed)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            return baseAttackSpeed;
        }
        
        // 攻撃速度は減少（間隔が短くなる）ので、強化レベル分だけ減算
        float upgradedSpeed = baseAttackSpeed - (upgradeData[characterType].attackSpeedUpgrade * attackSpeedUpgradeAmount);
        return Mathf.Max(0.1f, upgradedSpeed); // 最小値0.1秒
    }
    
    /// <summary>
    /// 強化された移動速度を取得
    /// </summary>
    public float GetUpgradedMoveSpeed(CharacterType characterType, float baseMoveSpeed)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            return baseMoveSpeed;
        }
        
        return baseMoveSpeed + (upgradeData[characterType].moveSpeedUpgrade * moveSpeedUpgradeAmount);
    }
    
    /// <summary>
    /// キャラクターの強化レベルを取得
    /// </summary>
    public CharacterUpgradeData GetUpgradeData(CharacterType characterType)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            upgradeData[characterType] = new CharacterUpgradeData(characterType);
        }
        
        return upgradeData[characterType];
    }
    
    /// <summary>
    /// 強化データをリセット（ゲーム再開時など）
    /// </summary>
    public void ResetUpgrades()
    {
        InitializeUpgradeData();
        Debug.Log("[CharacterUpgradeManager] All upgrades reset.");
    }
}
