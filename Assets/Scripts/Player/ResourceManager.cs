using UnityEngine;
using System;

/// <summary>
/// リソース管理システム
/// お金の消費・取得を管理する（英単語問題に正解することでお金を獲得、時間経過による自動増加は無効化）
/// シングルトンパターンで実装（シーンに1つだけ存在する）
/// </summary>
public class ResourceManager : MonoBehaviour
{
    // シングルトンインスタンス
    private static ResourceManager instance;
    public static ResourceManager Instance
    {
        get
        {
            // インスタンスがnullまたは削除されている場合、再検索
            if (instance == null)
            {
                // すべてのResourceManagerを取得
                ResourceManager[] allInstances = FindObjectsOfType<ResourceManager>();
                if (allInstances.Length == 0)
                {
                    Debug.LogError("[ResourceManager] Instance not found. Please add ResourceManager to the scene.");
                }
                else if (allInstances.Length == 1)
                {
                    instance = allInstances[0];
                }
                else
                {
                    // 複数のインスタンスがある場合、「ResourceManager」という名前のGameObjectを優先
                    instance = Array.Find(allInstances, rm => rm.gameObject.name == "ResourceManager");
                    if (instance == null)
                    {
                        // 「ResourceManager」という名前のGameObjectがない場合、最初のものを使用
                        instance = allInstances[0];
                        Debug.LogWarning($"[ResourceManager] Multiple ResourceManager instances found ({allInstances.Length}). Using first instance: {instance.gameObject.name} (InstanceID: {instance.GetInstanceID()}). Please remove duplicate instances.");
                    }
                    else
                    {
                        Debug.Log($"[ResourceManager] Multiple ResourceManager instances found ({allInstances.Length}). Using instance named 'ResourceManager': {instance.gameObject.name} (InstanceID: {instance.GetInstanceID()}).");
                    }
                }
                
                if (instance != null)
                {
                    // Debug.Log($"[ResourceManager] Instance found: {instance.gameObject.name} (InstanceID: {instance.GetInstanceID()}, InitialMoney: {instance.initialMoney})");
                    // Instanceプロパティで取得した際、確実に初期化する
                    if (!instance.isInitialized)
                    {
                        instance.InitializeMoney();
                    }
                }
            }
            return instance;
        }
    }
    
    [Header("初期設定")]
    [SerializeField] private int initialMoney = 100;
    [SerializeField] private float moneyGenerationRate = 1.0f; // お金が増える間隔（秒）（※時間経過による自動増加は無効化、英単語問題に正解することでお金を獲得）
    [SerializeField] private int moneyPerGeneration = 10; // 1回あたりの増加量（※使用しない、英単語問題の正解時のお金はWordLearningSystemで設定）
    [SerializeField] private int maxMoney = -1; // -1で無制限
    [SerializeField] private bool enableAutoMoneyGeneration = false; // 時間経過によるお金の自動増加を有効にするか（英単語学習機能ではfalse）
    
    [Header("現在の状態")]
    [NonSerialized] private int currentMoney;
    
    private float timer = 0f;
    
    // ゲームが開始されたかどうか（ゲームモード選択後からtrueになる）
    private bool isGameStarted = false;
    
    public int CurrentMoney => currentMoney;
    public int MaxMoney => maxMoney;
    
    // お金が変更された時のイベント
    public event Action<int> OnMoneyChanged;
    
    // 初期化済みフラグ（OnEnableとAwakeの両方で初期化しないように）
    private bool isInitialized = false;
    
    private void OnEnable()
    {
        // OnEnableはAwakeより先に実行される可能性があるため、ここでも初期化を試みる
        if (!isInitialized)
        {
            InitializeMoney();
        }
    }
    
    /// <summary>
    /// お金の初期化（OnEnableとAwakeの両方から呼ばれる可能性がある）
    /// </summary>
    private void InitializeMoney()
    {
        // シングルトンパターンの実装
        if (instance == null)
        {
            // 複数のインスタンスがある場合、「ResourceManager」という名前のGameObjectを優先
            ResourceManager[] allInstances = FindObjectsOfType<ResourceManager>();
            if (allInstances.Length > 1)
            {
                // 「ResourceManager」という名前のGameObjectを探す
                ResourceManager preferredInstance = Array.Find(allInstances, rm => rm.gameObject.name == "ResourceManager");
                if (preferredInstance != null)
                {
                    instance = preferredInstance;
                    Debug.Log($"[ResourceManager] Multiple instances found. Using preferred instance: {instance.gameObject.name} (InstanceID: {instance.GetInstanceID()}, InitialMoney: {instance.initialMoney})");
                }
                else
                {
                    instance = this;
                }
            }
            else
            {
                instance = this;
            }
        }
        else if (instance != this)
        {
            // 既に別のインスタンスがシングルトンとして設定されている場合
            Debug.LogWarning($"[ResourceManager] Multiple ResourceManager instances detected! Destroying duplicate on {gameObject.name}. Using existing instance on {instance.gameObject.name} (InstanceID: {instance.GetInstanceID()}, CurrentMoney: {instance.CurrentMoney}).");
            Destroy(gameObject);
            return;
        }
        
        // シングルトンインスタンスとして設定されている場合のみ初期化
        if (instance == this && !isInitialized)
        {
            isInitialized = true;
            // initialMoneyを反映（Inspectorで設定した値が反映されるように）
            currentMoney = initialMoney;
            // Debug.Log($"[ResourceManager] InitializeMoney called on {gameObject.name} (InstanceID: {GetInstanceID()}). Initial money setting: {initialMoney}, Current money: {currentMoney}");
        }
    }
    
    private void Awake()
    {
        // Debug.Log($"[ResourceManager] Awake called on {gameObject.name} (InstanceID: {GetInstanceID()}). Initial money setting: {initialMoney}");
        
        // 初期化（まだ初期化されていない場合）
        if (!isInitialized)
        {
            InitializeMoney();
        }
    }
    
    private void Start()
    {
        // StartでもinitialMoneyを反映（確実にInspectorの値が適用されるように）
        if (instance == this)
        {
            currentMoney = initialMoney;
            // Debug.Log($"[ResourceManager] Start called on {gameObject.name} (InstanceID: {GetInstanceID()}). Setting money to initialMoney: {currentMoney}");
        }
        // 初期状態を通知
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    private void Update()
    {
        // 時間経過でお金を増やす（自動増加が有効で、かつゲームが開始されている場合のみ）
        if (enableAutoMoneyGeneration && isGameStarted)
        {
            timer += Time.deltaTime;
            
            if (timer >= moneyGenerationRate)
            {
                AddMoney(moneyPerGeneration);
                timer = 0f;
            }
        }
    }
    
    /// <summary>
    /// ゲームを開始する（ゲームモード選択後に呼び出す）
    /// これにより、お金の自動増加が開始される
    /// </summary>
    public void StartGame()
    {
        if (!isGameStarted)
        {
            isGameStarted = true;
            Debug.Log($"[ResourceManager] Game started. Auto money generation: {enableAutoMoneyGeneration}");
        }
    }
    
    /// <summary>
    /// お金を追加する
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        
        int oldMoney = currentMoney;
        
        if (maxMoney > 0)
        {
            currentMoney = Mathf.Min(maxMoney, currentMoney + amount);
        }
        else
        {
            currentMoney += amount;
        }
        
        // Debug.Log($"[ResourceManager] AddMoney: {oldMoney} -> {currentMoney} (added {amount})");
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    /// <summary>
    /// お金を消費する（可能かチェック）
    /// </summary>
    public bool TrySpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            int oldMoney = currentMoney;
            currentMoney -= amount;
        Debug.Log($"[ResourceManager] TrySpendMoney on {gameObject.name} (InstanceID: {GetInstanceID()}): {oldMoney} -> {currentMoney} (spent {amount})");
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"[ResourceManager] OnMoneyChanged event invoked with {currentMoney} on {gameObject.name} (InstanceID: {GetInstanceID()})");
            return true;
        }
        
        Debug.LogWarning($"[ResourceManager] TrySpendMoney failed: Cannot afford {amount} (current: {currentMoney})");
        return false;
    }
    
    /// <summary>
    /// お金が足りるかチェック
    /// </summary>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }
    
    /// <summary>
    /// お金を直接設定（デバッグ用）
    /// </summary>
    public void SetMoney(int amount)
    {
        if (amount < 0) amount = 0;
        
        if (maxMoney > 0)
        {
            currentMoney = Mathf.Min(maxMoney, amount);
        }
        else
        {
            currentMoney = amount;
        }
        
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    /// <summary>
    /// お金をリセット（初期値に戻す）
    /// </summary>
    public void ResetMoney()
    {
        currentMoney = initialMoney;
        isGameStarted = false;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"[ResourceManager] Money reset to initial value: {currentMoney}");
    }
}
