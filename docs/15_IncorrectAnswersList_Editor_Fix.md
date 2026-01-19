# 間違えた問題リスト - Unity Editor修正ガイド

## 問題の症状

- リストアイテムが上にはみ出している（タイトルバーに隠れている）
- テキストの位置が崩れている

## Unity Editorでの修正手順

### ステップ1: ContentのAnchor設定を修正

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport > Content`を選択

2. **Inspectorウィンドウ**で**Rect Transform**コンポーネントを確認

3. **Anchor Presets**を設定:
   - **Anchor Presets**のボタンを**左クリック長押し**
   - **Top Stretch**を選択（上部で左右に伸びる）
     - または、手動で設定:
       - **Min**: `X: 0, Y: 1`（左上）
       - **Max**: `X: 1, Y: 1`（右上）

4. **Pivot**を設定:
   - **Pivot**: `X: 0.5, Y: 1`（上部中央）

5. **Position**を確認:
   - **Pos X**: `0`
   - **Pos Y**: `0`（Viewportの上部に配置）

6. **Size**を確認:
   - **Width**: `0`（Stretchなので自動的にViewportの幅に合わせる）
   - **Height**: `0`（ContentSizeFitterが自動計算）

7. **Offset**を設定:
   - **Left**: `10`（左パディング）
   - **Right**: `-10`（右パディング）
   - **Top**: `0`（上部はViewportの上部に合わせる）
   - **Bottom**: `0`（下部はContentSizeFitterが制御）

### ステップ2: Viewportの設定を確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport`を選択

2. **Inspectorウィンドウ**で**Rect Transform**コンポーネントを確認

3. **Anchor Presets**を確認:
   - **Anchor Presets**: `Stretch Stretch`（親いっぱいに広がる）
   - **Min**: `X: 0, Y: 0`（左下）
   - **Max**: `X: 1, Y: 1`（右上）

4. **Pivot**を確認:
   - **Pivot**: `X: 0.5, Y: 0.5`（中央）

5. **Offset**を確認:
   - すべて`0`になっていることを確認

### ステップ3: ScrollViewの設定を確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView`を選択

2. **Inspectorウィンドウ**で**Scroll Rect**コンポーネントを確認

3. **Content**フィールドを確認:
   - `Content`（`Viewport`の子）が設定されていることを確認
   - 設定されていない場合は、`Viewport > Content`をドラッグ＆ドロップ

4. **Viewport**フィールドを確認:
   - `Viewport`が設定されていることを確認
   - 設定されていない場合は、`Viewport`をドラッグ＆ドロップ

5. **Vertical**が✅チェックされていることを確認

6. **Horizontal**は❌チェックを外す（横スクロールは不要）

### ステップ4: ContentのVertical Layout Groupを確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport > Content`を選択

2. **Inspectorウィンドウ**で**Vertical Layout Group**コンポーネントを確認

3. **Child Alignment**を確認:
   - **Child Alignment**: `Upper Left`（上部左揃え）

4. **Child Control Size**を確認:
   - ❌ **Width**のチェックを外す
   - ✅ **Height**をチェック

5. **Child Force Expand**を確認:
   - ❌ **Width**のチェックを外す
   - ❌ **Height**のチェックを外す

6. **Spacing**を確認:
   - **Spacing**: `5`（アイテム間のスペース）

7. **Padding**を確認:
   - **Left**: `5`
   - **Right**: `5`
   - **Top**: `5`
   - **Bottom**: `5`

### ステップ5: Content Size Fitterを確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport > Content`を選択

2. **Inspectorウィンドウ**で**Content Size Fitter**コンポーネントを確認

3. **Horizontal Fit**を確認:
   - **Horizontal Fit**: `Unconstrained`（幅はStretchで制御）

4. **Vertical Fit**を確認:
   - **Vertical Fit**: `Preferred Size`（高さは子要素の合計で自動計算）

### ステップ6: ScrollViewのRect Transformを確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView`を選択

2. **Inspectorウィンドウ**で**Rect Transform**コンポーネントを確認

3. **Anchor Presets**を確認:
   - **Anchor Presets**: `Stretch Stretch`（親いっぱいに広がる）

4. **Offset**を確認:
   - **Left**: `10`（左パディング）
   - **Right**: `-10`（右パディング）
   - **Top**: `-50`（タイトルの下、`TitleText`の高さに合わせる）
   - **Bottom**: `10`（下パディング）

### ステップ7: リストアイテムプレハブの確認

1. **Projectウィンドウ**で`Assets/Prefabs/UI/IncorrectAnswerListItem.prefab`を選択

2. **Inspectorウィンドウ**でプレハブを確認

3. **Content**の**Vertical Layout Group**を確認:
   - **Child Alignment**: `Upper Left`
   - **Child Control Size**: ❌ Width, ✅ Height
   - **Child Force Expand**: ❌ Width, ❌ Height

4. 各テキスト要素（`EnglishText`、`JapaneseText`、`CountText`）の**Rect Transform**を確認:
   - **Anchor Presets**: `Stretch Left`または`Stretch Right`（親の幅に合わせる）
   - **Width**: `0`（Stretchなので自動的に親の幅に合わせる）
   - **Height**: `0`（ContentSizeFitterが自動計算）

## 修正後の確認

1. **Sceneビュー**で`Content`を選択して、位置とサイズを確認
   - `Content`が`Viewport`の上部に配置されているか確認
   - `Content`の幅が`Viewport`の幅に合わせて伸びているか確認

2. **Gameビュー**でゲームを実行して確認
   - リストアイテムがタイトルバーに隠れていないか確認
   - テキストが正しく表示されているか確認
   - スクロールが正しく動作するか確認

## よくある問題と解決方法

### 問題1: ContentがViewportの上部に配置されない

**原因**: ContentのAnchorが正しく設定されていない

**解決方法**:
1. `Content`を選択
2. **Rect Transform**の**Anchor Presets**を`Top Stretch`に設定
3. **Pivot**を`X: 0.5, Y: 1`に設定
4. **Pos Y**を`0`に設定

### 問題2: テキストが小さく表示される

**原因**: テキスト要素のWidthが制約されている

**解決方法**:
1. 各テキスト要素（`EnglishText`、`JapaneseText`、`CountText`）を選択
2. **Rect Transform**の**Anchor Presets**を`Stretch Left`または`Stretch Right`に設定
3. **Width**を`0`に設定（Stretchさせる）
4. **Left**と**Right**のオフセットを設定（例: `Left: 5, Right: -5`）

### 問題3: リストアイテムが縦に並ばない

**原因**: ContentのVertical Layout Groupが正しく設定されていない

**解決方法**:
1. `Content`を選択
2. **Vertical Layout Group**の**Child Alignment**を`Upper Left`に設定
3. **Child Control Size**の**Height**を✅チェック
4. **Spacing**を`5`に設定

---

これで修正は完了です。ゲームを実行して動作を確認してください！
