# 間違えた問題リスト デバッグガイド

## 問題: パネルは表示されるが、リストアイテムが表示されない（真っ白）

## デバッグ手順

### ステップ1: Unity Editorでヒエラルキーを確認

1. **ゲームを実行中に一時停止**（Pauseボタンをクリック）
2. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel`を探す
3. **展開**して以下の構造を確認：

```
IncorrectAnswersListPanel
├── TitleText
└── ScrollView
    ├── Sliding Area
    │   └── Handle
    └── Viewport
        └── Content
            ├── IncorrectAnswerListItem (Clone) ← これが表示されているか確認
            ├── IncorrectAnswerListItem (Clone)
            └── IncorrectAnswerListItem (Clone)
```

### ステップ2: リストアイテムの存在確認

**Hierarchyで確認すること**：
- [ ] `Content`の子要素に`IncorrectAnswerListItem (Clone)`が存在するか
- [ ] 各`IncorrectAnswerListItem (Clone)`が**アクティブ**（チェックマークが入っている）か
- [ ] `Content`自体が**アクティブ**か
- [ ] `Viewport`が**アクティブ**か
- [ ] `ScrollView`が**アクティブ**か

### ステップ3: RectTransformの確認

**各オブジェクトを選択してInspectorで確認**：

#### Content
- **Rect Transform**の設定：
  - **Width**: 適切な値（例: 400以上）
  - **Height**: リストアイテムの数×アイテムの高さ（例: 3件×60 = 180以上）
  - **Anchor Presets**: `Top Left`または`Stretch Top`
  - **Pos X**: `0`付近
  - **Pos Y**: `0`付近（上端）

#### IncorrectAnswerListItem (Clone)
- **Rect Transform**の設定：
  - **Width**: 適切な値（例: 400）
  - **Height**: 適切な値（例: 60）
  - **Pos X**: `0`付近
  - **Pos Y**: 負の値（上から下に並ぶ）

### ステップ4: Layout Groupの確認

**Content**に**Vertical Layout Group**が設定されているか確認：

1. `Content`を選択
2. **Inspector**で**Vertical Layout Group**コンポーネントを確認
3. 設定を確認：
   - **Child Alignment**: `Upper Left`
   - **Child Control Size**:
     - ❌ **Width**のチェックを外す
     - ✅ **Height**をチェック
   - **Child Force Expand**:
     - ❌ **Width**のチェックを外す
     - ❌ **Height**のチェックを外す
   - **Spacing**: `5`以上
   - **Padding**: `Left: 5, Right: 5, Top: 5, Bottom: 5`

### ステップ5: Content Size Fitterの確認

**Content**に**Content Size Fitter**が設定されているか確認：

1. `Content`を選択
2. **Inspector**で**Content Size Fitter**コンポーネントを確認
3. 設定を確認：
   - **Horizontal Fit**: `Preferred Size`または`Unconstrained`
   - **Vertical Fit**: `Preferred Size`（重要！）

### ステップ6: ScrollRectの確認

**ScrollView**の**Scroll Rect**コンポーネントを確認：

1. `ScrollView`を選択
2. **Inspector**で**Scroll Rect**コンポーネントを確認
3. 設定を確認：
   - **Content**: `Content`（`Viewport/Content`）が設定されているか
   - **Viewport**: `Viewport`が設定されているか
   - **Vertical**: ✅ チェックが入っているか
   - **Horizontal**: ❌ チェックが外れているか

### ステップ7: リストアイテムの子要素確認

**IncorrectAnswerListItem (Clone)**の子要素を確認：

1. `IncorrectAnswerListItem (Clone)`を展開
2. 以下の構造を確認：

```
IncorrectAnswerListItem (Clone)
└── Content
    ├── EnglishText (TextMeshProUGUI)
    ├── JapaneseText (TextMeshProUGUI)
    └── CountText (TextMeshProUGUI)
```

3. 各テキスト要素が**アクティブ**か確認
4. 各テキスト要素に**TextMeshProUGUI**コンポーネントがアタッチされているか確認

### ステップ8: テキストの内容確認

**各テキスト要素**を選択して**Inspector**で確認：

- **Text**: 実際のテキストが設定されているか（空でないか）
- **Font Asset**: フォントアセットが設定されているか
- **Font Size**: 適切なサイズ（例: 16以上）が設定されているか
- **Color**: 白色（または見える色）が設定されているか

### ステップ9: Canvasの確認

**Canvas**の設定を確認：

1. `Canvas`を選択
2. **Inspector**で**Canvas**コンポーネントを確認
3. **Render Mode**: `Screen Space - Overlay`または`Screen Space - Camera`
4. **Sort Order**: 他のUIより高い値（例: 10以上）

### ステップ10: デバッグログの確認

**Consoleウィンドウ**で以下のログを確認：

- `[IncorrectAnswersListUI] Generating X list items...`
- `[IncorrectAnswersListUI] List item X created: ...`
- `[IncorrectAnswersListUI] Successfully created X list items. ContentParent child count: X`

**エラーや警告がないか確認**：
- `ContentParent is null!`
- `ListItemPrefab is null!`
- `TitleText is null!`

## よくある問題と解決方法

### 問題1: リストアイテムが生成されていない

**症状**: `Content`の子要素に`IncorrectAnswerListItem (Clone)`が存在しない

**原因**:
- `ListItemPrefab`が設定されていない
- `ContentParent`が設定されていない

**解決方法**:
1. `IncorrectAnswersListUI`コンポーネントの**ListItem Prefab**フィールドにプレハブを設定
2. **Content Parent**フィールドに`ScrollView/Viewport/Content`を設定

---

### 問題2: リストアイテムが非アクティブ

**症状**: `IncorrectAnswerListItem (Clone)`が存在するが、チェックマークが外れている

**原因**:
- プレハブが非アクティブで保存されている
- 生成時にアクティブ化されていない

**解決方法**:
1. プレハブ（`IncorrectAnswerListItem`）を開く
2. ルートオブジェクトが**アクティブ**になっているか確認
3. コードで`itemObj.SetActive(true)`が呼ばれているか確認（既に実装済み）

---

### 問題3: ContentのHeightが0

**症状**: リストアイテムは存在するが、`Content`の**Height**が0

**原因**:
- **Content Size Fitter**の**Vertical Fit**が`Unconstrained`になっている
- **Vertical Layout Group**が正しく設定されていない

**解決方法**:
1. `Content`の**Content Size Fitter**の**Vertical Fit**を`Preferred Size`に設定
2. **Vertical Layout Group**の設定を確認（上記ステップ4を参照）

---

### 問題4: リストアイテムの位置が画面外

**症状**: リストアイテムは存在するが、画面外に配置されている

**原因**:
- **Rect Transform**の**Pos Y**が正しく設定されていない
- **Anchor Presets**が正しく設定されていない

**解決方法**:
1. `Content`の**Anchor Presets**を`Top Left`に設定
2. **Pos Y**を`0`に設定
3. **Vertical Layout Group**の**Child Alignment**を`Upper Left`に設定

---

### 問題5: テキストが表示されない

**症状**: リストアイテムは表示されるが、テキストが表示されない

**原因**:
- テキスト要素が非アクティブ
- フォントアセットが設定されていない
- テキストの色が背景と同じ（白色で背景も白など）

**解決方法**:
1. 各テキスト要素が**アクティブ**か確認
2. **Font Asset**に適切なフォントアセットを設定
3. **Color**を確認（例: 黒色 `#000000`）

---

### 問題6: ScrollViewが表示されない

**症状**: パネルは表示されるが、ScrollViewの領域が表示されない

**原因**:
- `ScrollView`が非アクティブ
- `Viewport`が非アクティブ
- **Rect Transform**のサイズが0

**解決方法**:
1. `ScrollView`と`Viewport`が**アクティブ**か確認
2. **Rect Transform**の**Width**と**Height**を確認
3. **Anchor Presets**を`Stretch Stretch`に設定

---

## デバッグ用スクリプト（オプション）

以下のスクリプトを`IncorrectAnswersListUI`に追加すると、詳細な情報がログに出力されます：

```csharp
private void DebugHierarchy()
{
    if (contentParent != null)
    {
        Debug.Log($"[IncorrectAnswersListUI] ContentParent: {contentParent.name}, Active: {contentParent.gameObject.activeSelf}, ChildCount: {contentParent.childCount}");
        
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform child = contentParent.GetChild(i);
            RectTransform rect = child as RectTransform;
            Debug.Log($"[IncorrectAnswersListUI] Child {i}: {child.name}, Active: {child.gameObject.activeSelf}, Position: {rect?.anchoredPosition}, Size: {rect?.sizeDelta}");
        }
    }
}
```

このメソッドを`ShowIncorrectAnswersList()`の最後に呼び出すと、詳細な情報が出力されます。

---

## 確認チェックリスト

- [ ] `Content`の子要素に`IncorrectAnswerListItem (Clone)`が存在する
- [ ] 各リストアイテムが**アクティブ**
- [ ] `Content`に**Vertical Layout Group**が設定されている
- [ ] `Content`に**Content Size Fitter**が設定されている（**Vertical Fit**: `Preferred Size`）
- [ ] `Content`の**Height**が0より大きい
- [ ] 各リストアイテムの**Rect Transform**が正しく設定されている
- [ ] 各テキスト要素が**アクティブ**で、テキストが設定されている
- [ ] `ScrollView`の**Scroll Rect**の**Content**が正しく設定されている

---

これで問題の原因を特定できるはずです！
