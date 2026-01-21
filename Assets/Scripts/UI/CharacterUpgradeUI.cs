using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// キャラクター強化UI
/// ゲーム終了時に表示され、3つのボタンから1つだけ選択できる
/// </summary>
public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharacterUpgradeManager upgradeManager;
    
    [Header("強化ボタン")]
    [SerializeField] private List<CharacterUpgradeButton> upgradeButtons = new List<CharacterUpgradeButton>();
    
    [Header("初期強化設定")]
    [Tooltip("初期表示する強化タイプ（最初はHP強化のみ）")]
    [SerializeField] private CharacterUpgradeManager.UpgradeType initialUpgradeType = CharacterUpgradeManager.UpgradeType.Health;
    
    private bool isUpgradeSelected = false; // 強化が選択されたかどうか
    private bool isInitialized = false; // 初期化済みかどうか
    
    private void Awake()
    {
        // CharacterUpgradeManagerを自動検出
        if (upgradeManager == null)
        {
            upgradeManager = CharacterUpgradeManager.Instance;
        }
        
        // Awake()では非表示にしない（SetPanelVisible()で表示された後にAwake()が呼ばれる可能性があるため）
        // 初期状態は、シーンで非アクティブな状態で配置されていることを前提とする
    }
    
    private void Start()
    {
        Debug.Log($"[CharacterUpgradeUI] Start called on {gameObject.name}");
        
        // CharacterUpgradeManagerを再検索
        if (upgradeManager == null)
        {
            upgradeManager = CharacterUpgradeManager.Instance;
            if (upgradeManager == null)
            {
                Debug.LogError("[CharacterUpgradeUI] CharacterUpgradeManagerが見つかりません。シーンにCharacterUpgradeManagerを配置してください。");
            }
        }
        
        // 各ボタンを初期化
        InitializeButtons();
        
        // 初期状態では非表示（Start()が呼ばれた時点でアクティブな場合のみ）
        // ただし、SetPanelVisible()で既に表示されている場合は、そのまま表示状態を維持
        if (gameObject.activeSelf && !isInitialized)
        {
            SetPanelVisible(false);
        }
        
        isInitialized = true;
    }
    
    private void OnEnable()
    {
        // パネルがアクティブになった時、ボタンを確実に有効化
        if (isInitialized && gameObject.activeSelf)
        {
            // 選択状態をリセット
            isUpgradeSelected = false;
            
            // すべてのボタンを有効化
            foreach (var button in upgradeButtons)
            {
                if (button != null)
                {
                    button.SetInteractable(true);
                }
            }
            
            Debug.Log($"[CharacterUpgradeUI] OnEnable: All buttons enabled. Button count: {upgradeButtons.Count}");
        }
    }
    
    /// <summary>
    /// ボタンを初期化
    /// </summary>
    private void InitializeButtons()
    {
        // 子要素からCharacterUpgradeButtonを自動検出（設定されていない場合）
        if (upgradeButtons == null || upgradeButtons.Count == 0)
        {
            upgradeButtons = GetComponentsInChildren<CharacterUpgradeButton>(true).ToList();
        }
        
        Debug.Log($"[CharacterUpgradeUI] Found {upgradeButtons.Count} upgrade buttons.");
        
        // 各ボタンにキャラクタータイプと強化タイプを設定
        // Warrior, Archer, Mageの順に設定（初期はHP強化のみ）
        CharacterType[] characterTypes = { CharacterType.Warrior, CharacterType.Archer, CharacterType.Mage };
        
        for (int i = 0; i < upgradeButtons.Count && i < characterTypes.Length; i++)
        {
            if (upgradeButtons[i] != null)
            {
                upgradeButtons[i].SetUpgradeInfo(characterTypes[i], initialUpgradeType);
                
                // クリックイベントを設定
                upgradeButtons[i].OnButtonClicked = null; // 既存のイベントをクリア
                CharacterType charType = characterTypes[i]; // クロージャー用にローカル変数に保存
                upgradeButtons[i].OnButtonClicked += (type, upgrade) => OnUpgradeButtonClicked(charType, upgrade);
                
                Debug.Log($"[CharacterUpgradeUI] Button {i} set to: {charType} - {initialUpgradeType}");
            }
        }
    }
    
    /// <summary>
    /// 強化ボタンがクリックされた時の処理
    /// </summary>
    private void OnUpgradeButtonClicked(CharacterType characterType, CharacterUpgradeManager.UpgradeType upgradeType)
    {
        if (isUpgradeSelected)
        {
            Debug.LogWarning("[CharacterUpgradeUI] Upgrade already selected. Ignoring click.");
            return;
        }
        
        if (upgradeManager == null)
        {
            Debug.LogError("[CharacterUpgradeUI] CharacterUpgradeManager is null!");
            return;
        }
        
        Debug.Log($"[CharacterUpgradeUI] Upgrade button clicked: {characterType} - {upgradeType}");
        
        // 強化を実行
        upgradeManager.UpgradeCharacter(characterType, upgradeType);
        
        // 選択済みフラグを設定
        isUpgradeSelected = true;
        
        // すべてのボタンを無効化（1つ選択したら他のボタンは選べない）
        foreach (var button in upgradeButtons)
        {
            if (button != null)
            {
                button.SetInteractable(false);
            }
        }
        
        Debug.Log($"[CharacterUpgradeUI] Upgrade applied: {characterType} - {upgradeType}. All buttons disabled.");
        
        // パネルを非表示にして、ゲームモード選択パネルを表示
        HidePanelAndShowModeSelection();
    }
    
    /// <summary>
    /// パネルを非表示にして、ゲームモード選択パネルを表示
    /// </summary>
    private void HidePanelAndShowModeSelection()
    {
        // パネルを非表示
        SetPanelVisible(false);
        
        // GameEndHandlerを検索して、ゲームモード選択パネルを表示
        GameEndHandler gameEndHandler = FindObjectOfType<GameEndHandler>();
        if (gameEndHandler != null)
        {
            gameEndHandler.ShowGameModeSelection();
            Debug.Log("[CharacterUpgradeUI] GameModeSelectPanel shown after upgrade selection.");
        }
        else
        {
            Debug.LogWarning("[CharacterUpgradeUI] GameEndHandlerが見つかりません。ゲームモード選択パネルを表示できません。");
        }
    }
    
    /// <summary>
    /// パネルの表示/非表示を設定
    /// </summary>
    public void SetPanelVisible(bool visible)
    {
        Debug.Log($"[CharacterUpgradeUI] SetPanelVisible called: {visible} on {gameObject.name}. Current active: {gameObject.activeSelf}, isInitialized: {isInitialized}");
        
        // 初期化が完了していない場合、初期化を実行
        if (!isInitialized)
        {
            InitializeButtons();
            isInitialized = true;
        }
        
        // 表示する場合は、Awake()やStart()で非表示にされないように、先に初期化フラグを設定
        if (visible)
        {
            isInitialized = true;
            
            // パネルが表示される時、選択状態をリセット
            isUpgradeSelected = false;
        }
        
        gameObject.SetActive(visible);
        
        // SetActive()の後、OnEnable()が呼ばれる前にボタンを有効化
        // OnEnable()でも有効化するが、ここでも確実に有効化する
        if (visible)
        {
            // すべてのボタンを有効化
            foreach (var button in upgradeButtons)
            {
                if (button != null)
                {
                    button.SetInteractable(true);
                }
            }
            
            Debug.Log($"[CharacterUpgradeUI] Panel shown. All buttons enabled. Button count: {upgradeButtons.Count}");
        }
        else
        {
            Debug.Log($"[CharacterUpgradeUI] Panel hidden.");
        }
    }
    
    /// <summary>
    /// 強化が選択されたかどうか
    /// </summary>
    public bool IsUpgradeSelected()
    {
        return isUpgradeSelected;
    }
    
    /// <summary>
    /// 選択状態をリセット（ゲーム再開時など）
    /// </summary>
    public void ResetSelection()
    {
        isUpgradeSelected = false;
        
        // すべてのボタンを有効化
        foreach (var button in upgradeButtons)
        {
            if (button != null)
            {
                button.SetInteractable(true);
            }
        }
        
        Debug.Log("[CharacterUpgradeUI] Selection reset. All buttons enabled.");
    }
}
