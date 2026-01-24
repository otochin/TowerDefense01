using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 間違えた問題リスト表示UI
/// ゲーム終了時に間違えた問題を回数順で表示する
/// </summary>
public class IncorrectAnswersListUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject listPanel; // リスト全体のパネル
    [SerializeField] private TextMeshProUGUI titleText; // タイトルテキスト（例：「間違えた問題リスト」）
    [SerializeField] private ScrollRect scrollRect; // スクロール可能なコンテナ
    [SerializeField] private Transform contentParent; // リストアイテムの親オブジェクト
    [SerializeField] private GameObject listItemPrefab; // リストアイテムのプレハブ
    
    [Header("設定")]
    [SerializeField] private string titleFormat = "間違えた問題リスト ({0}件)"; // タイトルフォーマット
    [SerializeField] private bool hideIfEmpty = true; // 間違いがない場合はパネルを非表示にする
    
    private WordLearningSystem wordLearningSystem;
    
    private void Awake()
    {
        // WordLearningSystemを自動検出
        wordLearningSystem = FindObjectOfType<WordLearningSystem>();
        if (wordLearningSystem == null)
        {
            Debug.LogWarning("[IncorrectAnswersListUI] WordLearningSystemが見つかりません。");
        }
        
        // 初期状態では非表示
        if (listPanel != null)
        {
            listPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 間違いリストを表示（ゲーム終了時に呼び出す）
    /// </summary>
    public void ShowIncorrectAnswersList()
    {
        if (wordLearningSystem == null)
        {
            wordLearningSystem = FindObjectOfType<WordLearningSystem>();
            if (wordLearningSystem == null)
            {
                Debug.LogWarning("[IncorrectAnswersListUI] WordLearningSystemが見つかりません。リストを表示できません。");
                return;
            }
        }
        
        // listPanelが設定されていない場合は自分自身を参照
        if (listPanel == null)
        {
            listPanel = gameObject;
            // Debug.Log($"[IncorrectAnswersListUI] ListPanel was null. Using this GameObject: {gameObject.name}");
        }
        
        // ScrollRectが設定されていない場合は自動検出を試みる
        if (scrollRect == null && listPanel != null)
        {
            scrollRect = listPanel.GetComponentInChildren<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogWarning("[IncorrectAnswersListUI] ScrollRectが見つかりません。スクロール位置をリセットできません。");
            }
            else
            {
                Debug.Log($"[IncorrectAnswersListUI] ScrollRect auto-detected: {scrollRect.name}");
            }
        }
        
        // ContentParentが設定されていない場合は自動検出を試みる
        if (contentParent == null)
        {
            // まずScrollRectのContentを確認
            if (scrollRect != null && scrollRect.content != null)
            {
                contentParent = scrollRect.content;
            }
            else
            {
                // ScrollRectのContentが設定されていない場合、子要素からContentを探す
                if (scrollRect != null)
                {
                    // ScrollViewの子要素からViewportを探し、その中からContentを探す
                    Transform viewport = scrollRect.transform.Find("Viewport");
                    if (viewport != null)
                    {
                        Transform content = viewport.Find("Content");
                        if (content != null)
                        {
                            contentParent = content;
                            // ScrollRectのContentも設定する
                            scrollRect.content = content as RectTransform;
                            // Debug.Log("[IncorrectAnswersListUI] ContentParentを自動検出しました。");
                        }
                    }
                }
                
                if (contentParent == null)
                {
                    Debug.LogWarning("[IncorrectAnswersListUI] ContentParentが見つかりません。リストアイテムを生成できません。");
                }
            }
        }
        
        // 間違えた問題を取得（ソート済み）
        List<(WordData wordData, int count)> incorrectAnswers = wordLearningSystem.GetIncorrectAnswersSorted();
        
        // 間違いがない場合は非表示にする
        if (incorrectAnswers.Count == 0)
        {
            if (hideIfEmpty && listPanel != null)
            {
                listPanel.SetActive(false);
            }
            return;
        }
        
        // パネルを表示
        if (listPanel != null)
        {
            // Canvasまでの親オブジェクトをすべてアクティブにする
            Transform current = listPanel.transform;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                    // Debug.Log($"[IncorrectAnswersListUI] Activated parent: {current.name}");
                }
                current = current.parent;
            }
            
            // パネルをアクティブ化
            listPanel.SetActive(true);
            
            // アクティブ化が成功したか確認
            if (!listPanel.activeSelf)
            {
                Debug.LogError($"[IncorrectAnswersListUI] Failed to activate ListPanel! This should not happen.");
            }
            else if (!listPanel.activeInHierarchy)
            {
                // 親オブジェクトを再確認
                Transform parent = listPanel.transform.parent;
                Debug.LogWarning($"[IncorrectAnswersListUI] ListPanel is active but not in hierarchy. Parent: {parent?.name ?? "null"}, Parent active: {parent?.gameObject.activeSelf ?? false}");
                
                // 親を強制的にアクティブ化
                if (parent != null && !parent.gameObject.activeSelf)
                {
                    parent.gameObject.SetActive(true);
                    // Debug.Log($"[IncorrectAnswersListUI] Force-activated parent: {parent.name}");
                }
            }
            
            // Debug.Log($"[IncorrectAnswersListUI] ListPanel activated. Active: {listPanel.activeSelf}, ActiveInHierarchy: {listPanel.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[IncorrectAnswersListUI] ListPanel is null! Cannot display the list.");
            return;
        }
        
        // TitleTextが設定されていない場合は自動検出を試みる
        if (titleText == null && listPanel != null)
        {
            // まず「TitleText」という名前の子要素を探す
            Transform titleTransform = listPanel.transform.Find("TitleText");
            if (titleTransform != null)
            {
                titleText = titleTransform.GetComponent<TextMeshProUGUI>();
            }
            
            // 見つからない場合は、すべての子要素からTextMeshProUGUIを探す
            if (titleText == null)
            {
                titleText = listPanel.GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (titleText != null)
            {
                // Debug.Log($"[IncorrectAnswersListUI] TitleText auto-detected: {titleText.name}");
            }
        }
        
        // タイトルを更新
        if (titleText != null)
        {
            titleText.text = string.Format(titleFormat, incorrectAnswers.Count);
            // Debug.Log($"[IncorrectAnswersListUI] Title text updated: {titleText.text}");
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswersListUI] TitleText is null. Title will not be displayed.");
        }
        
        // 既存のリストアイテムを削除
        ClearListItems();
        
        // リストアイテムを生成
        if (contentParent != null && listItemPrefab != null)
        {
            // Debug.Log($"[IncorrectAnswersListUI] Generating {incorrectAnswers.Count} list items. ContentParent: {contentParent.name}, ListItemPrefab: {listItemPrefab.name}");
            
            int itemCount = 0;
            foreach (var (wordData, count) in incorrectAnswers)
            {
                GameObject itemObj = Instantiate(listItemPrefab, contentParent);
                itemObj.SetActive(true); // 念のため明示的にアクティブ化
                itemCount++;
                
                // プレハブ構造の問題をチェック（Canvasが子要素にある場合）
                Canvas childCanvas = itemObj.GetComponentInChildren<Canvas>();
                if (childCanvas != null && childCanvas.transform.parent == itemObj.transform)
                {
                    Debug.LogWarning($"[IncorrectAnswersListUI] Warning: ListItem prefab has a Canvas as a child. This may cause layout issues. Canvas should be removed from the prefab. See docs/14_IncorrectAnswerListItem_Prefab_Fix.md for details.");
                }
                
                // LayoutElementを追加または取得してPreferred Heightを設定
                LayoutElement layoutElement = itemObj.GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = itemObj.AddComponent<LayoutElement>();
                }
                
                // RectTransformからHeightを取得してPreferred Heightに設定
                RectTransform itemRect = itemObj.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    float preferredHeight = itemRect.sizeDelta.y;
                    if (preferredHeight <= 0)
                    {
                        // sizeDeltaが0以下の場合、デフォルトの高さを設定
                        preferredHeight = 60f; // デフォルトの高さ
                        itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, preferredHeight);
                    }
                    layoutElement.preferredHeight = preferredHeight;
                    
                    // リストアイテムのWidthをContentの幅に合わせる（Contentが設定されている場合）
                    if (contentParent != null)
                    {
                        RectTransform parentRect = contentParent as RectTransform;
                        if (parentRect != null && parentRect.sizeDelta.x > 0)
                        {
                            // ContentのWidthからパディングを引いた幅を設定
                            float itemWidth = parentRect.sizeDelta.x - 10f; // 左右のパディング5pxずつ
                            if (itemWidth > 0)
                            {
                                itemRect.sizeDelta = new Vector2(itemWidth, itemRect.sizeDelta.y);
                                layoutElement.preferredWidth = itemWidth;
                                // Debug.Log($"[IncorrectAnswersListUI] Set item {itemCount} width to {itemWidth} (content width: {parentRect.sizeDelta.x})");
                            }
                        }
                    }
                    
                    // Debug.Log($"[IncorrectAnswersListUI] Set LayoutElement preferredHeight: {preferredHeight} for item {itemCount}");
                }
                
                // IncorrectAnswerListItemUIコンポーネントを使用してデータを設定
                IncorrectAnswerListItemUI listItemUI = itemObj.GetComponent<IncorrectAnswerListItemUI>();
                
                if (listItemUI != null)
                {
                    // テキスト参照を確認
                    TextMeshProUGUI[] allTexts = itemObj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    // Debug.Log($"[IncorrectAnswersListUI] List item {itemCount}: Found {allTexts.Length} TextMeshProUGUI components in children");
                    
                    // 各テキスト要素にContentSizeFitterを追加してHeightを自動計算
                    foreach (TextMeshProUGUI text in allTexts)
                    {
                        RectTransform textRect = text.GetComponent<RectTransform>();
                        if (textRect != null)
                        {
                            // ContentSizeFitterを追加または取得
                            ContentSizeFitter sizeFitter = text.GetComponent<ContentSizeFitter>();
                            if (sizeFitter == null)
                            {
                                sizeFitter = text.gameObject.AddComponent<ContentSizeFitter>();
                            }
                            
                            // Vertical FitをPreferred Sizeに設定（Heightを自動計算）
                            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                            
                            // Horizontal FitはUnconstrainedのまま（Widthは親のレイアウトグループが制御）
                            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                            
                            // プレハブで設定された幅を尊重するため、AnchorやsizeDeltaは変更しない
                            // HeightのみContentSizeFitterで自動計算する
                            
                            // CountTextの場合は右寄せ、それ以外は左寄せ（Alignmentのみ設定）
                            bool isCountText = text.name.Contains("Count") || text.name == "CountText";
                            
                            // TextMeshProUGUIのAlignmentを設定
                            if (isCountText)
                            {
                                text.alignment = TMPro.TextAlignmentOptions.Right;
                            }
                            else
                            {
                                text.alignment = TMPro.TextAlignmentOptions.Left;
                            }
                            
                            // テキストが折り返されないように設定（幅が足りない場合の対策）
                            text.enableWordWrapping = false; // 折り返しを無効化
                            text.overflowMode = TMPro.TextOverflowModes.Overflow; // はみ出しても表示
                            
                            // プレハブで設定された幅をログに出力（確認用）
                            // Debug.Log($"[IncorrectAnswersListUI]   Text element '{text.name}': Width={textRect.sizeDelta.x}, Height={textRect.sizeDelta.y}, Anchor=({textRect.anchorMin.x}, {textRect.anchorMin.y}) to ({textRect.anchorMax.x}, {textRect.anchorMax.y})");
                            
                            // Debug.Log($"[IncorrectAnswersListUI]   Added ContentSizeFitter to {text.name} (isCountText: {isCountText}). Before: Size({textRect.sizeDelta.x}, {textRect.sizeDelta.y})");
                        }
                    }
                    
                    // テキストを設定してからレイアウトを再計算
                    listItemUI.SetData(wordData.English, wordData.Japanese, count);
                    
                    // レイアウトを強制的に再計算
                    foreach (TextMeshProUGUI text in allTexts)
                    {
                        RectTransform textRect = text.GetComponent<RectTransform>();
                        if (textRect != null)
                        {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);
                            // Debug.Log($"[IncorrectAnswersListUI]   After layout rebuild: {text.name} Size({textRect.sizeDelta.x}, {textRect.sizeDelta.y})");
                        }
                    }
                    
                    // Debug.Log($"[IncorrectAnswersListUI] List item {itemCount} created: {wordData.English} / {wordData.Japanese} ×{count}回");
                }
                else
                {
                    // フォールバック: コンポーネントがない場合は従来の方法で設定
                    TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
                    
                    if (texts.Length >= 2)
                    {
                        // 最初のテキスト: 英語/英熟語
                        texts[0].text = wordData.English;
                        // 2番目のテキスト: 日本語
                        texts[1].text = wordData.Japanese;
                        // 3番目のテキスト（ある場合）: 間違えた回数
                        if (texts.Length >= 3)
                        {
                            texts[2].text = $"×{count}回";
                        }
                    }
                    else if (texts.Length == 1)
                    {
                        // 1つのテキストにすべてを含める
                        texts[0].text = $"{wordData.English} / {wordData.Japanese} ×{count}回";
                    }
                    
                    Debug.LogWarning($"[IncorrectAnswersListUI] ListItem Prefab does not have IncorrectAnswerListItemUI component. Item {itemCount} created with fallback method.");
                }
            }
            
            // Debug.Log($"[IncorrectAnswersListUI] Successfully created {itemCount} list items. ContentParent child count: {contentParent.childCount}");
            
            // ContentのRectTransformを取得してアンカーを上部に設定
            RectTransform contentRect = contentParent as RectTransform;
            if (contentRect != null)
            {
                // Viewportのサイズを取得してContentのAnchorとWidthを設定
                RectTransform viewportRect = null;
                if (scrollRect != null)
                {
                    // まず、ScrollRectのviewportプロパティを確認
                    if (scrollRect.viewport != null)
                    {
                        viewportRect = scrollRect.viewport;
                    }
                    else
                    {
                        // Viewportが設定されていない場合は、ScrollRectの子要素から"Viewport"を探す
                        Transform viewportTransform = scrollRect.transform.Find("Viewport");
                        if (viewportTransform != null)
                        {
                            viewportRect = viewportTransform.GetComponent<RectTransform>();
                        }
                    }
                    
                    // Viewportが見つからない場合は、ScrollRect自体のサイズを使用
                    if (viewportRect == null)
                    {
                        viewportRect = scrollRect.GetComponent<RectTransform>();
                        Debug.LogWarning("[IncorrectAnswersListUI] Viewport not found. Using ScrollRect's RectTransform instead.");
                    }
                    
                    if (viewportRect != null)
                    {
                        // ContentのAnchorをTop Stretch（上部で左右に伸びる）に設定
                        contentRect.anchorMin = new Vector2(0f, 1f); // 左上
                        contentRect.anchorMax = new Vector2(1f, 1f); // 右上
                        contentRect.pivot = new Vector2(0.5f, 1f); // ピボットは上部中央
                        
                        // 左右のパディングを設定（Widthは自動的にStretchされる）
                        // offsetMin/offsetMaxは、anchorがStretchの場合、親のRectTransformからのオフセットを指定
                        // パディングを小さくして、幅を確保
                        contentRect.offsetMin = new Vector2(5f, 0f); // Left: 5px, Bottom: 0px（下方向への伸びはContentSizeFitterが制御）
                        contentRect.offsetMax = new Vector2(-5f, 0f); // Right: -5px, Top: 0px（上方向は固定）
                        
                        // HeightはContentSizeFitterが自動計算するので、sizeDeltaのYは0のまま
                        contentRect.sizeDelta = new Vector2(0f, 0f);
                        
                        // Contentの幅を確認してログに出力
                        float contentWidth = viewportRect.rect.width - 10f; // 左右のパディング5pxずつ
                        // Debug.Log($"[IncorrectAnswersListUI] Content anchor set to Top Stretch (viewport width: {viewportRect.rect.width}, content width: {contentWidth}, padding: 5px each side)");
                    }
                    else
                    {
                        // Viewportが見つからない場合のフォールバック
                        contentRect.anchorMin = new Vector2(0.5f, 1f);
                        contentRect.anchorMax = new Vector2(0.5f, 1f);
                        contentRect.pivot = new Vector2(0.5f, 1f);
                        contentRect.anchoredPosition = Vector2.zero;
                        Debug.LogWarning("[IncorrectAnswersListUI] Viewport not found. Using default anchor settings.");
                    }
                }
                else
                {
                    // ScrollRectがない場合のフォールバック
                    contentRect.anchorMin = new Vector2(0.5f, 1f);
                    contentRect.anchorMax = new Vector2(0.5f, 1f);
                    contentRect.pivot = new Vector2(0.5f, 1f);
                    contentRect.anchoredPosition = Vector2.zero;
                    Debug.LogWarning("[IncorrectAnswersListUI] ScrollRect is null. Using default anchor settings.");
                }
                
                // Debug.Log($"[IncorrectAnswersListUI] Content anchor: ({contentRect.anchorMin.x}, {contentRect.anchorMin.y}) to ({contentRect.anchorMax.x}, {contentRect.anchorMax.y}), Position: {contentRect.anchoredPosition}, Size: {contentRect.sizeDelta}");
            }
            
            // Content Size Fitterを強制的に更新（レイアウトを再計算）
            ContentSizeFitter contentSizeFitter = contentParent.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter != null)
            {
                // レイアウトを強制的に再計算
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
                
                // レイアウト再計算後、Contentの位置を上部にリセット
                if (contentRect != null)
                {
                    // AnchorがTop Stretchの場合、offsetMaxのYを0に設定することで上部に固定
                    // offsetMinのYは負の値（ContentのHeight分）になるので、そのままにする
                    contentRect.offsetMax = new Vector2(contentRect.offsetMax.x, 0f);
                    // anchoredPositionを0に設定して、Viewportの上部に配置
                    contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0f);
                    // Debug.Log($"[IncorrectAnswersListUI] ContentSizeFitter found. Layout rebuilt. Content size: {contentRect.sizeDelta}, Position: {contentRect.anchoredPosition}, OffsetMin: {contentRect.offsetMin}, OffsetMax: {contentRect.offsetMax}");
                }
            }
            else
            {
                Debug.LogWarning("[IncorrectAnswersListUI] ContentSizeFitter not found on ContentParent! Content size may not update correctly.");
            }
            
            // ScrollRectのContentを設定（まだ設定されていない場合）
            if (scrollRect != null && scrollRect.content == null && contentRect != null)
            {
                scrollRect.content = contentRect;
                // Debug.Log($"[IncorrectAnswersListUI] ScrollRect.content set to ContentParent.");
            }
            
            // ScrollRectの初期位置を上部に設定（リストが上から表示されるように）
            if (scrollRect != null && contentRect != null)
            {
                // レイアウト再計算後に位置をリセット
                Canvas.ForceUpdateCanvases();
                // 少し遅延を入れてから位置をリセット（レイアウト計算が完了するまで待つ）
                StartCoroutine(ResetScrollPosition());
            }
        }
        else
        {
            if (contentParent == null)
            {
                Debug.LogError("[IncorrectAnswersListUI] ContentParent is null! Cannot generate list items.");
            }
            if (listItemPrefab == null)
            {
                Debug.LogError("[IncorrectAnswersListUI] ListItemPrefab is null! Cannot generate list items.");
            }
        }
        
        // スクロール位置をリセット（一番上に）
        // ScrollRectのContentが設定されている場合のみスクロール位置を設定
        if (scrollRect != null && scrollRect.content != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
        else if (scrollRect != null && scrollRect.content == null)
        {
            Debug.LogWarning("[IncorrectAnswersListUI] ScrollRectのContentが設定されていません。スクロール位置をリセットできません。");
        }
        
        // Debug.Log($"[IncorrectAnswersListUI] Showed {incorrectAnswers.Count} incorrect answers.");
        
        // デバッグ: ヒエラルキーの状態を確認
        // DebugHierarchy();
    }
    
    /// <summary>
    /// スクロール位置をリセットするコルーチン
    /// </summary>
    private IEnumerator ResetScrollPosition()
    {
        // 1フレーム待機してレイアウト計算が完了するのを待つ
        yield return null;
        
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f; // 上部（1.0 = 上、0.0 = 下）
            // Debug.Log($"[IncorrectAnswersListUI] ScrollRect position reset to top (verticalNormalizedPosition: 1.0)");
        }
    }
    
    /// <summary>
    /// デバッグ用: ヒエラルキーの状態をログに出力
    /// </summary>
    private void DebugHierarchy()
    {
        if (contentParent != null)
        {
            // Debug.Log($"[IncorrectAnswersListUI] ContentParent: {contentParent.name}, Active: {contentParent.gameObject.activeSelf}, ActiveInHierarchy: {contentParent.gameObject.activeInHierarchy}, ChildCount: {contentParent.childCount}");
            
            RectTransform contentRect = contentParent as RectTransform;
            if (contentRect != null)
            {
                // Debug.Log($"[IncorrectAnswersListUI] ContentParent RectTransform - Position: {contentRect.anchoredPosition}, Size: {contentRect.sizeDelta}, AnchoredMin: {contentRect.anchorMin}, AnchoredMax: {contentRect.anchorMax}");
            }
            
            for (int i = 0; i < contentParent.childCount; i++)
            {
                Transform child = contentParent.GetChild(i);
                RectTransform rect = child as RectTransform;
                bool isActive = child.gameObject.activeSelf;
                bool isActiveInHierarchy = child.gameObject.activeInHierarchy;
                
                // Debug.Log($"[IncorrectAnswersListUI] Child {i}: {child.name}, Active: {isActive}, ActiveInHierarchy: {isActiveInHierarchy}");
                
                if (rect != null)
                {
                    // rect.rectが取得できない場合があるので、sizeDeltaを使用
                    // Debug.Log($"[IncorrectAnswersListUI]   - Position: {rect.anchoredPosition}, SizeDelta: {rect.sizeDelta}, Width: {rect.sizeDelta.x}, Height: {rect.sizeDelta.y}");
                }
                else
                {
                    Debug.LogWarning($"[IncorrectAnswersListUI]   - RectTransform is null for child {i}");
                }
                
                // 子要素のテキストも確認
                TextMeshProUGUI[] texts = child.GetComponentsInChildren<TextMeshProUGUI>();
                // Debug.Log($"[IncorrectAnswersListUI]   - TextMeshProUGUI count: {texts.Length}");
                foreach (var text in texts)
                {
                    // Debug.Log($"[IncorrectAnswersListUI]     - Text: {text.name}, Active: {text.gameObject.activeSelf}, Text: \"{text.text}\", FontSize: {text.fontSize}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[IncorrectAnswersListUI] ContentParent is null in DebugHierarchy!");
        }
        
        // ScrollViewの状態も確認
        if (scrollRect != null)
        {
            // ScrollRectのContentが設定されていない場合、エラーを避けるためにnullチェック
            if (scrollRect.content != null)
            {
                // Debug.Log($"[IncorrectAnswersListUI] ScrollRect: {scrollRect.name}, Active: {scrollRect.gameObject.activeSelf}, Content: {scrollRect.content.name}");
            }
            else
            {
                Debug.LogWarning($"[IncorrectAnswersListUI] ScrollRect: {scrollRect.name}, Active: {scrollRect.gameObject.activeSelf}, Content: null (not assigned)");
            }
            
            // Viewportのサイズと位置を確認
            if (scrollRect.viewport != null)
            {
                RectTransform viewportRect = scrollRect.viewport;
                Debug.Log($"[IncorrectAnswersListUI] Viewport - Position: {viewportRect.anchoredPosition}, Size: {viewportRect.rect.size}, Width: {viewportRect.rect.width}, Height: {viewportRect.rect.height}, Active: {viewportRect.gameObject.activeSelf}");
            }
            else
            {
                Debug.LogWarning("[IncorrectAnswersListUI] Viewport is null!");
            }
        }
    }
    
    /// <summary>
    /// リストアイテムをクリア
    /// </summary>
    private void ClearListItems()
    {
        if (contentParent != null)
        {
            for (int i = contentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(contentParent.GetChild(i).gameObject);
            }
        }
    }
    
    /// <summary>
    /// パネルを非表示にする
    /// </summary>
    public void HidePanel()
    {
        if (listPanel != null)
        {
            listPanel.SetActive(false);
        }
    }
}
