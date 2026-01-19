using UnityEngine;
using System;

/// <summary>
/// ゲーム終了処理
/// 城のHPが0になった時の処理を管理
/// </summary>
public class GameEndHandler : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PlayerCastle playerCastle;
    [SerializeField] private EnemyCastle enemyCastle;
    [SerializeField] private WordQuizUI wordQuizUI;
    [SerializeField] private WordLearningSystem wordLearningSystem;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameModeSelectUI gameModeSelectUI;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private IncorrectAnswersListUI incorrectAnswersListUI; // 間違えた問題リストUI
    
    [Header("待機画面BGM設定")]
    [SerializeField] private AudioClip victoryBGM; // 勝利時の待機画面BGM
    [SerializeField] private AudioClip defeatBGM; // 負け時の待機画面BGM
    [SerializeField] private AudioSource bgmAudioSource; // BGM再生用のAudioSource
    [SerializeField] private float bgmVolume = 0.5f; // BGMの音量（0.0〜1.0）
    
    private bool isGameEnded = false;
    private bool isVictory = false; // 勝利フラグ（ゲームモード選択時にStageを進めるため）
    
    private void Start()
    {
        // 自動検出
        if (playerCastle == null)
        {
            playerCastle = FindObjectOfType<PlayerCastle>();
        }
        
        if (enemyCastle == null)
        {
            enemyCastle = FindObjectOfType<EnemyCastle>();
        }
        
        if (wordQuizUI == null)
        {
            wordQuizUI = FindObjectOfType<WordQuizUI>(true);
        }
        
        if (wordLearningSystem == null)
        {
            wordLearningSystem = FindObjectOfType<WordLearningSystem>();
        }
        
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }
        
        if (gameModeSelectUI == null)
        {
            gameModeSelectUI = FindObjectOfType<GameModeSelectUI>(true);
        }
        
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
        }
        
        // IncorrectAnswersListUIを自動検出
        if (incorrectAnswersListUI == null)
        {
            incorrectAnswersListUI = FindObjectOfType<IncorrectAnswersListUI>(true);
        }
        
        // BGM用のAudioSourceを自動検出または作成
        if (bgmAudioSource == null)
        {
            bgmAudioSource = GetComponent<AudioSource>();
            
            // AudioSourceが存在しない場合は作成
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
                bgmAudioSource.playOnAwake = false;
                bgmAudioSource.loop = true; // 待機画面BGMはループ再生
                bgmAudioSource.volume = bgmVolume;
            }
        }
        
        // BGM用のAudioSourceの設定を更新
        if (bgmAudioSource != null)
        {
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = bgmVolume;
        }
        
        // 城のイベントを購読
        if (playerCastle != null)
        {
            playerCastle.OnDestroyed += OnPlayerCastleDestroyed;
        }
        
        if (enemyCastle != null)
        {
            enemyCastle.OnDestroyed += OnEnemyCastleDestroyed;
        }
    }
    
    private void OnDestroy()
    {
        // イベントの購読解除
        if (playerCastle != null)
        {
            playerCastle.OnDestroyed -= OnPlayerCastleDestroyed;
        }
        
        if (enemyCastle != null)
        {
            enemyCastle.OnDestroyed -= OnEnemyCastleDestroyed;
        }
    }
    
    /// <summary>
    /// プレイヤーの城が破壊された時の処理（ゲームオーバー）
    /// </summary>
    private void OnPlayerCastleDestroyed()
    {
        if (isGameEnded)
        {
            Debug.Log("[GameEndHandler] OnPlayerCastleDestroyed called but game already ended. Ignoring.");
            return;
        }
        isGameEnded = true;
        isVictory = false; // 敗北フラグを設定
        
        Debug.Log("[GameEndHandler] Player Castle destroyed. Game Over! CurrentHealth: " + (playerCastle != null ? playerCastle.CurrentHealth.ToString() : "null"));
        
        // フィードバックを表示（赤で「Lost!」）
        if (wordQuizUI != null)
        {
            wordQuizUI.ShowGameEndFeedback("Lost!", false);
        }
        
        // ゲームを停止
        StopGame();
        
        // 間違えた問題リストを表示
        ShowIncorrectAnswersList();
        
        // 負け時の待機画面BGMを再生
        PlayDefeatBGM();
        
        // すぐにゲームモード選択パネルを表示（Win/Lost表示は維持したまま）
        ShowModeSelectionPanel();
    }
    
    /// <summary>
    /// 敵の城が破壊された時の処理（ゲームクリア）
    /// </summary>
    private void OnEnemyCastleDestroyed()
    {
        Debug.Log($"[GameEndHandler] OnEnemyCastleDestroyed called. isGameEnded: {isGameEnded}, enemyCastle.CurrentHealth: {enemyCastle?.CurrentHealth ?? -1}");
        
        if (isGameEnded)
        {
            Debug.LogWarning("[GameEndHandler] OnEnemyCastleDestroyed called but game already ended. Ignoring.");
            return;
        }
        isGameEnded = true;
        isVictory = true; // 勝利フラグを設定
        
        Debug.Log("[GameEndHandler] Enemy Castle destroyed. Victory!");
        
        // フィードバックを表示（緑で「Win!」）
        if (wordQuizUI != null)
        {
            wordQuizUI.ShowGameEndFeedback("Win!", true);
        }
        
        // ゲームを停止
        StopGame();
        
        // 間違えた問題リストを表示
        ShowIncorrectAnswersList();
        
        // 勝利時の待機画面BGMを再生
        PlayVictoryBGM();
        
        // すぐにゲームモード選択パネルを表示（Win/Lost表示は維持したまま）
        ShowModeSelectionPanel();
    }
    
    /// <summary>
    /// ゲームを停止（タイマーやスポーンを停止、キャラクターとエネミーの動きを停止）
    /// </summary>
    private void StopGame()
    {
        // 英単語学習システムを停止（問題生成とタイマーを停止）
        if (wordLearningSystem != null)
        {
            wordLearningSystem.StopGame();
        }
        
        // 敵のスポーンを停止
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        
        // すべてのキャラクターとエネミーの動きを停止（Time.timeScaleを使用）
        Time.timeScale = 0f;
    }
    
    /// <summary>
    /// 間違えた問題リストを表示（ゲーム終了時）
    /// </summary>
    private void ShowIncorrectAnswersList()
    {
        if (incorrectAnswersListUI != null)
        {
            incorrectAnswersListUI.ShowIncorrectAnswersList();
        }
        else
        {
            // 再検索を試みる
            incorrectAnswersListUI = FindObjectOfType<IncorrectAnswersListUI>(true);
            if (incorrectAnswersListUI != null)
            {
                incorrectAnswersListUI.ShowIncorrectAnswersList();
            }
            else
            {
                Debug.Log("[GameEndHandler] IncorrectAnswersListUIが見つかりません。間違いリストは表示されません。");
            }
        }
    }
    
    /// <summary>
    /// ゲームモード選択パネルを表示（Win/Lost表示は維持したまま）
    /// </summary>
    private void ShowModeSelectionPanel()
    {
        // CharacterSelectPanelを非表示
        CharacterSelectUI characterSelectUI = FindObjectOfType<CharacterSelectUI>(true);
        if (characterSelectUI != null)
        {
            characterSelectUI.SetPanelVisible(false);
            characterSelectUI.SetButtonsEnabled(false);
        }
        
        // GameModeSelectPanelを表示（Win/Lost表示は維持したまま）
        if (gameModeSelectUI != null)
        {
            gameModeSelectUI.gameObject.SetActive(true);
        }
        else
        {
            // 再検索を試みる
            gameModeSelectUI = FindObjectOfType<GameModeSelectUI>(true);
            if (gameModeSelectUI != null)
            {
                gameModeSelectUI.gameObject.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// 勝利時の待機画面BGMを再生
    /// </summary>
    private void PlayVictoryBGM()
    {
        if (bgmAudioSource != null && victoryBGM != null)
        {
            bgmAudioSource.clip = victoryBGM;
            bgmAudioSource.volume = bgmVolume;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            Debug.Log("[GameEndHandler] Victory BGM started.");
        }
        else
        {
            if (bgmAudioSource == null)
            {
                Debug.LogWarning("[GameEndHandler] BGM AudioSource is not set. Victory BGM will not play.");
            }
            if (victoryBGM == null)
            {
                Debug.LogWarning("[GameEndHandler] Victory BGM AudioClip is not set. Please assign Victory BGM clip in Inspector.");
            }
        }
    }
    
    /// <summary>
    /// 負け時の待機画面BGMを再生
    /// </summary>
    private void PlayDefeatBGM()
    {
        if (bgmAudioSource != null && defeatBGM != null)
        {
            bgmAudioSource.clip = defeatBGM;
            bgmAudioSource.volume = bgmVolume;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            Debug.Log("[GameEndHandler] Defeat BGM started.");
        }
        else
        {
            if (bgmAudioSource == null)
            {
                Debug.LogWarning("[GameEndHandler] BGM AudioSource is not set. Defeat BGM will not play.");
            }
            if (defeatBGM == null)
            {
                Debug.LogWarning("[GameEndHandler] Defeat BGM AudioClip is not set. Please assign Defeat BGM clip in Inspector.");
            }
        }
    }
    
    /// <summary>
    /// 待機画面BGMを停止（ゲーム再開時などに呼び出す）
    /// </summary>
    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("[GameEndHandler] BGM stopped.");
        }
    }
    
    /// <summary>
    /// ゲーム状態をリセット（ゲーム再開時に呼び出す）
    /// </summary>
    public void ResetGameState()
    {
        isGameEnded = false;
        Debug.Log("[GameEndHandler] Game state reset. isGameEnded: false");
        
        // 間違えた問題リストを非表示にし、カウントをリセット
        if (incorrectAnswersListUI != null)
        {
            incorrectAnswersListUI.HidePanel();
        }
        if (wordLearningSystem != null)
        {
            wordLearningSystem.ClearIncorrectAnswers();
        }
        
        // Time.timeScaleもリセット（念のため）
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// 勝利フラグを取得（ゲームモード選択時にStageを進めるため）
    /// </summary>
    public bool IsVictory => isVictory;
}
