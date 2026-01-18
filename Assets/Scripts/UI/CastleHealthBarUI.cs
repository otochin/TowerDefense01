using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 城のHPバーUI管理
/// HPバーの表示を更新する
/// </summary>
public class CastleHealthBarUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthBarText;
    
    [Header("表示設定")]
    [SerializeField] private string healthFormat = "{0} / {1}";
    [SerializeField] private bool showHealthText = true;
    
    private HealthSystem healthSystem;
    
    private void Awake()
    {
        // 子オブジェクトから自動的に検出
        if (healthBarFill == null)
        {
            // まず名前で検索（HealthBarFillという名前のGameObjectからImageを取得）
            Transform fillTransform = transform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                healthBarFill = fillTransform.GetComponent<Image>();
            }
            
            // 名前で見つからない場合、子オブジェクトから検索
            if (healthBarFill == null)
            {
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    // HealthBarFillという名前のImageを優先的に使用
                    if (img.name.Contains("Fill") || img.name.Contains("HealthBarFill"))
                    {
                        healthBarFill = img;
                        break;
                    }
                }
                
                // まだ見つからない場合、最初のImageを使用
                if (healthBarFill == null && images.Length > 0)
                {
                    healthBarFill = images[0];
                }
            }
            
            // デバッグログ
            if (healthBarFill != null)
            {
                Debug.Log($"[CastleHealthBarUI] healthBarFill found: {healthBarFill.name} on {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[CastleHealthBarUI] healthBarFill not found on {gameObject.name}");
            }
        }
        
        if (healthBarText == null && showHealthText)
        {
            healthBarText = GetComponentInChildren<TextMeshProUGUI>();
            if (healthBarText != null)
            {
                Debug.Log($"[CastleHealthBarUI] healthBarText found: {healthBarText.name} on {gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// HealthSystemを初期化してイベントを購読
    /// </summary>
    public void Initialize(HealthSystem healthSystem)
    {
        // 既存の購読を解除
        if (this.healthSystem != null)
        {
            this.healthSystem.OnHealthChanged -= UpdateHealthDisplay;
        }
        
        this.healthSystem = healthSystem;
        
        // healthBarFillがまだ設定されていない場合、再度検出を試みる
        if (healthBarFill == null)
        {
            Transform fillTransform = transform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                healthBarFill = fillTransform.GetComponent<Image>();
            }
            
            if (healthBarFill == null)
            {
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.name.Contains("Fill") || img.name.Contains("HealthBarFill"))
                    {
                        healthBarFill = img;
                        break;
                    }
                }
                
                if (healthBarFill == null && images.Length > 0)
                {
                    healthBarFill = images[0];
                }
            }
        }
        
        // healthBarFillのImage Typeを確認し、必要に応じて自動設定
        if (healthBarFill != null)
        {
            if (healthBarFill.type != Image.Type.Filled)
            {
                Debug.LogWarning($"[CastleHealthBarUI] healthBarFill Image Type is not set to 'Filled'. Current type: {healthBarFill.type}. Auto-setting to 'Filled' on {gameObject.name}");
                healthBarFill.type = Image.Type.Filled;
                healthBarFill.fillMethod = Image.FillMethod.Horizontal;
                healthBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
        }
        
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
        // HPバーのフィルを更新
        if (healthBarFill != null)
        {
            float fillAmount = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            healthBarFill.fillAmount = Mathf.Clamp01(fillAmount);
            
            // デバッグログ（HPが変更された時のみ）
            Debug.Log($"[CastleHealthBarUI] UpdateHealthDisplay: {currentHealth}/{maxHealth}, fillAmount={fillAmount} on {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[CastleHealthBarUI] healthBarFill is null, cannot update display on {gameObject.name}");
        }
        
        // HPテキストを更新
        if (healthBarText != null && showHealthText)
        {
            healthBarText.text = string.Format(healthFormat, currentHealth, maxHealth);
        }
    }
    
    /// <summary>
    /// 手動でHP表示を更新（外部から呼び出し可能）
    /// </summary>
    public void UpdateDisplay(int currentHealth, int maxHealth)
    {
        UpdateHealthDisplay(currentHealth, maxHealth);
    }
}
