using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// キャラクター召喚システム
/// プレイヤーがキャラクターを召喚する処理を管理
/// </summary>
public class CharacterSpawner : MonoBehaviour
{
    [Header("召喚設定")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private bool autoFaceRight = true;
    [SerializeField] private float randomOffsetRangePixels = 1000f; // ランダムオフセットの範囲（ピクセル単位）
    [SerializeField] private float pixelsPerUnit = 100f; // スプライトのPixels Per Unit（通常は100）
    
    [Header("召喚可能なキャラクター")]
    [SerializeField] private List<CharacterData> availableCharacters = new List<CharacterData>();
    
    [Header("参照")]
    [SerializeField] private ResourceManager resourceManager;
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip spawnSound; // スポーン時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
    // イベント
    public event Action<CharacterData, GameObject> OnCharacterSpawned; // (characterData, spawnedGameObject)
    public event Action<CharacterData> OnSpawnFailed; // 召喚失敗時
    
    private void Awake()
    {
        // SpawnPointが設定されていない場合、このGameObjectの位置を使用
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
        
        // AudioSourceを自動検出または作成
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
    }
    
    private void Start()
    {
        // Debug.Log($"[CharacterSpawner] Start called on {gameObject.name}");
        
        // ResourceManagerのシングルトンインスタンスを強制的に取得（Startで取得することで、ResourceManagerのAwakeが確実に実行された後に取得できる）
        // Inspectorで設定されていても、シングルトンインスタンスを使用する
        ResourceManager singletonInstance = ResourceManager.Instance;
        if (singletonInstance != null)
        {
            if (resourceManager != singletonInstance)
            {
                Debug.LogWarning($"[CharacterSpawner] ResourceManager reference mismatch! Inspector setting: {(resourceManager != null ? resourceManager.gameObject.name : "null")}, Singleton: {singletonInstance.gameObject.name}. Using singleton instance.");
            }
            resourceManager = singletonInstance;
            // Debug.Log($"[CharacterSpawner] ResourceManager set in Start: {resourceManager.gameObject.name} (InstanceID: {resourceManager.GetInstanceID()}, CurrentMoney: {resourceManager.CurrentMoney})");
        }
        else
        {
            Debug.LogError("[CharacterSpawner] ResourceManager.Instanceが見つかりません。シーンにResourceManagerを配置してください。");
        }
    }
    
    /// <summary>
    /// キャラクターを召喚する
    /// </summary>
    /// <param name="characterData">召喚するキャラクターデータ</param>
    /// <returns>召喚されたGameObject（失敗時はnull）</returns>
    public GameObject SpawnCharacter(CharacterData characterData)
    {
        // Debug.Log($"[CharacterSpawner] SpawnCharacter called for: {(characterData != null ? characterData.CharacterName : "null")}");
        
        if (characterData == null)
        {
            Debug.LogWarning("[CharacterSpawner] CharacterData is null. Cannot spawn character.");
            OnSpawnFailed?.Invoke(characterData);
            return null;
        }
        
        // Debug.Log($"[CharacterSpawner] Attempting to spawn {characterData.CharacterName}...");
        
        // お金が足りるかチェック
        if (resourceManager == null)
        {
            Debug.LogError("[CharacterSpawner] ResourceManager is not found. Cannot spawn character.");
            OnSpawnFailed?.Invoke(characterData);
            return null;
        }
        
        if (!resourceManager.CanAfford(characterData.Cost))
        {
            Debug.LogWarning($"[CharacterSpawner] Not enough money to spawn {characterData.CharacterName}. Cost: {characterData.Cost}, Current: {resourceManager.CurrentMoney}");
            OnSpawnFailed?.Invoke(characterData);
            return null;
        }
        
        // 召喚可能なキャラクターリストに含まれているかチェック
        if (availableCharacters != null && availableCharacters.Count > 0)
        {
            if (!availableCharacters.Contains(characterData))
            {
                Debug.LogWarning($"[CharacterSpawner] Character {characterData.CharacterName} is not in the available characters list. Available characters: {string.Join(", ", availableCharacters.Select(c => c.CharacterName))}");
                OnSpawnFailed?.Invoke(characterData);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("[CharacterSpawner] Available characters list is empty or null. All characters should be spawnable.");
        }
        
        // プレハブが設定されているかチェック
        if (characterData.CharacterPrefab == null)
        {
            Debug.LogWarning($"[CharacterSpawner] Character prefab is not set for {characterData.CharacterName}. Money will not be spent.");
            OnSpawnFailed?.Invoke(characterData);
            return null;
        }
        
        // Debug.Log($"[CharacterSpawner] All checks passed. Attempting to spend {characterData.Cost} money...");
        
        // お金を消費
        if (resourceManager != null)
        {
            // Debug.Log($"[CharacterSpawner] Calling TrySpendMoney with cost: {characterData.Cost}, current money: {resourceManager.CurrentMoney}");
            if (!resourceManager.TrySpendMoney(characterData.Cost))
            {
                Debug.LogWarning($"[CharacterSpawner] Failed to spend money for {characterData.CharacterName}.");
                OnSpawnFailed?.Invoke(characterData);
                return null;
            }
            // Debug.Log($"[CharacterSpawner] Successfully spent {characterData.Cost} money. Remaining: {resourceManager.CurrentMoney}");
        }
        else
        {
            Debug.LogError("[CharacterSpawner] ResourceManager is null when trying to spend money!");
        }
        
        // 召喚位置を計算（ランダムオフセットを追加）
        // ピクセル単位をUnity単位に変換（Pixels Per Unitで割る）
        float halfRangeInPixels = randomOffsetRangePixels * 0.5f;
        float halfRangeInUnits = halfRangeInPixels / pixelsPerUnit;
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-halfRangeInUnits, halfRangeInUnits),
            UnityEngine.Random.Range(-halfRangeInUnits, halfRangeInUnits),
            0f
        );
        Vector3 spawnPosition = spawnPoint.position + spawnOffset + randomOffset;
        
        // キャラクターを生成
        GameObject spawnedCharacter = Instantiate(characterData.CharacterPrefab, spawnPosition, Quaternion.identity);
        
        // CharacterDataを設定（プレハブに設定されていない場合に備えて）
        CharacterBase characterBase = spawnedCharacter.GetComponent<CharacterBase>();
        if (characterBase != null)
        {
            // CharacterDataが設定されていない場合、または異なる場合に設定
            if (characterBase.CharacterData == null || characterBase.CharacterData != characterData)
            {
                characterBase.SetCharacterData(characterData);
                // Debug.Log($"[CharacterSpawner] CharacterData set for spawned character: {characterData.CharacterName}");
            }
            else
            {
                // Debug.Log($"[CharacterSpawner] CharacterData already set for spawned character: {characterData.CharacterName}. CharacterData matches.");
            }
        }
        else
        {
            Debug.LogWarning($"[CharacterSpawner] CharacterBase component not found on spawned character: {spawnedCharacter.name}");
        }
        
        // 右を向く設定
        if (autoFaceRight)
        {
            // スケールを反転して右を向かせる（2Dの場合）
            Vector3 scale = spawnedCharacter.transform.localScale;
            if (scale.x < 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            spawnedCharacter.transform.localScale = scale;
            
            // または、Y軸回転で右を向かせる（3Dの場合）
            // spawnedCharacter.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        // タグを設定（プレイヤーキャラクター）
        spawnedCharacter.tag = "Player";
        
        // 効果音を再生
        PlaySpawnSound();
        
        // イベントを発火
        OnCharacterSpawned?.Invoke(characterData, spawnedCharacter);
        
        // Debug.Log($"Spawned {characterData.CharacterName} at {spawnPosition}");
        
        return spawnedCharacter;
    }
    
    /// <summary>
    /// キャラクターを召喚可能かチェック
    /// </summary>
    /// <param name="characterData">チェックするキャラクターデータ</param>
    /// <returns>召喚可能な場合true</returns>
    public bool CanSpawnCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            return false;
        }
        
        // お金が足りるかチェック
        if (resourceManager != null)
        {
            if (!resourceManager.CanAfford(characterData.Cost))
            {
                return false;
            }
        }
        else
        {
            // ResourceManagerが見つからない場合
            return false;
        }
        
        // 召喚可能なキャラクターリストに含まれているかチェック
        if (availableCharacters != null && availableCharacters.Count > 0)
        {
            if (!availableCharacters.Contains(characterData))
            {
                return false;
            }
        }
        
        // プレハブのチェックはSpawnCharacter内で行う（エラーメッセージを分離するため）
        
        return true;
    }
    
    /// <summary>
    /// 召喚可能なキャラクターリストを取得
    /// </summary>
    public List<CharacterData> GetAvailableCharacters()
    {
        return availableCharacters;
    }
    
    /// <summary>
    /// 召喚可能なキャラクターリストを設定
    /// </summary>
    public void SetAvailableCharacters(List<CharacterData> characters)
    {
        availableCharacters = characters;
    }
    
    /// <summary>
    /// 召喚地点を設定
    /// </summary>
    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }
    
    /// <summary>
    /// 召喚地点を取得
    /// </summary>
    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
    
    /// <summary>
    /// スポーン時の効果音を再生する
    /// </summary>
    private void PlaySpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}
