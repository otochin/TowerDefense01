using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 背景管理システム
/// ステージごとに背景を切り替える
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class StageBackground
    {
        [Tooltip("ステージ番号（1から開始）")]
        public int stageNumber;
        
        [Tooltip("背景画像（Sprite）")]
        public Sprite backgroundSprite;
        
        [Tooltip("背景の色（背景画像がない場合に使用）")]
        public Color backgroundColor = Color.white;
    }
    
    [Header("背景設定")]
    [Tooltip("ステージごとの背景設定")]
    [SerializeField] private List<StageBackground> stageBackgrounds = new List<StageBackground>();
    
    [Header("背景オブジェクト参照")]
    [Tooltip("背景を表示するSpriteRenderer（Background GameObjectのSpriteRendererを設定）")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    
    [Header("カメラ参照")]
    [Tooltip("背景色を設定するカメラ（自動検出も可能）")]
    [SerializeField] private Camera mainCamera;
    
    private void Awake()
    {
        Debug.Log("[BackgroundManager] Awake() called");
        
        // 常に「Background」という名前のGameObjectを探す
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject != null)
        {
            Debug.Log($"[BackgroundManager] Background GameObject found: {backgroundObject.name}");
            SpriteRenderer foundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            if (foundRenderer != null)
            {
                // 設定されているbackgroundRendererが正しいか確認
                if (backgroundRenderer == null || backgroundRenderer.gameObject.name != "Background")
                {
                    Debug.Log($"[BackgroundManager] Updating Background Renderer reference. Old: {(backgroundRenderer != null ? backgroundRenderer.gameObject.name : "null")}, New: {backgroundObject.name}");
                    backgroundRenderer = foundRenderer;
                    Debug.Log($"[BackgroundManager] SpriteRenderer found on Background GameObject. Current sprite: {(backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null")}");
                }
                else
                {
                    Debug.Log($"[BackgroundManager] Background Renderer already correctly set to Background GameObject. Current sprite: {(backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null")}");
                }
            }
            else
            {
                Debug.LogWarning("[BackgroundManager] Background GameObject found but SpriteRenderer component not found.");
            }
        }
        else
        {
            Debug.LogWarning("[BackgroundManager] Background GameObject not found. Please create a GameObject named 'Background' with a SpriteRenderer component.");
        }
        
        // カメラの自動検出
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }
        
        Debug.Log($"[BackgroundManager] Main Camera: {(mainCamera != null ? mainCamera.name : "null")}");
    }
    
    private void Start()
    {
        Debug.Log("[BackgroundManager] Start() called");
        Debug.Log($"[BackgroundManager] Stage Backgrounds count: {stageBackgrounds.Count}");
        for (int i = 0; i < stageBackgrounds.Count; i++)
        {
            var bg = stageBackgrounds[i];
            Debug.Log($"[BackgroundManager] Stage {bg.stageNumber}: Sprite={(bg.backgroundSprite != null ? bg.backgroundSprite.name : "null")}, Color={bg.backgroundColor}");
        }
        
        // StageManagerのイベントを購読
        if (StageManager.Instance != null)
        {
            Debug.Log($"[BackgroundManager] StageManager found. Current Stage: {StageManager.Instance.CurrentStage}");
            StageManager.Instance.OnStageChanged += OnStageChanged;
            Debug.Log("[BackgroundManager] Subscribed to StageManager.OnStageChanged event");
            // 初期ステージの背景を設定
            UpdateBackground(StageManager.Instance.CurrentStage);
        }
        else
        {
            Debug.LogWarning("[BackgroundManager] StageManager not found. Background will not update automatically.");
            // StageManagerが見つからない場合、デフォルトでステージ1の背景を設定
            UpdateBackground(1);
        }
    }
    
    private void OnDestroy()
    {
        // イベントの購読を解除
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged -= OnStageChanged;
        }
    }
    
    /// <summary>
    /// ステージ変更時のコールバック
    /// </summary>
    private void OnStageChanged(int newStage)
    {
        Debug.Log($"[BackgroundManager] OnStageChanged called with stage: {newStage}");
        UpdateBackground(newStage);
    }
    
    /// <summary>
    /// 背景を更新
    /// </summary>
    public void UpdateBackground(int stageNumber)
    {
        Debug.Log($"[BackgroundManager] UpdateBackground called with stageNumber: {stageNumber}");
        
        // 常に「Background」という名前のGameObjectを探して確認
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject != null)
        {
            SpriteRenderer foundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            if (foundRenderer == null)
            {
                Debug.LogError("[BackgroundManager] Background GameObject found but SpriteRenderer component not found. Please add SpriteRenderer component to Background GameObject.");
                return;
            }
            
            // 設定されているbackgroundRendererが正しいか確認
            if (backgroundRenderer == null || backgroundRenderer.gameObject.name != "Background")
            {
                Debug.Log($"[BackgroundManager] Updating Background Renderer reference. Old: {(backgroundRenderer != null ? backgroundRenderer.gameObject.name : "null")}, New: {backgroundObject.name}");
                backgroundRenderer = foundRenderer;
            }
            
            Debug.Log($"[BackgroundManager] Background Renderer is set: {backgroundRenderer.gameObject.name}. Current sprite before update: {(backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null")}");
        }
        else
        {
            Debug.LogError("[BackgroundManager] Background GameObject not found. Please create a GameObject named 'Background' with a SpriteRenderer component.");
            return;
        }
        
        // ステージに対応する背景を検索
        Debug.Log($"[BackgroundManager] Searching for background for stage {stageNumber} in {stageBackgrounds.Count} configured backgrounds...");
        StageBackground stageBg = stageBackgrounds.Find(bg => bg.stageNumber == stageNumber);
        
        if (stageBg == null)
        {
            Debug.LogWarning($"[BackgroundManager] Background not found for stage {stageNumber}. Using default.");
            // デフォルトでステージ1の背景を使用
            stageBg = stageBackgrounds.Find(bg => bg.stageNumber == 1);
        }
        
        if (stageBg == null)
        {
            Debug.LogWarning("[BackgroundManager] No background configured. Please set up backgrounds in Inspector.");
            return;
        }
        
        Debug.Log($"[BackgroundManager] Found stage background: Stage={stageBg.stageNumber}, Sprite={(stageBg.backgroundSprite != null ? stageBg.backgroundSprite.name : "null")}, Color={stageBg.backgroundColor}");
        
        // 背景画像を設定
        if (stageBg.backgroundSprite != null)
        {
            string oldSpriteName = backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null";
            backgroundRenderer.sprite = stageBg.backgroundSprite;
            backgroundRenderer.color = Color.white;
            string newSpriteName = backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null";
            Debug.Log($"[BackgroundManager] Background sprite updated for stage {stageNumber}: {oldSpriteName} -> {newSpriteName}");
            Debug.Log($"[BackgroundManager] SpriteRenderer.sprite after update: {(backgroundRenderer.sprite != null ? backgroundRenderer.sprite.name : "null")}");
        }
        else
        {
            // 背景画像がない場合、背景色を使用
            backgroundRenderer.sprite = null;
            backgroundRenderer.color = stageBg.backgroundColor;
            Debug.Log($"[BackgroundManager] Background color updated for stage {stageNumber}: {stageBg.backgroundColor}");
        }
        
        // カメラの背景色を設定（背景画像がない場合のフォールバック）
        if (mainCamera != null && stageBg.backgroundSprite == null)
        {
            mainCamera.backgroundColor = stageBg.backgroundColor;
        }
        
        Debug.Log($"[BackgroundManager] Background update completed for stage {stageNumber}");
    }
    
    
    /// <summary>
    /// 背景を手動で設定（デバッグ用）
    /// </summary>
    public void SetBackground(int stageNumber)
    {
        UpdateBackground(stageNumber);
    }
}
