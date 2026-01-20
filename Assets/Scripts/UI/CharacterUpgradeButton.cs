using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// キャラクター強化ボタン
/// 個別の強化ボタンの管理
/// </summary>
public class CharacterUpgradeButton : MonoBehaviour
{
    [Header("キャラクター情報")]
    [SerializeField] private CharacterType characterType = CharacterType.Warrior;
    
    [Header("強化タイプ")]
    [SerializeField] private CharacterUpgradeManager.UpgradeType upgradeType = CharacterUpgradeManager.UpgradeType.Health;
    
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI upgradeText;
    
    [Header("参照")]
    [SerializeField] private Button button;
    
    // イベント
    public System.Action<CharacterType, CharacterUpgradeManager.UpgradeType> OnButtonClicked;
    
    private void Awake()
    {
        // Buttonコンポーネントを自動検出
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        
        // ボタンクリックイベントを設定
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        
        // 子要素を自動検出
        if (characterNameText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
            {
                characterNameText = texts[0];
            }
        }
        
        if (upgradeText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 1)
            {
                upgradeText = texts[1];
            }
            else if (texts.Length == 1 && characterNameText != texts[0])
            {
                upgradeText = texts[0];
            }
        }
    }
    
    private void Start()
    {
        // UIを初期化
        InitializeUI();
    }
    
    private void OnEnable()
    {
        // ボタンがアクティブになった時、確実に有効化
        if (button != null)
        {
            button.interactable = true;
            Debug.Log($"[CharacterUpgradeButton] OnEnable: Button enabled for {characterType} - {upgradeType}");
        }
    }
    
    /// <summary>
    /// キャラクタータイプと強化タイプを設定
    /// </summary>
    public void SetUpgradeInfo(CharacterType type, CharacterUpgradeManager.UpgradeType upgrade)
    {
        characterType = type;
        upgradeType = upgrade;
        InitializeUI();
    }
    
    /// <summary>
    /// UIを初期化
    /// </summary>
    private void InitializeUI()
    {
        // キャラクター名を設定
        if (characterNameText != null)
        {
            characterNameText.text = characterType.ToString();
        }
        
        // 強化内容を設定
        if (upgradeText != null)
        {
            string upgradeDescription = GetUpgradeDescription();
            upgradeText.text = upgradeDescription;
        }
    }
    
    /// <summary>
    /// 強化内容の説明を取得
    /// </summary>
    private string GetUpgradeDescription()
    {
        switch (upgradeType)
        {
            case CharacterUpgradeManager.UpgradeType.Health:
                return "HP +10";
            case CharacterUpgradeManager.UpgradeType.AttackPower:
                return "Attack +5";
            case CharacterUpgradeManager.UpgradeType.AttackSpeed:
                return "Attack Speed +0.1";
            case CharacterUpgradeManager.UpgradeType.MoveSpeed:
                return "Move Speed +0.5";
            default:
                return "Upgrade";
        }
    }
    
    /// <summary>
    /// ボタンがクリックされた時の処理
    /// </summary>
    private void OnButtonClick()
    {
        if (button != null && !button.interactable)
        {
            return;
        }
        
        // イベントを発火
        OnButtonClicked?.Invoke(characterType, upgradeType);
    }
    
    /// <summary>
    /// ボタンの有効/無効を設定
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }
    
    /// <summary>
    /// キャラクタータイプを取得
    /// </summary>
    public CharacterType GetCharacterType()
    {
        return characterType;
    }
    
    /// <summary>
    /// 強化タイプを取得
    /// </summary>
    public CharacterUpgradeManager.UpgradeType GetUpgradeType()
    {
        return upgradeType;
    }
}
