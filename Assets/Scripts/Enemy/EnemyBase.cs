using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 敵基底クラス
/// すべての敵の基本機能を提供
/// </summary>
public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("敵データ")]
    [SerializeField] private EnemyData enemyData;
    
    [Header("参照")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("ダメージエフェクト設定")]
    [SerializeField] private float damageFlashDuration = 0.1f; // ダメージを受けた時の赤色表示時間（秒）
    [SerializeField] private Color damageFlashColor = Color.red; // ダメージを受けた時の色
    
    [Header("ライフゲージ設定")]
    [SerializeField] private GameObject healthBarPrefab; // SimpleHealthBarプレハブ（またはWorldSpaceHealthBarUIプレハブ）
    [SerializeField] private bool autoCreateHealthBar = true; // 自動生成するか
    
    [Header("ダメージ表示設定")]
    [SerializeField] private GameObject damagePrefab; // ダメージ表示プレハブ
    [SerializeField] private float damageDisplayOffset = 0.5f; // ライフゲージからのオフセット（上方向）
    [SerializeField] private float damageDisplayOffsetX = 0f; // ライフゲージからのオフセット（左右方向、正の値で右、負の値で左）
    
    private Color originalColor;
    private int previousHealth;
    private SimpleHealthBar simpleHealthBar;
    private WorldSpaceHealthBarUI healthBarUI;
    private bool isInitializing = false; // 初期化中フラグ（SetMaxHealth呼び出し時はダメージ表示をスキップ）
    
    // プロパティ
    public EnemyData EnemyData => enemyData;
    public int MaxHealth => healthSystem != null ? healthSystem.MaxHealth : 0;
    public int CurrentHealth => healthSystem != null ? healthSystem.CurrentHealth : 0;
    public int AttackPower => enemyData != null ? enemyData.AttackPower : 0;
    public float AttackRange => enemyData != null ? enemyData.AttackRange : 0;
    public float AttackSpeed => enemyData != null ? enemyData.AttackSpeed : 1.0f;
    public float MoveSpeed => enemyData != null ? enemyData.MoveSpeed : 2.0f;
    public int Defense => enemyData != null ? enemyData.Defense : 0;
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
        // EnemyDataからステータスを初期化
        InitializeFromEnemyData();
        
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
    /// EnemyDataからステータスを初期化
    /// </summary>
    protected virtual void InitializeFromEnemyData()
    {
        if (enemyData == null)
        {
            Debug.LogWarning($"EnemyData is not set for {gameObject.name}.");
            return;
        }
        
        // HealthSystemの最大HPを設定
        if (healthSystem != null)
        {
            // 初期化中フラグを設定（SetMaxHealth呼び出し時はダメージ表示をスキップ）
            isInitializing = true;
            healthSystem.SetMaxHealth(enemyData.MaxHealth);
            isInitializing = false;
        }
    }
    
    /// <summary>
    /// EnemyDataを設定
    /// </summary>
    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;
        InitializeFromEnemyData();
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
        // 初期化中（SetMaxHealth呼び出し時）はダメージ表示をスキップ
        if (isInitializing)
        {
            previousHealth = currentHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            return;
        }
        
        // HPが減った場合（ダメージを受けた場合）に赤くする
        if (currentHealth < previousHealth && spriteRenderer != null)
        {
            StartCoroutine(FlashDamageColor());
            
            // ダメージ表示
            int damageAmount = previousHealth - currentHealth;
            if (damageAmount > 0)
            {
                ShowDamageDisplay(damageAmount);
            }
        }
        
        previousHealth = currentHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// ダメージ表示を表示
    /// </summary>
    private void ShowDamageDisplay(int damage)
    {
        if (damagePrefab == null)
        {
            Debug.LogWarning($"[EnemyBase] DamagePrefab is not set for {gameObject.name}. Please assign DamagePrefab in the inspector.");
            return;
        }
        
        // Debug.Log($"[EnemyBase] ShowDamageDisplay called: damage={damage}, damagePrefab={damagePrefab.name}");
        
        // ライフゲージの位置を取得
        Vector3 displayPosition = transform.position;
        
        // ライフゲージが存在する場合、その位置を基準にする
        if (healthBarUI != null)
        {
            RectTransform healthBarRect = healthBarUI.GetComponent<RectTransform>();
            if (healthBarRect != null)
            {
                displayPosition = healthBarRect.position;
                // Debug.Log($"[EnemyBase] Using healthBarUI position: {displayPosition}");
            }
        }
        else if (simpleHealthBar != null)
        {
            Transform healthBarTransform = simpleHealthBar.transform;
            if (healthBarTransform != null)
            {
                displayPosition = healthBarTransform.position;
                // Debug.Log($"[EnemyBase] Using simpleHealthBar position: {displayPosition}");
            }
        }
        else
        {
            // Debug.Log($"[EnemyBase] No health bar found, using enemy position: {displayPosition}");
        }
        
        // ライフゲージの少し上に配置（上下と左右のオフセットを適用）
        displayPosition += Vector3.up * damageDisplayOffset;
        displayPosition += Vector3.right * damageDisplayOffsetX;
        // Debug.Log($"[EnemyBase] Final display position: {displayPosition} (offset Y: {damageDisplayOffset}, offset X: {damageDisplayOffsetX})");
        
        // ダメージ表示をインスタンス化
        GameObject damageObj = Instantiate(damagePrefab, displayPosition, Quaternion.identity);
        // Debug.Log($"[EnemyBase] Damage object instantiated: {damageObj.name} at {displayPosition}");
        
        // World Space Canvasの場合、RectTransformの位置を設定
        Canvas canvas = damageObj.GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransform rectTransform = damageObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = displayPosition;
                // Debug.Log($"[EnemyBase] World Space Canvas detected, set RectTransform position: {displayPosition}");
            }
        }
        
        // DamageDisplayコンポーネントを検索（子要素も含む）
        DamageDisplay damageDisplay = damageObj.GetComponentInChildren<DamageDisplay>();
        if (damageDisplay != null)
        {
            // Debug.Log($"[EnemyBase] DamageDisplay component found, showing damage: {damage}");
            damageDisplay.ShowDamage(damage);
        }
        else
        {
            Debug.LogError($"[EnemyBase] DamageDisplay component not found on {damagePrefab.name} or its children. Please add DamageDisplay component to the prefab.");
        }
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
        OnEnemyDeath();
    }
    
    /// <summary>
    /// 敵が死亡した時の処理（オーバーライド可能）
    /// </summary>
    protected virtual void OnEnemyDeath()
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
            Debug.LogWarning($"[EnemyBase] healthBarPrefab is not set for {gameObject.name}. Please assign HealthBarPrefab in the prefab's inspector.");
            return;
        }
        
        if (healthSystem == null)
        {
            Debug.LogWarning($"[EnemyBase] healthSystem is null for {gameObject.name}. Cannot create health bar.");
            return;
        }
        
        // ライフゲージをインスタンス化（このGameObjectの子として）
        GameObject healthBarObj = Instantiate(healthBarPrefab, transform);
        
        // SimpleHealthBarを優先して使用（2D用）- 子要素も検索
        simpleHealthBar = healthBarObj.GetComponentInChildren<SimpleHealthBar>();
        if (simpleHealthBar != null && healthSystem != null)
        {
            simpleHealthBar.Initialize(healthSystem);
            // Debug.Log($"[EnemyBase] SimpleHealthBar created successfully for {gameObject.name}.");
            return;
        }
        
        // WorldSpaceHealthBarUIも試す（3D用）- 子要素も検索
        healthBarUI = healthBarObj.GetComponentInChildren<WorldSpaceHealthBarUI>();
        if (healthBarUI != null && healthSystem != null)
        {
            healthBarUI.Initialize(healthSystem);
            // Debug.Log($"[EnemyBase] WorldSpaceHealthBarUI created successfully for {gameObject.name}.");
        }
        else
        {
            Debug.LogWarning($"[EnemyBase] Failed to initialize health bar UI for {gameObject.name}. Make sure the prefab has SimpleHealthBar or WorldSpaceHealthBarUI component (can be on child objects).");
        }
    }
}
