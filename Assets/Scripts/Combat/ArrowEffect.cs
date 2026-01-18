using UnityEngine;
using System.Collections;

/// <summary>
/// 矢エフェクト
/// Archerから敵に向かって飛ぶ矢のビジュアルエフェクト
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ArrowEffect : MonoBehaviour
{
    [Header("エフェクト設定")]
    [SerializeField] private float moveSpeed = 20f; // 移動速度（矢はファイヤーボールより速い）
    [SerializeField] private float lifetime = 2f; // 生存時間（秒）
    [SerializeField] private Vector3 scale = Vector3.one; // エフェクトのサイズ（デフォルトは1,1,1）
    [SerializeField] private int sortingOrder = 100; // 描画順序（大きい値ほど前面に表示）
    
    [Header("参照")]
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float spawnTime;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // SortingOrderを設定して前面に表示
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
    
    private void Start()
    {
        spawnTime = Time.time;
        startPosition = transform.position;
    }
    
    private void Update()
    {
        // 生存時間を超えたら破棄
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
            return;
        }
        
        // ターゲット位置に向かって移動
        MoveToTarget();
        
        // ターゲット位置に到達したかチェック
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// エフェクトを初期化
    /// </summary>
    /// <param name="from">開始位置（Archerの位置）</param>
    /// <param name="to">目標位置（敵の位置）</param>
    /// <param name="arrowSprite">矢のスプライト</param>
    /// <param name="effectScale">エフェクトのサイズ（オプション）</param>
    public void Initialize(Vector3 from, Vector3 to, Sprite arrowSprite = null, Vector3? effectScale = null)
    {
        transform.position = from;
        targetPosition = to;
        startPosition = from;
        
        // スプライトが指定されている場合は設定
        if (arrowSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = arrowSprite;
        }
        
        // SortingOrderを設定して前面に表示（初期化時にも確実に設定）
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
        
        // サイズを設定（パラメータが指定されている場合はそれを使用、否則はInspectorの値を使用）
        if (effectScale.HasValue)
        {
            transform.localScale = effectScale.Value;
        }
        else
        {
            transform.localScale = scale;
        }
        
        // ターゲット方向を向く
        Vector3 direction = (to - from).normalized;
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    /// <summary>
    /// ターゲット位置に向かって移動
    /// </summary>
    private void MoveToTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        
        // ターゲット位置を超えないようにする
        float remainingDistance = Vector3.Distance(transform.position, targetPosition);
        if (movement.magnitude > remainingDistance)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position += movement;
        }
    }
}
