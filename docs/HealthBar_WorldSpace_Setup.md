# HealthBar プレハブ作成手順（World Space Canvas方式）

## 概要
キャラクター・エネミープレハブに最初からWorld Space Canvasを含めたHealthBarプレハブを作成します。

## 手順

### 1. HealthBarプレハブの構造を作成

```
HealthBarPrefab (GameObject)
├── Canvas (Canvas - World Space)
│   └── HealthBarPanel (GameObject)
│       ├── Background (Image)
│       └── Fill (Image)
```

### 2. 詳細な手順

#### ステップ1: 新しいGameObjectを作成
1. Hierarchyで右クリック → `Create Empty`
2. 名前を`HealthBarPrefab`に変更

#### ステップ2: Canvasを作成（World Space設定）
1. `HealthBarPrefab`を右クリック → `UI` → `Canvas`
2. 作成された`Canvas`を選択
3. Inspectorで以下の設定：
   - **Render Mode**: `World Space`
   - **Event Camera**: （空のまま）
   - **Rect Transform**:
     - **Position**: X: 0, Y: 0, Z: 0
     - **Width**: 100
     - **Height**: 10
     - **Scale**: X: 0.01, Y: 0.01, Z: 0.01

#### ステップ3: HealthBarPanelを作成
1. `Canvas`を右クリック → `UI` → `Panel`（または`Create Empty`）
2. 名前を`HealthBarPanel`に変更
3. Inspectorで以下の設定：
   - **Rect Transform**:
     - **Anchor Presets**: 中央（Alt+Shift+中央をクリック）
     - **Position**: X: 0, Y: 0, Z: 0
     - **Width**: 100
     - **Height**: 10

#### ステップ4: Background Imageを作成
1. `HealthBarPanel`を右クリック → `UI` → `Image`
2. 名前を`Background`に変更
3. Inspectorで以下の設定：
   - **Rect Transform**:
     - **Anchor Presets**: ストレッチ（Alt+Shift+ストレッチをクリック）
     - **Left**: 0, **Right**: 0, **Top**: 0, **Bottom**: 0
   - **Image**:
     - **Color**: R: 51, G: 51, B: 51, A: 200（ダークグレー）

#### ステップ5: Fill Imageを作成
1. **重要**: `Background`ではなく、`HealthBarPanel`を右クリック → `UI` → `Image`
   - `Fill`は`Background`の子ではなく、`HealthBarPanel`の直接の子として作成する必要があります
2. 名前を`Fill`に変更
3. Inspectorで以下の設定：

   **Rect Transformの設定**:
   - **Anchor Presets**: 左（Alt+Shift+左をクリック）
   - **Left**: 0, **Width**: 100, **Height**: 10
   - **Pivot**: X: 0, Y: 0.5（左端をピボットに）
   
   **Imageコンポーネントの設定**（重要）:
   - **Source Image**: **Unityのデフォルトスプライトを設定する必要があります**
     - `Source Image`が`None`のままでは、`Image Type`が表示されません
     - Unityのデフォルトスプライトを使用する方法：
       1. `Source Image`フィールドの右側にある円形のアイコンをクリック
       2. 検索ボックスに「default sprite」または「unity white sprite」と入力
       3. または、プロジェクトビューで右クリック → `Create` → `Sprite` → `Square`などで白いスプライトを作成
     - もしくは、Unityの組み込みスプライトを使用：
       - `Source Image`の円形アイコンをクリック
       - 検索で「Default-White」や「Knob」などを検索（Unityバージョンによって異なります）
   - **Color**: R: 0, G: 255, B: 0, A: 255（緑色）
   - **Image Type**: `Source Image`を設定すると表示されます。**`Filled`に変更する**
     - `Image Type`を`Filled`に変更すると、`Fill Method`と`Fill Origin`の項目が表示されます
   - **Fill Method**: `Horizontal`（`Image Type`を`Filled`に変更すると表示されます）
   - **Fill Origin**: `Left`（`Fill Method`を`Horizontal`に設定すると表示されます）
   - **Fill Amount**: `1`（初期値。コードで自動的に更新されます）

   **重要**: `Source Image`が`None`のままでは`Image Type`が表示されません。必ずスプライトを設定してください。

#### ステップ6: WorldSpaceHealthBarUIスクリプトを追加
1. `HealthBarPanel`を選択
2. `Add Component` → `WorldSpaceHealthBarUI`
3. Inspectorで以下の設定：
   - **Health Bar Fill**: `Fill`（Image）をドラッグ&ドロップ
   - **Health Bar Text**: （空のまま - テキスト不要の場合）
   - **Show Health Text**: `false`（チェックを外す）
   - **Offset**: X: 0, Y: 1, Z: 0

#### ステップ7: プレハブとして保存
1. Projectで`Assets/Prefabs/UI`フォルダを開く（なければ作成）
2. `HealthBarPrefab`を`Assets/Prefabs/UI`にドラッグ&ドロップ
3. Hierarchyの元のインスタンスを削除（プレハブはProjectに保存済み）

### 3. キャラクター・エネミープレハブに設定

1. Projectでキャラクター/エネミープレハブを開く（例：`Character_Warrior.prefab`）
2. ルートオブジェクトを選択
3. `Character Base`（または`Enemy Base`）コンポーネントで：
   - **Health Bar Prefab**: `HealthBarPrefab`をドラッグ&ドロップ
   - **Auto Create Health Bar**: ✓（チェックを入れる）
4. Prefabモードを閉じる

## サイズの調整

### Canvasのサイズを変更する場合
1. `Canvas`を選択
2. **Rect Transform** → **Width** / **Height**を変更
3. **Scale**を調整（例：`0.01` → `0.005`で小さく、`0.02`で大きく）

### バーの位置を変更する場合
1. `HealthBarPanel`を選択
2. `WorldSpaceHealthBarUI`コンポーネント → **Offset**を変更

## メリット

- ✅ Unityの標準UIシステムを使用（安定）
- ✅ エディタで視覚的に確認・調整可能
- ✅ `SpriteRenderer`よりも扱いやすい
- ✅ テキスト追加も簡単
