using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// キャラクター基底クラス
/// すべてのキャラクターの基本機能を提供
/// </summary>
public class CharacterBase : MonoBehaviour, IDamageable
{
    [Header("キャラクターデータ")]
    [SerializeField] private CharacterData characterData;
    
    [Header("参照")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("ダメージエフェクト設定")]
    [SerializeField] private float damageFlashDuration = 0.1f; // ダメージを受けた時の赤色表示時間（秒）
    [SerializeField] private Color damageFlashColor = Color.red; // ダメージを受けた時の色
    
    [Header("ライフゲージ設定")]
    [SerializeField] private GameObject healthBarPrefab; // SimpleHealthBarプレハブ（またはWorldSpaceHealthBarUIプレハブ）
    [SerializeField] private bool autoCreateHealthBar = true; // 自動生成するか
    
    private Color originalColor;
    private int previousHealth;
    private SimpleHealthBar simpleHealthBar;
    private WorldSpaceHealthBarUI healthBarUI;
    
    // プロパティ
    public CharacterData CharacterData => characterData;
    public int MaxHealth => healthSystem != null ? healthSystem.MaxHealth : 0;
    public int CurrentHealth => healthSystem != null ? healthSystem.CurrentHealth : 0;
    public int AttackPower
    {
        get
        {
            if (characterData == null) return 0;
            // 強化された攻撃力を取得
            if (CharacterUpgradeManager.Instance != null)
            {
                return CharacterUpgradeManager.Instance.GetUpgradedAttackPower(characterData.CharacterType, characterData.AttackPower);
            }
            return characterData.AttackPower;
        }
    }
    public float AttackRange => characterData != null ? characterData.AttackRange : 0;
    public float AttackSpeed
    {
        get
        {
            if (characterData == null) return 1.0f;
            // 強化された攻撃速度を取得
            if (CharacterUpgradeManager.Instance != null)
            {
                return CharacterUpgradeManager.Instance.GetUpgradedAttackSpeed(characterData.CharacterType, characterData.AttackSpeed);
            }
            return characterData.AttackSpeed;
        }
    }
    public float MoveSpeed
    {
        get
        {
            if (characterData == null) return 2.0f;
            // 強化された移動速度を取得
            if (CharacterUpgradeManager.Instance != null)
            {
                return CharacterUpgradeManager.Instance.GetUpgradedMoveSpeed(characterData.CharacterType, characterData.MoveSpeed);
            }
            return characterData.MoveSpeed;
        }
    }
    public int Defense => characterData != null ? characterData.Defense : 0;
    public bool IsDead => healthSystem != null && healthSystem.IsDead;
    
    // イベント
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnDeath;
    
    protected virtual void Awake()
    {
        // SpriteRendererを自動検出
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // 元の色を保存
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // HealthSystemがアタッチされていない場合は自動追加
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                healthSystem = gameObject.AddComponent<HealthSystem>();
            }
        }
        
        // HealthSystemのイベントを購読
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += HandleHealthChanged;
            healthSystem.OnHealthDepleted += HandleHealthDepleted;
            previousHealth = healthSystem.CurrentHealth;
        }
    }
    
    protected virtual void Start()
    {
        // CharacterDataからステータスを初期化
        InitializeFromCharacterData();
        
        // ライフゲージを自動生成
        if (autoCreateHealthBar && healthBarPrefab != null)
        {
            CreateHealthBar();
        }
    }
    
    protected virtual void OnDestroy()
    {
        // イベントの購読解除
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= HandleHealthChanged;
            healthSystem.OnHealthDepleted -= HandleHealthDepleted;
        }
    }
    
    /// <summary>
    /// CharacterDataからステータスを初期化
    /// </summary>
    protected virtual void InitializeFromCharacterData()
    {
        if (characterData == null)
        {
            Debug.LogWarning($"CharacterData is not set for {gameObject.name}.");
            return;
        }
        
        // HealthSystemの最大HPを設定（強化されたHPを適用）
        if (healthSystem != null)
        {
            int maxHealth = characterData.MaxHealth;
            
            // 強化されたHPを取得
            if (CharacterUpgradeManager.Instance != null)
            {
                maxHealth = CharacterUpgradeManager.Instance.GetUpgradedHealth(characterData.CharacterType, characterData.MaxHealth);
            }
            
            healthSystem.SetMaxHealth(maxHealth);
        }
    }
    
    /// <summary>
    /// CharacterDataを設定
    /// </summary>
    public void SetCharacterData(CharacterData data)
    {
        characterData = data;
        InitializeFromCharacterData();
    }
    
    /// <summary>
    /// ダメージを受ける（IDamageable実装）
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (healthSystem == null || healthSystem.IsDead) return;
        
        // 防御力を考慮したダメージ計算
        int actualDamage = Mathf.Max(1, damage - Defense);
        healthSystem.TakeDamage(actualDamage);
    }
    
    /// <summary>
    /// HPが変更された時の処理
    /// </summary>
    protected virtual void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        // HPが減った場合（ダメージを受けた場合）に赤くする
        if (currentHealth < previousHealth && spriteRenderer != null)
        {
            StartCoroutine(FlashDamageColor());
        }
        
        previousHealth = currentHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// ダメージを受けた時に一瞬赤くする
    /// </summary>
    private IEnumerator FlashDamageColor()
    {
        if (spriteRenderer == null) yield break;
        
        // 赤色に変更
        spriteRenderer.color = damageFlashColor;
        
        // 指定時間待つ
        yield return new WaitForSeconds(damageFlashDuration);
        
        // 元の色に戻す
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// HPが0になった時の処理
    /// </summary>
    protected virtual void HandleHealthDepleted()
    {
        OnDeath?.Invoke();
        OnCharacterDeath();
    }
    
    /// <summary>
    /// キャラクターが死亡した時の処理（オーバーライド可能）
    /// </summary>
    protected virtual void OnCharacterDeath()
    {
        // デフォルトでは破棄（オブジェクトプーリングを使用する場合は非アクティブ化）
        Destroy(gameObject);
    }
    
    /// <summary>
    /// HPを回復
    /// </summary>
    public void Heal(int amount)
    {
        if (healthSystem != null)
        {
            healthSystem.Heal(amount);
        }
    }
    
    /// <summary>
    /// ライフゲージを作成
    /// </summary>
    private void CreateHealthBar()
    {
        if (healthBarPrefab == null)
        {
            Debug.LogWarning($"[CharacterBase] healthBarPrefab is not set for {gameObject.name}. Please assign HealthBarPrefab in the prefab's inspector.");
            return;
        }
        
        if (healthSystem == null)
        {
            Debug.LogWarning($"[CharacterBase] healthSystem is null for {gameObject.name}. Cannot create health bar.");
            return;
        }
        
        // ライフゲージをインスタンス化（このGameObjectの子として）
        GameObject healthBarObj = Instantiate(healthBarPrefab, transform);
        
        // SimpleHealthBarを優先して使用（2D用）- 子要素も検索
        simpleHealthBar = healthBarObj.GetComponentInChildren<SimpleHealthBar>();
        if (simpleHealthBar != null && healthSystem != null)
        {
            simpleHealthBar.Initialize(healthSystem);
            Debug.Log($"[CharacterBase] SimpleHealthBar created successfully for {gameObject.name}.");
            return;
        }
        
        // WorldSpaceHealthBarUIも試す（3D用）- 子要素も検索
        healthBarUI = healthBarObj.GetComponentInChildren<WorldSpaceHealthBarUI>();
        if (healthBarUI != null && healthSystem != null)
        {
            healthBarUI.Initialize(healthSystem);
            Debug.Log($"[CharacterBase] WorldSpaceHealthBarUI created successfully for {gameObject.name}.");
        }
        else
        {
            Debug.LogWarning($"[CharacterBase] Failed to initialize health bar UI for {gameObject.name}. Make sure the prefab has SimpleHealthBar or WorldSpaceHealthBarUI component (can be on child objects).");
        }
    }
}
