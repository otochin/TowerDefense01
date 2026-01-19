# Contentが左上にはみ出る問題の修正

## 問題の症状

- リストアイテムが左上にはみ出している
- ContentがViewportの上部に正しく配置されていない

## 原因

ContentのAnchor設定や位置設定が正しくない可能性があります。

## Unity Editorでの修正手順

### ステップ1: Contentの設定を確認・修正

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport > Content`を選択

2. **Inspectorウィンドウ**で**Rect Transform**コンポーネントを確認

3. **Anchor Presets**を設定:
   - **Anchor Presets**のボタンを**左クリック長押し**
   - **Top Stretch**を選択
   - 確認:
     - **Min**: `X: 0, Y: 1`（左上）
     - **Max**: `X: 1, Y: 1`（右上）

4. **Pivot**を設定:
   - **Pivot**: `X: 0.5, Y: 1`（上部中央）

5. **Position**を確認・修正:
   - **Pos X**: `0`（中央）
   - **Pos Y**: `0`（Viewportの上部に配置）← **重要！**
   - **Pos Z**: `0`

6. **Offset**を確認・修正:
   - **Left**: `10`（左パディング）
   - **Right**: `-10`（右パディング）
   - **Top**: `0`（上部はViewportの上部に合わせる）← **重要！**
   - **Bottom**: `0`（下部はContentSizeFitterが制御）

7. **Size**を確認:
   - **Width**: `0`（Stretchなので自動的にViewportの幅に合わせる）
   - **Height**: `0`（ContentSizeFitterが自動計算）

### ステップ2: Viewportの設定を確認

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel > ScrollView > Viewport`を選択

2. **Inspectorウィンドウ**で**Rect Transform**コンポーネントを確認

3. **Anchor Presets**を確認:
   - **Anchor Presets**: `Stretch Stretch`
   - **Min**: `X: 0, Y: 0`（左下）
   - **Max**: `X: 1, Y: 1`（右上）

4. **Pivot**を確認:
   - **Pivot**: `X: 0.5, Y: 0.5`（中央）

5. **Offset**を確認:
   - すべて`0`になっていることを確認

### ステップ3: ScrollRectの設定を確認

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

### ステップ4: Contentの位置を手動でリセット

1. **Hierarchyウィンドウ**で`Content`を選択

2. **Sceneビュー**で`Content`の位置を確認
   - `Content`が`Viewport`の上部に配置されているか確認
   - 左上にはみ出している場合は、手動で位置を調整

3. **Inspectorウィンドウ**で**Rect Transform**の**Pos Y**を`0`に設定

4. **Sceneビュー**で確認:
   - `Content`が`Viewport`の上部中央に配置されていることを確認

### ステップ5: ゲームを実行して確認

1. **Playボタン**をクリックしてゲームを開始

2. ゲームを勝利するまで進める

3. 間違えた問題リストが表示されたら確認:
   - リストアイテムがViewport内に正しく表示されているか
   - 左上にはみ出していないか
   - スクロールが正しく動作するか

## よくある問題と解決方法

### 問題1: Contentが左上にはみ出している

**原因**: ContentのPos Yが0以外の値になっている、またはAnchor設定が間違っている

**解決方法**:
1. `Content`を選択
2. **Rect Transform**の**Pos Y**を`0`に設定
3. **Anchor Presets**を`Top Stretch`に設定
4. **Pivot**を`X: 0.5, Y: 1`に設定

### 問題2: ContentがViewportの上部に配置されない

**原因**: ContentのAnchorがTop Stretchになっていない

**解決方法**:
1. `Content`を選択
2. **Rect Transform**の**Anchor Presets**を`Top Stretch`に設定
3. **Pivot**を`X: 0.5, Y: 1`に設定
4. **Pos Y**を`0`に設定

### 問題3: リストアイテムが縦に並ばない

**原因**: ContentのVertical Layout Groupが正しく設定されていない

**解決方法**:
1. `Content`を選択
2. **Vertical Layout Group**の**Child Alignment**を`Upper Left`に設定
3. **Child Control Size**の**Height**を✅チェック
4. **Spacing**を`5`に設定

---

これで修正は完了です。ゲームを実行して動作を確認してください！
