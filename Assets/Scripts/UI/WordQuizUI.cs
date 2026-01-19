using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 英単語問題UI
/// 画面中央に問題と選択肢を表示し、ユーザーの入力を処理する
/// </summary>
public class WordQuizUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> choiceButtons = new List<Button>();
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI correctAnswerText;
    [SerializeField] private GameObject feedbackPanel;
    
    [Header("設定")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color feedbackPanelCorrectColor = new Color(0f, 1f, 0f, 0.3f); // 緑（半透明）
    [SerializeField] private Color feedbackPanelIncorrectColor = new Color(1f, 0f, 0f, 0.3f); // 赤（半透明）
    
    // WordLearningSystemへの参照
    private WordLearningSystem wordLearningSystem;
    
    // MacOSTextToSpeechへの参照
    private MacOSTextToSpeech textToSpeech;
    
    private void Awake()
    {
        // WordLearningSystemを検索
        wordLearningSystem = FindObjectOfType<WordLearningSystem>();
        
        if (wordLearningSystem == null)
        {
            Debug.LogWarning("[WordQuizUI] WordLearningSystemが見つかりません。UIの設定を確認してください。");
        }
        
        // MacOSTextToSpeechを検索（存在しない場合は作成）
        textToSpeech = FindObjectOfType<MacOSTextToSpeech>();
        if (textToSpeech == null)
        {
            // WordQuizUIと同じGameObjectにMacOSTextToSpeechを追加
            textToSpeech = gameObject.AddComponent<MacOSTextToSpeech>();
        }
        
        // WordQuizPanel全体を初期状態で非表示にする（ゲームモード選択後に表示される）
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
        
        // タイマーテキストを自動検出
        if (timerText == null)
        {
            timerText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // 選択肢ボタンを自動検出
        if (choiceButtons.Count == 0)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            choiceButtons.AddRange(buttons);
        }
        
        // ボタンのイベントを設定
        SetupButtons();
    }
    
    private void Start()
    {
        // WordQuizPanelはAwake()で既に非表示にしているため、Start()では何もしない
        // （ゲームモード選択後にShowPanel()が呼ばれて表示される）
        
        // フィードバックを初期状態で非表示
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
        
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
        
        if (correctAnswerText != null)
        {
            correctAnswerText.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// WordQuizPanelを表示する（ゲームモード選択後に呼び出す）
    /// </summary>
    public void ShowPanel()
    {
        if (gameObject != null)
        {
            Debug.Log($"[WordQuizUI] ShowPanel called. gameObject: {gameObject.name}, current active state: {gameObject.activeSelf}");
            Debug.Log($"[WordQuizUI] Parent Canvas active: {(gameObject.transform.parent != null ? gameObject.transform.parent.gameObject.activeSelf : "no parent")}");
            
            gameObject.SetActive(true);
            
            Debug.Log($"[WordQuizUI] After SetActive(true). Active state: {gameObject.activeSelf}, Active in hierarchy: {gameObject.activeInHierarchy}");
            
            // パネルが表示された後に、UI要素を再検索（非アクティブな状態では検出できないため）
            if (timerText == null)
            {
                timerText = GetComponentInChildren<TextMeshProUGUI>(true);
                if (timerText != null && timerText.gameObject.name.Contains("Timer"))
                {
                    Debug.Log($"[WordQuizUI] TimerText found after ShowPanel: {timerText.gameObject.name}");
                }
                else
                {
                    // TimerTextを探す（すべての子オブジェクトから）
                    TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI text in allTexts)
                    {
                        if (text.gameObject.name == "TimerText")
                        {
                            timerText = text;
                            Debug.Log($"[WordQuizUI] TimerText found by name: {text.gameObject.name}");
                            break;
                        }
                    }
                }
            }
            
            if (questionText == null)
            {
                TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI text in allTexts)
                {
                    if (text.gameObject.name == "QuestionText")
                    {
                        questionText = text;
                        Debug.Log($"[WordQuizUI] QuestionText found: {text.gameObject.name}");
                        break;
                    }
                }
            }
            
            if (choiceButtons.Count == 0)
            {
                Button[] buttons = GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    if (button.gameObject.name.Contains("ChoiceButton"))
                    {
                        choiceButtons.Add(button);
                    }
                }
                Debug.Log($"[WordQuizUI] Found {choiceButtons.Count} choice buttons after ShowPanel");
            }
            
            // ゲーム再開時：タイマー、問題、選択肢ボタンを再表示（ShowGameEndFeedbackで非表示にしたものを表示）
            if (timerText != null && !timerText.gameObject.activeSelf)
            {
                timerText.gameObject.SetActive(true);
            }
            
            if (questionText != null && !questionText.gameObject.activeSelf)
            {
                questionText.gameObject.SetActive(true);
            }
            
            foreach (Button button in choiceButtons)
            {
                if (button != null && !button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);
                }
            }
        }
    }
    
    /// <summary>
    /// WordQuizPanelを非表示にする
    /// </summary>
    public void HidePanel()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// ボタンのイベントを設定
    /// </summary>
    private void SetupButtons()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            int index = i; // クロージャー用にローカル変数にコピー
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClicked(index));
            }
        }
    }
    
    /// <summary>
    /// 選択肢ボタンがクリックされた時の処理
    /// </summary>
    private void OnChoiceButtonClicked(int choiceIndex)
    {
        if (wordLearningSystem != null)
        {
            wordLearningSystem.OnChoiceSelected(choiceIndex);
        }
    }
    
    /// <summary>
    /// タイマーを更新
    /// </summary>
    public void UpdateTimer(float remainingTime)
    {
        // TimerTextがnullの場合は再検索を試みる
        if (timerText == null)
        {
            TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (text.gameObject.name == "TimerText" || text.gameObject.name.Contains("Timer"))
                {
                    timerText = text;
                    Debug.Log($"[WordQuizUI] TimerText found in UpdateTimer: {text.gameObject.name}");
                    break;
                }
            }
        }
        
        if (timerText != null)
        {
            timerText.text = $"Time: {remainingTime:F1}s";
            
            // 残り時間が少ない場合は色を変更
            if (remainingTime <= 3f)
            {
                timerText.color = Color.red;
            }
            else if (remainingTime <= 5f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = normalColor;
            }
        }
        else
        {
            Debug.LogWarning("[WordQuizUI] TimerText is null! Cannot update timer display. Please set TimerText in Inspector.");
        }
    }
    
    /// <summary>
    /// 問題を表示
    /// </summary>
    public void DisplayQuestion(string question)
    {
        // まず問題テキストを表示（即座に表示）
        if (questionText != null)
        {
            questionText.text = question;
        }
        
        // テキスト読み上げ（表示後に少し遅延してから開始）
        if (textToSpeech == null)
        {
            // textToSpeechがnullの場合は再検索を試みる
            textToSpeech = FindObjectOfType<MacOSTextToSpeech>();
            if (textToSpeech == null)
            {
                // 見つからない場合は、このGameObjectに追加
                textToSpeech = gameObject.AddComponent<MacOSTextToSpeech>();
                Debug.Log("[WordQuizUI] MacOSTextToSpeech component created in DisplayQuestion");
            }
            else
            {
                Debug.Log("[WordQuizUI] MacOSTextToSpeech component found in DisplayQuestion");
            }
        }
        
        if (textToSpeech != null)
        {
            Debug.Log($"[WordQuizUI] Calling Speak: {question}");
            // 少し遅延してから音声を再生（問題表示が先に完了してから）
            StartCoroutine(SpeakWithDelay(question, 0.1f));
        }
        else
        {
            Debug.LogWarning("[WordQuizUI] textToSpeech is null! Cannot speak text.");
        }
    }
    
    /// <summary>
    /// 遅延後に音声を再生するコルーチン
    /// </summary>
    private System.Collections.IEnumerator SpeakWithDelay(string text, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (textToSpeech != null)
        {
            textToSpeech.Speak(text);
        }
    }
    
    /// <summary>
    /// 選択肢を表示
    /// </summary>
    public void DisplayChoices(List<string> choices)
    {
        for (int i = 0; i < choiceButtons.Count && i < choices.Count; i++)
        {
            if (choiceButtons[i] != null)
            {
                TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = choices[i];
                }
                
                // ボタンを有効化
                choiceButtons[i].interactable = true;
            }
        }
    }
    
    /// <summary>
    /// フィードバックを表示
    /// </summary>
    public void ShowFeedback(string feedback, bool isCorrect)
    {
        if (feedbackText != null)
        {
            feedbackText.text = feedback;
            feedbackText.color = isCorrect ? correctColor : incorrectColor;
            feedbackText.gameObject.SetActive(true);
        }
        
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
            
            // FeedbackPanelの背景色を変更（正解時は緑、不正解時は赤）
            Image panelImage = feedbackPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = isCorrect ? feedbackPanelCorrectColor : feedbackPanelIncorrectColor;
            }
        }
        
        // 選択肢ボタンを無効化（フィードバック表示中は選択できないようにする）
        SetButtonsInteractable(false);
    }
    
    /// <summary>
    /// 正解を表示（時間切れ時）
    /// </summary>
    public void ShowCorrectAnswer(string correctAnswer)
    {
        if (correctAnswerText != null)
        {
            correctAnswerText.text = correctAnswer;
            correctAnswerText.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// フィードバックを非表示
    /// </summary>
    public void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
        
        if (correctAnswerText != null)
        {
            correctAnswerText.gameObject.SetActive(false);
        }
        
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
        
        // 選択肢ボタンを有効化
        SetButtonsInteractable(true);
    }
    
    /// <summary>
    /// ゲーム終了時のフィードバックを表示（Win!/Lost!）
    /// </summary>
    public void ShowGameEndFeedback(string message, bool isWin)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = isWin ? correctColor : incorrectColor;
            feedbackText.gameObject.SetActive(true);
        }
        
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
            
            // FeedbackPanelの背景色を変更（Win時は緑、Lost時は赤）
            Image panelImage = feedbackPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = isWin ? feedbackPanelCorrectColor : feedbackPanelIncorrectColor;
            }
        }
        
        // 選択肢ボタンを無効化
        SetButtonsInteractable(false);
        
        // タイマーを非表示（オプション）
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        
        // QuestionTextは表示したまま（勝利・敗北時の待機画面でも表示）
        // if (questionText != null)
        // {
        //     questionText.gameObject.SetActive(false);
        // }
        
        // CorrectAnswerTextも表示したまま（勝利・敗北時の待機画面でも表示）
        // if (correctAnswerText != null)
        // {
        //     correctAnswerText.gameObject.SetActive(false);
        // }
        
        // 選択肢ボタンを非表示
        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// ボタンの有効/無効を設定
    /// </summary>
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }
}
