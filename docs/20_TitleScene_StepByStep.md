# タイトルシーン作成後の詳細ステップガイド

## 前提条件

- ✅ タイトルシーン（`TitleScene.unity`）を作成済み
- ✅ 現在のゲームシーン（`SampleScene.unity`）は保存済み

---

## ステップ1：タイトルシーンの基本UI設定

### 1.1 Canvasの作成と設定

1. **Hierarchy**ウィンドウで右クリック
2. **UI > Canvas**を選択
3. **Canvas**が作成されたら、**Inspector**で以下を確認・設定：

   **Canvas コンポーネント：**
   - **Render Mode**: `Screen Space - Overlay`（デフォルト）
   - **Pixel Perfect**: チェックを外してもOK（必要に応じて）

   **Canvas Scaler コンポーネント（自動追加）：**
   - **UI Scale Mode**: `Scale With Screen Size`を選択
   - **Reference Resolution**: 
     - **X**: `1920`
     - **Y**: `1080`
   - **Screen Match Mode**: `Match Width Or Height`
   - **Match**: `0.5`（デフォルト）

   **Graphic Raycaster コンポーネント（自動追加）：**
   - そのままでOK

### 1.2 EventSystemの確認

- **Canvas**を作成すると、自動的に**EventSystem**が作成されます
- **Hierarchy**に`EventSystem`が表示されていることを確認
- 存在しない場合は、**Hierarchy**で右クリック → **UI > Event System**で作成

---

## ステップ2：GameModeSelectPanelのコピー

### 2.1 ゲームシーン（SampleScene）を開く

1. **Project**ウィンドウで`Assets/Scenes/SampleScene.unity`をダブルクリック
2. または、**File > Open Scene**で`SampleScene.unity`を選択
3. **Hierarchy**で`Canvas > GameModeSelectPanel`が存在することを確認

### 2.2 GameModeSelectPanelをコピー

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Edit > Copy**（または`Ctrl+C` / `Cmd+C`）を実行
3. **File > Save**で現在のシーンを保存（念のため）

### 2.3 タイトルシーンに貼り付け

1. **Project**ウィンドウで`Assets/Scenes/TitleScene.unity`をダブルクリック
2. **Hierarchy**で`Canvas`を選択（選択状態にする）
3. **Edit > Paste**（または`Ctrl+V` / `Cmd+V`）を実行
4. `GameModeSelectPanel`が`Canvas`の子として追加されたことを確認

### 2.4 GameModeSelectPanelの位置とサイズを調整

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Inspector**で**Rect Transform**を確認：
   - **Anchor Presets**: 中央に配置（`Alt+Shift+Middle`で中央配置）
   - **Pos X**: `0`
   - **Pos Y**: `0`
   - **Width**: `800`（適切なサイズに調整）
   - **Height**: `600`（適切なサイズに調整）

---

## ステップ3：GameModeSelectUIコンポーネントの設定

### 3.1 GameModeSelectUIコンポーネントの確認

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Inspector**で`GameModeSelectUI`コンポーネントを確認
3. コンポーネントが存在しない場合は、**Add Component** → `GameModeSelectUI`を検索して追加

### 3.2 ボタンの参照を設定

**GameModeSelectPanel**の子要素に4つのボタンがあることを確認：

1. **Hierarchy**で`GameModeSelectPanel`を展開して、以下のボタンが存在するか確認：
   - `EnglishToJapaneseButton`（または類似の名前）
   - `JapaneseToEnglishButton`（または類似の名前）
   - `PhraseEnglishToJapaneseButton`（または類似の名前）
   - `PhraseJapaneseToEnglishButton`（または類似の名前）

2. **Inspector**で`GameModeSelectUI`コンポーネントの各ボタンフィールドに、対応するボタンをドラッグ＆ドロップ：
   - **English To Japanese Button**: `EnglishToJapaneseButton`をドラッグ
   - **Japanese To English Button**: `JapaneseToEnglishButton`をドラッグ
   - **Phrase English To Japanese Button**: `PhraseEnglishToJapaneseButton`をドラッグ
   - **Phrase Japanese To English Button**: `PhraseJapaneseToEnglishButton`をドラッグ

### 3.3 タイトルシーンでは不要な参照について

**注意**：タイトルシーンでは以下の参照は**空のままでOK**です：
- **Word Learning System**: （空のまま）
- **Character Select UI**: （空のまま）
- **Enemy Spawner**: （空のまま）

これらはゲームシーンに遷移した後に設定されます。

---

## ステップ4：GameSceneManagerの追加

### 4.1 GameSceneManagerオブジェクトの作成

1. **Hierarchy**で右クリック
2. **Create Empty**を選択
3. 作成された`GameObject`を選択して、**Inspector**の名前を`GameSceneManager`に変更

### 4.2 GameSceneManagerコンポーネントの追加

1. **Inspector**で**Add Component**をクリック
2. `GameSceneManager`を検索して選択
3. コンポーネントが追加されたことを確認

### 4.3 GameSceneManagerの設定

**Inspector**で`GameSceneManager`コンポーネントの以下を設定：

- **Title Scene Name**: `TitleScene`（タイトルシーンの名前）
- **Game Scene Name**: `SampleScene`（ゲームシーンの名前）

---

## ステップ5：Build Settingsの設定

### 5.1 Build Settingsウィンドウを開く

1. **File > Build Settings**を選択
2. **Build Settings**ウィンドウが開きます

### 5.2 シーンを追加

**方法1：Add Open Scenesを使用**
1. タイトルシーン（`TitleScene.unity`）が開いていることを確認
2. **Add Open Scenes**ボタンをクリック
3. `TitleScene`が**Scenes In Build**に追加されます

**方法2：ドラッグ＆ドロップ**
1. **Project**ウィンドウで`Assets/Scenes/TitleScene.unity`を選択
2. **Build Settings**ウィンドウの**Scenes In Build**セクションにドラッグ＆ドロップ

### 5.3 ゲームシーンも追加

1. **Project**ウィンドウで`Assets/Scenes/SampleScene.unity`を選択
2. **Build Settings**ウィンドウの**Scenes In Build**セクションにドラッグ＆ドロップ

### 5.4 シーンの順序を確認・調整

**Scenes In Build**の順序が以下になっていることを確認：

- **インデックス0**: `TitleScene`（最初のシーン）
- **インデックス1**: `SampleScene`（ゲームシーン）

順序を変更する場合は、シーンをドラッグ＆ドロップして並び替えます。

### 5.5 Build Settingsを閉じる

- **Build Settings**ウィンドウを閉じます（右上の×ボタン）

---

## ステップ6：タイトルシーンの保存

1. **File > Save**（または`Ctrl+S` / `Cmd+S`）を実行
2. **Hierarchy**のシーン名に`*`マークがないことを確認（保存完了）

---

## ステップ7：ゲームシーンのGameEndHandler設定

### 7.1 ゲームシーン（SampleScene）を開く

1. **Project**ウィンドウで`Assets/Scenes/SampleScene.unity`をダブルクリック

### 7.2 GameEndPanelの作成

1. **Hierarchy**で`Canvas`を選択
2. 右クリック → **UI > Panel**を選択
3. 作成された`Panel`の名前を`GameEndPanel`に変更

### 7.3 GameEndPanelの設定

1. **Hierarchy**で`Canvas > GameEndPanel`を選択
2. **Inspector**で**Rect Transform**を設定：
   - **Anchor Presets**: `Stretch Stretch`を選択（`Shift+Alt+Click`で設定）
   - **Left**: `0`
   - **Right**: `0`
   - **Top**: `0`
   - **Bottom**: `0`
   - これで画面全体を覆うようになります

3. **Image**コンポーネントの設定：
   - **Color**: 半透明の黒（例：RGBA `0, 0, 0, 200`）など、適切な背景色に設定
   - **Raycast Target**: チェックを入れる（ボタンクリックを受け取るため）

4. **初期状態で非表示にする**：
   - **Inspector**の左上のチェックボックスを**外す**（`SetActive(false)`）

### 7.4 ボタンの作成

#### 7.4.1 「タイトルへ戻る」ボタン

1. **Hierarchy**で`Canvas > GameEndPanel`を選択
2. 右クリック → **UI > Button - TextMeshPro**を選択
3. 作成された`Button`の名前を`ReturnToTitleButton`に変更
4. **Hierarchy**で`ReturnToTitleButton > Text (TMP)`を選択
5. **Inspector**で**TextMeshProUGUI**コンポーネントの**Text**に「タイトルへ戻る」と入力

#### 7.4.2 「次のステージへ」ボタン

1. **Hierarchy**で`Canvas > GameEndPanel`を選択
2. 右クリック → **UI > Button - TextMeshPro**を選択
3. 作成された`Button`の名前を`NextStageButton`に変更
4. **Hierarchy**で`NextStageButton > Text (TMP)`を選択
5. **Inspector**で**TextMeshProUGUI**コンポーネントの**Text**に「次のステージへ」と入力

#### 7.4.3 「もう一度やる」ボタン

1. **Hierarchy**で`Canvas > GameEndPanel`を選択
2. 右クリック → **UI > Button - TextMeshPro**を選択
3. 作成された`Button`の名前を`RetryButton`に変更
4. **Hierarchy**で`RetryButton > Text (TMP)`を選択
5. **Inspector**で**TextMeshProUGUI**コンポーネントの**Text**に「もう一度やる」と入力

### 7.5 ボタンのレイアウト調整

#### 方法1：Vertical Layout Groupを使用（推奨）

1. **Hierarchy**で`Canvas > GameEndPanel`を選択
2. **Inspector**で**Add Component**をクリック
3. `Vertical Layout Group`を検索して追加
4. **Vertical Layout Group**コンポーネントの設定：
   - **Padding**: 
     - **Left**: `20`
     - **Right**: `20`
     - **Top**: `20`
     - **Bottom**: `20`
   - **Spacing**: `20`（ボタン間の間隔）
   - **Child Alignment**: `Middle Center`
   - **Child Force Expand**:
     - **Width**: チェックを外す
     - **Height**: チェックを外す

5. 各ボタンの**Rect Transform**を調整：
   - **Width**: `300`
   - **Height**: `60`

#### 方法2：手動で配置

1. 各ボタンの**Rect Transform**を手動で調整：
   - **Anchor Presets**: 中央に配置
   - **Pos X**: `0`
   - **Pos Y**: それぞれ異なる値（例：`100`, `0`, `-100`）
   - **Width**: `300`
   - **Height**: `60`

### 7.6 GameEndHandlerの設定

1. **Hierarchy**で`GameEndHandler`を検索
   - `GameManager`の子にある可能性があります
   - または、シーンのルートに存在する可能性があります

2. **Inspector**で`GameEndHandler`コンポーネントを確認

3. 以下を設定（各フィールドに対応するオブジェクトをドラッグ＆ドロップ）：
   - **Game End Panel**: `GameEndPanel`をドラッグ
   - **Return To Title Button**: `ReturnToTitleButton`をドラッグ
   - **Next Stage Button**: `NextStageButton`をドラッグ
   - **Retry Button**: `RetryButton`をドラッグ

### 7.7 GameModeSelectUIの設定確認（ゲームシーン）

1. **Hierarchy**で`Canvas > GameModeSelectPanel`を選択
2. **Inspector**で`GameModeSelectUI`コンポーネントを確認
3. 以下が設定されていることを確認：
   - **Word Learning System**: `WordLearningSystem`をドラッグ＆ドロップ
   - **Character Select UI**: `CharacterSelectUI`をドラッグ＆ドロップ
   - **Enemy Spawner**: `EnemySpawner`をドラッグ＆ドロップ

### 7.8 ゲームシーンの保存

1. **File > Save**（または`Ctrl+S` / `Cmd+S`）を実行

---

## ステップ8：動作確認

### 8.1 タイトルシーンから開始

1. **Project**ウィンドウで`Assets/Scenes/TitleScene.unity`をダブルクリック
2. **Play**ボタン（▶）をクリックして再生
3. **Game**ビューで`GameModeSelectPanel`が表示されることを確認
4. ゲームモードを選択すると、ゲームシーンに遷移することを確認

### 8.2 ゲーム終了時の動作確認

1. ゲームをプレイして、勝利または敗北する
2. **GameEndPanel**が表示されることを確認
3. **勝利時**：
   - 「次のステージへ」ボタンが表示される
   - 「タイトルへ戻る」ボタンが表示される
   - 「もう一度やる」ボタンは非表示
4. **敗北時**：
   - 「もう一度やる」ボタンが表示される
   - 「タイトルへ戻る」ボタンが表示される
   - 「次のステージへ」ボタンは非表示
5. 各ボタンをクリックして、正しくシーン遷移することを確認

---

## トラブルシューティング

### 問題：タイトルシーンからゲームシーンに遷移しない

**確認事項：**
1. `GameSceneManager`の**Game Scene Name**が`SampleScene`になっているか
2. Build Settingsで`SampleScene`が追加されているか
3. シーン名にスペースや特殊文字が含まれていないか

**解決策：**
- `GameSceneManager`の**Game Scene Name**を確認し、正確なシーン名を入力

### 問題：ゲームシーンでGameModeSelectUIが動作しない

**確認事項：**
1. `GameModeSelectUI`の**Word Learning System**などの参照が設定されているか
2. Consoleウィンドウにエラーメッセージが表示されていないか

**解決策：**
- `GameModeSelectUI`の各参照を確認し、正しく設定されているか確認

### 問題：GameEndPanelが表示されない

**確認事項：**
1. `GameEndHandler`の**Game End Panel**が設定されているか
2. `GameEndPanel`が初期状態で非表示になっているか（`SetActive(false)`）
3. Consoleウィンドウにエラーメッセージが表示されていないか

**解決策：**
- `GameEndHandler`の各参照を確認し、正しく設定されているか確認

---

## 完了

これでタイトルシーンのセットアップが完了しました！

**次のステップ：**
- タイトルシーンのUIデザインを調整
- ボタンのスタイルやレイアウトを調整
- タイトルロゴや背景画像を追加（オプション）
