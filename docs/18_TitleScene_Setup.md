# タイトルシーンセットアップガイド

## 概要

タイトルシーンを別シーンとして作成し、ゲームモード選択パネルをタイトルシーンに移植する手順を説明します。

## 手順

### 1. タイトルシーンの作成

1. **Unity Editor**で`File > New Scene`を選択
2. **Basic (Built-in)**テンプレートを選択して`Create`をクリック
3. シーンを保存：`Assets/Scenes/TitleScene.unity`

### 2. タイトルシーンの基本設定

1. **Hierarchy**で`Main Camera`を選択
2. **Background**を適切な色に設定（例：黒、青など）
3. **Canvas**を作成：`GameObject > UI > Canvas`
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler**: `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`

### 3. GameModeSelectUIのセットアップ

1. **Canvas**の子として`GameModeSelectPanel`を作成：`GameObject > UI > Panel`
2. `GameModeSelectPanel`に`GameModeSelectUI`コンポーネントを追加
3. **GameModeSelectUI**のInspectorで以下を設定：
   - **English To Japanese Button**: 英単語「English to Japanese」ボタンをドラッグ＆ドロップ
   - **Japanese To English Button**: 英単語「Japanese to English」ボタンをドラッグ＆ドロップ
   - **Phrase English To Japanese Button**: 英熟語「English to Japanese」ボタンをドラッグ＆ドロップ
   - **Phrase Japanese To English Button**: 英熟語「Japanese to English」ボタンをドラッグ＆ドロップ

### 4. ゲームモード選択ボタンの作成

1. `GameModeSelectPanel`の子として4つのボタンを作成：
   - `EnglishToJapaneseButton`（英単語：English to Japanese）
   - `JapaneseToEnglishButton`（英単語：Japanese to English）
   - `PhraseEnglishToJapaneseButton`（英熟語：English to Japanese）
   - `PhraseJapaneseToEnglishButton`（英熟語：Japanese to English）

2. 各ボタンの**Text**子オブジェクトに適切なテキストを設定

### 5. GameSceneManagerのセットアップ

1. **Hierarchy**で空のGameObjectを作成：`GameObject > Create Empty`
2. 名前を`GameSceneManager`に変更
3. `GameSceneManager`コンポーネントを追加
4. **Inspector**で以下を設定：
   - **Title Scene Name**: `TitleScene`
   - **Game Scene Name**: `SampleScene`（またはゲームシーンの名前）

### 6. Build Settingsの設定

1. **File > Build Settings**を開く
2. **Scenes In Build**に以下を追加（順序が重要）：
   - `Assets/Scenes/TitleScene.unity`（インデックス0：最初のシーン）
   - `Assets/Scenes/SampleScene.unity`（インデックス1：ゲームシーン）

### 7. ゲームシーンのGameModeSelectUIの設定

1. **ゲームシーン**（`SampleScene`）を開く
2. `GameModeSelectPanel`を選択
3. **GameModeSelectUI**のInspectorで以下を設定：
   - **Word Learning System**: `WordLearningSystem`をドラッグ＆ドロップ
   - **Character Select UI**: `CharacterSelectUI`をドラッグ＆ドロップ
   - **Enemy Spawner**: `EnemySpawner`をドラッグ＆ドロップ

### 8. GameEndHandlerの設定（ゲームシーン）

1. **ゲームシーン**で`GameEndHandler`を選択
2. **Inspector**で以下を設定：
   - **Game End Panel**: ゲーム終了パネル（新規作成が必要）
   - **Return To Title Button**: 「タイトルへ戻る」ボタン
   - **Next Stage Button**: 「次のステージへ」ボタン（勝利時のみ表示）
   - **Retry Button**: 「もう一度やる」ボタン（敗北時のみ表示）

### 9. ゲーム終了UIパネルの作成（ゲームシーン）

1. **Canvas**の子として`GameEndPanel`を作成：`GameObject > UI > Panel`
2. `GameEndPanel`の子として3つのボタンを作成：
   - `ReturnToTitleButton`（「タイトルへ戻る」）
   - `NextStageButton`（「次のステージへ」）
   - `RetryButton`（「もう一度やる」）
3. 各ボタンの**Text**子オブジェクトに適切なテキストを設定
4. 初期状態で`GameEndPanel`を非表示にする（`SetActive(false)`）

## 動作確認

1. **タイトルシーン**からゲームを開始
2. ゲームモードを選択すると、**ゲームシーン**に遷移することを確認
3. ゲーム終了時に適切なボタンが表示されることを確認：
   - **勝利時**: 「次のステージへ」と「タイトルへ戻る」ボタン
   - **敗北時**: 「もう一度やる」と「タイトルへ戻る」ボタン
4. 各ボタンをクリックして、正しくシーン遷移することを確認

## 注意事項

- **タイトルシーン**には`WordLearningSystem`、`CharacterSelectUI`、`EnemySpawner`は不要です
- **ゲームシーン**には`GameModeSelectUI`が必要ですが、タイトルシーンから遷移した場合のみ使用されます
- `GameSceneManager`は`DontDestroyOnLoad`で実装されているため、シーン遷移後も保持されます
