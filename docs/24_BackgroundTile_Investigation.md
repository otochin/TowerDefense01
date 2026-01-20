# 背景タイルが2枚分しか表示されない問題の調査

## 問題の概要

背景タイルが画面全体に表示されず、中央に2マス分しか表示されない問題が発生しています。

## 考えられる原因

### 1. **リフレクションによる設定の失敗**

`SpriteRenderer`の`DrawMode`と`Size`プロパティは、リフレクションを使用して設定していますが、以下の理由で失敗する可能性があります：

- Unityのバージョンによってプロパティ名が異なる
- プロパティが読み取り専用の場合がある
- 実行時にプロパティが変更できない場合がある

**確認方法:**
- Playモードで実行し、コンソールにデバッグログを確認
- `[BackgroundManager] SetupTiledBackground - New DrawMode:` と `New Size:` のログを確認
- エラーメッセージが表示されていないか確認

### 2. **`Size`プロパティが正しく反映されていない**

`DrawMode`が`Tiled`の場合、`Size`プロパティでタイルの繰り返し範囲を指定しますが、以下の理由で反映されない可能性があります：

- `DrawMode`が`Tiled`に設定されていない
- `Size`プロパティの値が実行時にリセットされている
- `Transform`の`Scale`と`Size`の関係が正しくない

**確認方法:**
- Unity Editorで`Sprite Renderer`コンポーネントを確認
- `Draw Mode`が`Tiled`になっているか確認
- `Size`フィールドが表示されているか確認
- `Size`の値を変更しても反映されないか確認

### 3. **`Transform`の`Scale`と`Size`の競合**

`DrawMode`が`Tiled`の場合：
- `Size`プロパティ: タイルの繰り返し範囲を指定
- `Transform`の`Scale`: 個々のタイルのサイズを制御

しかし、`Transform`の`Scale`が`Size`と競合している可能性があります。

**確認方法:**
- `Transform`の`Scale`を`(1, 1, 1)`に設定
- `Size`プロパティのみで調整
- タイルが正しく表示されるか確認

### 4. **`Sprite`の`bounds.size`が正しく取得できていない**

スプライトのサイズ計算が間違っている可能性があります。

**確認方法:**
- コンソールの`[BackgroundManager] SetupTiledBackground - Sprite:` ログを確認
- `sprite.bounds.size`の値が正しいか確認
- 画像のインポート設定（`Pixels Per Unit`）を確認

### 5. **カメラのサイズ計算が間違っている**

カメラのサイズ計算が間違っている可能性があります。

**確認方法:**
- コンソールの`[BackgroundManager] SetupTiledBackground - Camera:` ログを確認
- `camera.orthographicSize`と`camera.aspect`の値が正しいか確認
- `totalWidth`と`totalHeight`の計算が正しいか確認

## 調査手順

### ステップ1: デバッグログの確認

1. Playモードで実行
2. コンソールに以下のログを確認:
   - `[BackgroundManager] SetupTiledBackground - Camera:`
   - `[BackgroundManager] SetupTiledBackground - Sprite:`
   - `[BackgroundManager] SetupTiledBackground - TotalSize:`
   - `[BackgroundManager] SetupTiledBackground - Calculated Scale:`
   - `[BackgroundManager] SetupTiledBackground - Current DrawMode:`
   - `[BackgroundManager] SetupTiledBackground - New DrawMode:`
   - `[BackgroundManager] SetupTiledBackground - Current Size:`
   - `[BackgroundManager] SetupTiledBackground - New Size:`

3. 各値が期待通りか確認

### ステップ2: Unity Editorでの手動設定

1. `BackgroundManager` GameObjectを選択
2. Inspectorで`Sprite Renderer`コンポーネントを確認
3. 以下の設定を確認・変更:
   - `Draw Mode`: `Tiled`に設定
   - `Size`: `(30, 20)`以上に設定（画面全体を覆う）
   - `Transform`の`Scale`: `(1, 1, 1)`に設定（`Size`で制御する場合）

4. Playモードで実行し、タイルが正しく表示されるか確認

### ステップ3: `Transform`の`Scale`のみで調整

1. `Sprite Renderer`の`Draw Mode`を`Simple`に変更
2. `Transform`の`Scale`を大きく設定（例: `(50, 30, 1)`）
3. タイルが画面全体に表示されるか確認

### ステップ4: 画像のインポート設定を確認

1. Projectウィンドウで`maptile_tsuchi_01`を選択
2. Inspectorで以下の設定を確認:
   - `Texture Type`: `Sprite (2D and UI)`
   - `Wrap Mode`: `Repeat`（重要）
   - `Pixels Per Unit`: `100`（推奨）
3. `Apply`をクリック

## 解決策

### 解決策1: Unity Editorで手動設定（推奨）

リフレクションが失敗する場合、Unity Editorで直接設定します：

1. `BackgroundManager` GameObjectを選択
2. `Sprite Renderer`コンポーネントで:
   - `Draw Mode`: `Tiled`
   - `Size`: `(30, 20)`以上（画面全体を覆う）
3. `Transform`の`Scale`: `(1, 1, 1)`に設定
4. `Background Manager`コンポーネントで:
   - `Tile Size`: `(0.5, 0.5)`または適切な値

### 解決策2: `Transform`の`Scale`のみを使用

`DrawMode`が`Tiled`で動作しない場合、`Transform`の`Scale`のみで調整します：

1. `Sprite Renderer`の`Draw Mode`を`Simple`に変更
2. `Transform`の`Scale`を大きく設定（例: `(50, 30, 1)`）
3. 画像の`Wrap Mode`を`Repeat`に設定
4. シェーダーでタイル表示を実装（高度）

### 解決策3: スクリプトの修正

リフレクションの代わりに、より確実な方法を使用します：

- `EditorUtility.SetDirty()`を使用して変更を保存
- `PrefabUtility.RecordPrefabInstancePropertyModifications()`を使用
- または、実行時に`Size`を設定する代わりに、Inspectorで手動設定を推奨

## 確認事項チェックリスト

- [ ] デバッグログで`DrawMode`が`Tiled`に設定されているか確認
- [ ] デバッグログで`Size`が正しく設定されているか確認
- [ ] Unity Editorで`Draw Mode`が`Tiled`になっているか確認
- [ ] Unity Editorで`Size`が画面全体を覆う値になっているか確認
- [ ] `Transform`の`Scale`が適切か確認
- [ ] 画像の`Wrap Mode`が`Repeat`になっているか確認
- [ ] カメラのサイズが正しく計算されているか確認
- [ ] スプライトのサイズが正しく取得できているか確認

## 次のステップ

1. デバッグログを確認して、どの段階で問題が発生しているか特定
2. Unity Editorで手動設定して、手動設定で動作するか確認
3. 動作する設定を特定したら、スクリプトを修正して自動設定できるようにする
