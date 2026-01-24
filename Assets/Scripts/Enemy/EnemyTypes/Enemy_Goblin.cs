using UnityEngine;
using System.Collections;

/// <summary>
/// ゴブリン（弱い敵）
/// 近距離でプレイヤーキャラクターを攻撃する（オークより弱い）
/// </summary>
[RequireComponent(typeof(EnemyBase))]
[RequireComponent(typeof(EnemyController))]
public class Enemy_Goblin : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private EnemyController enemyController;
    
    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private IDamageable currentTarget;
    
    [Header("攻撃アニメーション設定")]
    [SerializeField] private float attackLungeDistance = 0.2f; // 攻撃時の突進距離
    [SerializeField] private float attackLungeDuration = 0.1f; // 突進の時間（秒）
    
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
                // 移動を再開
                if (enemyController != null)
                {
                    enemyController.ResumeMovement();
                    // Debug.Log($"[Enemy_Goblin] No target found. Resuming movement.");
                }
            }
        }
    }
    
    /// <summary>
    /// 最も近いターゲットを探す（プレイヤーキャラクターまたはプレイヤー城）
    /// </summary>
    private IDamageable FindNearestTarget()
    {
        float attackRange = enemyBase.AttackRange;
        
        // 攻撃範囲が0以下の場合は何もしない
        if (attackRange <= 0f)
        {
            Debug.LogWarning($"[Enemy_Goblin] AttackRange is {attackRange}. Please set a valid attack range in EnemyData asset.");
            return null;
        }
        
        // 現在のターゲットが死んでいる場合はnullに設定
        if (currentTarget != null && currentTarget.IsDead)
        {
            currentTarget = null;
        }
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        // 自分自身のColliderを除外
        Collider2D selfCollider = GetComponent<Collider2D>();
        
        IDamageable nearestTarget = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D col in colliders)
        {
            // 自分自身のColliderをスキップ
            if (col == selfCollider) continue;
            
            // Playerタグを持つオブジェクト（プレイヤーキャラクター）を探す
            if (col.CompareTag("Player"))
            {
                IDamageable target = col.GetComponent<IDamageable>();
                if (target != null && !target.IsDead)
                {
                    // 実際の距離を計算
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    
                    // 攻撃範囲内かどうかを再確認（Colliderのサイズを考慮）
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
                    // 実際の距離を計算
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    
                    // 攻撃範囲内かどうかを再確認（Colliderのサイズを考慮）
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
    /// 攻撃を実行
    /// </summary>
    private void Attack(IDamageable target)
    {
        if (target == null || enemyBase == null) return;
        
        int damage = enemyBase.AttackPower;
        target.TakeDamage(damage);
        
        // 攻撃時の効果音を再生
        PlayAttackSound();
        
        // 攻撃時の突進アニメーション
        StartCoroutine(AttackLunge(target));
        
        // Debug.Log($"[Enemy_Goblin] {enemyBase.EnemyData?.EnemyName} attacked {target} for {damage} damage!");
    }
    
    /// <summary>
    /// 攻撃時の突進アニメーション（移動方向に少し前進して戻る）
    /// </summary>
    private IEnumerator AttackLunge(IDamageable target)
    {
        if (enemyController == null) yield break;
        
        Vector3 originalPosition = transform.position;
        Vector3 lungeDirection = enemyController.MoveDirection;
        
        // ターゲット方向を使用する場合（より自然）
        if (target != null)
        {
            MonoBehaviour targetMono = target as MonoBehaviour;
            if (targetMono != null && targetMono.transform != null)
            {
                Vector3 toTarget = (targetMono.transform.position - transform.position).normalized;
                // X軸方向のみ使用（2D横スクロールなので）
                lungeDirection = new Vector3(toTarget.x, 0, 0).normalized;
            }
        }
        
        Vector3 lungePosition = originalPosition + lungeDirection * attackLungeDistance;
        
        // 突進（前進）
        float elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f) yield break; // ゲーム停止時は中断
            
            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
            yield return null;
        }
        
        // 元の位置に戻る
        elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f) yield break; // ゲーム停止時は中断
            
            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(lungePosition, originalPosition, t);
            yield return null;
        }
        
        // 確実に元の位置に戻す
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
    
    /// <summary>
    /// 攻撃範囲を可視化（デバッグ用）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (enemyBase == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyBase.AttackRange);
    }
}
