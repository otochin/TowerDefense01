using UnityEngine;
using TMPro;

/// <summary>
/// ステージ表示UI
/// 画面右上に「Stage: X」を表示する
/// </summary>
public class StageUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI stageText; // ステージ表示用のテキスト
    
    [Header("表示設定")]
    [SerializeField] private string stageFormat = "Stage: {0}"; // ステージ表示フォーマット
    
    private StageManager stageManager;
    
    private void Start()
    {
        // StageManagerを自動検出
        stageManager = StageManager.Instance;
        
        if (stageManager == null)
        {
            Debug.LogError("[StageUI] StageManagerが見つかりません。シーンにStageManagerを配置してください。");
            return;
        }
        
        // StageTextを自動検出
        if (stageText == null)
        {
            stageText = GetComponentInChildren<TextMeshProUGUI>();
            if (stageText == null)
            {
                Debug.LogError("[StageUI] StageTextが見つかりません。TextMeshProUGUIコンポーネントを設定してください。");
                return;
            }
        }
        
        // 初期ステージを表示
        UpdateStageDisplay();
        
        // ステージ変更イベントを購読
        stageManager.OnStageChanged += OnStageChanged;
    }
    
    private void OnDestroy()
    {
        // イベントの購読解除
        if (stageManager != null)
        {
            stageManager.OnStageChanged -= OnStageChanged;
        }
    }
    
    /// <summary>
    /// ステージが変更された時の処理
    /// </summary>
    private void OnStageChanged(int newStage)
    {
        UpdateStageDisplay();
    }
    
    /// <summary>
    /// ステージ表示を更新
    /// </summary>
    private void UpdateStageDisplay()
    {
        if (stageText != null && stageManager != null)
        {
            stageText.text = string.Format(stageFormat, stageManager.CurrentStage);
        }
    }
}
