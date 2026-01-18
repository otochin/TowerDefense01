using UnityEngine;

/// <summary>
/// 投射物（矢、魔法弾など）
/// ターゲットに向かって移動し、衝突時にダメージを与える
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("投射物設定")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f; // 生存時間（秒）
    [SerializeField] private bool destroyOnHit = true; // 衝突時に破棄するか
    
    [Header("参照")]
    private Transform target; // ターゲット
    private Vector3 targetPosition; // ターゲット位置（固定）
    private Rigidbody2D rb2D;
    
    private float spawnTime;
    private bool hasTarget = false;
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // 投射物は物理演算に影響されないように設定
        rb2D.gravityScale = 0f;
        rb2D.bodyType = RigidbodyType2D.Kinematic;
    }
    
    private void Start()
    {
        spawnTime = Time.time;
    }
    
    private void Update()
    {
        // 生存時間を超えたら破棄
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
            return;
        }
        
        // ターゲットが死亡または削除された場合は破棄
        if (hasTarget && target != null)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsDead)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
    
    private void FixedUpdate()
    {
        // 移動処理
        Move();
    }
    
    /// <summary>
    /// 投射物を初期化
    /// </summary>
    /// <param name="targetTransform">ターゲットのTransform</param>
    /// <param name="damageAmount">ダメージ量</param>
    /// <param name="projectileSpeed">投射物の速度</param>
    public void Initialize(Transform targetTransform, int damageAmount, float projectileSpeed = 10f)
    {
        target = targetTransform;
        damage = damageAmount;
        speed = projectileSpeed;
        hasTarget = true;
        
        // ターゲット位置を固定（移動中のターゲットにも対応）
        if (target != null)
        {
            targetPosition = target.position;
        }
    }
    
    /// <summary>
    /// 投射物を初期化（位置指定版）
    /// </summary>
    /// <param name="targetPos">ターゲット位置</param>
    /// <param name="damageAmount">ダメージ量</param>
    /// <param name="projectileSpeed">投射物の速度</param>
    public void Initialize(Vector3 targetPos, int damageAmount, float projectileSpeed = 10f)
    {
        target = null;
        targetPosition = targetPos;
        damage = damageAmount;
        speed = projectileSpeed;
        hasTarget = false;
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        Vector3 direction;
        
        // ターゲットがある場合はターゲット位置を更新
        if (hasTarget && target != null)
        {
            targetPosition = target.position;
            direction = (targetPosition - transform.position).normalized;
        }
        else
        {
            // 固定位置に向かう
            direction = (targetPosition - transform.position).normalized;
        }
        
        // 移動
        Vector2 movement = direction * speed * Time.fixedDeltaTime;
        rb2D.MovePosition(rb2D.position + movement);
        
        // 向きを調整（2Dの場合）
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    /// <summary>
    /// 衝突検出（2D）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }
    
    /// <summary>
    /// 衝突検出（3D）
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }
    
    /// <summary>
    /// 衝突処理
    /// </summary>
    private void HandleCollision(GameObject hitObject)
    {
        // 自分自身や発射元との衝突は無視
        if (hitObject == gameObject) return;
        
        // ターゲットがある場合、ターゲット以外との衝突は無視
        if (hasTarget && target != null && hitObject.transform != target)
        {
            return;
        }
        
        // IDamageableを取得してダメージを与える
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(damage);
            
            // 衝突時に破棄する場合
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
        // ターゲットが設定されていない場合、Enemyタグを持つオブジェクトにダメージを与える
        else if (!hasTarget && hitObject.CompareTag("Enemy"))
        {
            damageable = hitObject.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead)
            {
                damageable.TakeDamage(damage);
                
                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    
    /// <summary>
    /// ダメージを設定
    /// </summary>
    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }
    
    /// <summary>
    /// 速度を設定
    /// </summary>
    public void SetSpeed(float projectileSpeed)
    {
        speed = projectileSpeed;
    }
}
