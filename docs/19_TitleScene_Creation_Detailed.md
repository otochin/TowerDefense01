# タイトルシーン作成の詳細手順（現在のシーンを保持）

## 重要：作業前に必ず現在のシーンを保存

1. **Unity Editor**で`File > Save`（または`Ctrl+S` / `Cmd+S`）を実行
2. 現在のシーン（`SampleScene`）が保存されていることを確認（Hierarchyウィンドウのシーン名に`*`マークがないことを確認）

## 手順1：タイトルシーンの作成

1. **Unity Editor**で`File > New Scene`を選択
2. **Basic (Built-in)**テンプレートを選択して`Create`をクリック
3. 新しいシーンが開きます（空のシーン）
4. **File > Save As**を選択
5. `Assets/Scenes/TitleScene.unity`として保存
   - フォルダが存在しない場合は、`Assets/Scenes`フォルダを先に作成してください

## 手順2：タイトルシーンの基本設定

### 2.1 Canvasの作成

1. **Hierarchy**で`GameObject > UI > Canvas`を選択
2. **Canvas**のInspectorで以下を確認・設定：
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler**コンポーネントが自動追加されていることを確認
   - **UI Scale Mode**: `Scale With Screen Size`
   - **Reference Resolution**: `X: 1920`, `Y: 1080`

### 2.2 EventSystemの確認

- **Canvas**を作成すると、自動的に**EventSystem**が作成されます
- 存在しない場合は、`GameObject > UI > Event System`で作成

## 手順3：GameModeSelectPanelのコピー（現在のシーンから）

### 3.1 現在のシーン（SampleScene）を開く

1. **Project**ウィンドウで`Assets/Scenes/SampleScene.unity`をダブルクリック
2. または、**File > Open Scene**で`SampleScene.unity`を開く
3. **Hierarchy**で`Canvas > GameModeSelectPanel`を確認

### 3.2 GameModeSelectPanelをコピー

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Edit > Copy**（または`Ctrl+C` / `Cmd+C`）を実行
3. **File > Save**で現在のシーンを保存（念のため）

### 3.3 タイトルシーンに貼り付け

1. **Project**ウィンドウで`Assets/Scenes/TitleScene.unity`をダブルクリックしてタイトルシーンを開く
2. **Hierarchy**で`Canvas`を選択
3. **Edit > Paste**（または`Ctrl+V` / `Cmd+V`）を実行
4. `GameModeSelectPanel`が`Canvas`の子として追加されます

### 3.4 GameModeSelectPanelの設定確認

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Inspector**で`GameModeSelectUI`コンポーネントを確認
3. **注意**：タイトルシーンでは以下の参照は不要です（空のままでOK）：
   - **Word Learning System**（空のまま）
   - **Character Select UI**（空のまま）
   - **Enemy Spawner**（空のまま）
4. **ボタンの参照**は設定されていることを確認：
   - **English To Japanese Button**
   - **Japanese To English Button**
   - **Phrase English To Japanese Button**
   - **Phrase Japanese To English Button**

## 手順4：GameSceneManagerの追加（タイトルシーン）

1. **Hierarchy**で`GameObject > Create Empty`を選択
2. 名前を`GameSceneManager`に変更
3. **Inspector**で`Add Component`をクリック
4. `GameSceneManager`を検索して追加
5. **Inspector**で以下を設定：
   - **Title Scene Name**: `TitleScene`
   - **Game Scene Name**: `SampleScene`

## 手順5：Build Settingsの設定

1. **File > Build Settings**を開く
2. **Scenes In Build**セクションを確認
3. 現在のシーン（`SampleScene`）が追加されていることを確認
4. **Add Open Scenes**をクリックして、現在開いているシーンを追加（タイトルシーンの場合）
5. または、**Project**ウィンドウから`Assets/Scenes/TitleScene.unity`をドラッグ＆ドロップ
6. **Scenes In Build**の順序を確認：
   - **インデックス0**: `TitleScene`（最初のシーン）
   - **インデックス1**: `SampleScene`（ゲームシーン）
7. 順序を変更する場合は、ドラッグ＆ドロップで並び替え

## 手順6：ゲームシーンのGameEndHandler設定

### 6.1 ゲームシーン（SampleScene）を開く

1. **Project**ウィンドウで`Assets/Scenes/SampleScene.unity`をダブルクリック

### 6.2 GameEndPanelの作成

1. **Hierarchy**で`Canvas`を選択
2. `GameObject > UI > Panel`を選択して`GameEndPanel`を作成
3. **Inspector**で以下を設定：
   - **Rect Transform**: 画面全体を覆うように設定（Anchor: Stretch Stretch）
   - **Image > Color**: 半透明の黒（例：RGBA(0, 0, 0, 200)）など、適切な背景色
4. 初期状態で非表示にする：**Inspector**の左上のチェックボックスを外す（`SetActive(false)`）

### 6.3 ボタンの作成

1. **GameEndPanel**の子として3つのボタンを作成：
   - `GameObject > UI > Button - TextMeshPro`を3回選択
   - それぞれの名前を変更：
     - `ReturnToTitleButton`（「タイトルへ戻る」）
     - `NextStageButton`（「次のステージへ」）
     - `RetryButton`（「もう一度やる」）

2. 各ボタンの**Text (TMP)**子オブジェクトにテキストを設定：
   - `ReturnToTitleButton > Text (TMP)`: 「タイトルへ戻る」
   - `NextStageButton > Text (TMP)`: 「次のステージへ」
   - `RetryButton > Text (TMP)`: 「もう一度やる」

3. レイアウトの調整：
   - **Vertical Layout Group**を`GameEndPanel`に追加して、ボタンを縦に並べる
   - または、各ボタンの**Rect Transform**を手動で配置

### 6.4 GameEndHandlerの設定

1. **Hierarchy**で`GameEndHandler`を検索（`GameManager`の子にある可能性があります）
2. **Inspector**で`GameEndHandler`コンポーネントを確認
3. 以下を設定：
   - **Game End Panel**: `GameEndPanel`をドラッグ＆ドロップ
   - **Return To Title Button**: `ReturnToTitleButton`をドラッグ＆ドロップ
   - **Next Stage Button**: `NextStageButton`をドラッグ＆ドロップ
   - **Retry Button**: `RetryButton`をドラッグ＆ドロップ

### 6.5 GameModeSelectUIの設定確認（ゲームシーン）

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Inspector**で`GameModeSelectUI`コンポーネントを確認
3. 以下が設定されていることを確認：
   - **Word Learning System**: `WordLearningSystem`をドラッグ＆ドロップ
   - **Character Select UI**: `CharacterSelectUI`をドラッグ＆ドロップ
   - **Enemy Spawner**: `EnemySpawner`をドラッグ＆ドロップ

## 手順7：動作確認

### 7.1 タイトルシーンから開始

1. **File > Build Settings**を開く
2. **TitleScene**を選択して`Ctrl+P`（または`Cmd+P`）で再生
3. または、**Project**ウィンドウで`TitleScene.unity`を開いて再生
4. **GameModeSelectPanel**が表示されることを確認
5. ゲームモードを選択すると、ゲームシーンに遷移することを確認

### 7.2 ゲーム終了時の動作確認

1. ゲームをプレイして、勝利または敗北する
2. **GameEndPanel**が表示されることを確認
3. 勝利時：「次のステージへ」と「タイトルへ戻る」ボタンが表示されることを確認
4. 敗北時：「もう一度やる」と「タイトルへ戻る」ボタンが表示されることを確認
5. 各ボタンをクリックして、正しくシーン遷移することを確認

## トラブルシューティング

### 問題：タイトルシーンからゲームシーンに遷移しない

- **解決策**：
  1. `GameSceneManager`の**Game Scene Name**が`SampleScene`になっているか確認
  2. Build Settingsで`SampleScene`が追加されているか確認
  3. シーン名にスペースや特殊文字が含まれていないか確認

### 問題：ゲームシーンでGameModeSelectUIが動作しない

- **解決策**：
  1. `GameModeSelectUI`の**Word Learning System**などの参照が設定されているか確認
  2. `PlayerPrefs`に`SelectedGameMode`が保存されているか確認（Consoleウィンドウで確認）

### 問題：GameEndPanelが表示されない

- **解決策**：
  1. `GameEndHandler`の**Game End Panel**が設定されているか確認
  2. `GameEndPanel`が初期状態で非表示になっているか確認（`SetActive(false)`）
  3. ゲーム終了時に`ShowGameEndUI()`が呼ばれているか確認（Consoleウィンドウで確認）

## 注意事項

- **現在のシーン（SampleScene）は変更されません**。`GameModeSelectPanel`はコピーしただけなので、元のシーンにも残っています。
- タイトルシーンとゲームシーンの両方に`GameModeSelectPanel`が存在しますが、これは正常です。
- タイトルシーンの`GameModeSelectPanel`はゲームモード選択のみを行い、ゲームシーンに遷移します。
- ゲームシーンの`GameModeSelectPanel`は、タイトルシーンから遷移した場合に自動的にゲームを初期化します。
