using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームモード選択UI
/// ゲーム開始前に「English to Japanese」または「Japanese to English」を選択する
/// </summary>
public class GameModeSelectUI : MonoBehaviour
{
    [Header("ゲームモード選択ボタン（英単語）")]
    [SerializeField] private Button englishToJapaneseButton;
    [SerializeField] private Button japaneseToEnglishButton;
    
    [Header("ゲームモード選択ボタン（英熟語）")]
    [SerializeField] private Button phraseEnglishToJapaneseButton;
    [SerializeField] private Button phraseJapaneseToEnglishButton;
    
    [Header("参照")]
    [SerializeField] private WordLearningSystem wordLearningSystem;
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private EnemySpawner enemySpawner;
    
    private void Start()
    {
        // WordLearningSystemを自動検出
        if (wordLearningSystem == null)
        {
            wordLearningSystem = FindObjectOfType<WordLearningSystem>();
            if (wordLearningSystem == null)
            {
                Debug.LogError("[GameModeSelectUI] WordLearningSystemが見つかりません。シーンにWordLearningSystemを配置してください。");
            }
        }
        
        // CharacterSelectUIを自動検出（非アクティブなオブジェクトも含める）
        if (characterSelectUI == null)
        {
            // FindObjectsOfTypeを使用して非アクティブなオブジェクトも検索
            CharacterSelectUI[] allSelectUIs = FindObjectsOfType<CharacterSelectUI>(true);
            if (allSelectUIs.Length > 0)
            {
                characterSelectUI = allSelectUIs[0];
                Debug.Log($"[GameModeSelectUI] CharacterSelectUI found: {characterSelectUI.gameObject.name} (Active: {characterSelectUI.gameObject.activeSelf})");
            }
            else
            {
                Debug.LogWarning("[GameModeSelectUI] CharacterSelectUIが見つかりません。CharacterSelectPanelを表示できません。");
            }
        }
        else
        {
            // ゲームモード選択前はCharacterSelectPanelを非表示にする
            if (characterSelectUI != null)
            {
                characterSelectUI.SetPanelVisible(false);
                characterSelectUI.SetButtonsEnabled(false);
            }
        }
        
        // EnemySpawnerを自動検出
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner == null)
            {
                Debug.LogWarning("[GameModeSelectUI] EnemySpawnerが見つかりません。ゲーム開始後に敵がスポーンされない可能性があります。");
            }
        }
        
        // ボタンのクリックイベントを設定
        if (englishToJapaneseButton != null)
        {
            englishToJapaneseButton.onClick.AddListener(() => OnModeSelected(GameMode.EnglishToJapanese));
        }
        else
        {
            Debug.LogWarning("[GameModeSelectUI] EnglishToJapaneseButtonが設定されていません。");
        }
        
        if (japaneseToEnglishButton != null)
        {
            japaneseToEnglishButton.onClick.AddListener(() => OnModeSelected(GameMode.JapaneseToEnglish));
        }
        else
        {
            Debug.LogWarning("[GameModeSelectUI] JapaneseToEnglishButtonが設定されていません。");
        }
        
        // 英熟語用ボタンのクリックイベントを設定
        if (phraseEnglishToJapaneseButton != null)
        {
            phraseEnglishToJapaneseButton.onClick.AddListener(() => OnModeSelected(GameMode.PhraseEnglishToJapanese));
        }
        else
        {
            Debug.LogWarning("[GameModeSelectUI] PhraseEnglishToJapaneseButtonが設定されていません。");
        }
        
        if (phraseJapaneseToEnglishButton != null)
        {
            phraseJapaneseToEnglishButton.onClick.AddListener(() => OnModeSelected(GameMode.PhraseJapaneseToEnglish));
        }
        else
        {
            Debug.LogWarning("[GameModeSelectUI] PhraseJapaneseToEnglishButtonが設定されていません。");
        }
    }
    
    /// <summary>
    /// ゲームモードが選択された時の処理
    /// </summary>
    private void OnModeSelected(GameMode selectedMode)
    {
        Debug.Log($"[GameModeSelectUI] Mode selected: {selectedMode}");
        
        // ステージ管理（勝利時はStageを進める、敗北時はStageをリセット）
        HandleStageManagement();
        
        // ゲーム再開時のリセット処理
        ResetGame();
        
        // WordLearningSystemにゲームモードを設定
        if (wordLearningSystem != null)
        {
            // ゲームモードを設定
            wordLearningSystem.SetGameMode(selectedMode);
            Debug.Log($"[GameModeSelectUI] Game mode set to: {selectedMode}");
            
            // ゲームを開始
            wordLearningSystem.StartGame();
        }
        else
        {
            Debug.LogError("[GameModeSelectUI] WordLearningSystemが見つかりません。ゲームを開始できません。");
            return;
        }
        
        // CharacterSelectPanelを表示して有効化
        // Start()で見つからなかった場合、再検索を試みる（非アクティブなオブジェクトも含める）
        if (characterSelectUI == null)
        {
            CharacterSelectUI[] allSelectUIs = FindObjectsOfType<CharacterSelectUI>(true);
            if (allSelectUIs.Length > 0)
            {
                characterSelectUI = allSelectUIs[0];
                Debug.Log($"[GameModeSelectUI] CharacterSelectUI re-searched. Found: {characterSelectUI.gameObject.name} (Active: {characterSelectUI.gameObject.activeSelf})");
            }
            else
            {
                Debug.LogError("[GameModeSelectUI] CharacterSelectUI re-search failed. CharacterSelectPanel cannot be shown.");
            }
        }
        
        if (characterSelectUI != null)
        {
            Debug.Log($"[GameModeSelectUI] Showing CharacterSelectPanel. Current active state: {characterSelectUI.gameObject.activeSelf}");
            characterSelectUI.SetPanelVisible(true);
            Debug.Log($"[GameModeSelectUI] After SetPanelVisible(true). Active state: {characterSelectUI.gameObject.activeSelf}");
            characterSelectUI.SetButtonsEnabled(true);
            Debug.Log($"[GameModeSelectUI] After SetButtonsEnabled(true). Active state: {characterSelectUI.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("[GameModeSelectUI] CharacterSelectUIが見つかりません。CharacterSelectPanelを表示できません。");
        }
        
        // CharacterUpgradePanelを非表示にする（ゲーム開始時）
        CharacterUpgradeUI characterUpgradeUI = FindObjectOfType<CharacterUpgradeUI>(true);
        if (characterUpgradeUI != null)
        {
            characterUpgradeUI.SetPanelVisible(false);
            characterUpgradeUI.ResetSelection();
            Debug.Log("[GameModeSelectUI] CharacterUpgradePanel hidden.");
        }
        
        // EnemySpawnerのスポーンを開始（ゲームモード選択後）
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
            Debug.Log("[GameModeSelectUI] EnemySpawner.StartSpawning() called.");
        }
        else
        {
            // 再検索を試みる
            enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null)
            {
                enemySpawner.StartSpawning();
                Debug.Log("[GameModeSelectUI] EnemySpawner re-searched and StartSpawning() called.");
            }
            else
            {
                Debug.LogWarning("[GameModeSelectUI] EnemySpawnerが見つかりません。敵がスポーンされない可能性があります。");
            }
        }
        
        // ゲームモード選択UIパネルを非表示にする
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// ゲームをリセット（Powerと城のライフをリセット）
    /// </summary>
    private void ResetGame()
    {
        // Time.timeScaleをリセット（ゲーム再開時）
        Time.timeScale = 1f;
        
        // 待機画面BGMを停止し、ゲーム状態をリセット（新しいゲーム開始時）
        GameEndHandler gameEndHandler = FindObjectOfType<GameEndHandler>();
        if (gameEndHandler != null)
        {
            gameEndHandler.StopBGM();
            gameEndHandler.ResetGameState(); // ゲーム状態（isGameEndedフラグ）をリセット
        }
        
        // WordQuizUIのフィードバックを非表示（新しいゲーム開始時）
        WordQuizUI wordQuizUI = FindObjectOfType<WordQuizUI>(true);
        if (wordQuizUI != null)
        {
            wordQuizUI.HideFeedback();
        }
        
        // Powerをリセット
        ResourceManager resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            resourceManager.ResetMoney();
            Debug.Log("[GameModeSelectUI] Power reset.");
        }
        
        // プレイヤーの城のライフをリセット
        PlayerCastle playerCastle = FindObjectOfType<PlayerCastle>();
        if (playerCastle != null)
        {
            playerCastle.ResetHealth();
            Debug.Log("[GameModeSelectUI] PlayerCastle health reset.");
        }
        
        // 敵の城のライフをリセット
        EnemyCastle enemyCastle = FindObjectOfType<EnemyCastle>();
        if (enemyCastle != null)
        {
            enemyCastle.ResetHealth();
            Debug.Log("[GameModeSelectUI] EnemyCastle health reset.");
        }
        
        // フィールド上のすべてのキャラクターとエネミーを削除
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject character in characters)
        {
            if (character.GetComponent<CharacterBase>() != null)
            {
                Destroy(character);
            }
        }
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyBase>() != null)
            {
                Destroy(enemy);
            }
        }
        
        Debug.Log("[GameModeSelectUI] All characters and enemies removed.");
    }
    
    /// <summary>
    /// ステージ管理（勝利時はStageを進める、敗北時はStageをリセット）
    /// </summary>
    private void HandleStageManagement()
    {
        GameEndHandler gameEndHandler = FindObjectOfType<GameEndHandler>();
        if (gameEndHandler == null)
        {
            Debug.LogWarning("[GameModeSelectUI] GameEndHandlerが見つかりません。ステージ管理をスキップします。");
            return;
        }
        
        StageManager stageManager = StageManager.Instance;
        if (stageManager == null)
        {
            Debug.LogWarning("[GameModeSelectUI] StageManagerが見つかりません。ステージ管理をスキップします。");
            return;
        }
        
        // 勝利時はStageを進める、敗北時はStageをリセット
        if (gameEndHandler.IsVictory)
        {
            stageManager.AdvanceStage();
            Debug.Log($"[GameModeSelectUI] Stage advanced to: {stageManager.CurrentStage}");
        }
        else
        {
            stageManager.ResetStage();
            Debug.Log("[GameModeSelectUI] Stage reset to: 1");
        }
    }
}
