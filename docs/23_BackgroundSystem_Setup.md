# 背景システムセットアップガイド

## 概要

背景システムは、ステージごとに背景を切り替えるシステムです。まずはステージ1の背景を設定します。

## 機能

- ステージごとに背景画像を設定可能
- ステージが変更されると自動的に背景が切り替わる
- 背景画像がない場合は背景色を使用
- 背景を画面全体に自動的に拡大

## Unity Editorでのセットアップ

### 1. 背景画像の準備

1. **背景画像を配置**
   - `Assets/Textures/` フォルダに背景画像を配置
   - 例: `stage1_background.png`、`stage1_tile.png` など
   - 推奨サイズ: 1920x1080（Full HD）またはそれ以上
   - タイル状の画像を使用する場合は、シームレス（継ぎ目がない）な画像を推奨

2. **画像のインポート設定（重要）**
   - Projectウィンドウで背景画像を選択
   - Inspectorで以下の設定を確認・変更：
     - `Texture Type`: `Sprite (2D and UI)` に設定
     - `Sprite Mode`: 
       - 単一の背景画像の場合: `Single`
       - タイル状の画像の場合: `Single`（タイル表示はBackgroundManagerで制御）
     - **`Mesh Type`: `Full Rect`（タイル表示の場合、必須！）**
       - デフォルトは`Tight`ですが、タイル表示には`Full Rect`が必要です
       - これが設定されていないと、`DrawMode`が`Tiled`から`Sliced`に変わってしまいます
     - `Pixels Per Unit`: `100`（デフォルト、必要に応じて調整）
     - `Filter Mode`: `Bilinear` または `Trilinear`（滑らかな表示）
     - `Wrap Mode`: `Repeat`（**タイル表示の場合、重要！**）
     - `Compression`: `None`（高品質）または `Normal`（バランス）
     - `Max Size`: 画像の最大サイズ（2048以上を推奨）
   - `Apply` をクリックして設定を適用
   
   **重要**: 
   - タイル状に表示する場合は、`Wrap Mode` を `Repeat` に設定してください
   - **`Mesh Type` を `Full Rect` に設定してください**（これがないとタイル表示が正しく動作しません）

### 2. BackgroundManagerの作成

1. **Background GameObjectを作成**
   - Hierarchyで `GameManager` を選択
   - `GameManager` を右クリック → `Create Empty`
   - 名前を `BackgroundManager` に変更
   - これにより、`GameManager` の子として `BackgroundManager` が作成されます
   - 他のマネージャー（`ResourceManager`、`EnemySpawner`、`WordLearningSystem`、`StageManager`）と同じ階層に配置されます

2. **Background GameObjectにSpriteRendererを追加**
   - `BackgroundManager` GameObjectを選択
   - Inspectorで `Add Component` → `Sprite Renderer` を検索して追加
   - これにより、背景画像を表示するためのコンポーネントが追加されます

3. **BackgroundManagerスクリプトをアタッチ**
   - `BackgroundManager` GameObjectを選択
   - Inspectorで `Add Component` → `Background Manager` を検索して追加

4. **BackgroundManagerの設定**
   - `BackgroundManager` GameObjectが選択された状態で、Inspectorの `Background Manager` コンポーネントを確認
   - `Background Renderer`: 
     - Hierarchyから `BackgroundManager` GameObject自体をドラッグ&ドロップ
     - または、自動検出されるため空欄でもOK
   - `Main Camera`: 
     - Hierarchyから `Main Camera` をドラッグ&ドロップ
     - または、自動検出されるため空欄でもOK
   - `Fit To Screen`: `true`（通常モードの場合、背景を画面全体に合わせて拡大）
   - `Background Z Position`: `10`（カメラより後ろに配置、デフォルト値）
   - `Use Tiled Mode`: `true`（**タイル状に繰り返し表示する場合**）
   - `Tile Size`: `(1, 1)`（タイルのサイズ、必要に応じて調整）
     - 例: タイル画像が100x100ピクセルで、Pixels Per Unitが100の場合、`Tile Size` は `(1, 1)`
     - 例: タイル画像が50x50ピクセルで、Pixels Per Unitが100の場合、`Tile Size` は `(0.5, 0.5)`
   
   **補足: Unity Editorで手動設定する場合**
   - `BackgroundManager` GameObjectの `SpriteRenderer` コンポーネントを確認
   - `Draw Mode` を `Tiled` に変更（Unity 2018.1以降）
   - `Size` フィールドで画面全体を覆うサイズを設定
   - この方法でもタイル表示が可能です（スクリプトの自動設定が動作しない場合）

### 3. ステージ1の背景を設定

1. **Stage Backgroundsリストに追加**
   - `BackgroundManager` GameObjectが選択された状態で、Inspectorの `Background Manager` コンポーネントを確認
   - `Stage Backgrounds` リストの `Size` を `1` に設定
   - `Element 0` を展開：
     - `Stage Number`: `1`（ステージ1）
     - `Background Sprite`: 
       - Projectウィンドウから背景画像（例: `stage1_background.png`）をドラッグ&ドロップ
       - または、右側の円形アイコンをクリックして選択
     - `Background Color`: `White`（背景画像がある場合は使用されませんが、フォールバックとして設定）

2. **背景画像の確認**
   - `Background Sprite` に画像が設定されていることを確認
   - 画像が正しく表示されることを確認

### 4. 背景の位置とサイズの調整（オプション）

1. **背景の位置調整**
   - `BackgroundManager` GameObjectを選択
   - Sceneビューで背景の位置を確認
   - Transformコンポーネントで位置を調整：
     - `Position`: `(0, 0, 10)`（カメラの後ろに配置）
     - カメラが `(0, 0, -10)` にある場合、背景は `(0, 0, 10)` に配置

2. **背景のサイズ調整**
   - **タイルモードの場合**:
     - `Use Tiled Mode` が `true` の場合、自動的にタイル状に繰り返し表示されます
     - `Tile Size` を調整してタイルのサイズを変更できます
     - タイルサイズの計算方法:
       - `Tile Size = 画像のピクセルサイズ / Pixels Per Unit`
       - 例: 100x100ピクセルの画像でPixels Per Unitが100の場合、`Tile Size` は `(1, 1)`
       - 例: 50x50ピクセルの画像でPixels Per Unitが100の場合、`Tile Size` は `(0.5, 0.5)`
   - **通常モードの場合**:
     - `Fit To Screen` が `true` の場合、自動的に画面全体に拡大されます
     - 手動で調整する場合：
       - `Fit To Screen` を `false` に設定
       - Transformコンポーネントの `Scale` を調整
       - 例: `(10, 10, 1)` など

### 5. カメラの設定確認

1. **Main Cameraの確認**
   - Hierarchyで `Main Camera` を選択
   - Inspectorで `Camera` コンポーネントを確認：
     - `Projection`: `Orthographic`（2Dゲームの場合）
     - `Size`: `5`（デフォルト、必要に応じて調整）
     - `Background`: 背景画像がある場合は使用されませんが、フォールバックとして設定可能

2. **カメラの位置確認**
   - Transformコンポーネントで位置を確認：
     - `Position`: `(0, 0, -10)`（デフォルト）
     - 背景が `(0, 0, 10)` にある場合、カメラは `(0, 0, -10)` に配置

### 6. 最終的なヒエラルキー構造の確認

セットアップ完了後、以下のような構造になっていることを確認してください：

```
SampleScene
├── Main Camera
├── GameManager
│   ├── CharacterSpawner
│   ├── ResourceManager
│   ├── EnemySpawner
│   ├── WordLearningSystem
│   ├── StageManager
│   └── BackgroundManager ← 新規追加
│       └── (SpriteRendererコンポーネント)
├── Canvas
│   ├── EventSystem
│   ├── ResourcePanel
│   ├── CharacterSelectPanel
│   ├── WordQuizPanel
│   ├── GameModeSelectPanel
│   ├── IncorrectAnswersListPanel
│   └── StagePanel
├── PlayerCastle
└── EnemyCastle
```

### 7. 動作確認

1. **Playモードで確認**
   - Unity Editorで `Play` ボタンをクリック
   - ゲームを実行
   - 背景が画面全体に表示されることを確認
   - 背景画像が正しく表示されることを確認

2. **ステージ変更時の背景切り替え確認**
   - ゲームを勝利する（敵の城のHPを0にする）
   - ゲームモード選択パネルが表示される
   - ゲームモードを選択（例: 「English to Japanese」）
   - ステージが進むと、対応する背景に切り替わることを確認
   - コンソールに `[BackgroundManager] Background updated for stage X` が表示されることを確認

3. **背景のサイズ確認**
   - 背景が画面全体を覆っていることを確認
   - 背景がカメラの後ろ（Z=10）に配置されていることを確認
   - 背景が他のオブジェクト（キャラクター、城など）の後ろに表示されることを確認

4. **タイル表示の確認（タイルモードの場合）**
   - 背景画像がタイル状に繰り返し表示されることを確認
   - タイルの継ぎ目が自然に見えることを確認
   - `Tile Size` を調整してタイルのサイズが適切か確認

## トラブルシューティング

### 背景が表示されない

1. **SpriteRendererの確認**
   - `BackgroundManager` GameObjectに `SpriteRenderer` コンポーネントがアタッチされているか確認
   - `Sprite` フィールドに背景画像が設定されているか確認

2. **画像のインポート設定確認**
   - Projectウィンドウで背景画像を選択
   - Inspectorで `Texture Type` が `Sprite (2D and UI)` になっているか確認
   - `Apply` をクリックして設定を適用

3. **カメラの位置確認**
   - カメラが背景の前（Z < 背景のZ位置）にあることを確認
   - カメラの `Culling Mask` で背景のレイヤーが含まれているか確認

4. **背景の位置確認**
   - `BackgroundManager` GameObjectの `Position` を確認
   - カメラの視野内にあることを確認

### 背景が画面全体を覆わない

1. **タイルモードの場合**
   - `Use Tiled Mode` が `true` になっているか確認
   - `Tile Size` が適切に設定されているか確認
   - 画像の `Wrap Mode` が `Repeat` になっているか確認

2. **通常モードの場合**
   - `Fit To Screen` が `true` になっているか確認
   - `BackgroundManager` GameObjectの `Scale` を確認
   - 必要に応じて手動でスケールを調整

3. **カメラのサイズ確認**
   - カメラの `Size` が適切か確認
   - 背景のサイズとカメラのサイズが合っているか確認

### タイルが繰り返し表示されない / Tile Sizeを変更してもサイズが変わらない

1. **画像のインポート設定確認（重要）**
   - Projectウィンドウで背景画像を選択
   - Inspectorで `Wrap Mode` が `Repeat` になっているか確認
   - `Apply` をクリックして設定を適用
   - **重要**: `Wrap Mode` が `Repeat` でないと、タイル表示が正しく動作しません

2. **SpriteRendererのDraw Modeを手動で設定（推奨）**
   - `BackgroundManager` GameObjectを選択
   - Inspectorで `Sprite Renderer` コンポーネントを確認
   - `Draw Mode` を `Simple` から `Tiled` に変更
   - `Size` フィールドが表示されるので、以下の手順で設定:
     
     **Sizeの計算方法:**
     - カメラのサイズを確認（例: Orthographic Size = 5 の場合、高さ = 10）
     - カメラの幅 = 高さ × アスペクト比（例: 10 × 1.78 = 17.8）
     - `Size` = `(カメラ幅, カメラ高さ)` またはそれより大きく設定
     - 例: `Size = (20, 20)` または `(30, 20)` など
   
   **Tile Sizeの調整:**
   - `Background Manager` コンポーネントの `Tile Size` を調整
   - タイルが大きすぎる場合: `Tile Size` を小さく（例: `(1, 1)` → `(0.5, 0.5)` → `(0.3, 0.3)`）
   - タイルが小さすぎる場合: `Tile Size` を大きく（例: `(0.5, 0.5)` → `(1, 1)` → `(2, 2)`）
   - **注意**: `Draw Mode` が `Tiled` の場合、`Tile Size` の変更は即座に反映されない場合があります
   - その場合は、Playモードを一度停止して再開するか、`Size` フィールドを手動で調整してください

3. **BackgroundManagerの設定確認**
   - `Use Tiled Mode` が `true` になっているか確認
   - `Tile Size` が適切に設定されているか確認
   - タイルサイズの計算:
     - `Tile Size = 画像のピクセルサイズ / Pixels Per Unit`
     - 例: 100x100ピクセルの画像でPixels Per Unitが100の場合、`Tile Size` は `(1, 1)`
     - 例: 50x50ピクセルの画像でPixels Per Unitが100の場合、`Tile Size` は `(0.5, 0.5)`

4. **手動でSizeを調整する方法（Tile Sizeが反映されない場合）**
   - `Sprite Renderer` の `Draw Mode` を `Tiled` に設定
   - `Size` フィールドを直接編集
   - 例: タイルが大きすぎる場合:
     - `Size` を `(20, 20)` から `(40, 40)` に変更（タイルが小さく見える）
     - または、`Size` を `(10, 10)` に変更（タイルが大きく見える）
   - **重要**: `Size` を大きくすると、同じタイルがより多く繰り返されます（タイルが小さく見える）
   - **重要**: `Size` を小さくすると、タイルが少なく繰り返されます（タイルが大きく見える）

5. **デバッグ方法**
   - Playモードで実行
   - コンソールに `[BackgroundManager] Tiled background set:` というログが表示されるか確認
   - ログが表示されない場合、`Draw Mode` が `Tiled` に設定されていない可能性があります
   - Unity Editorで `Sprite Renderer` の `Draw Mode` を `Tiled` に手動で設定してください

### 背景が他のオブジェクトの前に表示される

1. **Z位置の確認**
   - `BackgroundManager` GameObjectの `Position Z` を確認
   - カメラより後ろ（Z > 0）に配置されていることを確認
   - `Background Z Position` を `10` 以上に設定

2. **Sorting Layerの確認**
   - `SpriteRenderer` の `Sorting Layer` を確認
   - 背景用の `Sorting Layer` を作成し、他のオブジェクトより後ろに設定
   - 例: `Background` レイヤーを作成し、`Order in Layer` を `-10` に設定

### ステージ変更時に背景が切り替わらない

1. **StageManagerの確認**
   - `StageManager` GameObjectがシーンに存在するか確認
   - `StageManager` コンポーネントがアタッチされているか確認

2. **BackgroundManagerの確認**
   - `BackgroundManager` GameObjectがシーンに存在するか確認
   - `BackgroundManager` コンポーネントがアタッチされているか確認
   - `Stage Backgrounds` リストにステージに対応する背景が設定されているか確認

3. **ログの確認**
   - コンソールで `[BackgroundManager] Background updated for stage X` が出力されているか確認
   - エラーメッセージがないか確認

## 次のステップ

ステージ1の背景が正常に表示されることを確認したら、以下の手順で他のステージの背景も追加できます：

1. **ステージ2の背景を追加**
   - `Stage Backgrounds` リストの `Size` を `2` に設定
   - `Element 1` を展開：
     - `Stage Number`: `2`
     - `Background Sprite`: ステージ2の背景画像を設定
   - 同様に、ステージ3、4...と追加可能

2. **背景画像の追加**
   - `Assets/Textures/` フォルダに各ステージの背景画像を配置
   - 例: `stage2_background.png`、`stage3_background.png` など

## 参考

- `Assets/Scripts/Core/BackgroundManager.cs`: 背景管理システム
- `Assets/Scripts/Core/StageManager.cs`: ステージ管理システム（背景切り替えのトリガー）
- Unity公式ドキュメント: [SpriteRenderer](https://docs.unity3d.com/ja/current/Manual/class-SpriteRenderer.html)
- Unity公式ドキュメント: [Camera](https://docs.unity3d.com/ja/current/Manual/class-Camera.html)
