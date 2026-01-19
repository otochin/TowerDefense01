# IncorrectAnswerListItemプレハブ修正ガイド

## 問題の原因

現在のプレハブ構造に以下の問題があります：

1. **Canvasが子要素にある**: リストアイテムの中にCanvasがあると、レイアウトシステムが正しく動作しません
2. **二重のIncorrectAnswerListItem**: rootとchildの両方に同じ名前がある
3. **Imageの配置**: ImageがCanvasの子になっている

## 正しい構造

```
IncorrectAnswerListItem (root)
├── Image (背景)
└── Content
    ├── EnglishText
    ├── JapaneseText
    └── CountText
```

## 修正手順

### ステップ1: プレハブを開く

1. **Projectウィンドウ**で`Assets/Prefabs/UI/IncorrectAnswerListItem.prefab`をダブルクリック
2. プレハブ編集モードに入る

### ステップ2: 不要な要素を削除

1. **Hierarchyウィンドウ**で`IncorrectAnswerListItem`（child）を選択
2. **Delete**キーを押して削除
   - これで`Canvas`とその子要素が削除されます

### ステップ3: 正しい構造を作成

#### ステップ3-1: Imageを追加（背景）

1. **Hierarchyウィンドウ**で`IncorrectAnswerListItem`（root）を選択
2. **Inspectorウィンドウ**で**Add Component**
3. **UI > Image**を追加
4. **Image**の設定:
   - **Color**: 半透明のグレー（例: `#80808080`）
   - **Raycast Target**: ✅ チェックを外す（クリックイベントを受け取らないようにする）

#### ステップ3-2: Contentを作成

1. `IncorrectAnswerListItem`（root）を右クリック → **Create Empty**
   - 名前を`Content`に変更

2. **Rect Transform**の設定:
   - **Anchor Presets**: `Stretch Stretch`（親いっぱいに広がる）
   - **Left**: `5`
   - **Right**: `-5`
   - **Top**: `-5`
   - **Bottom**: `5`
   - これで、ContentがImageの内側に配置されます

#### ステップ3-3: テキスト要素を作成

1. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`EnglishText`に変更
   - **Text**: `English Text`（仮のテキスト）
   - **Font Size**: `18`
   - **Alignment**: 左揃え

2. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`JapaneseText`に変更
   - **Text**: `Japanese Text`（仮のテキスト）
   - **Font Size**: `16`
   - **Alignment**: 左揃え

3. `Content`を右クリック → **UI > Text - TextMeshPro**
   - 名前を`CountText`に変更
   - **Text**: `×3回`（仮のテキスト）
   - **Font Size**: `16`
   - **Alignment**: 右揃え
   - **Color**: 赤色（例: `#FF0000`）

#### ステップ3-4: Vertical Layout Groupを追加

1. `Content`を選択
2. **Inspectorウィンドウ**で**Add Component**
3. **Layout > Vertical Layout Group**を追加
4. **Vertical Layout Group**の設定:
   - **Child Alignment**: `Upper Left`
   - **Child Control Size**:
     - ❌ **Width**のチェックを外す
     - ✅ **Height**をチェック
   - **Child Force Expand**:
     - ❌ **Width**のチェックを外す
     - ❌ **Height**のチェックを外す
   - **Spacing**: `5`（要素間のスペース）
   - **Padding**: `Left: 5, Right: 5, Top: 5, Bottom: 5`

5. **Content Size Fitter**を追加（`Content`に）
   - **Horizontal Fit**: `Preferred Size`
   - **Vertical Fit**: `Preferred Size`

#### ステップ3-5: IncorrectAnswerListItemUIコンポーネントを設定

1. `IncorrectAnswerListItem`（root）を選択
2. **Inspectorウィンドウ**で**Add Component**
3. **Incorrect Answer List Item UI**を追加
4. 各フィールドに参照を設定:
   - **English Text**: `EnglishText`をドラッグ＆ドロップ
   - **Japanese Text**: `JapaneseText`をドラッグ＆ドロップ
   - **Count Text**: `CountText`をドラッグ＆ドロップ

#### ステップ3-6: Rect Transformの設定

1. `IncorrectAnswerListItem`（root）を選択
2. **Rect Transform**の設定:
   - **Width**: `400`（幅）
   - **Height**: `60`（高さ）
   - **Anchor Presets**: `Top Left`

### ステップ4: プレハブを保存

1. **Prefab**ウィンドウの上部にある**Overrides**ボタンをクリック
2. **Apply All**をクリックして変更を保存

### ステップ5: 動作確認

1. プレハブ編集モードを終了（**Scene**タブをクリック）
2. ゲームを実行して勝利する
3. 間違えた問題リストが表示されることを確認

## 修正前後の比較

### 修正前（問題のある構造）
```
IncorrectAnswerListItem (root)
└── IncorrectAnswerListItem (child)
    └── Canvas
        ├── Image
        └── Content
            ├── EnglishText
            ├── JapaneseText
            └── CountText
```

### 修正後（正しい構造）
```
IncorrectAnswerListItem (root)
├── Image (背景)
└── Content
    ├── EnglishText
    ├── JapaneseText
    └── CountText
```

## 重要なポイント

1. **Canvasは不要**: リストアイテムは既存のCanvasの子として配置されるため、個別のCanvasは必要ありません
2. **Imageはrootの直接の子**: 背景として機能するため、rootの直接の子である必要があります
3. **Contentはレイアウトグループ**: Vertical Layout GroupとContent Size Fitterを使用して、テキスト要素を縦に並べます

## トラブルシューティング: テキストが見えない場合

Canvasを削除した後、シーン上でテキストが見えない場合は、以下の点を確認してください：

### 確認項目1: テキスト要素が存在するか

1. **Hierarchyウィンドウ**で`Content`を展開
2. `EnglishText`、`JapaneseText`、`CountText`が存在することを確認
3. 存在しない場合は、**ステップ3-3**に戻ってテキスト要素を作成してください

### 確認項目2: テキスト要素のRectTransformサイズ

各テキスト要素（`EnglishText`、`JapaneseText`、`CountText`）を選択して、**Inspectorウィンドウ**で以下を確認：

1. **Rect Transform**の**Width**と**Height**が`0`になっていないか確認
   - `0`の場合は、**Width**: `200`、**Height**: `30`など適切な値を設定
2. **Anchor Presets**を確認
   - `Top Left`または`Stretch Left`を推奨

### 確認項目3: テキスト要素の色とフォントサイズ

各テキスト要素を選択して、**Inspectorウィンドウ**で以下を確認：

1. **TextMeshProUGUI**コンポーネントの**Color**を確認
   - 白い背景の場合は、テキストの色が黒（`#000000`）または濃い色になっているか確認
   - 背景が白いのにテキストも白いと見えません
2. **Font Size**を確認
   - `0`や`1`などの非常に小さい値になっていないか確認
   - 推奨値: `EnglishText`: `18`、`JapaneseText`: `16`、`CountText`: `16`
3. **Text**フィールドに仮のテキストが入っているか確認
   - 空の場合は、`English Text`、`Japanese Text`、`×3回`などの仮のテキストを入力

### 確認項目4: ContentのRectTransform設定

`Content`を選択して、**Inspectorウィンドウ**で以下を確認：

1. **Rect Transform**の**Width**と**Height**が`0`になっていないか確認
   - `0`の場合は、**ステップ3-2**に戻って正しく設定してください
2. **Anchor Presets**が`Stretch Stretch`になっているか確認
3. **Left**、**Right**、**Top**、**Bottom**の値が適切か確認
   - 推奨値: `Left: 5, Right: -5, Top: -5, Bottom: 5`

### 確認項目5: プレハブのルート要素のサイズ

`IncorrectAnswerListItem`（root）を選択して、**Inspectorウィンドウ**で以下を確認：

1. **Rect Transform**の**Width**と**Height**が適切な値になっているか確認
   - 推奨値: `Width: 400, Height: 60`
2. **Anchor Presets**が`Top Left`になっているか確認

### 確認項目6: シーン上での表示確認

1. **Sceneビュー**で`2D`モードになっているか確認（ツールバーの`2D`ボタン）
2. **Sceneビュー**で`IncorrectAnswerListItem`（root）を選択
3. **Sceneビュー**でテキスト要素が表示されているか確認
   - 表示されない場合は、**Gameビュー**で確認してみてください

### 確認項目7: フォントアセットの設定

各テキスト要素を選択して、**Inspectorウィンドウ**で以下を確認：

1. **TextMeshProUGUI**コンポーネントの**Font Asset**が設定されているか確認
   - 設定されていない場合は、**TMP Settings**からデフォルトフォントを設定するか、プロジェクト内のフォントアセットを割り当ててください

### よくある問題と解決方法

#### 問題: テキストが完全に見えない（真っ白）

**原因**: テキストの色が背景と同じ、またはRectTransformのサイズが0

**解決方法**:
1. テキスト要素の**Color**を黒（`#000000`）に変更
2. テキスト要素の**RectTransform**の**Width**と**Height**を適切な値に設定（例: `Width: 200, Height: 30`）

#### 問題: テキストが一部しか見えない

**原因**: ContentのRectTransformのサイズが小さすぎる、またはAnchor設定が間違っている

**解決方法**:
1. `Content`の**Rect Transform**を確認
2. **Anchor Presets**を`Stretch Stretch`に設定
3. **Left**、**Right**、**Top**、**Bottom**の値を調整

#### 問題: テキストが縦に並んでいない

**原因**: Vertical Layout Groupが正しく設定されていない

**解決方法**:
1. `Content`に**Vertical Layout Group**コンポーネントが追加されているか確認
2. **ステップ3-4**に戻って正しく設定してください

---

これで修正は完了です。プレハブを修正した後、ゲームを実行して動作を確認してください。
