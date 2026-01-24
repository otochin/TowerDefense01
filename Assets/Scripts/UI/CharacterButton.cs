using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private RuntimeAnimatorController upgradeCharaController; // CharacterButton_1用のUpgradeChara.controller
    
    // イベント
    public UnityEvent<CharacterData> OnButtonClicked = new UnityEvent<CharacterData>();
    
    private ResourceManager resourceManager;
    private bool isLocked = false;
    private bool isAnimationPlaying = false; // アニメーションが再生中かどうか
    private bool lastAnimationState = true; // 前回のアニメーション状態（無駄なPlay()呼び出しを防ぐため）
    
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
            if (animator != null)
            {
                Debug.Log($"[CharacterButton] Awake: Animator検出 - GameObject={gameObject.name}, Icon={iconImage.name}, Animator={animator.name}, Controller={(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "null")}, Enabled={animator.enabled}");
                
                // Controllerが設定されている場合、利用可能なステートを確認
                if (animator.runtimeAnimatorController != null)
                {
                    // Animator Controllerの名前を確認（SelectCharaかUpgradeCharaか）
                    string controllerName = animator.runtimeAnimatorController.name;
                    Debug.Log($"[CharacterButton] Controller名: {controllerName}");
                    
                    // CharacterButton_1の場合は、UpgradeChara.controllerを強制的に設定
                    if (gameObject.name == "CharacterButton_1" && controllerName != "UpgradeChara")
                    {
                        RuntimeAnimatorController upgradeController = upgradeCharaController;
                        
                        // Inspectorで設定されていない場合は、アセットデータベースから読み込む（エディターのみ）
                        if (upgradeController == null)
                        {
                            #if UNITY_EDITOR
                            string assetPath = "Assets/Animation/UpgradeChara.controller";
                            upgradeController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assetPath);
                            #endif
                        }
                        
                        if (upgradeController != null)
                        {
                            animator.runtimeAnimatorController = upgradeController;
                            Debug.Log($"[CharacterButton] CharacterButton_1のControllerをUpgradeCharaに変更しました");
                        }
                        else
                        {
                            Debug.LogWarning($"[CharacterButton] UpgradeChara.controllerが見つかりません。InspectorでupgradeCharaControllerを設定するか、プレハブの設定を確認してください。");
                        }
                    }
                    
                    // AnimatorのUpdate ModeをUnscaled Timeに設定（Time.timeScaleが0でもアニメーションが再生されるように）
                    if (animator.updateMode != AnimatorUpdateMode.UnscaledTime)
                    {
                        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                        Debug.Log($"[CharacterButton] AnimatorのUpdate ModeをUnscaledTimeに設定しました - GameObject={gameObject.name}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[CharacterButton] Awake: Animatorが見つかりません - GameObject={gameObject.name}, Icon={iconImage.name}");
            }
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
        // Animatorを再取得（Awake()で取得できなかった場合に備えて）
        if (animator == null && iconImage != null)
        {
            animator = iconImage.GetComponent<Animator>();
            Debug.Log($"[CharacterButton] Start: Animator再取得 - GameObject={gameObject.name}, Icon={iconImage.name}, Animator={(animator != null ? animator.name : "null")}, Controller={(animator != null && animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "null")}");
        }
        
        // UIを初期化
        InitializeUI();
    }
    
    private void OnEnable()
    {
        // オブジェクトがアクティブになった時、アニメーションを確実に再生
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // ボタンが有効な場合のみアニメーションを再生
            if (button != null && button.interactable)
            {
                animator.enabled = true;
                animator.Play("front", 0, 0f);
                Debug.Log($"[CharacterButton] OnEnable: アニメーション 'front' を再生しました - GameObject={gameObject.name}, Controller={animator.runtimeAnimatorController.name}");
                
                // 次のフレームでアニメーションの状態を確認
                StartCoroutine(CheckAnimationStateNextFrame());
            }
        }
        else if (animator == null && iconImage != null)
        {
            // Animatorがまだ取得できていない場合は再取得を試みる
            animator = iconImage.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null && button != null && button.interactable)
            {
                animator.enabled = true;
                animator.Play("front", 0, 0f);
                Debug.Log($"[CharacterButton] OnEnable: Animator再取得後にアニメーション 'front' を再生しました - GameObject={gameObject.name}, Controller={animator.runtimeAnimatorController.name}");
                StartCoroutine(CheckAnimationStateNextFrame());
            }
        }
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
        // 前回と同じ状態の場合は何もしない（無駄なPlay()呼び出しを防ぐ）
        if (isEnabled == lastAnimationState && animator != null && animator.enabled == isEnabled)
        {
            // 既に同じ状態が設定されている場合は、アニメーションが再生中かどうかを確認
            if (isEnabled && animator.runtimeAnimatorController != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("front") && stateInfo.normalizedTime > 0f)
                {
                    // 既に"front"アニメーションが再生中で、進行している場合は何もしない
                    return;
                }
            }
            else if (!isEnabled)
            {
                // 既に無効になっている場合は何もしない
                return;
            }
        }
        
        Debug.Log($"[CharacterButton] UpdateAnimationState called: isEnabled={isEnabled}, GameObject={gameObject.name}, LastState={lastAnimationState}");
        lastAnimationState = isEnabled;
        
        // Animatorが取得できていない場合は、再取得を試みる
        if (animator == null && iconImage != null)
        {
            animator = iconImage.GetComponent<Animator>();
            Debug.Log($"[CharacterButton] Animator再取得: animator={(animator != null ? animator.name : "null")}, iconImage={iconImage.name}");
        }

        if (animator != null)
        {
            Debug.Log($"[CharacterButton] Animator found: enabled={animator.enabled}, controller={(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "null")}");
            
            if (isEnabled)
            {
                // 有効な場合はアニメーションを再生
                animator.enabled = true;
                Debug.Log($"[CharacterButton] Animator enabled set to true");
                
                // Animator Controllerが設定されているか確認
                if (animator.runtimeAnimatorController != null)
                {
                    // 既に"front"ステートが再生中で、進行している場合はPlay()を呼ばない
                    AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                    if (!currentState.IsName("front") || currentState.normalizedTime == 0f)
                    {
                        // "front"アニメーションを再生
                        animator.Play("front", 0, 0f);
                        isAnimationPlaying = true;
                        Debug.Log($"[CharacterButton] Play animation 'front' called");
                        
                        // 次のフレームでアニメーションの状態を確認するコルーチンを開始
                        StartCoroutine(CheckAnimationStateNextFrame());
                    }
                    else
                    {
                        Debug.Log($"[CharacterButton] Animation 'front' already playing. Normalized time: {currentState.normalizedTime:F3}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[CharacterButton] Animator Controllerが設定されていません。GameObject: {gameObject.name}, Icon: {(iconImage != null ? iconImage.gameObject.name : "null")}");
                }
            }
            else
            {
                // 無効な場合はアニメーションを停止
                animator.enabled = false;
                isAnimationPlaying = false;
                Debug.Log($"[CharacterButton] Animator enabled set to false");
            }
        }
        else
        {
            Debug.LogWarning($"[CharacterButton] Animatorが見つかりません。GameObject: {gameObject.name}, Icon: {(iconImage != null ? iconImage.gameObject.name : "null")}");
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
    
    /// <summary>
    /// 次のフレームでアニメーションの状態を確認するコルーチン
    /// </summary>
    private IEnumerator CheckAnimationStateNextFrame()
    {
        // 1フレーム待機（アニメーションが切り替わるのを待つ）
        yield return null;
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool isFrontState = stateInfo.IsName("front");
            
            Debug.Log($"[CharacterButton] アニメーション状態確認: State name='front': {isFrontState}, State hash: {stateInfo.fullPathHash}, Normalized time: {stateInfo.normalizedTime:F3}, Length: {stateInfo.length:F3}, Controller: {animator.runtimeAnimatorController.name}, Speed: {animator.speed}");
            
            // アニメーションが再生されていない場合は警告
            if (!isFrontState)
            {
                Debug.LogWarning($"[CharacterButton] アニメーション 'front' が再生されていません。現在のステートハッシュ: {stateInfo.fullPathHash}, Controller: {animator.runtimeAnimatorController.name}");
                
                // Animator Controllerに "front" ステートが存在するか確認
                if (animator.runtimeAnimatorController != null)
                {
                    // Animator Controllerの名前を確認
                    string controllerName = animator.runtimeAnimatorController.name;
                    Debug.LogWarning($"[CharacterButton] Controller名: {controllerName}。'front' ステートが存在するか確認してください。");
                }
            }
            else
            {
                // アニメーションが再生されている場合、数フレーム後に再度確認して、アニメーションが進行しているか確認
                yield return new WaitForSeconds(0.1f);
                AnimatorStateInfo stateInfo2 = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo2.IsName("front"))
                {
                    Debug.Log($"[CharacterButton] アニメーション進行確認: Normalized time: {stateInfo.normalizedTime:F3} -> {stateInfo2.normalizedTime:F3} (進行: {stateInfo2.normalizedTime > stateInfo.normalizedTime})");
                }
            }
        }
    }
    
    private void Update()
    {
        // CharacterButton_1の場合のみ、アニメーションの状態を継続的に監視
        if (gameObject.name == "CharacterButton_1" && animator != null && animator.runtimeAnimatorController != null && animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("front"))
            {
                // アニメーションが進行していない場合（Normalized timeが0のまま）は警告
                if (stateInfo.normalizedTime == 0f && isAnimationPlaying)
                {
                    // 最初の数フレームは0のままでも正常なので、数フレーム待ってから警告
                    // ここでは詳細なログは出さない（Update()で毎フレーム呼ばれるため）
                }
            }
        }
    }
}
