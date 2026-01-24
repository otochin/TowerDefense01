using UnityEngine;

/// <summary>
/// 敵移動コントローラー
/// 敵の移動と基本動作を制御（左方向へ移動）
/// </summary>
[RequireComponent(typeof(EnemyBase))]
public class EnemyController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private bool moveLeft = true; // 左方向へ移動（敵）
    
    [Header("歩行エフェクト設定")]
    [SerializeField] private float walkBobbingAmount = 0.08f; // 上下の揺れ幅
    [SerializeField] private float walkBobbingSpeed = 5f; // 揺れの速度
    [SerializeField] private bool enableWalkEffect = true; // 歩行エフェクトを有効にするか
    
    [Header("参照")]
    [SerializeField] private EnemyBase enemyBase;
    
    // 2D用コンポーネント
    private Rigidbody2D rb2D;
    
    // 3D用コンポーネント
    private UnityEngine.CharacterController charController3D;
    
    // 移動状態
    private bool isMoving = false;
    private Vector3 moveDirection;
    
    // 歩行エフェクト用
    private Vector3 basePosition;
    private Vector3 originalRotation;
    private float bobbingTimer = 0f;
    
    private void Awake()
    {
        // EnemyBaseを自動検出
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }
        
        // 2D用コンポーネントを検出
        rb2D = GetComponent<Rigidbody2D>();
        
        // 3D用コンポーネントを検出
        charController3D = GetComponent<UnityEngine.CharacterController>();
    }
    
    private void Start()
    {
        // 移動方向を設定（左方向）
        moveDirection = moveLeft ? Vector3.left : Vector3.right;
        isMoving = true; // 初期状態では移動を開始
        
        // 歩行エフェクト用の初期化
        basePosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }
    
    private void Update()
    {
        // 敵が死亡している場合は移動しない
        if (enemyBase != null && enemyBase.IsDead)
        {
            StopMovement();
            return;
        }
    }
    
    private void LateUpdate()
    {
        // 歩行エフェクトを適用（物理演算の後に実行）
        // 注意: transform.positionを直接変更すると物理演算と競合するため、rb2D.positionから読み取って適用
        ApplyWalkEffect();
    }
    
    private void FixedUpdate()
    {
        // 敵が死亡している場合は移動しない
        if (enemyBase != null && enemyBase.IsDead)
        {
            StopMovement();
            return;
        }
        
        // 移動中の場合のみ移動処理（物理演算と同期）
        if (isMoving)
        {
            Move();
        }
        else
        {
            // 移動停止時にvelocityを確実に0にする
            if (rb2D != null)
            {
                // Kinematicの場合、MovePositionを使っていないので、velocityを0にするだけで良い
                // しかし、念のため現在の位置を保持するためにMovePositionを現在位置で呼ぶこともできる
                rb2D.velocity = Vector2.zero;
                
                // デバッグ: 移動停止中であることを定期的にログ出力
                if (Time.frameCount % 60 == 0) // 1秒ごと
                {
                    // Debug.Log($"[EnemyController] Movement stopped on {gameObject.name}. Position: {transform.position}, Velocity: {rb2D.velocity}");
                }
            }
        }
        
        // 移動処理後にbasePositionを更新（rb2D.positionから取得、Y座標は歩行エフェクトで制御）
        if (rb2D != null && isMoving)
        {
            // 実際に移動した位置からbasePositionのX座標を更新
            // Y座標は歩行エフェクトの基準として保持（歩行エフェクトが無効な場合はrb2D.position.yを使用）
            if (!enableWalkEffect)
            {
                basePosition = new Vector3(rb2D.position.x, rb2D.position.y, 0f);
            }
            else
            {
                basePosition = new Vector3(rb2D.position.x, basePosition.y, 0f);
            }
        }
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (enemyBase == null)
        {
            return;
        }
        
        float moveSpeed = enemyBase.MoveSpeed;
        
        // FixedUpdateではTime.fixedDeltaTimeを使用
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        Vector3 movement = moveDirection * moveSpeed * deltaTime;
        
        // 2Dの場合
        if (rb2D != null)
        {
            // KinematicのRigidbody2Dの場合、MovePositionを使う（物理演算と正しく同期される）
            if (rb2D.bodyType == RigidbodyType2D.Kinematic)
            {
                Vector2 newPosition = rb2D.position + new Vector2(movement.x, movement.y);
                rb2D.MovePosition(newPosition);
            }
            else
            {
                // Dynamicの場合はvelocityを使用
                rb2D.velocity = new Vector2(movement.x, movement.y);
            }
        }
        // 3Dの場合
        else if (charController3D != null)
        {
            charController3D.Move(movement);
        }
        // Transform直接移動（フォールバック）
        else
        {
            Debug.LogWarning($"[EnemyController] No Rigidbody2D or CharacterController found on {gameObject.name}. Using Transform movement.");
            transform.position += movement;
        }
    }
    
    /// <summary>
    /// 移動を停止
    /// </summary>
    public void StopMovement()
    {
        bool wasMoving = isMoving;
        isMoving = false;
        
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
            if (wasMoving)
            {
                // Debug.Log($"[EnemyController] StopMovement called on {gameObject.name}. Position: {transform.position}, Velocity: {rb2D.velocity}");
            }
        }
        else
        {
            if (wasMoving)
            {
                // Debug.Log($"[EnemyController] StopMovement called on {gameObject.name} (no Rigidbody2D). Position: {transform.position}");
            }
        }
    }
    
    /// <summary>
    /// 移動を再開
    /// </summary>
    public void ResumeMovement()
    {
        isMoving = true;
    }
    
    /// <summary>
    /// 移動方向を設定
    /// </summary>
    public void SetMoveDirection(bool moveLeft)
    {
        this.moveLeft = moveLeft;
        moveDirection = moveLeft ? Vector3.left : Vector3.right;
        
        // 2Dの場合、スプライトの向きを変更
        if (rb2D != null)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        // 3Dの場合、Y軸回転
        else
        {
            transform.rotation = Quaternion.Euler(0, moveLeft ? 180 : 0, 0);
        }
    }
    
    /// <summary>
    /// 移動中かどうか
    /// </summary>
    public bool IsMoving => isMoving;
    
    /// <summary>
    /// 移動方向を取得
    /// </summary>
    public Vector3 MoveDirection => moveDirection;
    
    /// <summary>
    /// 歩行エフェクトを適用（上下の揺れと軽い傾き）
    /// </summary>
    private void ApplyWalkEffect()
    {
        if (!enableWalkEffect || !isMoving || (enemyBase != null && enemyBase.IsDead))
        {
            // 停止時または死亡時は元の状態に戻す
            bobbingTimer = 0f;
            transform.rotation = Quaternion.Euler(originalRotation);
            return;
        }
        
        if (enemyBase == null) return;
        
        float moveSpeed = enemyBase.MoveSpeed;
        
        // タイマーを更新（移動速度に応じて変化させる）
        bobbingTimer += walkBobbingSpeed * moveSpeed * Time.deltaTime;
        
        // 上下の揺れ（Y軸） - rb2D.positionに適用
        float bobbing = Mathf.Sin(bobbingTimer) * walkBobbingAmount;
        
        // rb2D.positionから現在の位置を取得してY座標に歩行エフェクトを適用
        if (rb2D != null)
        {
            Vector2 pos = rb2D.position;
            // 物理演算の位置（X座標）は保持し、Y座標に歩行エフェクトを加える
            rb2D.position = new Vector2(pos.x, basePosition.y + bobbing);
        }
        
        // 回転は元の状態を保持（傾きエフェクトなし）
        transform.rotation = Quaternion.Euler(originalRotation);
    }
}
