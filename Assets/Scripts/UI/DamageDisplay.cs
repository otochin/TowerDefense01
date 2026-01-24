using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// ダメージ表示コンポーネント
/// ダメージ値を表示し、アニメーション（上に移動、フェードアウト）を実行
/// </summary>
public class DamageDisplay : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI damageText;
    
    [Header("アニメーション設定")]
    [SerializeField] private float moveDistance = 1.0f; // 移動距離
    [SerializeField] private float fadeOutDuration = 0.5f; // フェードアウト時間
    [SerializeField] private float displayDuration = 1.0f; // 表示時間（秒）
    
    private Vector3 startPosition;
    private Color originalColor;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;
    
    private void Awake()
    {
        Debug.Log($"[DamageDisplay] Awake called on {gameObject.name}");
        
        // CanvasとRectTransformを取得
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = GetComponentInParent<RectTransform>();
        }
        
        // DamageTextを自動検出
        if (damageText == null)
        {
            damageText = GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log($"[DamageDisplay] DamageText auto-detected: {(damageText != null ? damageText.name : "null")}");
        }
        
        if (damageText == null)
        {
            Debug.LogError($"[DamageDisplay] DamageText not found on {gameObject.name} or its children. Please add a TextMeshProUGUI component.");
        }
        
        // CanvasGroupを取得または追加（フェードアウト用）
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log($"[DamageDisplay] CanvasGroup added to {gameObject.name}");
        }
        
        // 元の色を保存
        if (damageText != null)
        {
            originalColor = damageText.color;
        }
        
        // 開始位置を保存（World Space Canvasの場合はRectTransformの位置を使用）
        if (rectTransform != null && canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            startPosition = rectTransform.position;
        }
        else
        {
            startPosition = transform.position;
        }
        // Debug.Log($"[DamageDisplay] Start position: {startPosition}, Canvas mode: {(canvas != null ? canvas.renderMode.ToString() : "null")}");
    }
    
    /// <summary>
    /// ダメージ値を設定してアニメーションを開始
    /// </summary>
    public void ShowDamage(int damage)
    {
        // Debug.Log($"[DamageDisplay] ShowDamage called: damage={damage}");
        
        if (damageText != null)
        {
            damageText.text = damage.ToString();
            // Debug.Log($"[DamageDisplay] Damage text set to: {damageText.text}");
        }
        else
        {
            Debug.LogError($"[DamageDisplay] DamageText is null! Cannot display damage.");
            return;
        }
        
        // アニメーションを開始
        StartCoroutine(AnimateDamage());
        // Debug.Log($"[DamageDisplay] Animation coroutine started");
    }
    
    /// <summary>
    /// ダメージアニメーション（上に移動、フェードアウト）
    /// </summary>
    private IEnumerator AnimateDamage()
    {
        // Debug.Log($"[DamageDisplay] AnimateDamage started. Start position: {startPosition}, Move distance: {moveDistance}");
        
        float elapsedTime = 0f;
        Vector3 endPosition = startPosition + Vector3.up * moveDistance;
        
        // フェードアウト開始時間
        float fadeStartTime = displayDuration - fadeOutDuration;
        
        while (elapsedTime < displayDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / displayDuration;
            
            // 上に移動（World Space Canvasの場合はRectTransformを使用）
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            if (rectTransform != null && canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                rectTransform.position = currentPosition;
            }
            else
            {
                transform.position = currentPosition;
            }
            
            // フェードアウト
            if (elapsedTime >= fadeStartTime)
            {
                float fadeT = (elapsedTime - fadeStartTime) / fadeOutDuration;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
                }
                else if (damageText != null)
                {
                    Color color = damageText.color;
                    color.a = Mathf.Lerp(originalColor.a, 0f, fadeT);
                    damageText.color = color;
                }
            }
            
            yield return null;
        }
        
        Debug.Log($"[DamageDisplay] Animation completed, destroying {gameObject.name}");
        // アニメーション終了後、破棄
        Destroy(gameObject);
    }
}
