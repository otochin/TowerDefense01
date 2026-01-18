using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// 敵マジシャン（魔法攻撃型敵）
/// 遠距離から魔法弾を放ってプレイヤーキャラクターを攻撃する
/// </summary>
[RequireComponent(typeof(EnemyBase))]
[RequireComponent(typeof(EnemyController))]
public class Enemy_Mage : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private EnemyController enemyController;
    
    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private IDamageable currentTarget;
    
    [Header("投射物設定")]
    [SerializeField] private GameObject magicProjectilePrefab; // 魔法弾のプレハブ
    [SerializeField] private Transform shootPoint; // 魔法弾の発射位置
    
    [Header("ファイヤーボールエフェクト設定")]
    [SerializeField] private GameObject fireballEffectPrefab; // ファイヤーボールエフェクトのプレハブ
    [SerializeField] private Sprite fireballSprite; // ファイヤーボールのスプライト
    [SerializeField] private Vector3 fireballScale = new Vector3(0.5f, 0.5f, 1f); // ファイヤーボールのサイズ
    
    [Header("攻撃アニメーション設定")]
    [SerializeField] private float attackLungeDistance = 0.15f; // 攻撃時の突進距離
    [SerializeField] private float attackLungeDuration = 0.08f; // 突進の時間（秒）
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip attackSound; // 攻撃時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
    private void Awake()
    {
        // コンポーネントを自動検出
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }
        
        if (enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }
        
        // ShootPointが設定されていない場合、このGameObjectの位置を使用
        if (shootPoint == null)
        {
            shootPoint = transform;
        }
        
        // AudioSourceを自動検出または作成
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
        }
    }
    
    private void Update()
    {
        if (enemyBase == null || enemyBase.IsDead) return;
        
        // 攻撃クールダウンを更新
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // プレイヤーキャラクターを探して攻撃
        CheckForTargetsAndAttack();
    }
    
    /// <summary>
    /// ターゲットを探して攻撃する
    /// </summary>
    private void CheckForTargetsAndAttack()
    {
        // 以前のターゲット状態を保存
        bool hadTarget = currentTarget != null;
        
        // 攻撃範囲内のプレイヤーキャラクターを探す
        IDamageable target = FindNearestTarget();
        
        if (target != null)
        {
            // ターゲットが見つかったら移動を停止
            if (enemyController != null)
            {
                enemyController.StopMovement();
            }
            
            // 攻撃可能なら攻撃
            if (attackCooldown <= 0f)
            {
                Attack(target);
                attackCooldown = enemyBase.AttackSpeed;
            }
        }
        else
        {
            // ターゲットが見つからない場合、以前ターゲットがいた場合は移動を再開
            if (hadTarget)
            {
                if (enemyController != null)
                {
                    enemyController.ResumeMovement();
                }
            }
        }
    }
    
    /// <summary>
    /// 最も近いターゲットを探す（プレイヤーキャラクターまたはプレイヤー城）
    /// </summary>
    private IDamageable FindNearestTarget()
    {
        if (enemyBase == null) return null;
        
        float attackRange = enemyBase.AttackRange;
        
        if (attackRange <= 0f)
        {
            return null;
        }
        
        // 現在のターゲットが死んでいる場合はnullに設定
        if (currentTarget != null && currentTarget.IsDead)
        {
            currentTarget = null;
        }
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Collider2D selfCollider = GetComponent<Collider2D>();
        
        IDamageable nearestTarget = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D col in colliders)
        {
            if (col == selfCollider) continue;
            
            // Playerタグを持つオブジェクト（プレイヤーキャラクター）を探す
            if (col.CompareTag("Player"))
            {
                IDamageable target = col.GetComponent<IDamageable>();
                if (target != null && !target.IsDead)
                {
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance <= attackRange)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestTarget = target;
                        }
                    }
                }
            }
            // PlayerCastleタグを持つオブジェクト（プレイヤー城）を探す
            else if (col.CompareTag("PlayerCastle"))
            {
                IDamageable target = col.GetComponent<IDamageable>();
                if (target != null && !target.IsDead)
                {
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance <= attackRange)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestTarget = target;
                        }
                    }
                }
            }
        }
        
        currentTarget = nearestTarget;
        return nearestTarget;
    }
    
    /// <summary>
    /// 攻撃を実行（魔法弾を放つ）
    /// </summary>
    private void Attack(IDamageable target)
    {
        if (target == null || enemyBase == null) return;
        
        // ターゲットの位置を取得
        Vector3 targetPosition = GetTargetPosition(target);
        
        // 投射物が設定されている場合は投射物を生成
        if (magicProjectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(magicProjectilePrefab, shootPoint.position, Quaternion.identity);
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                MonoBehaviour targetMono = target as MonoBehaviour;
                if (targetMono != null && targetMono.transform != null)
                {
                    projectileComponent.Initialize(targetMono.transform, enemyBase.AttackPower, 10f);
                }
                else
                {
                    projectileComponent.Initialize(targetPosition, enemyBase.AttackPower, 10f);
                }
            }
        }
        else
        {
            // 投射物がない場合は直接ダメージを与える（暫定）
            int damage = enemyBase.AttackPower;
            target.TakeDamage(damage);
        }
        
        // ファイヤーボールエフェクトを表示
        SpawnFireballEffect(targetPosition);
        
        // 攻撃時の効果音を再生
        PlayAttackSound();
        
        // 攻撃時の突進アニメーション
        StartCoroutine(AttackLunge(target));
    }
    
    /// <summary>
    /// ターゲットの位置を取得
    /// </summary>
    private Vector3 GetTargetPosition(IDamageable target)
    {
        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono != null && targetMono.transform != null)
        {
            return targetMono.transform.position;
        }
        
        Collider2D targetCollider = Physics2D.OverlapCircleAll(transform.position, enemyBase.AttackRange)
            .FirstOrDefault(col => col.GetComponent<IDamageable>() == target);
        if (targetCollider != null)
        {
            return targetCollider.transform.position;
        }
        
        return transform.position + Vector3.left * enemyBase.AttackRange;
    }
    
    /// <summary>
    /// ファイヤーボールエフェクトを生成
    /// </summary>
    private void SpawnFireballEffect(Vector3 targetPosition)
    {
        Vector3 startPosition = shootPoint != null ? shootPoint.position : transform.position;
        
        if (fireballEffectPrefab != null)
        {
            GameObject effect = Instantiate(fireballEffectPrefab, startPosition, Quaternion.identity);
            FireballEffect fireballComponent = effect.GetComponent<FireballEffect>();
            if (fireballComponent != null)
            {
                fireballComponent.Initialize(startPosition, targetPosition, null, fireballScale);
            }
        }
        else if (fireballSprite != null)
        {
            GameObject effectObj = new GameObject("FireballEffect");
            effectObj.transform.position = startPosition;
            
            FireballEffect fireballComponent = effectObj.AddComponent<FireballEffect>();
            fireballComponent.Initialize(startPosition, targetPosition, fireballSprite, fireballScale);
        }
    }
    
    /// <summary>
    /// 攻撃時の突進アニメーション
    /// </summary>
    private IEnumerator AttackLunge(IDamageable target)
    {
        if (enemyController == null) yield break;
        
        Vector3 originalPosition = transform.position;
        Vector3 lungeDirection = enemyController.MoveDirection;
        
        if (target != null)
        {
            MonoBehaviour targetMono = target as MonoBehaviour;
            if (targetMono != null && targetMono.transform != null)
            {
                Vector3 toTarget = (targetMono.transform.position - transform.position).normalized;
                lungeDirection = new Vector3(toTarget.x, 0, 0).normalized;
            }
        }
        
        Vector3 lungePosition = originalPosition + lungeDirection * attackLungeDistance;
        
        float elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f) yield break;
            
            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f) yield break;
            
            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(lungePosition, originalPosition, t);
            yield return null;
        }
        
        transform.position = originalPosition;
    }
    
    /// <summary>
    /// 攻撃時の効果音を再生する
    /// </summary>
    private void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
}
