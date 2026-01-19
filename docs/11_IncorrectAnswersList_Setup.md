# 間違えた問題リストUIセットアップ手順

## 概要

ゲーム終了時に表示される「間違えた問題リスト」のUIセットアップ手順を詳しく説明します。

## 必要な前提条件

- Unity Editorが起動している
- ゲームシーンが開いている
- Canvasが存在している

---

## ステップ1: リストアイテムプレハブの作成

> **注意**: このステップでは、一時的にHierarchyにプレハブの元となるGameObjectを作成します。後でプレハブ化するため、一時的な場所に作成します。

### ステップ1-1: 一時的なGameObjectを作成

1. **Hierarchyウィンドウ**で、`SampleScene`（またはルート）を右クリック
2. **Create Empty**を選択
   - 名前を`IncorrectAnswerListItem_Temp`に変更（一時的）

### ステップ1-2: レイアウトを設定

1. **Inspectorウィンドウ**で`IncorrectAnswerListItem_Temp`を選択
2. **Rect Transform**コンポーネントがない場合は、**Add Component > Layout > Rect Transform**を追加
3. **Rect Transform**の設定:
   - **Width**: `400`（幅）
   - **Height**: `60`（高さ）
   - **Anchor Presets**: 左クリック長押しで`Top Left`を選択

### ステップ1-3: リストアイテムの構造を作成

1. `IncorrectAnswerListItem_Temp`を右クリック → **Create Empty**
   - 名前を`Content`に変更

2. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`EnglishText`に変更
   - **Text**: `English Text`（仮のテキスト）
   - **Font Size**: `18`
   - **Alignment**: 左揃え

3. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`JapaneseText`に変更
   - **Text**: `Japanese Text`（仮のテキスト）
   - **Font Size**: `16`
   - **Alignment**: 左揃え

4. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`CountText`に変更
   - **Text**: `×3回`（仮のテキスト）
   - **Font Size**: `16`
   - **Alignment**: 右揃え
   - **Color**: 赤色（例: `#FF0000`）

### ステップ1-4: レイアウトグループを追加

1. `Content`を選択
2. **Inspectorウィンドウ**で**Add Component**
3. **Layout > Vertical Layout Group**を追加
4. **Vertical Layout Group**の設定:
   - **Child Alignment**: `Upper Left`
   - **Child Control Size**:
     - ✅ **Width**をチェック
     - ✅ **Height**をチェック
   - **Child Force Expand**:
     - ❌ **Width**のチェックを外す
     - ❌ **Height**のチェックを外す
   - **Spacing**: `5`（要素間のスペース）

5. **Content Size Fitter**を追加（`Content`に）
   - **Horizontal Fit**: `Preferred Size`
   - **Vertical Fit**: `Preferred Size`

### ステップ1-5: 背景を追加（オプション）

1. `IncorrectAnswerListItem_Temp`を選択
2. **Inspectorウィンドウ**で**Add Component**
3. **UI > Image**を追加
4. **Image**の設定:
   - **Color**: 半透明のグレー（例: `#80808080`）

### ステップ1-6: プレハブ化

1. **Projectウィンドウ**で、`Assets/Prefabs/UI/`フォルダを開く
   - 既に存在することを確認（他のプレハブがあるフォルダ）

2. **Hierarchyウィンドウ**から`IncorrectAnswerListItem_Temp`を**Projectウィンドウ**の`Assets/Prefabs/UI/`フォルダにドラッグ＆ドロップ
   - プレハブ化されると、名前が`IncorrectAnswerListItem`に変わります（青く表示）

3. **Hierarchyウィンドウ**から元の`IncorrectAnswerListItem_Temp`を削除
   - プレハブはProjectに保存されているため、削除しても問題ありません

---

## ステップ2: リストパネルの作成

> **注意**: `Canvas`の子要素として、他のパネル（`GameModeSelectPanel`、`WordQuizPanel`など）と同じ階層に作成します。

### ステップ2-1: パネルのベースを作成

1. **Hierarchyウィンドウ**で、`Canvas`を右クリック
2. **UI > Panel**を選択
   - 名前を`IncorrectAnswersListPanel`に変更
   - `GameModeSelectPanel`や`WordQuizPanel`と同じ階層に配置されます

### ステップ2-2: パネルの位置とサイズを設定

1. `IncorrectAnswersListPanel`を選択
2. **Rect Transform**コンポーネントの設定:
   - **Anchor Presets**: `Center`（中央）または`Bottom Center`（下部中央）
   - **Pos X**: `0`
   - **Pos Y**: `-150`（画面下部寄り、他のパネルと重ならない位置）
   - **Width**: `600`
   - **Height**: `400`
   - **Pivot**: `X: 0.5, Y: 1.0`（上部中央）

### ステップ2-3: タイトルテキストを追加

1. `IncorrectAnswersListPanel`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`TitleText`に変更

2. **Rect Transform**の設定:
   - **Anchor Presets**: `Top Stretch`（上部いっぱい）
   - **Top**: `0`
   - **Height**: `40`

3. **TextMeshProUGUI**の設定:
   - **Text**: `間違えた問題リスト`
   - **Font Size**: `24`
   - **Alignment**: 中央揃え
   - **Color**: 白色（例: `#FFFFFF`）

### ステップ2-4: スクロールビューを作成

1. `IncorrectAnswersListPanel`を右クリック → **UI > Scroll View**
   - 名前を`ScrollView`に変更

2. **Rect Transform**の設定:
   - **Anchor Presets**: `Stretch Stretch`（親いっぱいに広がる）
   - **Left**: `10`
   - **Right**: `-10`
   - **Top**: `-50`（タイトルの下）
   - **Bottom**: `10`

3. Unityが自動で作成した不要な要素を削除:
   - `ScrollView`の子要素の中から以下を確認:
     - `Viewport`（これは残す）
     - `Scrollbar Horizontal`（削除）
     - `Scrollbar Vertical`（削除または非表示にする）

### ステップ2-5: ViewportとContentの設定

1. `ScrollView/Viewport`を選択
2. **Rect Transform**の設定:
   - **Anchor Presets**: `Stretch Stretch`
   - すべてのオフセットを`0`に

3. `ScrollView/Viewport/Content`を選択
   - 名前を`Content`に変更（もしまだContentという名前でない場合）

4. **Content**に**Vertical Layout Group**を追加:
   - **Child Alignment**: `Upper Left`
   - **Child Control Size**:
     - ❌ **Width**のチェックを外す
     - ✅ **Height**をチェック
   - **Child Force Expand**:
     - ❌ **Width**のチェックを外す
     - ❌ **Height**のチェックを外す
   - **Spacing**: `5`
   - **Padding**: `Left: 5, Right: 5, Top: 5, Bottom: 5`

5. **Content Size Fitter**を追加:
   - **Horizontal Fit**: `Preferred Size`
   - **Vertical Fit**: `Preferred Size`

### ステップ2-6: スクロールバーの設定（オプション）

1. `ScrollView/Scrollbar Vertical`がある場合:
   - **Rect Transform**の**Width**を`20`に設定
   - 必要に応じて位置を調整

---

## ステップ3: IncorrectAnswersListUIコンポーネントの設定

### ステップ3-1: コンポーネントを追加

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel`を選択
2. **Inspectorウィンドウ**で**Add Component**をクリック
3. 検索欄に`Incorrect`と入力して**Incorrect Answers List UI**を選択
   - または、スクリプトの名前で検索

### ステップ3-2: 参照を設定

1. **Inspectorウィンドウ**で**Incorrect Answers List UI**コンポーネントを確認

2. 各フィールドにオブジェクトをドラッグ＆ドロップ（Hierarchyウィンドウから）:
   - **List Panel**: `IncorrectAnswersListPanel`自身をドラッグ
   - **Title Text**: `TitleText`（`IncorrectAnswersListPanel`の子）をドラッグ
   - **Scroll Rect**: `ScrollView`（`IncorrectAnswersListPanel`の子）を選択して、**Scroll Rect**コンポーネントをドラッグ
   - **Content Parent**: `ScrollView/Viewport/Content`をドラッグ
   - **ListItem Prefab**: **Projectウィンドウ**の`Assets/Prefabs/UI/IncorrectAnswerListItem`プレハブをドラッグ

3. **設定**セクション:
   - **Title Format**: `間違えた問題リスト ({0}件)`（デフォルトのまま）
   - **Hide If Empty**: ✅ チェックを入れる（間違いがない場合は非表示）

### ステップ3-3: 初期状態を非表示に設定

1. **Hierarchyウィンドウ**で`IncorrectAnswersListPanel`を選択
2. **Inspectorウィンドウ**の上部、**Active**のチェックを外す
   - これで初期状態では非表示になり、ゲーム終了時に自動表示されます
   - 他のパネル（`GameModeSelectPanel`など）と同じように、初期状態では非表示にしておきます

---

## ステップ4: GameEndHandlerの設定

### ステップ4-1: GameEndHandlerを確認

1. **Hierarchyウィンドウ**で`GameManager`を探す
   - 画像では`GameManager`がルートにあります
   - `GameEndHandler`は`GameManager`の子にあるか、または別の場所にある可能性があります

2. **Hierarchyウィンドウ**で`GameEndHandler`という名前のGameObjectを検索:
   - **Hierarchyウィンドウ**の上部の検索欄に`GameEndHandler`と入力
   - 見つからない場合は、**GameObject > Create Empty**で作成し、名前を`GameEndHandler`に変更

3. `GameEndHandler`に**Game End Handler**コンポーネントがあることを確認
   - ない場合は**Add Component > Game End Handler**を追加

### ステップ4-2: IncorrectAnswersListUIの参照を設定

1. `GameEndHandler`を選択
2. **Inspectorウィンドウ**で**Game End Handler**コンポーネントを確認

3. **参照**セクションの**Incorrect Answers List UI**フィールドに:
   - `IncorrectAnswersListPanel`（Hierarchyの`Canvas`の子）をドラッグ＆ドロップ
   - または、フィールドの右側の**ターゲットアイコン**（丸いアイコン）をクリックして、`IncorrectAnswersListPanel`を選択

---

## ステップ5: 動作確認

### ステップ5-1: テスト実行

1. **Playボタン**（▶）をクリックしてゲームを開始

2. ゲーム中に**意図的に間違えた問題を作る**:
   - 英単語問題で間違えた選択肢を選ぶ
   - 複数回間違えると、回数がカウントされる

3. ゲーム終了（城が破壊される）を待つ

4. **間違えた問題リスト**が表示されることを確認:
   - 間違えた回数の多い順に並んでいる
   - 英語/英熟語、日本語、間違えた回数が表示されている
   - スクロールできる（項目が多い場合）

### ステップ5-2: エラーがある場合の確認

1. **Consoleウィンドウ**（Window > General > Console）を開く

2. エラーメッセージを確認:
   - `IncorrectAnswersListUI`が`WordLearningSystem`を見つけられない場合
   - 参照が設定されていない場合
   - プレハブが設定されていない場合

---

## トラブルシューティング

### 問題1: リストが表示されない

**原因**: パネルが非表示になっている、または参照が設定されていない

**解決方法**:
1. Hierarchyで`IncorrectAnswersListPanel`の**Active**チェックを確認
2. `GameEndHandler`の**Incorrect Answers List UI**参照を確認
3. `IncorrectAnswersListUI`コンポーネントの各参照を確認

### 問題2: リストアイテムが表示されない

**原因**: プレハブが設定されていない、または`Content`の参照が間違っている

**解決方法**:
1. `IncorrectAnswersListUI`の**ListItem Prefab**にプレハブが設定されているか確認
2. **Content Parent**が`ScrollView/Viewport/Content`を指しているか確認

### 問題3: スクロールできない

**原因**: ScrollRectの設定が正しくない、またはContentにVertical Layout Groupがない

**解決方法**:
1. `ScrollView`の**Scroll Rect**コンポーネントの**Content**参照を確認
2. `Content`に**Vertical Layout Group**が追加されているか確認

### 問題4: レイアウトが崩れている

**原因**: Rect Transformの設定やLayout Groupの設定が正しくない

**解決方法**:
1. `Content`の**Vertical Layout Group**の設定を確認
2. `Content`に**Content Size Fitter**が追加されているか確認
3. 各リストアイテムの**Rect Transform**のサイズを確認

---

## 完成例

完成したHierarchy構造:

```
Canvas
└── IncorrectAnswersListPanel (GameObject + IncorrectAnswersListUI)
    ├── TitleText (TextMeshProUGUI)
    └── ScrollView (ScrollRect)
        └── Viewport
            └── Content (Vertical Layout Group + Content Size Fitter)
                └── (リストアイテムが動的に生成される)
```

---

## 補足: カスタマイズ

### スタイルのカスタマイズ

- **背景色**: `IncorrectAnswersListPanel`の**Image**コンポーネントの**Color**を変更
- **テキスト色**: 各TextMeshProUGUIの**Color**を変更
- **フォントサイズ**: 各TextMeshProUGUIの**Font Size**を変更

### レイアウトのカスタマイズ

- **パネルサイズ**: `IncorrectAnswersListPanel`の**Rect Transform**の**Width/Height**を変更
- **アイテム間隔**: `Content`の**Vertical Layout Group**の**Spacing**を変更

---

これで設定は完了です。ゲームを実行して動作を確認してください！
