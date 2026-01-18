using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 敵スポナー
/// 敵のスポーンを管理
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("スポーン設定")]
    [SerializeField] private List<EnemyData> availableEnemies = new List<EnemyData>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 3.0f; // スポーン間隔（秒）
    [SerializeField] private bool autoSpawn = false; // 自動スポーン開始（ゲームモード選択後に手動で開始）
    [SerializeField] private float randomOffsetRangePixels = 1000f; // ランダムオフセットの範囲（ピクセル単位）
    [SerializeField] private float pixelsPerUnit = 100f; // スプライトのPixels Per Unit（通常は100）
    
    [Header("スポーン制限")]
    [SerializeField] private int maxEnemiesOnScreen = 10; // 画面上の最大敵数
    [SerializeField] private bool limitEnemies = false; // 敵数を制限するか
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip spawnSound; // スポーン時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
    private float spawnTimer = 0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    private void Awake()
    {
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
        // SpawnPointが設定されていない場合、自身のTransform位置を使用
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
        
        // ゲームモード選択前は自動スポーンしない（ゲームモード選択後に手動でStartSpawning()を呼ぶ）
        // InspectorでautoSpawnがtrueに設定されていても、強制的にfalseにする
        autoSpawn = false;
    }
    
    private void Update()
    {
        if (!autoSpawn) return;
        
        // スポーンタイマーを更新
        spawnTimer += Time.deltaTime;
        
        // スポーン間隔が経過したら敵をスポーン
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
        
        // 死亡した敵をリストから削除
        CleanupDeadEnemies();
    }
    
    /// <summary>
    /// スポーンを開始
    /// </summary>
    public void StartSpawning()
    {
        autoSpawn = true;
        spawnTimer = 0f;
    }
    
    /// <summary>
    /// スポーンを停止
    /// </summary>
    public void StopSpawning()
    {
        autoSpawn = false;
    }
    
    /// <summary>
    /// 敵をスポーンしようとする
    /// </summary>
    private void TrySpawnEnemy()
    {
        // 敵数制限チェック
        if (limitEnemies && spawnedEnemies.Count >= maxEnemiesOnScreen)
        {
            return;
        }
        
        // 利用可能な敵がない場合は何もしない
        if (availableEnemies == null || availableEnemies.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No available enemies to spawn.");
            return;
        }
        
        // ランダムに敵を選択
        EnemyData enemyData = availableEnemies[UnityEngine.Random.Range(0, availableEnemies.Count)];
        
        // 敵をスポーン
        SpawnEnemy(enemyData);
    }
    
    /// <summary>
    /// 指定された敵をスポーン
    /// </summary>
    public GameObject SpawnEnemy(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            Debug.LogWarning("[EnemySpawner] EnemyData is null.");
            return null;
        }
        
        if (enemyData.EnemyPrefab == null)
        {
            Debug.LogWarning($"[EnemySpawner] EnemyPrefab is not set for {enemyData.EnemyName}.");
            return null;
        }
        
        // スポーン位置を決定（ランダムオフセットを追加）
        // ピクセル単位をUnity単位に変換（Pixels Per Unitで割る）
        Vector3 basePosition = spawnPoint != null ? spawnPoint.position : transform.position;
        float halfRangeInPixels = randomOffsetRangePixels * 0.5f;
        float halfRangeInUnits = halfRangeInPixels / pixelsPerUnit;
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-halfRangeInUnits, halfRangeInUnits),
            UnityEngine.Random.Range(-halfRangeInUnits, halfRangeInUnits),
            0f
        );
        Vector3 spawnPosition = basePosition + randomOffset;
        
        // 敵をインスタンス化
        GameObject enemyInstance = Instantiate(enemyData.EnemyPrefab, spawnPosition, Quaternion.identity);
        
        // EnemyBaseにEnemyDataを設定
        EnemyBase enemyBase = enemyInstance.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.SetEnemyData(enemyData);
        }
        
        // 敵にEnemyタグを設定
        enemyInstance.tag = "Enemy";
        
        // スポーンした敵をリストに追加
        spawnedEnemies.Add(enemyInstance);
        
        // 敵の死亡イベントを購読（リストから削除するため）
        if (enemyBase != null)
        {
            enemyBase.OnDeath += () => OnEnemyDeath(enemyInstance);
        }
        
        // 効果音を再生
        PlaySpawnSound();
        
        return enemyInstance;
    }
    
    /// <summary>
    /// 敵が死亡した時の処理
    /// </summary>
    private void OnEnemyDeath(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }
    
    /// <summary>
    /// 死亡した敵をリストから削除
    /// </summary>
    private void CleanupDeadEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null || (enemy.GetComponent<EnemyBase>() != null && enemy.GetComponent<EnemyBase>().IsDead));
    }
    
    /// <summary>
    /// スポーン間隔を設定
    /// </summary>
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.1f, interval);
    }
    
    /// <summary>
    /// 利用可能な敵リストを設定
    /// </summary>
    public void SetAvailableEnemies(List<EnemyData> enemies)
    {
        availableEnemies = enemies;
    }
    
    /// <summary>
    /// 現在画面上にいる敵の数
    /// </summary>
    public int CurrentEnemyCount => spawnedEnemies.Count;
    
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
