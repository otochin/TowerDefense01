using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// 魔法使い（魔法攻撃型キャラクター）
/// 魔法弾を放って敵を攻撃する
/// </summary>
[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(CharacterMovementController))]
public class Mage : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] private CharacterMovementController characterController;
    
    [Header("攻撃設定")]
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private IDamageable currentTarget;
    
    [Header("投射物設定")]
    [SerializeField] private GameObject magicProjectilePrefab; // 魔法弾のプレハブ（後で実装）
    [SerializeField] private Transform shootPoint; // 魔法弾の発射位置
    
    [Header("ファイヤーボールエフェクト設定")]
    [SerializeField] private GameObject fireballEffectPrefab; // ファイヤーボールエフェクトのプレハブ
    [SerializeField] private Sprite fireballSprite; // ファイヤーボールのスプライト（エフェクトプレハブが設定されていない場合に使用）
    [SerializeField] private Vector3 fireballScale = new Vector3(0.5f, 0.5f, 1f); // ファイヤーボールのサイズ（デフォルトは0.5倍）
    
    [Header("攻撃アニメーション設定")]
    [SerializeField] private float attackLungeDistance = 0.15f; // 攻撃時の突進距離
    [SerializeField] private float attackLungeDuration = 0.08f; // 突進の時間（秒）
    
    [Header("効果音設定")]
    [SerializeField] private AudioClip attackSound; // 攻撃時の効果音
    [SerializeField] private AudioSource audioSource; // 効果音再生用のAudioSource
    
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
        
        // ShootPointが設定されていない場合、このGameObjectの位置を使用
        if (shootPoint == null)
        {
            shootPoint = transform;
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
                    Debug.Log($"[Mage] No enemy found. Resuming movement.");
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
            Debug.LogWarning("[Mage] CharacterBase is null!");
            return null;
        }
        
        float attackRange = characterBase.AttackRange;
        
        // 攻撃範囲が0以下の場合は何もしない
        if (attackRange <= 0f)
        {
            Debug.LogWarning($"[Mage] AttackRange is {attackRange}. Please set a valid attack range in CharacterData asset.");
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
    /// 攻撃を実行（魔法弾を放つ）
    /// </summary>
    private void Attack(IDamageable target)
    {
        if (target == null || characterBase == null) return;
        
        // ターゲットの位置を取得
        Vector3 targetPosition = GetTargetPosition(target);
        
        // 投射物が設定されている場合は投射物を生成
        if (magicProjectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(magicProjectilePrefab, shootPoint.position, Quaternion.identity);
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                // ターゲットのTransformを取得
                MonoBehaviour targetMono = target as MonoBehaviour;
                if (targetMono != null && targetMono.transform != null)
                {
                    projectileComponent.Initialize(targetMono.transform, characterBase.AttackPower, 10f);
                }
                else
                {
                    // Transformが取得できない場合は位置を指定
                    projectileComponent.Initialize(targetPosition, characterBase.AttackPower, 10f);
                }
            }
        }
        else
        {
            // 投射物がない場合は直接ダメージを与える（暫定）
            int damage = characterBase.AttackPower;
            target.TakeDamage(damage);
            Debug.Log($"{characterBase.CharacterData?.CharacterName} cast magic for {damage} damage!");
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
        
        // Transformが取得できない場合はColliderから位置を取得
        Collider2D targetCollider = Physics2D.OverlapCircleAll(transform.position, characterBase.AttackRange)
            .FirstOrDefault(col => col.GetComponent<IDamageable>() == target);
        if (targetCollider != null)
        {
            return targetCollider.transform.position;
        }
        
        // フォールバック：Mageの位置から攻撃範囲の端を計算
        return transform.position + Vector3.right * characterBase.AttackRange;
    }
    
    /// <summary>
    /// ファイヤーボールエフェクトを生成
    /// </summary>
    private void SpawnFireballEffect(Vector3 targetPosition)
    {
        Vector3 startPosition = shootPoint != null ? shootPoint.position : transform.position;
        
        // エフェクトプレハブが設定されている場合はプレハブから生成
        if (fireballEffectPrefab != null)
        {
            GameObject effect = Instantiate(fireballEffectPrefab, startPosition, Quaternion.identity);
            FireballEffect fireballComponent = effect.GetComponent<FireballEffect>();
            if (fireballComponent != null)
            {
                fireballComponent.Initialize(startPosition, targetPosition, null, fireballScale);
            }
        }
        // エフェクトプレハブが設定されていない場合は、FireballEffectコンポーネントを持つGameObjectを作成
        else if (fireballSprite != null)
        {
            GameObject effectObj = new GameObject("FireballEffect");
            effectObj.transform.position = startPosition;
            
            FireballEffect fireballComponent = effectObj.AddComponent<FireballEffect>();
            fireballComponent.Initialize(startPosition, targetPosition, fireballSprite, fireballScale);
        }
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
}
