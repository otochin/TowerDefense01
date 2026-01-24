using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// ワールドスペースのHPバーUI管理
/// キャラクターやエネミーの上部に追従して表示されるHPバー
/// </summary>
public class WorldSpaceHealthBarUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthBarText;
    
    [Header("表示設定")]
    [SerializeField] private string healthFormat = "{0} / {1}";
    [SerializeField] private bool showHealthText = true; // HPテキストを表示するか（デフォルトは表示）
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0); // キャラクター上部からのオフセット
    [SerializeField] private bool useBillboard = true; // カメラの方を向くか（2Dではfalse推奨）
    
    [Header("追従設定")]
    [SerializeField] private Transform targetTransform; // 追従する対象（自動検出）
    
    private HealthSystem healthSystem;
    private Camera mainCamera;
    private RectTransform rectTransform;
    private Canvas worldSpaceCanvas;
    
    private void Awake()
    {
        // RectTransformを取得
        rectTransform = GetComponent<RectTransform>();
        
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
        
        // 子オブジェクトから自動的に検出（Fillという名前のImageを探す）
        if (healthBarFill == null)
        {
            Debug.Log($"[WorldSpaceHealthBarUI] Searching for Fill Image in {gameObject.name}...");
            // まず子要素から「Fill」という名前のImageを探す
            Image[] allImages = GetComponentsInChildren<Image>(true); // 非アクティブなものも含める
            Debug.Log($"[WorldSpaceHealthBarUI] Found {allImages.Length} Image(s) in children.");
            
            foreach (Image img in allImages)
            {
                Debug.Log($"[WorldSpaceHealthBarUI] Checking Image: {img.gameObject.name} (Path: {GetGameObjectPath(img.gameObject)})");
                if (img.gameObject.name == "Fill" || img.gameObject.name.Contains("Fill"))
                {
                    healthBarFill = img;
                    Debug.Log($"[WorldSpaceHealthBarUI] Found Fill Image: {img.gameObject.name}");
                    break;
                }
            }
            
            // 見つからなかった場合、Background以外のImageを探す
            if (healthBarFill == null)
            {
                Debug.Log("[WorldSpaceHealthBarUI] Fill not found by name, searching for non-Background Image...");
                foreach (Image img in allImages)
                {
                    if (!img.gameObject.name.Contains("Background"))
                    {
                        healthBarFill = img;
                        Debug.Log($"[WorldSpaceHealthBarUI] Using non-Background Image: {img.gameObject.name}");
                        break;
                    }
                }
            }
            
            // それでも見つからなかった場合、最初のImageを使用
            if (healthBarFill == null && allImages.Length > 0)
            {
                healthBarFill = allImages[0];
                Debug.LogWarning($"[WorldSpaceHealthBarUI] Using first Image found: {allImages[0].gameObject.name}");
            }
        }
        
        if (healthBarFill == null)
        {
            Debug.LogError($"[WorldSpaceHealthBarUI] healthBarFill is still null after search in {gameObject.name}!");
        }
        
        // HPテキストを自動検出（showHealthTextがtrueの場合）
        if (healthBarText == null && showHealthText)
        {
            healthBarText = GetComponentInChildren<TextMeshProUGUI>(true); // 非アクティブなものも含める
            if (healthBarText == null)
            {
                Debug.LogWarning($"[WorldSpaceHealthBarUI] healthBarText not found in {gameObject.name}. HP text will not be displayed. Please add a TextMeshProUGUI component to HealthBarPanel.");
            }
            else
            {
                Debug.Log($"[WorldSpaceHealthBarUI] Found healthBarText: {healthBarText.gameObject.name}");
            }
        }
        
        // ワールドスペースCanvasを探す（なければ作成）
        worldSpaceCanvas = GetComponentInParent<Canvas>();
        if (worldSpaceCanvas == null)
        {
            // Canvasが見つからない場合、親に追加する必要がある
            // または、シーンのCanvasにWorld Space設定をする必要がある
            Debug.LogWarning("[WorldSpaceHealthBarUI] Canvas not found. Please add a World Space Canvas as parent.");
        }
    }
    
    private void LateUpdate()
    {
        // カメラの方を向く（Billboard効果）- 2Dゲームでは通常不要
        if (useBillboard && mainCamera != null && transform != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
        else
        {
            // 2Dゲームでは回転を固定（正面を向く）
            transform.rotation = Quaternion.identity;
        }
        
        // ターゲットの位置に追従
        if (targetTransform != null && rectTransform != null)
        {
            Vector3 worldPosition = targetTransform.position + offset;
            rectTransform.position = worldPosition;
        }
    }
    
    /// <summary>
    /// HealthSystemを初期化してイベントを購読
    /// </summary>
    public void Initialize(HealthSystem healthSystem)
    {
        Debug.Log($"[WorldSpaceHealthBarUI] Initialize called on {gameObject.name} for {healthSystem?.GetInstanceID()}");
        
        // 既存の購読を解除
        if (this.healthSystem != null)
        {
            this.healthSystem.OnHealthChanged -= UpdateHealthDisplay;
        }
        
        this.healthSystem = healthSystem;
        
        if (healthSystem != null)
        {
            // healthBarFillが設定されているか確認
            if (healthBarFill == null)
            {
                Debug.LogWarning($"[WorldSpaceHealthBarUI] healthBarFill is null on {gameObject.name}. Trying to find it again...");
                // 子要素から「Fill」という名前のImageを探す
                Image[] allImages = GetComponentsInChildren<Image>();
                foreach (Image img in allImages)
                {
                    if (img.gameObject.name == "Fill" || img.gameObject.name.Contains("Fill"))
                    {
                        healthBarFill = img;
                        break;
                    }
                }
                
                if (healthBarFill == null)
                {
                    Debug.LogError($"[WorldSpaceHealthBarUI] healthBarFill not found on {gameObject.name} or its children. Make sure the Fill Image is set up correctly.");
                }
                else
                {
                    Debug.Log($"[WorldSpaceHealthBarUI] Found healthBarFill: {healthBarFill.gameObject.name}");
                }
            }
            else
            {
                Debug.Log($"[WorldSpaceHealthBarUI] healthBarFill already set: {healthBarFill.gameObject.name}");
            }
            
            // イベントを購読
            healthSystem.OnHealthChanged += UpdateHealthDisplay;
            Debug.Log($"[WorldSpaceHealthBarUI] Subscribed to OnHealthChanged event. Current health: {healthSystem.CurrentHealth}/{healthSystem.MaxHealth}");
            
            // 初期表示を更新
            UpdateHealthDisplay(healthSystem.CurrentHealth, healthSystem.MaxHealth);
        }
        else
        {
            Debug.LogError("[WorldSpaceHealthBarUI] healthSystem is null! Cannot initialize health bar.");
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
    /// GameObjectのパスを取得（デバッグ用）
    /// </summary>
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
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
            fillAmount = Mathf.Clamp01(fillAmount);
            healthBarFill.fillAmount = fillAmount;
            // Debug.Log($"[WorldSpaceHealthBarUI] UpdateHealthDisplay: {currentHealth}/{maxHealth}, fillAmount: {fillAmount}");
        }
        else
        {
            Debug.LogWarning("[WorldSpaceHealthBarUI] healthBarFill is null! Cannot update health bar display.");
        }
        
        // HPテキストを更新
        if (healthBarText != null && showHealthText)
        {
            healthBarText.text = string.Format(healthFormat, currentHealth, maxHealth);
            // テキストが非アクティブな場合は有効化
            if (!healthBarText.gameObject.activeSelf)
            {
                healthBarText.gameObject.SetActive(true);
            }
        }
        else if (showHealthText && healthBarText == null)
        {
            // テキストが設定されていない場合、再検索を試みる
            healthBarText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (healthBarText != null)
            {
                healthBarText.text = string.Format(healthFormat, currentHealth, maxHealth);
                healthBarText.gameObject.SetActive(true);
            }
        }
    }
}
