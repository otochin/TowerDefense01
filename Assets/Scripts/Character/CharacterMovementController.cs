using UnityEngine;

/// <summary>
/// キャラクター移動コントローラー
/// キャラクターの移動と基本動作を制御
/// </summary>
[RequireComponent(typeof(CharacterBase))]
public class CharacterMovementController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private bool moveRight = true; // 右方向へ移動（プレイヤーキャラクター）
    
    [Header("歩行エフェクト設定")]
    [SerializeField] private float walkBobbingAmount = 0.08f; // 上下の揺れ幅
    [SerializeField] private float walkBobbingSpeed = 5f; // 揺れの速度
    [SerializeField] private float walkWobbleAngle = 1.5f; // 傾き角度（度）
    [SerializeField] private bool enableWalkEffect = true; // 歩行エフェクトを有効にするか
    
    [Header("参照")]
    [SerializeField] private CharacterBase characterBase;
    
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
        // CharacterBaseを自動検出
        if (characterBase == null)
        {
            characterBase = GetComponent<CharacterBase>();
        }
        
        // 2D用コンポーネントを検出
        rb2D = GetComponent<Rigidbody2D>();
        
        // 3D用コンポーネントを検出
        charController3D = GetComponent<UnityEngine.CharacterController>();
    }
    
    private void Start()
    {
        // 移動方向を設定
        moveDirection = moveRight ? Vector3.right : Vector3.left;
        
        // 歩行エフェクト用の初期化
        basePosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        
        // デバッグ: 初期化状態を確認
        Debug.Log($"[CharacterMovementController] Start called on {gameObject.name}. MoveRight: {moveRight}, MoveDirection: {moveDirection}");
        
        if (characterBase != null)
        {
            Debug.Log($"[CharacterMovementController] CharacterBase found. MoveSpeed: {characterBase.MoveSpeed}");
        }
        else
        {
            Debug.LogWarning($"[CharacterMovementController] CharacterBase is null on {gameObject.name} in Start");
        }
        
        // 初期状態では移動を開始
        isMoving = true;
    }
    
    private void Update()
    {
        // キャラクターが死亡している場合は移動しない
        if (characterBase != null && characterBase.IsDead)
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
        // キャラクターが死亡している場合は移動しない
        if (characterBase != null && characterBase.IsDead)
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
                    Debug.Log($"[CharacterMovementController] Movement stopped on {gameObject.name}. Position: {transform.position}, Velocity: {rb2D.velocity}");
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
    
    private void OnEnable()
    {
        // デバッグ: コンポーネントの状態を確認
        if (characterBase == null)
        {
            Debug.LogWarning($"[CharacterMovementController] CharacterBase is null on {gameObject.name} in OnEnable");
        }
        
        if (rb2D == null)
        {
            Debug.LogWarning($"[CharacterMovementController] Rigidbody2D is null on {gameObject.name} in OnEnable");
        }
        else
        {
            Debug.Log($"[CharacterMovementController] Rigidbody2D found on {gameObject.name}. Body Type: {rb2D.bodyType}");
        }
    }
    
    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (characterBase == null)
        {
            Debug.LogWarning($"[CharacterMovementController] CharacterBase is null on {gameObject.name}");
            return;
        }
        
        float moveSpeed = characterBase.MoveSpeed;
        
        // デバッグ: MoveSpeedが0でないか確認（最初の数フレームのみ）
        if (Time.frameCount <= 5 && moveSpeed <= 0f)
        {
            Debug.LogWarning($"[CharacterMovementController] MoveSpeed is {moveSpeed} on {gameObject.name}. CharacterData may not be set.");
        }
        
        // FixedUpdateではTime.fixedDeltaTimeを使用
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        Vector3 movement = moveDirection * moveSpeed * deltaTime;
        
        // デバッグ: 最初の数フレームのみ詳細ログ
        if (Time.frameCount <= 5)
        {
            Debug.Log($"[CharacterMovementController] Move called on {gameObject.name}. MoveSpeed: {moveSpeed}, Movement: {movement}, MoveDirection: {moveDirection}");
        }
        
        // 2Dの場合
        if (rb2D != null)
        {
            // デバッグ: Rigidbody2DのBody Typeを確認（最初の数フレームのみ）
            if (Time.frameCount <= 5 && rb2D.bodyType != RigidbodyType2D.Kinematic)
            {
                Debug.LogWarning($"[CharacterMovementController] Rigidbody2D Body Type is {rb2D.bodyType} on {gameObject.name}. Should be Kinematic.");
            }
            
            // KinematicのRigidbody2Dの場合、MovePositionを使う（物理演算と正しく同期される）
            if (rb2D.bodyType == RigidbodyType2D.Kinematic)
            {
                Vector2 newPosition = rb2D.position + new Vector2(movement.x, movement.y);
                rb2D.MovePosition(newPosition);
                
                // デバッグ: 最初の数フレームのみ詳細を確認
                if (Time.frameCount <= 5)
                {
                    Debug.Log($"[CharacterMovementController] MovePosition to {newPosition} on {gameObject.name}");
                }
            }
            else
            {
                // Dynamicの場合はvelocityを使用
                rb2D.velocity = new Vector2(movement.x, movement.y);
                
                // デバッグ: 最初の数フレームのみvelocityを確認
                if (Time.frameCount <= 5)
                {
                    Debug.Log($"[CharacterMovementController] Set velocity to {rb2D.velocity} on {gameObject.name}");
                }
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
            Debug.LogWarning($"[CharacterMovementController] No Rigidbody2D or CharacterController found on {gameObject.name}. Using Transform movement.");
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
                Debug.Log($"[CharacterMovementController] StopMovement called on {gameObject.name}. Position: {transform.position}, Velocity: {rb2D.velocity}");
            }
        }
        else
        {
            if (wasMoving)
            {
                Debug.Log($"[CharacterMovementController] StopMovement called on {gameObject.name} (no Rigidbody2D). Position: {transform.position}");
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
    public void SetMoveDirection(bool moveRight)
    {
        this.moveRight = moveRight;
        moveDirection = moveRight ? Vector3.right : Vector3.left;
        
        // 2Dの場合、スプライトの向きを変更
        if (rb2D != null)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        // 3Dの場合、Y軸回転
        else
        {
            transform.rotation = Quaternion.Euler(0, moveRight ? 0 : 180, 0);
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
        if (!enableWalkEffect || !isMoving || (characterBase != null && characterBase.IsDead))
        {
            // 停止時または死亡時は元の状態に戻す
            bobbingTimer = 0f;
            transform.rotation = Quaternion.Euler(originalRotation);
            return;
        }
        
        if (characterBase == null) return;
        
        float moveSpeed = characterBase.MoveSpeed;
        
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
