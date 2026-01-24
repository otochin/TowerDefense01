using UnityEngine;
using TMPro;

/// <summary>
/// リソースUI管理
/// お金の表示を更新する
/// </summary>
public class ResourceUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("表示設定")]
    [SerializeField] private string moneyFormat = "Power: {0}";
    
    private ResourceManager resourceManager;
    
    private void Awake()
    {
        // ResourceManagerのシングルトンインスタンスを取得
        resourceManager = ResourceManager.Instance;
        
        if (resourceManager == null)
        {
            Debug.LogError("[ResourceUI] ResourceManager.Instanceが見つかりません。シーンにResourceManagerを配置してください。");
            return;
        }
        
        // Debug.Log($"[ResourceUI] ResourceManager found in Awake: {resourceManager.gameObject.name} (InstanceID: {resourceManager.GetInstanceID()}, CurrentMoney: {resourceManager.CurrentMoney}). Subscribing to OnMoneyChanged event.");
        
        // お金変更イベントを購読
        resourceManager.OnMoneyChanged += UpdateMoneyDisplay;
        
        // Debug.Log($"[ResourceUI] Subscribed to OnMoneyChanged event in Awake.");
    }
    
    private void Start()
    {
        // Debug.Log($"[ResourceUI] Start called on {gameObject.name}");
        
        // ResourceManagerのシングルトンインスタンスを再取得（Awakeで見つからなかった場合）
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
            if (resourceManager != null)
            {
                // Debug.Log($"[ResourceUI] ResourceManager found in Start. Subscribing to OnMoneyChanged event.");
                resourceManager.OnMoneyChanged += UpdateMoneyDisplay;
            }
        }
        
        // MoneyTextが設定されていない場合、子オブジェクトから自動的に探す
        if (moneyText == null)
        {
            moneyText = GetComponentInChildren<TextMeshProUGUI>();
            // if (moneyText != null)
            // {
            //     Debug.Log($"[ResourceUI] Found MoneyText in Start: {moneyText.gameObject.name}");
            // }
        }
        
        // 初期表示を更新
        if (resourceManager != null)
        {
            // Debug.Log($"[ResourceUI] Initializing display with money: {resourceManager.CurrentMoney}");
            UpdateMoneyDisplay(resourceManager.CurrentMoney);
        }
        else
        {
            Debug.LogWarning($"[ResourceUI] ResourceManager is null in Start on {gameObject.name}");
        }
    }
    
    private void OnDestroy()
    {
        // イベントの購読を解除
        if (resourceManager != null)
        {
            resourceManager.OnMoneyChanged -= UpdateMoneyDisplay;
        }
    }
    
    /// <summary>
    /// お金の表示を更新
    /// </summary>
    private void UpdateMoneyDisplay(int money)
    {
        // Debug.Log($"[ResourceUI] UpdateMoneyDisplay called with money: {money} on GameObject: {gameObject.name}. ResourceManager InstanceID: {(resourceManager != null ? resourceManager.GetInstanceID().ToString() : "null")}");
        
        // MoneyTextがnullの場合、子オブジェクトから自動的に探す
        if (moneyText == null)
        {
            Debug.LogWarning("[ResourceUI] MoneyText is null, searching in children...");
            moneyText = GetComponentInChildren<TextMeshProUGUI>();
            if (moneyText == null)
            {
                Debug.LogWarning($"[ResourceUI] MoneyText is null and could not be found in children of {gameObject.name}.");
                return;
            }
            // Debug.Log($"[ResourceUI] Found MoneyText: {moneyText.gameObject.name}");
        }
        
        // テキストを設定
        string formattedText = string.Format(moneyFormat, money);
        string oldText = moneyText.text;
        moneyText.text = formattedText;
        
        // Debug.Log($"[ResourceUI] Money text updated on {gameObject.name}: '{oldText}' -> '{formattedText}'");
    }
}
