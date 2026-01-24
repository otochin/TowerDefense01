using UnityEngine;
using System;

/// <summary>
/// ステージ管理システム
/// ステージ数の管理、勝利時の増加、敗北時のリセットを行う
/// </summary>
public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    
    public static StageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StageManager>();
            }
            return instance;
        }
    }
    
    [Header("ステージ設定")]
    [SerializeField] private int currentStage = 1; // 現在のステージ数
    
    [Header("難易度設定")]
    [SerializeField] private float difficultyMultiplier = 0.9f; // ステージが進むごとのスポーン間隔倍率
    
    /// <summary>
    /// 現在のステージ数
    /// </summary>
    public int CurrentStage => currentStage;
    
    /// <summary>
    /// ステージが進むごとの難易度倍率（スポーン間隔に適用）
    /// </summary>
    public float DifficultyMultiplier => difficultyMultiplier;
    
    /// <summary>
    /// ステージ変更イベント
    /// </summary>
    public event Action<int> OnStageChanged;
    
    private void Awake()
    {
        // シングルトンパターン
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoadはルートGameObjectでのみ動作する
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning($"[StageManager] DontDestroyOnLoad requires root GameObject. {gameObject.name} is not a root GameObject. Consider moving it to root level.");
            }
        }
        else if (instance != this)
        {
            Debug.LogWarning("[StageManager] Duplicate StageManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// ステージを進める（勝利時）
    /// </summary>
    public void AdvanceStage()
    {
        currentStage++;
        Debug.Log($"[StageManager] Stage advanced to: {currentStage}");
        OnStageChanged?.Invoke(currentStage);
    }
    
    /// <summary>
    /// ステージをリセット（敗北時）
    /// </summary>
    public void ResetStage()
    {
        currentStage = 1;
        Debug.Log("[StageManager] Stage reset to: 1");
        OnStageChanged?.Invoke(currentStage);
    }
    
    /// <summary>
    /// ステージに応じたスポーン間隔倍率を取得
    /// Stage 1: 1.0
    /// Stage 2: 0.9
    /// Stage 3: 0.81 (0.9^2)
    /// Stage 4: 0.729 (0.9^3)
    /// </summary>
    public float GetSpawnIntervalMultiplier()
    {
        // Stage 1の場合は1.0を返す
        if (currentStage <= 1)
        {
            return 1.0f;
        }
        
        // Stage 2以降は (0.9)^(stage-1) を返す
        return Mathf.Pow(difficultyMultiplier, currentStage - 1);
    }
    
    /// <summary>
    /// ステージを設定（デバッグ用）
    /// </summary>
    public void SetStage(int stage)
    {
        if (stage < 1)
        {
            Debug.LogWarning($"[StageManager] Invalid stage: {stage}. Setting to 1.");
            stage = 1;
        }
        
        currentStage = stage;
        Debug.Log($"[StageManager] Stage set to: {currentStage}");
        OnStageChanged?.Invoke(currentStage);
    }
}
