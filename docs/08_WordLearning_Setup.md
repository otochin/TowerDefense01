# 英単語学習システム Unity Editorセットアップガイド

## 概要

このガイドでは、Unity Editorで英単語学習システムをセットアップする手順を詳しく説明します。

## 前提条件

- Unity 2022.3 LTS以降
- 以下のスクリプトが実装済みであること：
  - `GameMode.cs`
  - `WordData.cs`
  - `WordLearningSystem.cs`
  - `WordQuizUI.cs`
  - `GameModeSelectUI.cs`
- `Assets/Resources/Data/WordData.csv`が存在すること

## セットアップ手順

### ステップ1: WordLearningSystemのGameObjectを作成

1. **Hierarchyウィンドウで右クリック** → `Create Empty`を選択
2. **GameObjectの名前を`WordLearningSystem`に変更**
3. **Inspectorウィンドウで`Add Component`をクリック**
4. **`WordLearningSystem`コンポーネントを検索して追加**

#### WordLearningSystemの設定

Inspectorで以下の項目を設定：

| 項目 | 値 | 説明 |
|------|-----|------|
| `Current Game Mode` | `EnglishToJapanese` | デフォルトのゲームモード |
| `Word Data Csv File` | (空) | 後で設定 |
| `Word Data Csv Path` | `Data/WordData` | Resourcesフォルダからの相対パス（拡張子なし） |
| `Money Reward On Correct` | `10` | 正解時のPowerの増加量 |
| `Answer Time Limit` | `10` | 回答制限時間（秒） |
| `Correct Answer Display Time` | `5` | 時間切れ時の正解表示時間（秒） |

**注意**: `Word Data Csv File`が設定されている場合は、それが優先されます。設定しない場合は`Word Data Csv Path`が使用されます。

### ステップ2: WordQuizPanelのUI要素を作成

#### 2.1 Canvasの確認

1. **Hierarchyで`Canvas`が存在することを確認**（存在しない場合は`UI` → `Canvas`で作成）

#### 2.2 WordQuizPanelの作成

1. **Hierarchyで`Canvas`を右クリック** → `UI` → `Panel`を選択
2. **Panelの名前を`WordQuizPanel`に変更**
3. **RectTransformを設定**（画面中央に配置）：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `800`
   - `Height`: `600`
   - `Position X`: `0`
   - `Position Y`: `0`

#### 2.3 タイマーテキストの作成

1. **`WordQuizPanel`を右クリック** → `UI` → `Text - TextMeshPro`を選択
   - 初回は「TMP Importer」ダイアログが表示される場合は`Import TMP Essentials`をクリック
2. **GameObjectの名前を`TimerText`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Top Center`
   - `Width`: `200`
   - `Height`: `50`
   - `Position X`: `0`
   - `Position Y`: `-30`
4. **TextMeshProUGUIコンポーネントを設定**：
   - `Text`: `Time: 10.0s`
   - `Font Size`: `24`
   - `Alignment`: `Center`
   - `Color`: 白色

#### 2.4 問題テキストの作成

1. **`WordQuizPanel`を右クリック** → `UI` → `Text - TextMeshPro`を選択
2. **GameObjectの名前を`QuestionText`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `600`
   - `Height`: `100`
   - `Position X`: `0`
   - `Position Y`: `100`
4. **TextMeshProUGUIコンポーネントを設定**：
   - `Text`: `Question`
   - `Font Size`: `36`
   - `Alignment`: `Center`
   - `Color`: 白色

#### 2.5 ChoicesPanelの作成

1. **`WordQuizPanel`を右クリック** → `UI` → `Panel`を選択
2. **GameObjectの名前を`ChoicesPanel`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `600`
   - `Height`: `200`
   - `Position X`: `0`
   - `Position Y`: `-100`
4. **Horizontal Layout Groupコンポーネントを追加**：
   - `Spacing`: `20`
   - `Child Alignment`: `Middle Center`
   - `Child Force Expand`: `Width`を有効化

#### 2.6 選択肢ボタンの作成（3つ）

各ボタンを作成します：

1. **`ChoicesPanel`を右クリック** → `UI` → `Button - TextMeshPro`を選択
2. **GameObjectの名前を`ChoiceButton_1`に変更**
3. **RectTransformを設定**：
   - `Width`: `180`
   - `Height`: `60`
4. **Buttonコンポーネントの設定**：
   - `Interactable`: ✅ チェック
   - `Transition`: `Color Tint`
   - `Normal Color`: 白色
   - `Highlighted Color`: 薄いグレー
   - `Pressed Color`: 濃いグレー
   - `Disabled Color`: グレー
5. **TextMeshProUGUIコンポーネント（子オブジェクト）の設定**：
   - `Text`: `Choice 1`
   - `Font Size`: `20`
   - `Alignment`: `Center`
   - `Color`: 黒色

**同じ手順で`ChoiceButton_2`と`ChoiceButton_3`を作成**

#### 2.7 FeedbackPanelの作成

1. **`WordQuizPanel`を右クリック** → `UI` → `Panel`を選択
2. **GameObjectの名前を`FeedbackPanel`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `400`
   - `Height`: `150`
   - `Position X`: `0`
   - `Position Y`: `-200`
4. **Imageコンポーネントの設定**：
   - `Color`: 半透明（Alpha: 200/255程度）

#### 2.8 FeedbackTextの作成

1. **`FeedbackPanel`を右クリック** → `UI` → `Text - TextMeshPro`を選択
2. **GameObjectの名前を`FeedbackText`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `350`
   - `Height`: `60`
   - `Position X`: `0`
   - `Position Y`: `20`
4. **TextMeshProUGUIコンポーネントを設定**：
   - `Text`: `Good!`
   - `Font Size`: `40`
   - `Alignment`: `Center`
   - `Color`: 緑色（正解用）または赤色（不正解用）

#### 2.9 CorrectAnswerTextの作成

1. **`FeedbackPanel`を右クリック** → `UI` → `Text - TextMeshPro`を選択
2. **GameObjectの名前を`CorrectAnswerText`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `350`
   - `Height`: `60`
   - `Position X`: `0`
   - `Position Y`: `-30`
4. **TextMeshProUGUIコンポーネントを設定**：
   - `Text`: `Correct Answer: ...`
   - `Font Size`: `24`
   - `Alignment`: `Center`
   - `Color`: 黄色

#### 2.10 WordQuizUIコンポーネントの設定

1. **`WordQuizPanel`に`WordQuizUI`コンポーネントを追加**
2. **Inspectorで以下の参照を設定**：

| 項目 | 参照先 |
|------|--------|
| `Timer Text` | `TimerText` (子オブジェクト) |
| `Question Text` | `QuestionText` (子オブジェクト) |
| `Choice Buttons` | `ChoiceButton_1`, `ChoiceButton_2`, `ChoiceButton_3` (子オブジェクト) |
| `Feedback Text` | `FeedbackText` (子オブジェクト) |
| `Correct Answer Text` | `CorrectAnswerText` (子オブジェクト) |
| `Feedback Panel` | `FeedbackPanel` (子オブジェクト) |

**注意**: `Choice Buttons`は配列なので、Sizeを`3`に設定してから各要素にボタンをドラッグ&ドロップします。

### ステップ3: GameModeSelectPanelのUI要素を作成

#### 3.1 GameModeSelectPanelの作成

1. **Hierarchyで`Canvas`を右クリック** → `UI` → `Panel`を選択
2. **Panelの名前を`GameModeSelectPanel`に変更**
3. **RectTransformを設定**（画面中央に配置）：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `600`
   - `Height`: `400`
   - `Position X`: `0`
   - `Position Y`: `0`

#### 3.2 タイトルテキストの作成

1. **`GameModeSelectPanel`を右クリック** → `UI` → `Text - TextMeshPro`を選択
2. **GameObjectの名前を`TitleText`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Top Center`
   - `Width`: `500`
   - `Height`: `80`
   - `Position X`: `0`
   - `Position Y`: `-30`
4. **TextMeshProUGUIコンポーネントを設定**：
   - `Text`: `Select Game Mode`
   - `Font Size`: `40`
   - `Alignment`: `Center`
   - `Color`: 白色

#### 3.3 モード選択ボタンの作成（2つ）

##### EnglishToJapaneseButtonの作成

1. **`GameModeSelectPanel`を右クリック** → `UI` → `Button - TextMeshPro`を選択
2. **GameObjectの名前を`EnglishToJapaneseButton`に変更**
3. **RectTransformを設定**：
   - `Anchor Presets`: `Middle Center`
   - `Width`: `400`
   - `Height`: `80`
   - `Position X`: `0`
   - `Position Y`: `50`
4. **Buttonコンポーネントの設定**：
   - `Interactable`: ✅ チェック
   - `Transition`: `Color Tint`
5. **TextMeshProUGUIコンポーネント（子オブジェクト）の設定**：
   - `Text`: `English to Japanese`
   - `Font Size`: `28`
   - `Alignment`: `Center`
   - `Color`: 黒色

##### JapaneseToEnglishButtonの作成

1. **同じ手順で`JapaneseToEnglishButton`を作成**
2. **RectTransformの`Position Y`を`-50`に設定**
3. **TextMeshProUGUIコンポーネントの`Text`を`Japanese to English`に設定**

#### 3.4 GameModeSelectUIコンポーネントの設定

1. **`GameModeSelectPanel`に`GameModeSelectUI`コンポーネントを追加**
2. **Inspectorで以下の参照を設定**：

| 項目 | 参照先 |
|------|--------|
| `English To Japanese Button` | `EnglishToJapaneseButton` (子オブジェクト) |
| `Japanese To English Button` | `JapaneseToEnglishButton` (子オブジェクト) |
| `Mode Select Panel` | `GameModeSelectPanel` (自分自身) |

### ステップ4: ResourceManagerの設定

1. **Hierarchyで`ResourceManager`を選択**（または作成）
2. **Inspectorで`ResourceManager`コンポーネントを確認**
3. **`Enable Auto Money Generation`のチェックを外す**（英単語学習機能では自動増加を無効化、Powerは英単語問題に正解することで獲得）

### ステップ5: 動作確認

#### 5.1 実行前の確認項目

- [ ] `WordLearningSystem`が存在し、コンポーネントがアタッチされている
- [ ] `WordQuizPanel`が存在し、`WordQuizUI`コンポーネントが正しく設定されている
- [ ] `GameModeSelectPanel`が存在し、`GameModeSelectUI`コンポーネントが正しく設定されている
- [ ] `ResourceManager`の`Enable Auto Money Generation`が無効になっている
- [ ] `WordData.csv`が`Assets/Resources/Data/`フォルダに存在する

#### 5.2 実行して確認

1. **Playボタンをクリック**
2. **以下を確認**：
   - ゲーム開始時に`GameModeSelectPanel`が表示される
   - モード選択ボタンをクリックすると`GameModeSelectPanel`が非表示になる
   - `WordQuizPanel`に問題と選択肢が表示される
   - タイマーがカウントダウンする（10秒から）
   - 正解を選択すると「Good! Get X Power!」が表示され、Powerが増える
   - 不正解を選択すると「Bad!」が表示される
   - 時間切れになると「Bad!」と正解が表示される（5秒間）

### ステップ6: UIのレイアウト調整（オプション）

実際のゲーム画面に合わせて、以下の調整を行ってください：

- **WordQuizPanelの位置とサイズ**
- **フォントサイズ**
- **色の調整**
- **アニメーションの追加**（オプション）

## トラブルシューティング

### CSVファイルが読み込まれない

**原因**: パスが間違っている、またはCSVファイルが正しい場所にない

**解決方法**:
1. `Assets/Resources/Data/WordData.csv`が存在することを確認
2. `WordLearningSystem`の`Word Data Csv Path`が`Data/WordData`（拡張子なし）であることを確認
3. CSVファイルの形式が正しいか確認（`English,Japanese`形式）

### UI要素が表示されない

**原因**: UI要素の参照が設定されていない

**解決方法**:
1. `WordQuizUI`コンポーネントの各参照フィールドが正しく設定されているか確認
2. Hierarchyで対応するGameObjectが存在するか確認

### ボタンがクリックできない

**原因**: EventSystemが存在しない

**解決方法**:
1. Hierarchyで`EventSystem`が存在することを確認（存在しない場合は自動的に作成される）
2. Canvasの`GraphicRaycaster`コンポーネントが有効になっているか確認

### Powerが増えない

**原因**: ResourceManagerの設定が間違っている

**解決方法**:
1. `WordLearningSystem`の`Money Reward On Correct`が`0`より大きい値になっているか確認
2. `ResourceManager`が正しく動作しているか確認（シングルトンインスタンスが存在するか）

## 次のステップ

セットアップが完了したら、以下を実施してください：

1. 動作確認とテスト
2. UIデザインの改善
3. アニメーションの追加（オプション）
4. 英単語データの追加（CSVファイルに単語を追加）

---

## 最終更新日

2026年1月17日

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-17 | 初版作成 | - |
