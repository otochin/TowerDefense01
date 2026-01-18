using UnityEngine;
using System;

/// <summary>
/// シンプルな2Dライフゲージ
/// SpriteRendererを使用してキャラクターやエネミーの上部に表示されるHPバー
/// </summary>
public class SimpleHealthBar : MonoBehaviour
{
    [Header("スプライト参照")]
    [SerializeField] private SpriteRenderer backgroundSprite; // 背景のスプライト
    [SerializeField] private SpriteRenderer fillSprite; // HPバーのフィルスプライト
    
    [Header("表示設定")]
    [SerializeField] private Vector3 offset = new Vector3(0, 10.0f, 0); // キャラクター上部からのオフセット（下に移動）
    [SerializeField] private Vector2 barSize = new Vector2(60.0f, 10.0f); // バーのサイズ（Width, Height）- 大きく設定
    [SerializeField] private bool useBillboard = false; // 2Dゲームでは通常不要
    
    [Header("色設定")]
    [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 背景色（ダークグレー）
    [SerializeField] private Color fillColor = new Color(0f, 1f, 0f, 1f); // フィル色（緑）
    
    [Header("追従設定")]
    [SerializeField] private Transform targetTransform; // 追従する対象（自動検出）
    
    private HealthSystem healthSystem;
    private Camera mainCamera;
    private float maxHealthBarWidth;
    private float currentHealthBarWidth;
    private Vector2 lastBarSize; // 前回のbarSizeを記録（変更検知用）
    
    private void Awake()
    {
        // メインカメラを取得
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // ターゲットが設定されていない場合、親オブジェクトを使用
        if (targetTransform == null)
        {
            targetTransform = transform.parent;
        }
        
        // スプライトを自動検出
        if (backgroundSprite == null)
        {
            backgroundSprite = GetComponent<SpriteRenderer>();
        }
        
        if (fillSprite == null && transform.childCount > 0)
        {
            fillSprite = GetComponentInChildren<SpriteRenderer>();
        }
        
        // 最大のHPバー幅を保存
        maxHealthBarWidth = barSize.x;
        lastBarSize = barSize;
    }
    
    private void Start()
    {
        // スプライトを作成（まだ存在しない場合）
        CreateSpritesIfNeeded();
        
        // 初期設定
        SetupSprites();
    }
    
    private void LateUpdate()
    {
        // カメラの方を向く（Billboard効果）- 2Dゲームでは通常不要
        if (useBillboard && mainCamera != null && transform != null)
        {
            // 2Dなので、Z軸回転のみ（Y軸は回転させない）
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.z = 0; // Z軸成分を無視
            if (directionToCamera != Vector3.zero)
            {
                float angle = Mathf.Atan2(directionToCamera.y, directionToCamera.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            // 2Dゲームでは回転を固定（正面を向く）
            transform.rotation = Quaternion.identity;
        }
        
        // ターゲットの位置に追従
        if (targetTransform != null)
        {
            transform.position = targetTransform.position + offset;
        }
    }
    
    /// <summary>
    /// スプライトを作成（まだ存在しない場合）
    /// </summary>
    private void CreateSpritesIfNeeded()
    {
        // 既存の子オブジェクトを検索（重複作成を防ぐ）
        if (backgroundSprite == null)
        {
            // 子オブジェクトから既存のBackgroundを検索
            Transform bgTransform = transform.Find("Background");
            if (bgTransform != null)
            {
                backgroundSprite = bgTransform.GetComponent<SpriteRenderer>();
            }
        }
        
        if (fillSprite == null)
        {
            // 子オブジェクトから既存のFillを検索
            Transform fillTransform = transform.Find("Fill");
            if (fillTransform != null)
            {
                fillSprite = fillTransform.GetComponent<SpriteRenderer>();
            }
        }
        
        // Backgroundスプライトを作成（まだ存在しない場合）
        if (backgroundSprite == null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            bgObj.transform.localPosition = Vector3.zero;
            bgObj.transform.localRotation = Quaternion.identity;
            bgObj.transform.localScale = Vector3.one;
            
            backgroundSprite = bgObj.AddComponent<SpriteRenderer>();
            
            // 白いスプライトを作成（Unityの標準スプライト）
            backgroundSprite.sprite = CreateWhiteSprite((int)(barSize.x * 100), (int)(barSize.y * 100));
        }
        
        // Fillスプライトを作成（まだ存在しない場合）
        if (fillSprite == null)
        {
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(transform);
            fillObj.transform.localPosition = new Vector3(-barSize.x / 2f, 0, -0.01f); // 背景の少し前
            fillObj.transform.localRotation = Quaternion.identity;
            fillObj.transform.localScale = Vector3.one;
            
            fillSprite = fillObj.AddComponent<SpriteRenderer>();
            
            // 白いスプライトを作成
            fillSprite.sprite = CreateWhiteSprite((int)(barSize.x * 100), (int)(barSize.y * 100));
        }
    }
    
    /// <summary>
    /// 白いスプライトを作成
    /// </summary>
    private Sprite CreateWhiteSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
    }
    
    /// <summary>
    /// スプライトを設定
    /// </summary>
    private void SetupSprites()
    {
        // barSizeが変更された場合はスプライトを再作成
        if (barSize != lastBarSize)
        {
            // 既存のスプライトを削除
            if (backgroundSprite != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(backgroundSprite.gameObject);
                }
                else
                {
                    DestroyImmediate(backgroundSprite.gameObject);
                }
                backgroundSprite = null;
            }
            if (fillSprite != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(fillSprite.gameObject);
                }
                else
                {
                    DestroyImmediate(fillSprite.gameObject);
                }
                fillSprite = null;
            }
            
            // スプライトを再作成
            CreateSpritesIfNeeded();
            lastBarSize = barSize;
            maxHealthBarWidth = barSize.x;
        }
        
        // Backgroundスプライトの設定
        if (backgroundSprite != null)
        {
            backgroundSprite.color = backgroundColor;
            backgroundSprite.sortingOrder = 100; // 前面に表示
        }
        
        // Fillスプライトの設定
        if (fillSprite != null)
        {
            fillSprite.color = fillColor;
            fillSprite.sortingOrder = 101; // 背景より前面に表示
            fillSprite.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    
    /// <summary>
    /// エディタで値が変更された時に呼ばれる（エディタ専用）
    /// </summary>
    private void OnValidate()
    {
        // エディタでbarSizeが変更された場合、スプライトを再作成
        // ただし、実行中はStart()で処理されるため、ここでは何もしない
        if (!Application.isPlaying && barSize != lastBarSize)
        {
            lastBarSize = barSize;
            maxHealthBarWidth = barSize.x;
            
            // 既存のスプライトを削除（子オブジェクトを検索して削除）
            Transform bgTransform = transform.Find("Background");
            if (bgTransform != null)
            {
                DestroyImmediate(bgTransform.gameObject);
                backgroundSprite = null;
            }
            Transform fillTransform = transform.Find("Fill");
            if (fillTransform != null)
            {
                DestroyImmediate(fillTransform.gameObject);
                fillSprite = null;
            }
            
            // スプライトを再作成（Start()で呼ばれるが、即座に反映させる）
            CreateSpritesIfNeeded();
            
            // SetupSprites()はStart()で呼ばれるため、ここでは色などの設定のみ
            if (backgroundSprite != null)
            {
                backgroundSprite.color = backgroundColor;
                backgroundSprite.sortingOrder = 100;
            }
            if (fillSprite != null)
            {
                fillSprite.color = fillColor;
                fillSprite.sortingOrder = 101;
                fillSprite.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
    
    /// <summary>
    /// HealthSystemを初期化してイベントを購読
    /// </summary>
    public void Initialize(HealthSystem healthSystem)
    {
        // スプライトがまだ作成されていない場合は先に作成（初期化タイミングの問題に対応）
        CreateSpritesIfNeeded();
        SetupSprites();
        
        // 既存の購読を解除
        if (this.healthSystem != null)
        {
            this.healthSystem.OnHealthChanged -= UpdateHealthDisplay;
        }
        
        this.healthSystem = healthSystem;
        
        if (healthSystem != null)
        {
            // イベントを購読
            healthSystem.OnHealthChanged += UpdateHealthDisplay;
            
            // 初期表示を更新
            UpdateHealthDisplay(healthSystem.CurrentHealth, healthSystem.MaxHealth);
        }
    }
    
    private void OnDestroy()
    {
        // イベントの購読を解除
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthDisplay;
        }
    }
    
    /// <summary>
    /// HP表示を更新
    /// </summary>
    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        // HPバーの幅を更新
        if (fillSprite != null && maxHealth > 0)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            fillAmount = Mathf.Clamp01(fillAmount);
            
            // スケールで幅を調整
            fillSprite.transform.localScale = new Vector3(fillAmount, 1f, 1f);
            
            // 左端に固定して縮小するように、位置を調整
            float fillWidth = barSize.x * fillAmount;
            fillSprite.transform.localPosition = new Vector3(-barSize.x / 2f + fillWidth / 2f, 0, -0.01f);
        }
    }
}
