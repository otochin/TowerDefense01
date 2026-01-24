using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/// <summary>
/// 英単語学習システム
/// CSVファイルから英単語データを読み込み、問題を生成・管理する
/// </summary>
public class WordLearningSystem : MonoBehaviour
{
    [Header("ゲームモード設定")]
    [SerializeField] private GameMode currentGameMode = GameMode.EnglishToJapanese;
    
    [Header("データ設定")]
    [SerializeField] private TextAsset wordDataCsvFile; // CSVファイルをUnityのTextAssetとして読み込む（未設定時は自動選択）
    [SerializeField] private string wordDataCsvPath = "Data/WordData"; // Resourcesフォルダからの相対パス（拡張子なし、ゲームモードで自動選択）
    [SerializeField] private string phraseDataCsvPath = "Data/PhraseData"; // 英熟語データのパス（Resourcesフォルダからの相対パス、拡張子なし）
    
    [Header("リワード設定")]
    [SerializeField] private float powerRewardMultiplier = 10f; // 正解時のパワー倍率（残りタイマー秒数×この値）
    
    [Header("タイマー設定")]
    [SerializeField] private float answerTimeLimit = 10.0f; // 回答制限時間（秒）
    [SerializeField] private float correctAnswerDisplayTime = 5.0f; // 時間切れ時の正解表示時間（秒）
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip correctAnswerSound; // 正解時の効果音
    [SerializeField] private AudioClip incorrectAnswerSound; // 不正解時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
    [Header("BGM設定")]
    [SerializeField] private AudioClip bgmClip; // BGM用のAudioClip
    [SerializeField] private AudioSource bgmAudioSource; // BGM再生用のAudioSource
    [SerializeField] private float bgmVolume = 0.5f; // BGMの音量（0.0〜1.0）
    
    [Header("現在の状態")]
    [SerializeField] private List<WordData> wordDataList = new List<WordData>();
    [SerializeField] private WordData currentQuestion;
    [SerializeField] private List<string> currentChoices = new List<string>();
    [SerializeField] private int correctChoiceIndex;
    [SerializeField] private float currentTimer;
    [SerializeField] private bool isAnswering = false;
    [SerializeField] private bool isShowingCorrectAnswer = false;
    
    [Header("再出題設定")]
    [SerializeField] private bool enableRetryIncorrectQuestions = true; // 間違えた問題の再出題機能を有効にするか
    [SerializeField] private int retryInterval = 3; // 間違えた問題を何問後に再出題するか（2〜3問後）
    
    // 間違えた問題のキュー（再出題用）
    private Queue<WordData> incorrectQuestionsQueue = new Queue<WordData>();
    private int questionCount = 0; // 問題カウンター
    
    // 間違えた問題の回数を記録（WordDataをキーとして間違えた回数を記録）
    private Dictionary<WordData, int> incorrectAnswersCount = new Dictionary<WordData, int>();
    
    // ResourceManagerへの参照
    private ResourceManager resourceManager;
    
    // WordQuizUIへの参照
    private WordQuizUI wordQuizUI;
    
    // イベント
    public event Action<WordData> OnQuestionGenerated;
    public event Action OnQuestionAnsweredCorrectly;
    public event Action OnQuestionAnsweredIncorrectly;
    
    // プロパティ
    public GameMode CurrentGameMode => currentGameMode;
    public WordData CurrentQuestion => currentQuestion;
    public List<string> CurrentChoices => currentChoices;
    public int CorrectChoiceIndex => correctChoiceIndex;
    public float CurrentTimer => currentTimer;
    public float AnswerTimeLimit => answerTimeLimit;
    public bool IsAnswering => isAnswering;
    public bool IsShowingCorrectAnswer => isShowingCorrectAnswer;
    
    private void Awake()
    {
        // ResourceManagerのシングルトンインスタンスを取得
        resourceManager = ResourceManager.Instance;
        
        if (resourceManager == null)
        {
            Debug.LogError("[WordLearningSystem] ResourceManager.Instanceが見つかりません。シーンにResourceManagerを配置してください。");
        }
        
        // AudioSourceを自動検出または作成（効果音用）
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            // AudioSourceが存在しない場合は作成
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
        }
        
        // BGM用のAudioSourceを自動検出または作成
        if (bgmAudioSource == null)
        {
            // 子オブジェクトからBGM用のAudioSourceを探す
            AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
            foreach (AudioSource source in audioSources)
            {
                if (source != audioSource) // 効果音用とは別のAudioSourceを使用
                {
                    bgmAudioSource = source;
                    break;
                }
            }
            
            // BGM用のAudioSourceが見つからない場合は作成（子オブジェクトとして）
            if (bgmAudioSource == null)
            {
                GameObject bgmObject = new GameObject("BGMAudioSource");
                bgmObject.transform.SetParent(transform);
                bgmAudioSource = bgmObject.AddComponent<AudioSource>();
                bgmAudioSource.playOnAwake = false;
                bgmAudioSource.loop = true; // BGMはループ再生
                bgmAudioSource.volume = bgmVolume;
            }
        }
        
        // BGM用のAudioSourceの設定を更新
        if (bgmAudioSource != null)
        {
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = bgmVolume;
        }
    }
    
    private void Start()
    {
        // WordQuizUIを検索（非アクティブなオブジェクトも含める）
        wordQuizUI = FindObjectOfType<WordQuizUI>(true);
        
        if (wordQuizUI == null)
        {
            Debug.LogWarning("[WordLearningSystem] WordQuizUIが見つかりません。UIの設定を確認してください。");
        }
        
        // 英単語データを読み込む
        LoadWordDataFromCsv();
        
        // ゲームモード選択後にStartGame()が呼ばれるまで問題は生成しない
        // これにより、ゲームモード選択後にゲームが開始される
    }
    
    private void Update()
    {
        // 問題回答中の場合、タイマーを更新
        if (isAnswering && !isShowingCorrectAnswer)
        {
            currentTimer -= Time.deltaTime;
            
            // タイマー更新をUIに通知
            if (wordQuizUI != null)
            {
                wordQuizUI.UpdateTimer(currentTimer);
            }
            
            // 時間切れ
            if (currentTimer <= 0f)
            {
                OnTimeUp();
            }
        }
        
        // 正解表示中の場合はタイマーをカウント
        if (isShowingCorrectAnswer)
        {
            currentTimer -= Time.deltaTime;
            
            if (currentTimer <= 0f)
            {
                isShowingCorrectAnswer = false;
                NextQuestion();
            }
        }
    }
    
    /// <summary>
    /// CSVファイルから英単語データを読み込む
    /// </summary>
    public void LoadWordDataFromCsv()
    {
        wordDataList.Clear();
        
        TextAsset csvFile = null;
        
        // TextAssetが設定されている場合はそれを使用、そうでなければResourcesから読み込む
        if (wordDataCsvFile != null)
        {
            csvFile = wordDataCsvFile;
        }
        else if (!string.IsNullOrEmpty(wordDataCsvPath))
        {
            csvFile = Resources.Load<TextAsset>(wordDataCsvPath);
        }
        
        if (csvFile == null)
        {
            Debug.LogError($"[WordLearningSystem] CSVファイルが見つかりません。パス: {wordDataCsvPath}");
            return;
        }
        
        // CSVファイルをパース
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        // ヘッダー行をスキップ（最初の行が"English,Japanese"の場合）
        int startIndex = 0;
        if (lines.Length > 0 && (lines[0].ToLower().Contains("english") || lines[0].ToLower().Contains("japanese")))
        {
            startIndex = 1;
        }
        
        for (int i = startIndex; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            // カンマで分割
            string[] values = line.Split(',');
            
            if (values.Length >= 2)
            {
                string english = values[0].Trim();
                string japanese = values[1].Trim();
                
                if (!string.IsNullOrEmpty(english) && !string.IsNullOrEmpty(japanese))
                {
                    wordDataList.Add(new WordData(english, japanese));
                }
            }
        }
        
        // Debug.Log($"[WordLearningSystem] 英単語データを{wordDataList.Count}件読み込みました。");
    }
    
    /// <summary>
    /// ランダムな問題を生成（間違えた問題は2〜3問後に再出題）
    /// </summary>
    public void GenerateRandomQuestion()
    {
        if (wordDataList.Count == 0)
        {
            Debug.LogError("[WordLearningSystem] 英単語データがありません。");
            return;
        }
        
        questionCount++;
        
        // 間違えた問題の再出題機能が有効で、キューに間違えた問題がある場合
        if (enableRetryIncorrectQuestions && incorrectQuestionsQueue.Count > 0)
        {
            // retryInterval問ごとに間違えた問題を再出題
            if (questionCount % retryInterval == 0)
            {
                currentQuestion = incorrectQuestionsQueue.Dequeue();
                // Debug.Log($"[WordLearningSystem] Retrying incorrect question: {GetQuestionText(currentQuestion)} (Question #{questionCount})");
            }
            else
            {
                // 通常通りランダムに問題を選択
                currentQuestion = wordDataList[UnityEngine.Random.Range(0, wordDataList.Count)];
            }
        }
        else
        {
            // ランダムに問題を選択
            currentQuestion = wordDataList[UnityEngine.Random.Range(0, wordDataList.Count)];
        }
        
        // 選択肢を生成
        currentChoices = GenerateChoices(currentQuestion);
        
        // 正解のインデックスを設定
        correctChoiceIndex = currentChoices.IndexOf(GetCorrectAnswer(currentQuestion));
        
        // タイマーをリセット
        currentTimer = answerTimeLimit;
        isAnswering = true;
        isShowingCorrectAnswer = false;
        
        // UIを更新
        if (wordQuizUI != null)
        {
            string questionText = GetQuestionText(currentQuestion);
            // Debug.Log($"[WordLearningSystem] Generating question: {questionText}");
            wordQuizUI.DisplayQuestion(questionText);
            wordQuizUI.DisplayChoices(currentChoices);
            wordQuizUI.UpdateTimer(currentTimer);
            wordQuizUI.HideFeedback();
            // Debug.Log($"[WordLearningSystem] UI updated. Timer: {currentTimer}, Choices: {currentChoices.Count}");
        }
        else
        {
            Debug.LogError("[WordLearningSystem] WordQuizUI is null when generating question!");
        }
        
        // イベントを発火
        OnQuestionGenerated?.Invoke(currentQuestion);
    }
    
    /// <summary>
    /// 選択肢を生成（正解1つ、重複なし）
    /// </summary>
    public List<string> GenerateChoices(WordData question)
    {
        List<string> choices = new List<string>();
        
        // 正解を追加
        string correctAnswer = GetCorrectAnswer(question);
        choices.Add(correctAnswer);
        
        // 誤答を2つ追加（重複なし）
        int attempts = 0;
        while (choices.Count < 3 && attempts < 100)
        {
            WordData randomWord = wordDataList[UnityEngine.Random.Range(0, wordDataList.Count)];
            string wrongAnswer = GetWrongAnswer(randomWord);
            
            if (!choices.Contains(wrongAnswer))
            {
                choices.Add(wrongAnswer);
            }
            
            attempts++;
        }
        
        // 選択肢が3つに満たない場合は、残りを正解で埋める（通常は発生しない）
        while (choices.Count < 3)
        {
            choices.Add(correctAnswer);
        }
        
        // 選択肢をシャッフル
        for (int i = 0; i < choices.Count; i++)
        {
            string temp = choices[i];
            int randomIndex = UnityEngine.Random.Range(i, choices.Count);
            choices[i] = choices[randomIndex];
            choices[randomIndex] = temp;
        }
        
        return choices;
    }
    
    /// <summary>
    /// 問題文を取得（ゲームモードに応じて）
    /// </summary>
    private string GetQuestionText(WordData wordData)
    {
        // EnglishToJapanese または PhraseEnglishToJapanese の場合は英語/英熟語を表示
        // それ以外は日本語を表示
        return (currentGameMode == GameMode.EnglishToJapanese || currentGameMode == GameMode.PhraseEnglishToJapanese) 
            ? wordData.English 
            : wordData.Japanese;
    }
    
    /// <summary>
    /// 正解を取得（ゲームモードに応じて）
    /// </summary>
    private string GetCorrectAnswer(WordData wordData)
    {
        // EnglishToJapanese または PhraseEnglishToJapanese の場合は日本語が正解
        // それ以外は英語/英熟語が正解
        return (currentGameMode == GameMode.EnglishToJapanese || currentGameMode == GameMode.PhraseEnglishToJapanese) 
            ? wordData.Japanese 
            : wordData.English;
    }
    
    /// <summary>
    /// 誤答を取得（ゲームモードに応じて）
    /// </summary>
    private string GetWrongAnswer(WordData wordData)
    {
        // GetCorrectAnswerと同じロジック（現在は未使用の可能性あり）
        return GetCorrectAnswer(wordData);
    }
    
    /// <summary>
    /// 選択肢をクリックした時の処理
    /// </summary>
    public void OnChoiceSelected(int choiceIndex)
    {
        if (!isAnswering || isShowingCorrectAnswer)
        {
            return;
        }
        
        isAnswering = false;
        
        if (choiceIndex == correctChoiceIndex)
        {
            // 正解
            OnCorrectAnswer();
        }
        else
        {
            // 不正解
            OnIncorrectAnswer();
        }
    }
    
    /// <summary>
    /// 正解時の処理
    /// </summary>
    private void OnCorrectAnswer()
    {
        // 効果音を再生
        PlaySound(correctAnswerSound);
        
        // タイマーの残り秒数を基にパワーを計算（残り秒数×倍率、四捨五入）
        float remainingTime = Mathf.Max(0f, currentTimer); // 残り時間が負の値にならないように
        int powerReward = Mathf.RoundToInt(remainingTime * powerRewardMultiplier);
        
        // お金を追加
        if (resourceManager != null)
        {
            resourceManager.AddMoney(powerReward);
        }
        
        // UIにフィードバックを表示（獲得パワーを表示）
        if (wordQuizUI != null)
        {
            string feedbackMessage = $"{powerReward} Power Up!";
            wordQuizUI.ShowFeedback(feedbackMessage, true);
            // 正解を表示
            wordQuizUI.ShowCorrectAnswer(GetCorrectAnswer(currentQuestion));
        }
        
        // イベントを発火
        OnQuestionAnsweredCorrectly?.Invoke();
        
        // 少し待ってから次の問題へ
        Invoke(nameof(NextQuestion), 1.5f);
    }
    
    /// <summary>
    /// 不正解時の処理
    /// </summary>
    private void OnIncorrectAnswer()
    {
        // 間違えた問題の回数をカウント
        if (currentQuestion != null)
        {
            if (incorrectAnswersCount.ContainsKey(currentQuestion))
            {
                incorrectAnswersCount[currentQuestion]++;
            }
            else
            {
                incorrectAnswersCount[currentQuestion] = 1;
            }
            // Debug.Log($"[WordLearningSystem] Incorrect answer count for '{GetQuestionText(currentQuestion)}': {incorrectAnswersCount[currentQuestion]}");
        }
        
        // 間違えた問題をキューに追加（再出題用）
        if (enableRetryIncorrectQuestions && currentQuestion != null)
        {
            // 既にキューに同じ問題が含まれていない場合のみ追加
            if (!incorrectQuestionsQueue.Contains(currentQuestion))
            {
                incorrectQuestionsQueue.Enqueue(currentQuestion);
                // Debug.Log($"[WordLearningSystem] Added incorrect question to retry queue: {GetQuestionText(currentQuestion)} (Queue size: {incorrectQuestionsQueue.Count})");
            }
        }
        
        // 効果音を再生
        PlaySound(incorrectAnswerSound);
        
        // UIにフィードバックを表示
        if (wordQuizUI != null)
        {
            wordQuizUI.ShowFeedback("Wrong!", false);
            // 正解を表示
            wordQuizUI.ShowCorrectAnswer(GetCorrectAnswer(currentQuestion));
        }
        
        // イベントを発火
        OnQuestionAnsweredIncorrectly?.Invoke();
        
        // 少し待ってから次の問題へ
        Invoke(nameof(NextQuestion), 1.5f);
    }
    
    /// <summary>
    /// 時間切れ時の処理
    /// </summary>
    private void OnTimeUp()
    {
        if (!isAnswering || isShowingCorrectAnswer)
        {
            return;
        }
        
        isAnswering = false;
        isShowingCorrectAnswer = true;
        currentTimer = correctAnswerDisplayTime;
        
        // 間違えた問題の回数をカウント（時間切れも不正解として扱う）
        if (currentQuestion != null)
        {
            if (incorrectAnswersCount.ContainsKey(currentQuestion))
            {
                incorrectAnswersCount[currentQuestion]++;
            }
            else
            {
                incorrectAnswersCount[currentQuestion] = 1;
            }
            // Debug.Log($"[WordLearningSystem] Time-over answer count for '{GetQuestionText(currentQuestion)}': {incorrectAnswersCount[currentQuestion]}");
        }
        
        // 間違えた問題をキューに追加（再出題用）
        if (enableRetryIncorrectQuestions && currentQuestion != null)
        {
            // 既にキューに同じ問題が含まれていない場合のみ追加
            if (!incorrectQuestionsQueue.Contains(currentQuestion))
            {
                incorrectQuestionsQueue.Enqueue(currentQuestion);
                // Debug.Log($"[WordLearningSystem] Added time-over question to retry queue: {GetQuestionText(currentQuestion)} (Queue size: {incorrectQuestionsQueue.Count})");
            }
        }
        
        // 効果音を再生（不正解と同じ）
        PlaySound(incorrectAnswerSound);
        
        // UIにフィードバックを表示
        if (wordQuizUI != null)
        {
            wordQuizUI.ShowFeedback("Time over!", false);
            wordQuizUI.ShowCorrectAnswer(GetCorrectAnswer(currentQuestion));
        }
        
        // イベントを発火
        OnQuestionAnsweredIncorrectly?.Invoke();
    }
    
    /// <summary>
    /// 次の問題へ
    /// </summary>
    public void NextQuestion()
    {
        // 実行中のInvokeをキャンセル
        CancelInvoke();
        
        // 次の問題を生成
        GenerateRandomQuestion();
    }
    
    /// <summary>
    /// ゲームモードを設定し、対応するCSVファイルを読み込む
    /// </summary>
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        
        // ゲームモードに応じてCSVファイルパスを設定
        switch (mode)
        {
            case GameMode.EnglishToJapanese:
            case GameMode.JapaneseToEnglish:
                // 英単語データを使用
                wordDataCsvPath = "Data/WordData";
                break;
            case GameMode.PhraseEnglishToJapanese:
            case GameMode.PhraseJapaneseToEnglish:
                // 英熟語データを使用
                wordDataCsvPath = phraseDataCsvPath;
                break;
        }
        
        // データを再読み込み
        LoadWordDataFromCsv();
        
        // Debug.Log($"[WordLearningSystem] Game mode set to: {mode}, CSV path: {wordDataCsvPath}");
    }
    
    /// <summary>
    /// 間違えた問題の回数をクリア（ゲーム開始時に呼び出す）
    /// </summary>
    public void ClearIncorrectAnswers()
    {
        incorrectAnswersCount.Clear();
        // Debug.Log("[WordLearningSystem] Incorrect answers count cleared.");
    }
    
    /// <summary>
    /// 間違えた問題を回数順でソートして返す（間違えた回数が多い順）
    /// </summary>
    /// <returns>間違えた問題のリスト（WordData, 間違えた回数）</returns>
    public List<(WordData wordData, int count)> GetIncorrectAnswersSorted()
    {
        return incorrectAnswersCount
            .OrderByDescending(x => x.Value) // 間違えた回数が多い順
            .Select(x => (x.Key, x.Value))
            .ToList();
    }
    
    /// <summary>
    /// ゲームを開始する（ゲームモード選択後に呼び出す）
    /// </summary>
    public void StartGame()
    {
        // Debug.Log($"[WordLearningSystem] StartGame called. Current mode: {currentGameMode}");
        
        // 間違えた問題の記録をクリア（新しいゲーム開始時）
        ClearIncorrectAnswers();
        
        // 英単語データが読み込まれているか確認
        if (wordDataList.Count == 0)
        {
            Debug.LogError("[WordLearningSystem] 英単語データが読み込まれていません。CSVファイルを確認してください。");
            return;
        }
        
        // WordQuizUIが設定されているか確認（非アクティブなオブジェクトも含める）
        if (wordQuizUI == null)
        {
            // Debug.Log("[WordLearningSystem] WordQuizUI is null, searching for it...");
            wordQuizUI = FindObjectOfType<WordQuizUI>(true);
            if (wordQuizUI != null)
            {
                // Debug.Log($"[WordLearningSystem] WordQuizUI found: {wordQuizUI.gameObject.name} (Active: {wordQuizUI.gameObject.activeSelf}, ActiveInHierarchy: {wordQuizUI.gameObject.activeInHierarchy})");
            }
            else
            {
                Debug.LogError("[WordLearningSystem] WordQuizUIが見つかりません。UIの設定を確認してください。");
                return;
            }
        }
        else
        {
            // Debug.Log($"[WordLearningSystem] WordQuizUI already set: {wordQuizUI.gameObject.name} (Active: {wordQuizUI.gameObject.activeSelf}, ActiveInHierarchy: {wordQuizUI.gameObject.activeInHierarchy})");
        }
        
        // WordQuizPanelを表示する（ShowPanel()が呼ばれる前にwordQuizUIがnullでないことを確認）
        if (wordQuizUI != null)
        {
            // Debug.Log("[WordLearningSystem] Calling ShowPanel()...");
            wordQuizUI.ShowPanel();
            // Debug.Log($"[WordLearningSystem] ShowPanel() called. WordQuizPanel active: {wordQuizUI.gameObject.activeSelf}, activeInHierarchy: {wordQuizUI.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[WordLearningSystem] WordQuizUI is null when trying to start game!");
            return;
        }
        
        // ゲーム開始時に間違えた問題のキューをリセット
        incorrectQuestionsQueue.Clear();
        questionCount = 0;
        
        // BGMを開始
        PlayBGM();
        
        // ResourceManagerにゲーム開始を通知（お金の自動増加を開始）
        if (resourceManager != null)
        {
            resourceManager.StartGame();
            // Debug.Log("[WordLearningSystem] Notified ResourceManager that game has started.");
        }
        
        // 初回問題を生成（これによりタイマーが開始される）
        // Debug.Log("[WordLearningSystem] Generating first question...");
        GenerateRandomQuestion();
        
        // Debug.Log($"[WordLearningSystem] Game started with mode: {currentGameMode}");
    }
    
    /// <summary>
    /// ゲームを停止する（タイマーや問題生成を停止）
    /// </summary>
    public void StopGame()
    {
        // 実行中のInvokeをキャンセル
        CancelInvoke();
        
        // 問題回答を停止
        isAnswering = false;
        isShowingCorrectAnswer = false;
        
        // BGMを停止
        StopBGM();
        
        // Debug.Log("[WordLearningSystem] Game stopped.");
    }
    
    /// <summary>
    /// 効果音を再生する
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// BGMを再生する（ゲーム開始時）
    /// </summary>
    private void PlayBGM()
    {
        if (bgmAudioSource != null && bgmClip != null)
        {
            if (!bgmAudioSource.isPlaying || bgmAudioSource.clip != bgmClip)
            {
                bgmAudioSource.clip = bgmClip;
                bgmAudioSource.volume = bgmVolume;
                bgmAudioSource.loop = true;
                bgmAudioSource.Play();
                // Debug.Log("[WordLearningSystem] BGM started.");
            }
        }
        else
        {
            if (bgmAudioSource == null)
            {
                Debug.LogWarning("[WordLearningSystem] BGM AudioSource is not set. BGM will not play.");
            }
            if (bgmClip == null)
            {
                Debug.LogWarning("[WordLearningSystem] BGM AudioClip is not set. Please assign BGM clip in Inspector.");
            }
        }
    }
    
    /// <summary>
    /// BGMを停止する（ゲーム終了時）
    /// </summary>
    private void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            // Debug.Log("[WordLearningSystem] BGM stopped.");
        }
    }
}
