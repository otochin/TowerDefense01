using UnityEngine;
using System.Collections;

/// <summary>
/// 戦士（近接戦闘型キャラクター）
/// 近距離で敵を攻撃する
/// </summary>
[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(CharacterMovementController))]
public class Warrior : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] private CharacterMovementController characterController;
    
    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private IDamageable currentTarget;
    
    [Header("攻撃アニメーション設定")]
    [SerializeField] private float attackLungeDistance = 0.2f; // 攻撃時の突進距離
    [SerializeField] private float attackLungeDuration = 0.1f; // 突進の時間（秒）
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip attackSound; // 攻撃時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource

    [Header("スプライト設定")]
    [SerializeField] private SpriteRenderer spriteRenderer; // スプライトレンダラーの参照
    [SerializeField] private Animator animator; // アニメーターの参照
    private Sprite originalSprite; // 元のスプライトを保存
    [SerializeField] private Sprite battleSprite; // 攻撃時のスプライト（Knight-battle - 2）
    
    private void Awake()
    {
        // コンポーネントを自動検出
        if (characterBase == null)
        {
            characterBase = GetComponent<CharacterBase>();
        }
        
        if (characterController == null)
        {
            characterController = GetComponent<CharacterMovementController>();
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

        // SpriteRendererを自動検出
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Animatorを自動検出
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // 元のスプライトを保存
        if (spriteRenderer != null && originalSprite == null)
        {
            originalSprite = spriteRenderer.sprite;
        }

        // 攻撃時のスプライトが設定されていない場合は警告を表示
        if (battleSprite == null)
        {
            Debug.LogWarning("[Warrior] Battle sprite is not set. Please assign 'Knight-battle - 2' sprite in the Inspector.");
        }
    }
    
    private void Update()
    {
        if (characterBase == null || characterBase.IsDead) return;
        
        // 攻撃クールダウンを更新
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // 敵を探して攻撃
        CheckForEnemiesAndAttack();
    }
    
    /// <summary>
    /// 敵を探して攻撃する
    /// </summary>
    private void CheckForEnemiesAndAttack()
    {
        // 以前のターゲット状態を保存
        bool hadTarget = currentTarget != null;
        
        // 攻撃範囲内の敵を探す
        IDamageable target = FindNearestEnemy();
        
        if (target != null)
        {
            // 敵が見つかったら移動を停止
            if (characterController != null)
            {
                characterController.StopMovement();
            }
            
            // 攻撃可能なら攻撃
            if (attackCooldown <= 0f)
            {
                Attack(target);
                attackCooldown = characterBase.AttackSpeed;
            }
        }
        else
        {
            // 敵が見つからない場合、以前ターゲットがいた場合は移動を再開
            if (hadTarget)
            {
                // 移動を再開
                if (characterController != null)
                {
                    characterController.ResumeMovement();
                }
            }
        }
    }
    
    /// <summary>
    /// 最も近い敵を探す（敵キャラクターまたは敵の城）
    /// </summary>
    private IDamageable FindNearestEnemy()
    {
        if (characterBase == null)
        {
            Debug.LogWarning("[Warrior] CharacterBase is null!");
            return null;
        }
        
        float attackRange = characterBase.AttackRange;
        
        // 攻撃範囲が0以下の場合は何もしない
        if (attackRange <= 0f)
        {
            Debug.LogWarning($"[Warrior] AttackRange is {attackRange}. Please set a valid attack range in CharacterData asset.");
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
        
        IDamageable nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D col in colliders)
        {
            // 自分自身のColliderをスキップ
            if (col == selfCollider) continue;
            
            // Enemyタグを持つオブジェクト（敵キャラクター）を探す
            if (col.CompareTag("Enemy"))
            {
                IDamageable enemy = col.GetComponent<IDamageable>();
                if (enemy != null && !enemy.IsDead)
                {
                    // 実際の距離を計算
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    
                    // 攻撃範囲内かどうかを再確認（Colliderのサイズを考慮）
                    if (distance <= attackRange)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                }
            }
            // EnemyCastleタグを持つオブジェクト（敵の城）を探す
            else if (col.CompareTag("EnemyCastle"))
            {
                IDamageable enemyCastle = col.GetComponent<IDamageable>();
                if (enemyCastle != null && !enemyCastle.IsDead)
                {
                    // 実際の距離を計算
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    
                    // 攻撃範囲内かどうかを再確認（Colliderのサイズを考慮）
                    if (distance <= attackRange)
                    {
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestEnemy = enemyCastle;
                        }
                    }
                }
            }
        }
        
        currentTarget = nearestEnemy;
        return nearestEnemy;
    }
    
    /// <summary>
    /// 攻撃を実行
    /// </summary>
    private void Attack(IDamageable target)
    {
        if (target == null || characterBase == null) return;
        
        int damage = characterBase.AttackPower;
        target.TakeDamage(damage);
        
        // 攻撃時の効果音を再生
        PlayAttackSound();
        
        // 攻撃時の突進アニメーション
        StartCoroutine(AttackLunge(target));
        
        // Debug.Log($"[Warrior] {characterBase.CharacterData?.CharacterName} attacked {target} for {damage} damage!");
    }
    
    /// <summary>
    /// 攻撃時の突進アニメーション（移動方向に少し前進して戻る）
    /// </summary>
    private IEnumerator AttackLunge(IDamageable target)
    {
        if (characterController == null) yield break;
        
        Vector3 originalPosition = transform.position;
        Vector3 lungeDirection = characterController.MoveDirection;
        
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

        // 攻撃時のスプライトに変更
        if (spriteRenderer != null && battleSprite != null)
        {
            // アニメーションを一時的に無効化
            if (animator != null)
            {
                animator.enabled = false;
            }
            spriteRenderer.sprite = battleSprite;
        }

        // 突進（前進）
        float elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f)
            {
                // ゲーム停止時はスプライトを元に戻して中断
                if (spriteRenderer != null && originalSprite != null)
                {
                    spriteRenderer.sprite = originalSprite;
                    if (animator != null)
                    {
                        animator.enabled = true;
                    }
                }
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(originalPosition, lungePosition, t);
            yield return null;
        }
        
        // 元の位置に戻る
        elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            if (Time.timeScale == 0f)
            {
                // ゲーム停止時はスプライトを元に戻して中断
                if (spriteRenderer != null && originalSprite != null)
                {
                    spriteRenderer.sprite = originalSprite;
                    if (animator != null)
                    {
                        animator.enabled = true;
                    }
                }
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector3.Lerp(lungePosition, originalPosition, t);
            yield return null;
        }
        
        // 確実に元の位置に戻す
        transform.position = originalPosition;

        // 元のスプライトに戻す
        if (spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
            // アニメーションを再び有効化
            if (animator != null)
            {
                animator.enabled = true;
            }
        }
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
        if (characterBase == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, characterBase.AttackRange);
    }
}
