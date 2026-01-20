# キャラクター強化システムセットアップガイド

## 概要

キャラクター強化システムは、ゲーム終了時に3つのボタンから1つだけ選択して、キャラクターを強化できるシステムです。

## 機能

- ゲーム終了時（勝利/敗北後）に強化UIを表示
- 3つのボタンから1つだけ選択可能（Warrior HP +10、Archer HP +10、Mage HP +10）
- コストは消費しない（無料で強化）
- 1つ選択すると他のボタンは無効化される
- 強化されたステータスは次回のゲームから適用される

## Unity Editorでのセットアップ

### 1. CharacterUpgradeManagerの作成

1. **GameManager GameObjectを選択**
   - Hierarchyで `GameManager` を選択

2. **CharacterUpgradeManager GameObjectを作成**
   - `GameManager` を右クリック → `Create Empty`
   - 名前を `CharacterUpgradeManager` に変更

3. **CharacterUpgradeManagerスクリプトをアタッチ**
   - `CharacterUpgradeManager` GameObjectを選択
   - Inspectorで `Add Component` → `Character Upgrade Manager` を検索して追加

4. **CharacterUpgradeManagerの設定**
   - `Health Upgrade Amount`: `10`（HP強化時の増加量）
   - `Attack Power Upgrade Amount`: `5`（攻撃力強化時の増加量）
   - `Attack Speed Upgrade Amount`: `0.1`（攻撃速度強化時の増加量）
   - `Move Speed Upgrade Amount`: `0.5`（移動速度強化時の増加量）

### 2. CharacterUpgradePanelの作成（CharacterSelectPanelを複製）

1. **CharacterSelectPanelを複製**
   - Hierarchyで `Canvas` → `CharacterSelectPanel` を選択
   - 右クリック → `Duplicate`
   - 複製したGameObjectの名前を `CharacterUpgradePanel` に変更

2. **CharacterSelectUIスクリプトを削除**
   - `CharacterUpgradePanel` を選択
   - Inspectorで `Character Select UI` コンポーネントを削除（右クリック → `Remove Component`）

3. **CharacterUpgradeUIスクリプトをアタッチ**
   - `CharacterUpgradePanel` を選択
   - Inspectorで `Add Component` → `Character Upgrade UI` を検索して追加

4. **CharacterUpgradeUIの設定**
   - `Upgrade Manager`: Hierarchyから `CharacterUpgradeManager` をドラッグ&ドロップ（自動検出も可能）
   - `Initial Upgrade Type`: `Health`（初期表示する強化タイプ）

### 3. CharacterButtonをCharacterUpgradeButtonに変更

1. **各CharacterButtonを確認**
   - `CharacterUpgradePanel` の子要素に `CharacterButton_1`、`CharacterButton_2`、`CharacterButton_3` があることを確認

2. **CharacterButtonスクリプトを削除**
   - 各 `CharacterButton_X` を選択
   - Inspectorで `Character Button` コンポーネントを削除

3. **CharacterUpgradeButtonスクリプトをアタッチ**
   - 各 `CharacterButton_X` を選択
   - Inspectorで `Add Component` → `Character Upgrade Button` を検索して追加

4. **各CharacterUpgradeButtonの設定**
   - `Character Button_1`:
     - `Character Type`: `Warrior`
     - `Upgrade Type`: `Health`
   - `Character Button_2`:
     - `Character Type`: `Archer`
     - `Upgrade Type`: `Health`
   - `Character Button_3`:
     - `Character Type`: `Mage`
     - `Upgrade Type`: `Health`

5. **UI要素の確認**
   - 各ボタンに以下の子要素があることを確認：
     - `CharacterNameText` (TextMeshProUGUI) - キャラクター名表示用
     - `UpgradeText` (TextMeshProUGUI) - 強化内容表示用（例: "HP +10"）
   - 必要に応じて、`CharacterSelectPanel` の構造を参考に子要素を調整

### 4. GameEndHandlerの設定

1. **GameEndHandler GameObjectを選択**
   - Hierarchyで `GameManager` → `GameEndHandler` を選択

2. **CharacterUpgradeUIの参照を設定**
   - Inspectorで `Character Upgrade UI` フィールドに `CharacterUpgradePanel` をドラッグ&ドロップ（自動検出も可能）

### 5. 最終的なヒエラルキー構造の確認

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
│   ├── BackgroundManager
│   ├── GameEndHandler
│   └── CharacterUpgradeManager ← 新規追加
├── Canvas
│   ├── EventSystem
│   ├── ResourcePanel
│   ├── CharacterSelectPanel
│   ├── CharacterUpgradePanel ← 新規追加（CharacterSelectPanelを複製）
│   │   ├── CharacterButton_1 (CharacterUpgradeButton)
│   │   │   ├── CharacterNameText (TextMeshProUGUI)
│   │   │   └── UpgradeText (TextMeshProUGUI)
│   │   ├── CharacterButton_2 (CharacterUpgradeButton)
│   │   └── CharacterButton_3 (CharacterUpgradeButton)
│   ├── WordQuizPanel
│   ├── GameModeSelectPanel
│   ├── IncorrectAnswersListPanel
│   └── StagePanel
├── PlayerCastle
├── EnemyCastle
└── Background
```

## 動作確認

1. **Playモードで確認**
   - Unity Editorで `Play` ボタンをクリック
   - ゲームを実行
   - ゲームを勝利または敗北する
   - ゲーム終了時に `CharacterUpgradePanel` が表示されることを確認
   - 3つのボタン（Warrior HP +10、Archer HP +10、Mage HP +10）が表示されることを確認

2. **強化ボタンの動作確認**
   - 1つのボタンをクリック
   - クリック後、すべてのボタンが無効化されることを確認
   - コンソールに `[CharacterUpgradeManager] Warrior HP upgraded to level 1 (+10 HP)` などのログが表示されることを確認

3. **強化の適用確認**
   - ゲームモードを再選択してゲームを再開
   - 強化したキャラクターを召喚
   - キャラクターのHPが強化されていることを確認（元のHP + 10）

## トラブルシューティング

### 強化UIが表示されない

1. **GameEndHandlerの設定確認**
   - `GameEndHandler` GameObjectの `Character Upgrade UI` フィールドに `CharacterUpgradePanel` が設定されているか確認
   - 設定されていない場合、自動検出されるが、手動で設定することを推奨

2. **CharacterUpgradePanelのアクティブ状態確認**
   - Hierarchyで `CharacterUpgradePanel` が非アクティブになっていないか確認
   - 初期状態では非アクティブでも問題ありません（`CharacterUpgradeUI`が自動的に制御します）

### 強化が適用されない

1. **CharacterUpgradeManagerの確認**
   - `CharacterUpgradeManager` GameObjectがシーンに存在するか確認
   - `CharacterUpgradeManager` コンポーネントがアタッチされているか確認

2. **CharacterBaseの確認**
   - キャラクターのプレハブに `CharacterBase` コンポーネントがアタッチされているか確認
   - `CharacterData` が正しく設定されているか確認

3. **ログの確認**
   - コンソールで `[CharacterUpgradeManager] Warrior HP upgraded to level 1 (+10 HP)` などのログが表示されているか確認
   - エラーメッセージがないか確認

## 次のステップ

現在はHP強化のみ実装されていますが、将来的に以下の拡張が可能です：

1. **他の強化タイプの追加**
   - 攻撃力強化（Attack Power +5）
   - 攻撃速度強化（Attack Speed +0.1）
   - 移動速度強化（Move Speed +0.5）

2. **強化レベルの表示**
   - 各ボタンに現在の強化レベルを表示（例: "HP +10 (Lv. 3)"）

3. **複数の強化選択**
   - 1回のゲーム終了で複数の強化を選択できるようにする

## 参考

- `Assets/Scripts/Core/CharacterUpgradeManager.cs`: 強化データ管理システム
- `Assets/Scripts/UI/CharacterUpgradeUI.cs`: 強化UI制御
- `Assets/Scripts/UI/CharacterUpgradeButton.cs`: 強化ボタン管理
- `Assets/Scripts/Core/GameEndHandler.cs`: ゲーム終了処理（強化UI表示のトリガー）
- `Assets/Scripts/Character/CharacterBase.cs`: キャラクター基底クラス（強化ステータスの適用）
