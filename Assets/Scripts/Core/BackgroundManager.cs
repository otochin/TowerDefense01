using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 背景管理システム
/// ステージごとにBackground GameObjectのSpriteを切り替える
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
    
    [Header("カメラ参照")]
    [Tooltip("背景色を設定するカメラ（自動検出も可能）")]
    [SerializeField] private Camera mainCamera;
    
    private SpriteRenderer backgroundRenderer;
    
    private void Awake()
    {
        // Background GameObjectを検索
        FindBackgroundRenderer();
        
        // カメラの自動検出
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }
    }
    
    private void Start()
    {
        // StageManagerのイベントを購読
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;
            // 初期ステージの背景を設定
            UpdateBackground(StageManager.Instance.CurrentStage);
        }
        else
        {
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
    /// Background GameObjectのSpriteRendererを検索
    /// </summary>
    private void FindBackgroundRenderer()
    {
        GameObject backgroundObject = GameObject.Find("Background");
        if (backgroundObject == null)
        {
            Debug.LogWarning("[BackgroundManager] Background GameObject not found. Please create a GameObject named 'Background' with a SpriteRenderer component.");
            return;
        }
        
        // まず自分自身から検索、見つからなければ子オブジェクトから検索（非アクティブも含む）
        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        if (backgroundRenderer == null)
        {
            backgroundRenderer = backgroundObject.GetComponentInChildren<SpriteRenderer>(true);
        }
        
        if (backgroundRenderer == null)
        {
            // Debug.LogWarning("[BackgroundManager] SpriteRenderer component not found on Background GameObject. Automatically adding SpriteRenderer component...");
            backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        }
    }
    
    /// <summary>
    /// ステージ変更時のコールバック
    /// </summary>
    private void OnStageChanged(int newStage)
    {
        UpdateBackground(newStage);
    }
    
    /// <summary>
    /// 背景を更新
    /// </summary>
    public void UpdateBackground(int stageNumber)
    {
        // Background GameObjectのSpriteRendererを再検索（念のため）
        if (backgroundRenderer == null)
        {
            FindBackgroundRenderer();
        }
        
        if (backgroundRenderer == null)
        {
            Debug.LogError("[BackgroundManager] Background Renderer not found. Cannot update background.");
            return;
        }
        
        // ステージに対応する背景を検索
        StageBackground stageBg = stageBackgrounds.Find(bg => bg.stageNumber == stageNumber);
        
        if (stageBg == null)
        {
            // デフォルトでステージ1の背景を使用
            stageBg = stageBackgrounds.Find(bg => bg.stageNumber == 1);
        }
        
        if (stageBg == null)
        {
            Debug.LogWarning("[BackgroundManager] No background configured. Please set up backgrounds in Inspector.");
            return;
        }
        
        // 背景画像を設定
        if (stageBg.backgroundSprite != null)
        {
            backgroundRenderer.sprite = stageBg.backgroundSprite;
            backgroundRenderer.color = Color.white;
        }
        else
        {
            // 背景画像がない場合、背景色を使用
            backgroundRenderer.sprite = null;
            backgroundRenderer.color = stageBg.backgroundColor;
        }
        
        // カメラの背景色を設定（背景画像がない場合のフォールバック）
        if (mainCamera != null && stageBg.backgroundSprite == null)
        {
            mainCamera.backgroundColor = stageBg.backgroundColor;
        }
    }
    
    /// <summary>
    /// 背景を手動で設定（デバッグ用）
    /// </summary>
    public void SetBackground(int stageNumber)
    {
        UpdateBackground(stageNumber);
    }
}
