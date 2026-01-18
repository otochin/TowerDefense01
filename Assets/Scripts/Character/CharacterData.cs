using UnityEngine;

/// <summary>
/// キャラクターデータ（ScriptableObject）
/// 各キャラクターのステータス情報を定義
/// </summary>
[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] private string characterName = "New Character";
    [SerializeField] private string description = "";
    [SerializeField] private Sprite icon;
    
    [Header("コスト")]
    [SerializeField] private int cost = 50;
    
    [Header("ステータス")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackSpeed = 1.0f; // 攻撃間隔（秒）
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private int defense = 0;
    
    [Header("プレハブ")]
    [SerializeField] private GameObject characterPrefab;
    
    [Header("キャラクタータイプ")]
    [SerializeField] private CharacterType characterType = CharacterType.Warrior;
    
    // プロパティ
    public string CharacterName => characterName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Cost => cost;
    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;
    public int Defense => defense;
    public GameObject CharacterPrefab => characterPrefab;
    public CharacterType CharacterType => characterType;
}

/// <summary>
/// キャラクタータイプ
/// </summary>
public enum CharacterType
{
    Warrior,    // 近接戦闘型
    Archer,     // 遠距離攻撃型
    Mage        // 魔法攻撃型
}
