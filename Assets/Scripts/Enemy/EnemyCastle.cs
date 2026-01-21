using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 敵の城
/// プレイヤーキャラクターからのダメージを受け、HPが0になるとクリア
/// </summary>
public class EnemyCastle : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int defense = 0;
    
    [Header("References")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private CastleHealthBarUI healthBarUI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("ダメージエフェクト設定")]
    [SerializeField] private float damageFlashDuration = 0.1f; // ダメージを受けた時の赤色表示時間（秒）
    [SerializeField] private Color damageFlashColor = Color.red; // ダメージを受けた時の色
    
    [Header("ダメージ表示設定")]
    [SerializeField] private GameObject damagePrefab; // ダメージ表示プレハブ
    [SerializeField] private float damageDisplayOffset = 0.5f; // ライフゲージからのオフセット（上方向）
    [SerializeField] private float damageDisplayOffsetX = 0f; // ライフゲージからのオフセット（左右方向、正の値で右、負の値で左）
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip destroyedSound; // 城が破壊された時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
    private Color originalColor;
    private int previousHealth;
    private bool isDestroyedHandled = false; // HandleHealthDepletedが呼ばれたかどうか
    private bool isInitializing = false; // 初期化中フラグ（SetMaxHealth呼び出し時はダメージ表示をスキップ）
    
    // プロパティ
    public int MaxHealth => healthSystem != null ? healthSystem.MaxHealth : maxHealth;
    public int CurrentHealth => healthSystem != null ? healthSystem.CurrentHealth : 0;
    public int Defense => defense;
    public bool IsDestroyed => healthSystem != null && healthSystem.IsDead;
    public bool IsDead => healthSystem != null && healthSystem.IsDead; // IDamageable実装
    
    // イベント
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnDestroyed;
    
    private void Awake()
    {
        // SpriteRendererを自動検出（自分自身または子オブジェクトから）
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
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
        
        // HealthSystemの初期化（初期化中フラグを設定）
        isInitializing = true;
        healthSystem.SetMaxHealth(maxHealth);
        isInitializing = false;
        
        // HealthSystemのイベントを購読
        healthSystem.OnHealthChanged += HandleHealthChanged;
        healthSystem.OnHealthDepleted += HandleHealthDepleted;
        previousHealth = healthSystem.CurrentHealth;
        
        // HealthBarUIの自動検出
        if (healthBarUI == null)
        {
            healthBarUI = GetComponentInChildren<CastleHealthBarUI>();
        }
        
        // AudioSourceを自動検出または作成
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            // AudioSourceが存在しない場合は作成
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
        }
    }
    
    private void Start()
    {
        // HPバーUIの初期化
        if (healthBarUI != null)
        {
            healthBarUI.Initialize(healthSystem);
        }
    }
    
    private void OnDestroy()
    {
        // イベントの購読解除
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= HandleHealthChanged;
            healthSystem.OnHealthDepleted -= HandleHealthDepleted;
        }
    }
    
    /// <summary>
    /// ダメージを受ける（IDamageable実装）
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (healthSystem == null) return;
        
        // 既に死亡している場合は何もしない（ただし、HPが0になった瞬間は処理する）
        // IsDeadのチェックを外して、HealthSystem.TakeDamage()内で重複処理を防ぐ
        if (healthSystem.IsDead) return;
        
        // 防御力を考慮したダメージ計算
        int actualDamage = Mathf.Max(1, damage - defense);
        healthSystem.TakeDamage(actualDamage);
    }
    
    /// <summary>
    /// HPが変更された時の処理
    /// </summary>
    private void HandleHealthChanged(int currentHealth, int maxHealth)
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
        
        // HPが0になったことを検出（OnHealthDepletedイベントが発火しなかった場合のフォールバック）
        // ただし、前回のHPが0より大きかった場合のみ（重複検出を防ぐ）
        if (currentHealth <= 0 && previousHealth > 0 && !isDestroyedHandled)
        {
            Debug.LogWarning($"[EnemyCastle] HP became 0 in HandleHealthChanged (fallback detection). currentHealth: {currentHealth}, previousHealth: {previousHealth}");
            // HandleHealthDepletedが呼ばれるはずだが、念のため再度確認
            if (healthSystem != null && healthSystem.IsDead)
            {
                HandleHealthDepleted();
            }
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
    private void HandleHealthDepleted()
    {
        // 重複呼び出しを防ぐ
        if (isDestroyedHandled)
        {
            Debug.LogWarning("[EnemyCastle] HandleHealthDepleted called multiple times. Ignoring duplicate call.");
            return;
        }
        
        isDestroyedHandled = true;
        Debug.Log($"[EnemyCastle] HandleHealthDepleted called. CurrentHealth: {healthSystem?.CurrentHealth ?? -1}, OnDestroyed subscribers: {(OnDestroyed?.GetInvocationList().Length ?? 0)}");
        
        // 効果音を再生
        PlayDestroyedSound();
        
        OnDestroyed?.Invoke();
        Debug.Log("[EnemyCastle] OnDestroyed event invoked.");
        
        // クリアイベントを発火（GameManagerが実装されたら使用）
        // GameManager.OnEnemyCastleDestroyed?.Invoke();
    }
    
    /// <summary>
    /// 城が破壊された時の効果音を再生
    /// </summary>
    private void PlayDestroyedSound()
    {
        if (audioSource != null && destroyedSound != null)
        {
            audioSource.PlayOneShot(destroyedSound);
            Debug.Log("[EnemyCastle] Destroyed sound played.");
        }
        else
        {
            if (audioSource == null)
            {
                Debug.LogWarning("[EnemyCastle] AudioSource is not set. Destroyed sound will not play.");
            }
            if (destroyedSound == null)
            {
                Debug.LogWarning("[EnemyCastle] Destroyed sound AudioClip is not set. Please assign Destroyed Sound clip in Inspector.");
            }
        }
    }
    
    /// <summary>
    /// 防御力を設定
    /// </summary>
    public void SetDefense(int newDefense)
    {
        defense = Mathf.Max(0, newDefense);
    }
    
    /// <summary>
    /// 最大HPを設定
    /// </summary>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        if (healthSystem != null)
        {
            isInitializing = true;
            healthSystem.SetMaxHealth(maxHealth);
            isInitializing = false;
        }
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
    /// HPをリセット（最大HPに戻す）
    /// </summary>
    public void ResetHealth()
    {
        // フラグをリセット（ゲーム再開時に使用）
        isDestroyedHandled = false;
        previousHealth = maxHealth;
        
        if (healthSystem != null)
        {
            isInitializing = true;
            healthSystem.SetMaxHealth(maxHealth);
            isInitializing = false;
        }
    }
    
    /// <summary>
    /// ダメージ表示を表示
    /// </summary>
    private void ShowDamageDisplay(int damage)
    {
        if (damagePrefab == null)
        {
            Debug.LogWarning($"[EnemyCastle] DamagePrefab is not set for {gameObject.name}. Please assign DamagePrefab in the inspector.");
            return;
        }
        
        Debug.Log($"[EnemyCastle] ShowDamageDisplay called: damage={damage}, damagePrefab={damagePrefab.name}");
        
        // ライフゲージの位置を取得
        Vector3 displayPosition = transform.position;
        
        // ライフゲージが存在する場合、その位置を基準にする
        if (healthBarUI != null)
        {
            RectTransform healthBarRect = healthBarUI.GetComponent<RectTransform>();
            if (healthBarRect != null)
            {
                displayPosition = healthBarRect.position;
                Debug.Log($"[EnemyCastle] Using healthBarUI position: {displayPosition}");
            }
        }
        else
        {
            Debug.Log($"[EnemyCastle] No health bar found, using castle position: {displayPosition}");
        }
        
        // ライフゲージの少し上に配置（上下と左右のオフセットを適用）
        displayPosition += Vector3.up * damageDisplayOffset;
        displayPosition += Vector3.right * damageDisplayOffsetX;
        Debug.Log($"[EnemyCastle] Final display position: {displayPosition} (offset Y: {damageDisplayOffset}, offset X: {damageDisplayOffsetX})");
        
        // ダメージ表示をインスタンス化
        GameObject damageObj = Instantiate(damagePrefab, displayPosition, Quaternion.identity);
        Debug.Log($"[EnemyCastle] Damage object instantiated: {damageObj.name} at {displayPosition}");
        
        // World Space Canvasの場合、RectTransformの位置を設定
        Canvas canvas = damageObj.GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransform rectTransform = damageObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = displayPosition;
                Debug.Log($"[EnemyCastle] World Space Canvas detected, set RectTransform position: {displayPosition}");
            }
        }
        
        // DamageDisplayコンポーネントを検索（子要素も含む）
        DamageDisplay damageDisplay = damageObj.GetComponentInChildren<DamageDisplay>();
        if (damageDisplay != null)
        {
            damageDisplay.ShowDamage(damage);
            Debug.Log($"[EnemyCastle] DamageDisplay.ShowDamage called with damage: {damage}");
        }
        else
        {
            Debug.LogWarning($"[EnemyCastle] DamageDisplay component not found in {damageObj.name} or its children.");
        }
    }
}
