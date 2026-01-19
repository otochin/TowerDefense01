# IncorrectAnswerListItemプレハブ設定チェックリスト

## 正しい構造

```
IncorrectAnswerListItem (root)
├── Image (背景)
└── Content
    ├── EnglishText
    ├── JapaneseText
    └── CountText
```

## チェックリスト

### 1. IncorrectAnswerListItem (root)

#### Rect Transform
- ✅ **Anchor Presets**: `Top Left`
- ✅ **Width**: `400`（幅）
- ✅ **Height**: `60`（高さ）
- ✅ **Pos X**: `0`
- ✅ **Pos Y**: `0`
- ✅ **Pos Z**: `0`

#### Image コンポーネント（背景）
- ✅ **Color**: 半透明のグレー（例: `#80808080`）
- ✅ **Raycast Target**: ❌ チェックを外す（クリックイベントを受け取らない）

#### IncorrectAnswerListItemUI コンポーネント
- ✅ **English Text**: `EnglishText`が設定されている
- ✅ **Japanese Text**: `JapaneseText`が設定されている
- ✅ **Count Text**: `CountText`が設定されている

---

### 2. Content

#### Rect Transform
- ✅ **Anchor Presets**: `Stretch Stretch`（親いっぱいに広がる）
- ✅ **Min**: `X: 0, Y: 0`（左下）
- ✅ **Max**: `X: 1, Y: 1`（右上）
- ✅ **Pivot**: `X: 0.5, Y: 0.5`（中央）
- ✅ **Left**: `5`（左パディング）
- ✅ **Right**: `-5`（右パディング）
- ✅ **Top**: `-5`（上パディング）
- ✅ **Bottom**: `5`（下パディング）

#### Vertical Layout Group コンポーネント
- ✅ **Child Alignment**: `Upper Left`
- ✅ **Child Control Size**:
  - ❌ **Width**のチェックを外す
  - ✅ **Height**をチェック
- ✅ **Child Force Expand**:
  - ❌ **Width**のチェックを外す
  - ❌ **Height**のチェックを外す
- ✅ **Spacing**: `5`（要素間のスペース）
- ✅ **Padding**:
  - **Left**: `5`
  - **Right**: `5`
  - **Top**: `5`
  - **Bottom**: `5`

#### Content Size Fitter コンポーネント
- ✅ **Horizontal Fit**: `Preferred Size`（または`Unconstrained`でも可）
- ✅ **Vertical Fit**: `Preferred Size`

---

### 3. EnglishText

#### Rect Transform
- ✅ **Anchor Presets**: `Stretch Left`または`Stretch Right`（親の幅に合わせる）
- ✅ **Min**: `X: 0, Y: 0.5`（左中央）
- ✅ **Max**: `X: 1, Y: 0.5`（右中央）
- ✅ **Pivot**: `X: 0.5, Y: 0.5`（中央）
- ✅ **Left**: `5`（左パディング）
- ✅ **Right**: `-5`（右パディング）
- ✅ **Width**: `0`（Stretchなので自動的に親の幅に合わせる）
- ✅ **Height**: `0`（ContentSizeFitterが自動計算）

#### TextMeshProUGUI コンポーネント
- ✅ **Text**: `English Text`（仮のテキスト）
- ✅ **Font Size**: `18`
- ✅ **Alignment**: 左揃え
- ✅ **Color**: 白色（例: `#FFFFFF`）

#### ContentSizeFitter コンポーネント（オプション）
- ✅ **Horizontal Fit**: `Unconstrained`
- ✅ **Vertical Fit**: `Preferred Size`

---

### 4. JapaneseText

#### Rect Transform
- ✅ **Anchor Presets**: `Stretch Left`または`Stretch Right`（親の幅に合わせる）
- ✅ **Min**: `X: 0, Y: 0.5`（左中央）
- ✅ **Max**: `X: 1, Y: 0.5`（右中央）
- ✅ **Pivot**: `X: 0.5, Y: 0.5`（中央）
- ✅ **Left**: `5`（左パディング）
- ✅ **Right**: `-5`（右パディング）
- ✅ **Width**: `0`（Stretchなので自動的に親の幅に合わせる）
- ✅ **Height**: `0`（ContentSizeFitterが自動計算）

#### TextMeshProUGUI コンポーネント
- ✅ **Text**: `Japanese Text`（仮のテキスト）
- ✅ **Font Size**: `16`
- ✅ **Alignment**: 左揃え
- ✅ **Color**: 白色（例: `#FFFFFF`）

#### ContentSizeFitter コンポーネント（オプション）
- ✅ **Horizontal Fit**: `Unconstrained`
- ✅ **Vertical Fit**: `Preferred Size`

---

### 5. CountText

#### Rect Transform
- ✅ **Anchor Presets**: `Stretch Left`または`Stretch Right`（親の幅に合わせる）
- ✅ **Min**: `X: 0, Y: 0.5`（左中央）
- ✅ **Max**: `X: 1, Y: 0.5`（右中央）
- ✅ **Pivot**: `X: 0.5, Y: 0.5`（中央）
- ✅ **Left**: `5`（左パディング）
- ✅ **Right**: `-5`（右パディング）
- ✅ **Width**: `0`（Stretchなので自動的に親の幅に合わせる）
- ✅ **Height**: `0`（ContentSizeFitterが自動計算）

#### TextMeshProUGUI コンポーネント
- ✅ **Text**: `×3回`（仮のテキスト）
- ✅ **Font Size**: `16`
- ✅ **Alignment**: 右揃え
- ✅ **Color**: 赤色（例: `#FF0000`）

#### ContentSizeFitter コンポーネント（オプション）
- ✅ **Horizontal Fit**: `Unconstrained`
- ✅ **Vertical Fit**: `Preferred Size`

---

## 重要なポイント

1. **Canvasは不要**: リストアイテムは既存のCanvasの子として配置されるため、個別のCanvasは必要ありません

2. **Imageはrootの直接の子**: 背景として機能するため、rootの直接の子である必要があります

3. **Contentはレイアウトグループ**: Vertical Layout GroupとContent Size Fitterを使用して、テキスト要素を縦に並べます

4. **テキスト要素のWidth**: テキスト要素は親（Content）の幅に合わせてStretchするように設定します

5. **テキスト要素のHeight**: ContentSizeFitterで自動計算されるように設定します

---

## 確認方法

1. **Projectウィンドウ**で`Assets/Prefabs/UI/IncorrectAnswerListItem.prefab`をダブルクリック
2. プレハブ編集モードに入る
3. 上記のチェックリストに従って、各要素の設定を確認
4. すべての設定が正しいことを確認したら、プレハブを保存

---

## よくある問題

### 問題1: テキストが小さく表示される

**原因**: テキスト要素のWidthが固定値になっている

**解決方法**:
1. 各テキスト要素を選択
2. **Rect Transform**の**Anchor Presets**を`Stretch Left`または`Stretch Right`に設定
3. **Width**を`0`に設定

### 問題2: テキストが縦に並ばない

**原因**: ContentのVertical Layout Groupが正しく設定されていない

**解決方法**:
1. `Content`を選択
2. **Vertical Layout Group**の**Child Alignment**を`Upper Left`に設定
3. **Child Control Size**の**Height**を✅チェック

### 問題3: テキストが見えない

**原因**: テキスト要素のColorが背景と同じ、またはRectTransformのサイズが0

**解決方法**:
1. テキスト要素の**Color**を確認（背景と異なる色にする）
2. テキスト要素に**ContentSizeFitter**を追加して、**Vertical Fit**を`Preferred Size`に設定

---

これでプレハブの設定は完了です。ゲームを実行して動作を確認してください！
