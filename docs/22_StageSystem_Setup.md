# ステージシステムセットアップガイド

## 概要

ステージシステムは、ゲームの進行に応じてステージ数を管理し、難易度を調整するシステムです。

## 機能

- 画面右上に現在のステージ数を表示（「Stage: 1」など）
- 最初は「Stage: 1」
- 勝利した後、ゲームモードを選ぶとStageが１つ進む
- Stageが１つ進むごとに、モンスターが表示される秒数が0.9倍になる（徐々に短くなって難易度が上がっていく）
- 負けた後、ゲームモードを選ぶとStage1からやり直しになる

## Unity Editorでのセットアップ

### 1. StageManagerの設定

1. **GameManagerの下にStageManagerを作成**
   - Hierarchyで `GameManager` を選択
   - `GameManager` を右クリック → `Create Empty`
   - 名前を `StageManager` に変更
   - これにより、`GameManager` の子として `StageManager` が作成されます
   - 他のマネージャー（`ResourceManager`、`EnemySpawner`、`WordLearningSystem`）と同じ階層に配置されます

2. **StageManagerスクリプトをアタッチ**
   - `StageManager` GameObjectを選択
   - Inspectorで `Add Component` → `Stage Manager` を検索して追加

3. **設定確認**
   - `Current Stage`: 1（初期値）
   - `Difficulty Multiplier`: 0.9（ステージが進むごとのスポーン間隔倍率）

### 2. StageUIの設定

1. **Canvasを確認**
   - Hierarchyで `Canvas` が存在することを確認
   - `Canvas` の下には以下のパネルが既に存在します：
     - `ResourcePanel`
     - `CharacterSelectPanel`
     - `WordQuizPanel`
     - `GameModeSelectPanel`
     - `IncorrectAnswersListPanel`

2. **StagePanelを作成**
   - Hierarchyで `Canvas` を選択
   - `Canvas` を右クリック → `UI > Panel`
   - 名前を `StagePanel` に変更
   - これにより、`Canvas` の子として `StagePanel` が作成されます
   - 他のパネル（`ResourcePanel`、`CharacterSelectPanel`など）と同じ階層に配置されます

3. **StagePanelの位置設定（右上）**
   - `StagePanel`を選択
   - Inspectorで `Rect Transform` を確認
   - `Anchor Presets` をクリック → `top-right` を選択（Shift+Altを押しながら）
   - `Pos X`: -100（右端から100ピクセル左）
   - `Pos Y`: -50（上端から50ピクセル下）
   - `Width`: 200
   - `Height`: 50

4. **TextMeshProUGUIを追加**
   - Hierarchyで `StagePanel` を選択
   - `StagePanel` を右クリック → `UI > Text - TextMeshPro`
   - 名前を `StageText` に変更
   - これにより、`StagePanel` の子として `StageText` が作成されます
   - 構造は `WordQuizPanel` の下に `TimerText`、`QuestionText` などがあるのと同じパターンです

5. **StageTextの設定**
   - Hierarchyで `StageText` を選択
   - Inspectorで `Rect Transform` を確認
   - `Anchor Presets` をクリック → `stretch-stretch` を選択（Shift+Altを押しながらクリック）
     - これにより、`StageText` が `StagePanel` 全体に広がります
   - `Left`, `Top`, `Right`, `Bottom`: すべて 0
   - `TextMeshProUGUI` コンポーネントで：
     - `Text`: `Stage: 1`（初期表示）
     - `Font Size`: 24（見やすいサイズに調整可能）
     - `Alignment`: `Center`（水平）、`Middle`（垂直）
     - `Color`: 白（または見やすい色、例: `#FFFFFF`）
     - `Font Asset`: TextMeshProのフォントアセットが設定されていることを確認
       - 既存のUI要素（`TimerText`、`QuestionText`など）と同じフォントアセットを使用することを推奨

6. **StageUIスクリプトをアタッチ**
   - Hierarchyで `StagePanel` を選択
   - Inspectorで `Add Component` をクリック
   - `Stage UI` を検索して追加

7. **参照設定**
   - `StagePanel` が選択された状態で、Inspectorの `Stage UI` コンポーネントを確認
   - `Stage Text`: Hierarchyから `StageText` GameObjectをドラッグ&ドロップ
   - `Stage Format`: `Stage: {0}`（デフォルト、変更不要）

### 3. 最終的なヒエラルキー構造の確認

セットアップ完了後、以下のような構造になっていることを確認してください：

```
SampleScene
├── Main Camera
├── GameManager
│   ├── CharacterSpawner
│   ├── ResourceManager
│   ├── EnemySpawner
│   ├── WordLearningSystem
│   └── StageManager ← 新規追加
├── Canvas
│   ├── EventSystem
│   ├── ResourcePanel
│   ├── CharacterSelectPanel
│   ├── WordQuizPanel
│   │   ├── TimerText
│   │   ├── QuestionText
│   │   ├── ChoicesPanel
│   │   └── FeedbackPanel
│   ├── GameModeSelectPanel
│   ├── IncorrectAnswersListPanel
│   └── StagePanel ← 新規追加
│       └── StageText
├── PlayerCastle
└── EnemyCastle
```

### 4. 動作確認

1. **Playモードで確認**
   - Unity Editorで `Play` ボタンをクリック
   - ゲームを実行
   - 画面右上に「Stage: 1」が表示されることを確認
   - `ResourcePanel`（左上）と重複していないことを確認

2. **ステージ進行の確認**
   - ゲームを勝利する（敵の城のHPを0にする）
   - ゲームモード選択パネルが表示される
   - ゲームモードを選択（例: 「English to Japanese」）
   - 画面右上が「Stage: 2」に更新されることを確認
   - コンソールに `[GameModeSelectUI] Stage advanced to: 2` が表示されることを確認

3. **ステージリセットの確認**
   - ゲームを敗北する（プレイヤーの城のHPを0にする）
   - ゲームモード選択パネルが表示される
   - ゲームモードを選択
   - 画面右上が「Stage: 1」に戻ることを確認
   - コンソールに `[GameModeSelectUI] Stage reset to: 1` が表示されることを確認

4. **難易度調整の確認**
   - `EnemySpawner` の `Spawn Interval` を確認（例: 3.0秒）
   - Stage 1: スポーン間隔 = 基本値（例: 3.0秒）
   - Stage 2: スポーン間隔 = 基本値 × 0.9（例: 2.7秒）
   - Stage 3: スポーン間隔 = 基本値 × 0.81（例: 2.43秒）
   - Stage 4: スポーン間隔 = 基本値 × 0.729（例: 2.187秒）
   - コンソールに `[EnemySpawner] Spawn interval updated for Stage X:` が表示されることを確認

## トラブルシューティング

### StageUIが表示されない

1. **Canvasの確認**
   - Canvasがアクティブになっているか確認
   - Canvasの `Render Mode` が `Screen Space - Overlay` になっているか確認

2. **StagePanelの位置確認**
   - `StagePanel`の `Rect Transform` で `Pos X` と `Pos Y` を確認
   - 画面外に出ていないか確認

3. **StageTextの設定確認**
   - `StageText`の `TextMeshProUGUI` コンポーネントで `Text` が設定されているか確認
   - `Font Asset` が設定されているか確認

### Stageが進まない/リセットされない

1. **StageManagerの確認**
   - `StageManager` GameObjectがシーンに存在するか確認
   - `StageManager` コンポーネントがアタッチされているか確認

2. **GameEndHandlerの確認**
   - `GameEndHandler` が正しく動作しているか確認
   - 勝利/敗北の判定が正しく行われているか確認

3. **GameModeSelectUIの確認**
   - `GameModeSelectUI` の `HandleStageManagement()` が呼ばれているか確認
   - ログで `[GameModeSelectUI] Stage advanced to:` または `[GameModeSelectUI] Stage reset to:` が出力されているか確認

### スポーン間隔が調整されない

1. **EnemySpawnerの確認**
   - `EnemySpawner` が `StageManager.Instance` を正しく参照しているか確認
   - `StartSpawning()` が呼ばれた時に `UpdateSpawnIntervalForStage()` が実行されているか確認

2. **ログの確認**
   - コンソールで `[EnemySpawner] Spawn interval updated for Stage X:` が出力されているか確認
   - 倍率が正しく計算されているか確認

## 参考

- `Assets/Scripts/Core/StageManager.cs`: ステージ管理システム
- `Assets/Scripts/UI/StageUI.cs`: ステージ表示UI
- `Assets/Scripts/Core/GameEndHandler.cs`: ゲーム終了処理（勝利/敗北フラグの管理）
- `Assets/Scripts/UI/GameModeSelectUI.cs`: ゲームモード選択UI（ステージ管理の呼び出し）
- `Assets/Scripts/Enemy/EnemySpawner.cs`: 敵スポナー（Stageに応じたスポーン間隔の調整）
