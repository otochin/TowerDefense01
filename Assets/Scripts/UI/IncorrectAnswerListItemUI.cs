using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 間違えた問題リストの各アイテムUI
/// </summary>
public class IncorrectAnswerListItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI englishText; // 英語/英熟語テキスト
    [SerializeField] private TextMeshProUGUI japaneseText; // 日本語テキスト
    [SerializeField] private TextMeshProUGUI countText; // 間違えた回数テキスト
    
    [Header("音声読み上げ設定")]
    [SerializeField] private bool enableSpeechOnClick = true; // クリックで音声読み上げを有効にするか
    
    private string currentEnglishText = ""; // 現在の英語テキストを保存
    private MacOSTextToSpeech textToSpeech; // 音声読み上げ用のコンポーネント

    private void Awake()
    {
        // MacOSTextToSpeechを検索または作成
        textToSpeech = FindObjectOfType<MacOSTextToSpeech>();
        if (textToSpeech == null)
        {
            // シーン内に存在しない場合は、このGameObjectに追加
            textToSpeech = gameObject.AddComponent<MacOSTextToSpeech>();
            // Debug.Log("[IncorrectAnswerListItemUI] MacOSTextToSpeech component created.");
        }
        else
        {
            // Debug.Log("[IncorrectAnswerListItemUI] MacOSTextToSpeech component found.");
        }
    }
    
    /// <summary>
    /// アイテムのデータを設定
    /// </summary>
    /// <param name="english">英語/英熟語</param>
    /// <param name="japanese">日本語</param>
    /// <param name="count">間違えた回数</param>
    public void SetData(string english, string japanese, int count)
    {
        // Debug.Log($"[IncorrectAnswerListItemUI] SetData called: english='{english}', japanese='{japanese}', count={count}");
        // Debug.Log($"[IncorrectAnswerListItemUI] Text references - englishText: {(englishText != null ? englishText.name : "NULL")}, japaneseText: {(japaneseText != null ? japaneseText.name : "NULL")}, countText: {(countText != null ? countText.name : "NULL")}");
        
        if (englishText != null)
        {
            englishText.text = english;
            currentEnglishText = english; // 英語テキストを保存
            // Debug.Log($"[IncorrectAnswerListItemUI] Set englishText.text to '{englishText.text}'");
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswerListItemUI] englishText is null! Please assign it in the Inspector.");
        }
        
        if (japaneseText != null)
        {
            japaneseText.text = japanese;
            // Debug.Log($"[IncorrectAnswerListItemUI] Set japaneseText.text to '{japaneseText.text}'");
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswerListItemUI] japaneseText is null! Please assign it in the Inspector.");
        }
        
        if (countText != null)
        {
            countText.text = $"×{count}回";
            // Debug.Log($"[IncorrectAnswerListItemUI] Set countText.text to '{countText.text}'");
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswerListItemUI] countText is null! Please assign it in the Inspector.");
        }
    }
    
    /// <summary>
    /// プレハブ全体（このアイテム）がクリックされた時の処理
    /// （UIのEventSystemから呼ばれる）
    /// </summary>
    public void OnItemClicked()
    {
        if (!string.IsNullOrEmpty(currentEnglishText))
        {
            // Debug.Log($"[IncorrectAnswerListItemUI] Item clicked: '{currentEnglishText}'");
            
            // MacOSTextToSpeechを使用して読み上げ
            if (textToSpeech != null)
            {
                textToSpeech.Speak(currentEnglishText);
            }
            else
            {
                // textToSpeechがnullの場合は再検索を試みる
                textToSpeech = FindObjectOfType<MacOSTextToSpeech>();
                if (textToSpeech != null)
                {
                    textToSpeech.Speak(currentEnglishText);
                }
                else
                {
                    Debug.LogWarning("[IncorrectAnswerListItemUI] MacOSTextToSpeech component not found. Cannot speak.");
                }
            }
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswerListItemUI] Current English text is empty. Cannot speak.");
        }
    }

    /// <summary>
    /// IPointerClickHandlerの実装：このアイテム全体をクリックしたときに呼ばれる
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (enableSpeechOnClick)
        {
            OnItemClicked();
        }
    }
}
