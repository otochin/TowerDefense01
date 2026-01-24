using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// キャラクター強化UI
/// ゲーム終了時に表示され、6つのボタン（HP強化3つ + Attack Power強化3つ）から1つだけ選択できる
/// </summary>
public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharacterUpgradeManager upgradeManager;
    
    [Header("強化ボタン")]
    [SerializeField] private List<CharacterUpgradeButton> upgradeButtons = new List<CharacterUpgradeButton>();
    
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
        // Debug.Log($"[CharacterUpgradeUI] Start called on {gameObject.name}");
        
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
        // 初期化完了フラグを設定（SetPanelVisible()内での重複ログを防ぐため）
        isInitialized = true;
        
        // 初期状態では非表示（Start()が呼ばれた時点でアクティブな場合のみ）
        // ただし、SetPanelVisible()で既に表示されている場合は、そのまま表示状態を維持
        if (gameObject.activeSelf)
        {
            SetPanelVisible(false);
        }
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
            
            // Debug.Log($"[CharacterUpgradeUI] OnEnable: All buttons enabled. Button count: {upgradeButtons.Count}");
        }
    }
    
    /// <summary>
    /// ボタンを初期化
    /// </summary>
    private void InitializeButtons()
    {
        // 子要素からCharacterUpgradeButtonを自動検出（常に最新の順番で取得）
        upgradeButtons = GetComponentsInChildren<CharacterUpgradeButton>(true).ToList();
        // 名前順にソート（CharacterButton_1, CharacterButton_2, ... の順番にする）
        upgradeButtons.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name));
        
        // 【調査用ログ】初回初期化時のみ詳細ログを出力（Start()から呼ばれる時）
        bool shouldLog = !isInitialized;
        
        if (shouldLog)
        {
            Debug.Log($"[CharacterUpgradeUI] ===== InitializeButtons START =====");
            Debug.Log($"[CharacterUpgradeUI] Found {upgradeButtons.Count} upgrade buttons.");
        }
        
        // Inspectorで設定された値を尊重し、各ボタンの設定を使用
        // すべてのボタンを処理
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            var button = upgradeButtons[i];
            if (button != null)
            {
                // Inspectorで設定された値を取得（既に設定されている場合はそれを使用）
                CharacterType charType = button.GetCharacterType();
                CharacterUpgradeManager.UpgradeType upgradeType = button.GetUpgradeType();
                
                // 【調査用ログ】初回のみ各ボタンのInspector設定値を詳細に出力
                if (shouldLog)
                {
                    Debug.Log($"[CharacterUpgradeUI] === Button {i + 1}/{upgradeButtons.Count}: {button.gameObject.name} ===");
                    Debug.Log($"[CharacterUpgradeUI]   Inspector設定値: CharacterType={charType}, UpgradeType={upgradeType}");
                }
                
                // UIテキストを更新（Inspectorで設定された値に基づいて）
                button.InitializeUI();

                // クリックイベントを設定（Inspectorで設定された値を使用）
                button.OnButtonClicked = null; // 既存のイベントをクリア
                CharacterType charTypeLocal = charType; // クロージャー用にローカル変数に保存
                CharacterUpgradeManager.UpgradeType upgradeTypeLocal = upgradeType; // クロージャー用にローカル変数に保存
                button.OnButtonClicked += (type, upgrade) => OnUpgradeButtonClicked(charTypeLocal, upgradeTypeLocal);

                // 【調査用ログ】初回のみクリックイベントに設定された値を確認
                if (shouldLog)
                {
                    Debug.Log($"[CharacterUpgradeUI]   クリックイベント設定値: CharacterType={charTypeLocal}, UpgradeType={upgradeTypeLocal}");
                    Debug.Log($"[CharacterUpgradeUI]   → このボタンをクリックすると、{charTypeLocal} の {upgradeTypeLocal} が強化されます");
                }
            }
        }
        
        if (shouldLog)
        {
            Debug.Log($"[CharacterUpgradeUI] ===== InitializeButtons END =====");
        }
        
        // 6つ未満の場合は警告
        if (upgradeButtons.Count < 6)
        {
            Debug.LogWarning($"[CharacterUpgradeUI] Expected 6 upgrade buttons, but found {upgradeButtons.Count}. Please add more buttons to the CharacterUpgradePanel.");
        }
    }
    
    /// <summary>
    /// 強化ボタンがクリックされた時の処理
    /// </summary>
    private void OnUpgradeButtonClicked(CharacterType characterType, CharacterUpgradeManager.UpgradeType upgradeType)
    {
        Debug.Log($"[CharacterUpgradeUI] ===== OnUpgradeButtonClicked START =====");
        Debug.Log($"[CharacterUpgradeUI] 【調査用】クリックイベントで受け取った値: CharacterType={characterType}, UpgradeType={upgradeType}");
        
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

        // 現在の強化レベルを確認
        var currentData = upgradeManager.GetUpgradeData(characterType);
        Debug.Log($"[CharacterUpgradeUI] 【調査用】強化前の状態: {characterType} - HP強化レベル={currentData.healthUpgrade}, 攻撃力強化レベル={currentData.attackPowerUpgrade}");

        // 強化を実行
        Debug.Log($"[CharacterUpgradeUI] 【調査用】強化を実行: {characterType} の {upgradeType} を強化します");
        upgradeManager.UpgradeCharacter(characterType, upgradeType);

        // 強化後のレベルを確認
        var afterData = upgradeManager.GetUpgradeData(characterType);
        Debug.Log($"[CharacterUpgradeUI] 【調査用】強化後の状態: {characterType} - HP強化レベル={afterData.healthUpgrade}, 攻撃力強化レベル={afterData.attackPowerUpgrade}");

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

        Debug.Log($"[CharacterUpgradeUI] ===== OnUpgradeButtonClicked END =====");

        // パネルを非表示にして、ゲームモード選択パネルを表示
        HidePanelAndShowModeSelection();
    }
    
    /// <summary>
    /// パネルを非表示にして、ゲームモード選択パネルを表示（敗北時）または次のステージへ進む（勝利時）
    /// </summary>
    private void HidePanelAndShowModeSelection()
    {
        // パネルを非表示
        SetPanelVisible(false);
        
        // GameEndHandlerを検索
        GameEndHandler gameEndHandler = FindObjectOfType<GameEndHandler>();
        if (gameEndHandler == null)
        {
            Debug.LogWarning("[CharacterUpgradeUI] GameEndHandlerが見つかりません。");
            return;
        }
        
        // 勝利時はゲームモード選択パネルを表示せず、直接次のステージへ進む
        if (gameEndHandler.IsVictory)
        {
            // Debug.Log("[CharacterUpgradeUI] Victory detected. Proceeding to next stage without showing game mode selection panel.");
            StartNextStageWithCurrentMode();
        }
        else
        {
            // 敗北時はゲームモード選択パネルを表示
            gameEndHandler.ShowGameModeSelection();
            // Debug.Log("[CharacterUpgradeUI] Defeat detected. GameModeSelectPanel shown after upgrade selection.");
        }
    }
    
    /// <summary>
    /// 現在のゲームモードのまま次のステージへ進む（勝利時）
    /// </summary>
    private void StartNextStageWithCurrentMode()
    {
        // WordLearningSystemから現在のゲームモードを取得
        WordLearningSystem wordLearningSystem = FindObjectOfType<WordLearningSystem>();
        if (wordLearningSystem == null)
        {
            Debug.LogError("[CharacterUpgradeUI] WordLearningSystemが見つかりません。次のステージへ進めません。");
            return;
        }
        
        GameMode currentMode = wordLearningSystem.CurrentGameMode;
        // Debug.Log($"[CharacterUpgradeUI] Starting next stage with current game mode: {currentMode}");
        
        // ステージ管理（勝利時はStageを進める）
        StageManager stageManager = StageManager.Instance;
        if (stageManager != null)
        {
            stageManager.AdvanceStage();
            // Debug.Log($"[CharacterUpgradeUI] Stage advanced to: {stageManager.CurrentStage}");
        }
        
        // ゲーム再開処理（現在のゲームモードのまま）
        GameModeSelectUI gameModeSelectUI = FindObjectOfType<GameModeSelectUI>(true);
        if (gameModeSelectUI != null)
        {
            // 現在のゲームモードのままゲームを再開（ステージ管理は既に実行済み）
            gameModeSelectUI.StartGameWithMode(currentMode);
        }
        else
        {
            Debug.LogError("[CharacterUpgradeUI] GameModeSelectUIが見つかりません。次のステージへ進めません。");
        }
    }
    
    /// <summary>
    /// パネルの表示/非表示を設定
    /// </summary>
    public void SetPanelVisible(bool visible)
    {
        // Debug.Log($"[CharacterUpgradeUI] ===== SetPanelVisible called: {visible} =====");
        // Debug.Log($"[CharacterUpgradeUI] SetPanelVisible called: {visible} on {gameObject.name}. Current active: {gameObject.activeSelf}, isInitialized: {isInitialized}");

        // ボタンの初期化を常に実行（順番が変わる可能性があるため）
        InitializeButtons();
        isInitialized = true;
        
        // 表示する場合は、Awake()やStart()で非表示にされないように、先に初期化フラグを設定
        if (visible)
        {
            isInitialized = true;
            
            // パネルが表示される時、選択状態をリセット
            isUpgradeSelected = false;
            
            // CharacterSelectPanelを非表示にする
            CharacterSelectUI characterSelectUI = FindObjectOfType<CharacterSelectUI>(true);
            if (characterSelectUI != null)
            {
                characterSelectUI.SetPanelVisible(false);
                // Debug.Log("[CharacterUpgradeUI] CharacterSelectPanel hidden while CharacterUpgradePanel is shown.");
            }
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
            
            // Debug.Log($"[CharacterUpgradeUI] Panel shown. All buttons enabled. Button count: {upgradeButtons.Count}");
        }
        // else
        // {
        //     Debug.Log($"[CharacterUpgradeUI] Panel hidden.");
        // }
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
        
        // Debug.Log("[CharacterUpgradeUI] Selection reset. All buttons enabled.");
    }
}
