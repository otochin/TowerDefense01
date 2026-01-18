using UnityEngine;

/// <summary>
/// 敵データ（ScriptableObject）
/// 各敵のステータス情報を定義
/// </summary>
[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data", order = 2)]
public class EnemyData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] private string enemyName = "New Enemy";
    [SerializeField] private string description = "";
    [SerializeField] private Sprite icon;
    
    [Header("ステータス")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackSpeed = 1.0f; // 攻撃間隔（秒）
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private int defense = 0;
    
    [Header("プレハブ")]
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("敵タイプ")]
    [SerializeField] private EnemyType enemyType = EnemyType.Orc;
    
    // プロパティ
    public string EnemyName => enemyName;
    public string Description => description;
    public Sprite Icon => icon;
    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public int Defense => defense;
    public GameObject EnemyPrefab => enemyPrefab;
    public EnemyType EnemyType => enemyType;
}

/// <summary>
/// 敵タイプ
/// </summary>
public enum EnemyType
{
    Orc,     // 基本敵
    Goblin   // 弱い敵
}
