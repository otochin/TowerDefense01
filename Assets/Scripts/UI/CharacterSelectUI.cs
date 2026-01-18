using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// キャラクター選択UI
/// 画面下部にキャラクターアイコンを表示し、タップ/クリックでキャラクターを召喚
/// </summary>
public class CharacterSelectUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharacterSpawner characterSpawner;
    [SerializeField] private ResourceManager resourceManager;
    
    [Header("キャラクターボタン")]
    [SerializeField] private List<CharacterButton> characterButtons = new List<CharacterButton>();
    
    // ゲームが開始されたかどうか（ゲームモード選択後からtrueになる）
    private bool isGameStarted = false;
    
    private void Awake()
    {
        // CharacterSpawnerを自動検出
        if (characterSpawner == null)
        {
            characterSpawner = FindObjectOfType<CharacterSpawner>();
        }
    }
    
    private void Start()
    {
        Debug.Log($"[CharacterSelectUI] Start called on {gameObject.name}");
        
        // CharacterSpawnerを再検索（Awakeで見つからなかった場合）
        if (characterSpawner == null)
        {
            characterSpawner = FindObjectOfType<CharacterSpawner>();
            if (characterSpawner == null)
            {
                Debug.LogError("[CharacterSelectUI] CharacterSpawnerが見つかりません。シーンにCharacterSpawnerを配置してください。");
            }
            else
            {
                Debug.Log($"[CharacterSelectUI] CharacterSpawner found in Start: {characterSpawner.gameObject.name}");
            }
        }
        else
        {
            Debug.Log($"[CharacterSelectUI] CharacterSpawner already set: {characterSpawner.gameObject.name}");
        }
        
        // ResourceManagerのシングルトンインスタンスを強制的に取得（Startで取得することで、ResourceManagerのAwakeが確実に実行された後に取得できる）
        // Inspectorで設定されていても、シングルトンインスタンスを使用する
        ResourceManager singletonInstance = ResourceManager.Instance;
        if (singletonInstance != null)
        {
            if (resourceManager != singletonInstance)
            {
                Debug.LogWarning($"[CharacterSelectUI] ResourceManager reference mismatch! Inspector setting: {(resourceManager != null ? resourceManager.gameObject.name : "null")}, Singleton: {singletonInstance.gameObject.name}. Using singleton instance.");
            }
            resourceManager = singletonInstance;
            Debug.Log($"[CharacterSelectUI] ResourceManager set in Start: {resourceManager.gameObject.name} (InstanceID: {resourceManager.GetInstanceID()}, CurrentMoney: {resourceManager.CurrentMoney})");
        }
        else
        {
            Debug.LogError("[CharacterSelectUI] ResourceManager.Instanceが見つかりません。シーンにResourceManagerを配置してください。");
        }
        
        // お金変更イベントを購読
        if (resourceManager != null)
        {
            resourceManager.OnMoneyChanged += OnMoneyChanged;
        }
        
        // 各ボタンにクリックイベントを設定
        InitializeButtons();
        
        // ゲームモード選択前はパネル全体を非表示にする
        SetPanelVisible(false);
        
        // 初期UI更新
        UpdateUI(resourceManager != null ? resourceManager.CurrentMoney : 0);
    }
    
    private void OnDestroy()
    {
        // イベントの購読を解除
        if (resourceManager != null)
        {
            resourceManager.OnMoneyChanged -= OnMoneyChanged;
        }
    }
    
    /// <summary>
    /// ボタンを初期化
    /// </summary>
    private void InitializeButtons()
    {
        // 子要素からCharacterButtonを自動検出（設定されていない場合）
        if (characterButtons == null || characterButtons.Count == 0)
        {
            characterButtons = GetComponentsInChildren<CharacterButton>().ToList();
        }
        
        Debug.Log($"[CharacterSelectUI] Found {characterButtons.Count} character buttons.");
        
        // CharacterSpawnerから利用可能なキャラクターリストを取得
        if (characterSpawner != null)
        {
            List<CharacterData> availableCharacters = characterSpawner.GetAvailableCharacters();
            
            Debug.Log($"[CharacterSelectUI] Available characters: {availableCharacters.Count}. Names: {string.Join(", ", availableCharacters.Select(c => c?.CharacterName ?? "null"))}");
            
            // 各ボタンにキャラクターデータを設定
            for (int i = 0; i < characterButtons.Count && i < availableCharacters.Count; i++)
            {
                if (characterButtons[i] != null && availableCharacters[i] != null)
                {
                    Debug.Log($"[CharacterSelectUI] Setting button {i} to character: {availableCharacters[i].CharacterName}");
                    characterButtons[i].SetCharacterData(availableCharacters[i]);
                    
                    // クリックイベントを設定
                    characterButtons[i].OnButtonClicked.RemoveAllListeners();
                    characterButtons[i].OnButtonClicked.AddListener(OnCharacterButtonClicked);
                }
                else
                {
                    Debug.LogWarning($"[CharacterSelectUI] Button {i} or CharacterData {i} is null.");
                }
            }
        }
        else
        {
            Debug.LogError("[CharacterSelectUI] CharacterSpawner is null!");
        }
    }
    
    /// <summary>
    /// キャラクターボタンがクリックされた時の処理
    /// </summary>
    private void OnCharacterButtonClicked(CharacterData characterData)
    {
        Debug.Log($"[CharacterSelectUI] OnCharacterButtonClicked called for: {(characterData != null ? characterData.CharacterName : "null")}");
        
        if (characterData == null)
        {
            Debug.LogWarning("[CharacterSelectUI] CharacterData is null when button clicked.");
            return;
        }
        
        if (characterSpawner == null)
        {
            Debug.LogError("[CharacterSelectUI] CharacterSpawner is null when button clicked. Attempting to find it...");
            characterSpawner = FindObjectOfType<CharacterSpawner>();
            if (characterSpawner == null)
            {
                Debug.LogError("[CharacterSelectUI] CharacterSpawner still not found!");
                return;
            }
            Debug.Log($"[CharacterSelectUI] CharacterSpawner found: {characterSpawner.gameObject.name}");
        }
        
        Debug.Log($"[CharacterSelectUI] Button clicked for {characterData.CharacterName}. Calling SpawnCharacter...");
        
        // キャラクターを召喚
        GameObject spawned = characterSpawner.SpawnCharacter(characterData);
        
        if (spawned == null)
        {
            Debug.LogWarning($"[CharacterSelectUI] Failed to spawn {characterData.CharacterName}");
        }
        else
        {
            Debug.Log($"[CharacterSelectUI] Successfully spawned {characterData.CharacterName}");
        }
        
        // UIを更新（お金が変更されるため）
        if (resourceManager != null)
        {
            UpdateUI(resourceManager.CurrentMoney);
        }
    }
    
    /// <summary>
    /// UI更新（お金が変わった時）
    /// </summary>
    public void UpdateUI(int currentMoney)
    {
        // 各ボタンのロック状態を更新
        UpdateButtonStates();
    }
    
    /// <summary>
    /// ボタンの有効/無効を更新
    /// </summary>
    private void UpdateButtonStates()
    {
        // ゲームが開始されていない場合は更新しない
        if (!isGameStarted)
        {
            return;
        }
        
        foreach (CharacterButton button in characterButtons)
        {
            if (button != null)
            {
                button.OnMoneyChanged(resourceManager != null ? resourceManager.CurrentMoney : 0);
            }
        }
    }
    
    /// <summary>
    /// パネルの表示/非表示を設定（ゲームモード選択後に呼び出す）
    /// </summary>
    public void SetPanelVisible(bool visible)
    {
        Debug.Log($"[CharacterSelectUI] SetPanelVisible called: {visible} on {gameObject.name}. Current active state: {gameObject.activeSelf}, Active in hierarchy: {gameObject.activeInHierarchy}");
        
        isGameStarted = visible;
        
        // CharacterSelectPanel自体を表示/非表示にする
        if (gameObject != null)
        {
            gameObject.SetActive(visible);
            Debug.Log($"[CharacterSelectUI] Panel {(visible ? "shown" : "hidden")}. isGameStarted: {isGameStarted}. After SetActive: activeSelf={gameObject.activeSelf}, activeInHierarchy={gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[CharacterSelectUI] gameObject is null in SetPanelVisible!");
        }
    }
    
    /// <summary>
    /// ボタンの有効/無効を設定（ゲームモード選択後に呼び出す）
    /// </summary>
    public void SetButtonsEnabled(bool enabled)
    {
        Debug.Log($"[CharacterSelectUI] SetButtonsEnabled called: {enabled} on {gameObject.name}. Current active state: {gameObject.activeSelf}");
        
        isGameStarted = enabled;
        
        // パネルを表示する
        SetPanelVisible(enabled);
        
        // ボタンの状態を更新
        if (isGameStarted)
        {
            foreach (CharacterButton button in characterButtons)
            {
                if (button != null)
                {
                    // ゲームが開始されている場合は、Powerが足りるかどうかで判定
                    button.OnMoneyChanged(resourceManager != null ? resourceManager.CurrentMoney : 0);
                }
            }
        }
        else
        {
            // ゲームが開始されていない場合は無効化
            foreach (CharacterButton button in characterButtons)
            {
                if (button != null)
                {
                    button.SetInteractable(false);
                }
            }
        }
        
        Debug.Log($"[CharacterSelectUI] Buttons {(enabled ? "enabled" : "disabled")}. isGameStarted: {isGameStarted}. Active state: {gameObject.activeSelf}");
    }
    
    /// <summary>
    /// お金が変更された時の処理
    /// </summary>
    private void OnMoneyChanged(int currentMoney)
    {
        UpdateUI(currentMoney);
    }
    
    /// <summary>
    /// キャラクターボタンのリストを設定
    /// </summary>
    public void SetCharacterButtons(List<CharacterButton> buttons)
    {
        characterButtons = buttons;
        InitializeButtons();
    }
    
    /// <summary>
    /// キャラクターボタンのリストを取得
    /// </summary>
    public List<CharacterButton> GetCharacterButtons()
    {
        return characterButtons;
    }
}
