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
    
    /// <summary>
    /// キャラクターごとの強化設定
    /// </summary>
    [System.Serializable]
    public class CharacterUpgradeSettings
    {
        [Header("キャラクタータイプ")]
        public CharacterType characterType;
        
        [Header("強化量設定")]
        [Tooltip("HP強化時の増加量")]
        public int healthUpgradeAmount = 10;
        
        [Tooltip("攻撃力強化時の増加量")]
        public int attackPowerUpgradeAmount = 5;
        
        [Tooltip("攻撃速度強化時の増加量（秒）")]
        public float attackSpeedUpgradeAmount = 0.1f;
        
        [Tooltip("移動速度強化時の増加量")]
        public float moveSpeedUpgradeAmount = 0.5f;
    }
    
    [Header("強化設定")]
    [Tooltip("各キャラクターごとの強化設定")]
    [SerializeField] private CharacterUpgradeSettings[] characterUpgradeSettings = new CharacterUpgradeSettings[]
    {
        new CharacterUpgradeSettings { characterType = CharacterType.Warrior, healthUpgradeAmount = 10, attackPowerUpgradeAmount = 3, attackSpeedUpgradeAmount = 0.1f, moveSpeedUpgradeAmount = 0.5f },
        new CharacterUpgradeSettings { characterType = CharacterType.Archer, healthUpgradeAmount = 5, attackPowerUpgradeAmount = 2, attackSpeedUpgradeAmount = 0.1f, moveSpeedUpgradeAmount = 0.5f },
        new CharacterUpgradeSettings { characterType = CharacterType.Mage, healthUpgradeAmount = 2, attackPowerUpgradeAmount = 5, attackSpeedUpgradeAmount = 0.1f, moveSpeedUpgradeAmount = 0.5f }
    };
    
    // 外部から強化量を取得するためのプロパティ（後方互換性のため）
    public int HealthUpgradeAmount => GetCharacterHealthUpgradeAmount(CharacterType.Warrior);
    public int AttackPowerUpgradeAmount => GetCharacterAttackPowerUpgradeAmount(CharacterType.Warrior);
    public float AttackSpeedUpgradeAmount => GetCharacterAttackSpeedUpgradeAmount(CharacterType.Warrior);
    public float MoveSpeedUpgradeAmount => GetCharacterMoveSpeedUpgradeAmount(CharacterType.Warrior);
    
    /// <summary>
    /// キャラクター固有のHP強化量を取得（内部用）
    /// </summary>
    private int GetCharacterHealthUpgradeAmount(CharacterType characterType)
    {
        var settings = GetUpgradeSettings(characterType);
        return settings != null ? settings.healthUpgradeAmount : 10; // デフォルト値
    }
    
    /// <summary>
    /// キャラクター固有のHP強化量を取得（外部用）
    /// </summary>
    public int GetHealthUpgradeAmount(CharacterType characterType)
    {
        return GetCharacterHealthUpgradeAmount(characterType);
    }
    
    /// <summary>
    /// キャラクター固有の攻撃力強化量を取得（内部用）
    /// </summary>
    private int GetCharacterAttackPowerUpgradeAmount(CharacterType characterType)
    {
        var settings = GetUpgradeSettings(characterType);
        return settings != null ? settings.attackPowerUpgradeAmount : 5; // デフォルト値
    }
    
    /// <summary>
    /// キャラクター固有の攻撃力強化量を取得（外部用）
    /// </summary>
    public int GetAttackPowerUpgradeAmount(CharacterType characterType)
    {
        return GetCharacterAttackPowerUpgradeAmount(characterType);
    }
    
    /// <summary>
    /// キャラクター固有の攻撃速度強化量を取得（内部用）
    /// </summary>
    private float GetCharacterAttackSpeedUpgradeAmount(CharacterType characterType)
    {
        var settings = GetUpgradeSettings(characterType);
        return settings != null ? settings.attackSpeedUpgradeAmount : 0.1f; // デフォルト値
    }
    
    /// <summary>
    /// キャラクター固有の攻撃速度強化量を取得（外部用）
    /// </summary>
    public float GetAttackSpeedUpgradeAmount(CharacterType characterType)
    {
        return GetCharacterAttackSpeedUpgradeAmount(characterType);
    }
    
    /// <summary>
    /// キャラクター固有の移動速度強化量を取得（内部用）
    /// </summary>
    private float GetCharacterMoveSpeedUpgradeAmount(CharacterType characterType)
    {
        var settings = GetUpgradeSettings(characterType);
        return settings != null ? settings.moveSpeedUpgradeAmount : 0.5f; // デフォルト値
    }
    
    /// <summary>
    /// キャラクター固有の移動速度強化量を取得（外部用）
    /// </summary>
    public float GetMoveSpeedUpgradeAmount(CharacterType characterType)
    {
        return GetCharacterMoveSpeedUpgradeAmount(characterType);
    }
    
    /// <summary>
    /// キャラクタータイプに対応する強化設定を取得
    /// </summary>
    private CharacterUpgradeSettings GetUpgradeSettings(CharacterType characterType)
    {
        if (characterUpgradeSettings == null) return null;
        
        foreach (var settings in characterUpgradeSettings)
        {
            if (settings != null && settings.characterType == characterType)
            {
                return settings;
            }
        }
        
        return null;
    }
    
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
            // Debug.Log($"[CharacterUpgradeManager] Instance created. Upgrade data initialized. Warrior HP level: {upgradeData[CharacterType.Warrior].healthUpgrade}, Archer HP level: {upgradeData[CharacterType.Archer].healthUpgrade}, Mage HP level: {upgradeData[CharacterType.Mage].healthUpgrade}");
        }
        else if (instance != this)
        {
            Debug.LogWarning("[CharacterUpgradeManager] Duplicate CharacterUpgradeManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        // else
        // {
        //     // 既存のインスタンスが存在する場合、強化データを確認
        //     Debug.Log($"[CharacterUpgradeManager] Existing instance found. Warrior HP level: {upgradeData[CharacterType.Warrior].healthUpgrade}, Archer HP level: {upgradeData[CharacterType.Archer].healthUpgrade}, Mage HP level: {upgradeData[CharacterType.Mage].healthUpgrade}");
        // }
    }
    
    /// <summary>
    /// 強化データを初期化
    /// </summary>
    private void InitializeUpgradeData()
    {
        // 既存の強化データがある場合は保持（リセット時以外）
        if (upgradeData.Count == 0)
        {
            upgradeData[CharacterType.Warrior] = new CharacterUpgradeData(CharacterType.Warrior);
            upgradeData[CharacterType.Archer] = new CharacterUpgradeData(CharacterType.Archer);
            upgradeData[CharacterType.Mage] = new CharacterUpgradeData(CharacterType.Mage);
            
            // Debug.Log("[CharacterUpgradeManager] Upgrade data initialized for all character types (new initialization).");
        }
        // else
        // {
        //     Debug.Log($"[CharacterUpgradeManager] Upgrade data already exists. Warrior HP level: {upgradeData[CharacterType.Warrior].healthUpgrade}, Archer HP level: {upgradeData[CharacterType.Archer].healthUpgrade}, Mage HP level: {upgradeData[CharacterType.Mage].healthUpgrade}");
        // }
    }
    
    /// <summary>
    /// キャラクターを強化する
    /// </summary>
    public void UpgradeCharacter(CharacterType characterType, UpgradeType upgradeType)
    {
        Debug.Log($"[CharacterUpgradeManager] ===== UpgradeCharacter START =====");
        Debug.Log($"[CharacterUpgradeManager] 【調査用】受け取ったパラメータ: CharacterType={characterType}, UpgradeType={upgradeType}");
        
        if (!upgradeData.ContainsKey(characterType))
        {
            upgradeData[characterType] = new CharacterUpgradeData(characterType);
            Debug.Log($"[CharacterUpgradeManager] 【調査用】{characterType} の強化データが存在しなかったため、新規作成しました");
        }
        
        CharacterUpgradeData data = upgradeData[characterType];
        Debug.Log($"[CharacterUpgradeManager] 【調査用】強化前の状態: HP強化レベル={data.healthUpgrade}, 攻撃力強化レベル={data.attackPowerUpgrade}, 攻撃速度強化レベル={data.attackSpeedUpgrade}, 移動速度強化レベル={data.moveSpeedUpgrade}");
        
        switch (upgradeType)
        {
            case UpgradeType.Health:
                data.healthUpgrade++;
                int healthAmount = GetCharacterHealthUpgradeAmount(characterType);
                int totalHealthIncrease = healthAmount * data.healthUpgrade;
                Debug.Log($"[CharacterUpgradeManager] 【調査用】{characterType} のHPを強化しました: レベル {data.healthUpgrade} (+{totalHealthIncrease} HP, +{healthAmount} per level)");
                break;
            case UpgradeType.AttackPower:
                data.attackPowerUpgrade++;
                int attackAmount = GetCharacterAttackPowerUpgradeAmount(characterType);
                int totalAttackIncrease = attackAmount * data.attackPowerUpgrade;
                Debug.Log($"[CharacterUpgradeManager] 【調査用】{characterType} の攻撃力を強化しました: レベル {data.attackPowerUpgrade} (+{totalAttackIncrease} Attack, +{attackAmount} per level)");
                break;
            case UpgradeType.AttackSpeed:
                data.attackSpeedUpgrade++;
                float attackSpeedAmount = GetCharacterAttackSpeedUpgradeAmount(characterType);
                Debug.Log($"[CharacterUpgradeManager] 【調査用】{characterType} の攻撃速度を強化しました: レベル {data.attackSpeedUpgrade} (+{attackSpeedAmount * data.attackSpeedUpgrade} Attack Speed)");
                break;
            case UpgradeType.MoveSpeed:
                data.moveSpeedUpgrade++;
                float moveSpeedAmount = GetCharacterMoveSpeedUpgradeAmount(characterType);
                Debug.Log($"[CharacterUpgradeManager] 【調査用】{characterType} の移動速度を強化しました: レベル {data.moveSpeedUpgrade} (+{moveSpeedAmount * data.moveSpeedUpgrade} Move Speed)");
                break;
        }
        
        Debug.Log($"[CharacterUpgradeManager] 【調査用】強化後の状態: HP強化レベル={data.healthUpgrade}, 攻撃力強化レベル={data.attackPowerUpgrade}, 攻撃速度強化レベル={data.attackSpeedUpgrade}, 移動速度強化レベル={data.moveSpeedUpgrade}");
        Debug.Log($"[CharacterUpgradeManager] ===== UpgradeCharacter END =====");
        
        OnUpgradeChanged?.Invoke(characterType, upgradeType);
    }
    
    /// <summary>
    /// 強化されたHPを取得
    /// </summary>
    public int GetUpgradedHealth(CharacterType characterType, int baseHealth)
    {
        if (!upgradeData.ContainsKey(characterType))
        {
            // Debug.LogWarning($"[CharacterUpgradeManager] GetUpgradedHealth: No upgrade data found for {characterType}. Returning base health: {baseHealth}");
            return baseHealth;
        }
        
        int healthAmount = GetCharacterHealthUpgradeAmount(characterType);
        int upgradeLevel = upgradeData[characterType].healthUpgrade;
        int totalUpgrade = upgradeLevel * healthAmount;
        int finalHealth = baseHealth + totalUpgrade;
        
        // Debug.Log($"[CharacterUpgradeManager] GetUpgradedHealth: {characterType} - Base: {baseHealth}, UpgradeLevel: {upgradeLevel}, HealthAmount: {healthAmount}, TotalUpgrade: {totalUpgrade}, Final: {finalHealth}");
        
        return finalHealth;
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
        
        int attackAmount = GetCharacterAttackPowerUpgradeAmount(characterType);
        return baseAttackPower + (upgradeData[characterType].attackPowerUpgrade * attackAmount);
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
        float attackSpeedAmount = GetCharacterAttackSpeedUpgradeAmount(characterType);
        float upgradedSpeed = baseAttackSpeed - (upgradeData[characterType].attackSpeedUpgrade * attackSpeedAmount);
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
        
        float moveSpeedAmount = GetCharacterMoveSpeedUpgradeAmount(characterType);
        return baseMoveSpeed + (upgradeData[characterType].moveSpeedUpgrade * moveSpeedAmount);
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
        // Debug.Log("[CharacterUpgradeManager] All upgrades reset.");
    }
}
