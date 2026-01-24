using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// キャラクターボタン
/// 個別のキャラクターボタンの管理
/// </summary>
public class CharacterButton : MonoBehaviour
{
    [Header("キャラクターデータ")]
    [SerializeField] private CharacterData characterData;
    
    [Header("UI参照")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image lockedOverlay;

    [Header("参照")]
    [SerializeField] private Button button;
    [SerializeField] private Animator animator;
    
    // イベント
    public UnityEvent<CharacterData> OnButtonClicked = new UnityEvent<CharacterData>();
    
    private ResourceManager resourceManager;
    private bool isLocked = false;
    
    private void Awake()
    {
        // Buttonコンポーネントを自動検出
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // Animatorコンポーネントを自動検出（Icon GameObjectから）
        if (animator == null && iconImage != null)
        {
            animator = iconImage.GetComponent<Animator>();
        }

        // ResourceManagerを自動検出
        resourceManager = FindObjectOfType<ResourceManager>();

        // ボタンクリックイベントを設定
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        // 子要素を自動検出
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
        }

        if (costText == null)
        {
            costText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    
    private void Start()
    {
        // UIを初期化
        InitializeUI();
    }
    
    /// <summary>
    /// キャラクターデータを設定
    /// </summary>
    public void SetCharacterData(CharacterData data)
    {
        characterData = data;
        InitializeUI();
    }
    
    /// <summary>
    /// UIを初期化
    /// </summary>
    private void InitializeUI()
    {
        if (characterData == null) return;
        
        // アイコンを設定
        if (iconImage != null && characterData.Icon != null)
        {
            iconImage.sprite = characterData.Icon;
        }
        
        // コストテキストを設定
        if (costText != null)
        {
            costText.text = characterData.Cost.ToString();
        }
        
        // ロック状態を更新
        UpdateLockedState();
    }
    
    /// <summary>
    /// ボタンがクリックされた時の処理
    /// </summary>
    private void OnButtonClick()
    {
        if (characterData == null || isLocked)
        {
            return;
        }
        
        // イベントを発火
        OnButtonClicked?.Invoke(characterData);
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

        // アニメーション状態も更新
        UpdateAnimationState(interactable);
    }

    /// <summary>
    /// アニメーション状態を更新
    /// </summary>
    private void UpdateAnimationState(bool isEnabled)
    {
        if (animator != null)
        {
            if (isEnabled)
            {
                // 有効な場合はアニメーションを再生
                animator.enabled = true;
                animator.Play("front", 0, 0f); // "front"アニメーションを再生
            }
            else
            {
                // 無効な場合はアニメーションを停止
                animator.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// ロック状態を表示
    /// </summary>
    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateLockedState();
    }
    
    /// <summary>
    /// ロック状態を更新
    /// </summary>
    private void UpdateLockedState()
    {
        // お金が足りるかチェック
        bool canAfford = resourceManager != null &&
                        characterData != null &&
                        resourceManager.CanAfford(characterData.Cost);

        isLocked = !canAfford;

        // ボタンの有効/無効を設定
        SetInteractable(canAfford);

        // アニメーションを制御
        UpdateAnimationState(canAfford);

        // ロックオーバーレイを表示/非表示
        if (lockedOverlay != null)
        {
            lockedOverlay.gameObject.SetActive(isLocked);
        }

        // ボタンの色を変更（オプション）
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = isLocked ? Color.gray : Color.white;
            button.colors = colors;
        }
    }
    
    /// <summary>
    /// お金が変更された時に呼び出される（CharacterSelectUIから呼び出される）
    /// </summary>
    public void OnMoneyChanged(int currentMoney)
    {
        UpdateLockedState();
    }
    
    /// <summary>
    /// キャラクターデータを取得
    /// </summary>
    public CharacterData GetCharacterData()
    {
        return characterData;
    }
}
