# 敵プレハブ作成ガイド

## 概要

このドキュメントでは、`Enemy_Orc.prefab`と`Enemy_Goblin.prefab`の作成手順を説明します。

## 前提条件

- Phase 4.1、4.2、4.3のスクリプト実装が完了していること
- Unity Editorが開いていること
- `EnemyData`アセットが作成されていること（後で作成）

## 敵プレハブの構造

### 基本構造

```
Enemy_Orc (GameObject)
├── EnemyBase (Component)
├── EnemyController (Component)
├── Enemy_Orc (Component)
├── HealthSystem (Component) [自動追加]
├── Rigidbody2D (Component)
├── Collider2D (Component)
└── SpriteRenderer (Component) [視覚表現用]
```

### 必要なコンポーネント

| コンポーネント | 必須 | 説明 |
|--------------|------|------|
| `EnemyBase` | ✓ | 敵基底クラス |
| `EnemyController` | ✓ | 敵移動コントローラー |
| `Enemy_Orc` / `Enemy_Goblin` | ✓ | 敵タイプ固有のスクリプト |
| `HealthSystem` | ✓ | HP管理システム（自動追加される） |
| `Rigidbody2D` | ✓ | 物理演算用 |
| `Collider2D` | ✓ | 衝突検出用 |
| `SpriteRenderer` | - | 視覚表現用（オプション） |

## 作成手順

### ステップ1: 基本GameObjectの作成

1. Hierarchyで右クリック → `Create Empty`
2. 名前を`Enemy_Orc`に変更
3. 位置を`(0, 0, 0)`に設定

### ステップ2: 必要なコンポーネントの追加

#### 2.1 Rigidbody2Dの追加

1. `Enemy_Orc`を選択
2. `Add Component` → `Physics 2D` → `Rigidbody 2D`
3. 設定：
   - **Body Type**: `Kinematic`
   - **Gravity Scale**: `0`
   - **Freeze Rotation**: Z軸をチェック（回転を固定）

#### 2.2 Collider2Dの追加

1. `Add Component` → `Physics 2D` → `Box Collider 2D`（または`Circle Collider 2D`）
2. 設定：
   - **Size**: `(1, 1, 0)`（適切なサイズに調整）
   - **Is Trigger**: `false`（物理衝突を検出する場合）または`true`（トリガーとして使用する場合）

#### 2.3 スクリプトコンポーネントの追加

1. `Add Component` → `Scripts` → `EnemyBase`
2. `Add Component` → `Scripts` → `EnemyController`
3. `Add Component` → `Scripts` → `Enemy_Orc`（または`Enemy_Goblin`）

**注意**: `HealthSystem`は`EnemyBase`が自動的に追加するため、手動で追加する必要はありません。

#### 2.4 SpriteRendererの追加（オプション）

1. `Add Component` → `Rendering` → `Sprite Renderer`
2. 適切なスプライトを設定（後で追加）

### ステップ3: タグとレイヤーの設定

1. `Enemy_Orc`を選択
2. **Tag**: `Enemy`に設定
3. **Layer**: `Enemy`（または`Default`）に設定

### ステップ4: EnemyBaseの設定

1. `EnemyBase`コンポーネントを確認
2. **Enemy Data**: （後で作成する`EnemyData`アセットを設定）
3. **Health System**: 自動検出される（手動設定不要）

### ステップ5: EnemyControllerの設定

1. `EnemyController`コンポーネントを確認
2. **Move Left**: `true`（左方向へ移動）
3. **Enemy Base**: 自動検出される（手動設定不要）

### ステップ6: Enemy_Orcの設定

1. `Enemy_Orc`コンポーネントを確認
2. **Enemy Base**: 自動検出される（手動設定不要）
3. **Enemy Controller**: 自動検出される（手動設定不要）

### ステップ7: プレハブとして保存

1. `Enemy_Orc`を`Assets/Prefabs/Enemies/`フォルダにドラッグ&ドロップ
2. プレハブが作成される
3. Hierarchyから元のGameObjectを削除（プレハブは残る）

### ステップ8: Enemy_Goblinの作成

`Enemy_Orc`と同じ手順で`Enemy_Goblin`を作成：
1. 基本GameObjectを作成（名前: `Enemy_Goblin`）
2. 同じコンポーネントを追加
3. `Enemy_Goblin`スクリプトを追加（`Enemy_Orc`の代わりに）
4. プレハブとして保存

## EnemyDataアセットの作成

### ステップ1: EnemyDataアセットの作成

1. `Assets/Data/ScriptableObjects/`フォルダで右クリック
2. `Create` → `Game` → `Enemy Data`
3. 名前を`EnemyData_Orc`に変更

### ステップ2: EnemyData_Orcの設定

| 項目 | 値（例） | 説明 |
|------|---------|------|
| **Enemy Name** | `Orc` | 敵の名前 |
| **Description** | `基本敵` | 説明 |
| **Max Health** | `150` | 最大HP |
| **Attack Power** | `15` | 攻撃力 |
| **Attack Range** | `1.5` | 攻撃範囲 |
| **Attack Speed** | `1.5` | 攻撃間隔（秒） |
| **Move Speed** | `1.5` | 移動速度 |
| **Defense** | `5` | 防御力 |
| **Enemy Prefab** | `Enemy_Orc.prefab` | 敵プレハブ |
| **Enemy Type** | `Orc` | 敵タイプ |

### ステップ3: EnemyData_Goblinの作成

同様の手順で`EnemyData_Goblin`を作成：

| 項目 | 値（例） | 説明 |
|------|---------|------|
| **Enemy Name** | `Goblin` | 敵の名前 |
| **Description** | `弱い敵` | 説明 |
| **Max Health** | `80` | 最大HP |
| **Attack Power** | `8` | 攻撃力 |
| **Attack Range** | `1.2` | 攻撃範囲 |
| **Attack Speed** | `1.2` | 攻撃間隔（秒） |
| **Move Speed** | `2.0` | 移動速度（ゴブリンは速い） |
| **Defense** | `2` | 防御力 |
| **Enemy Prefab** | `Enemy_Goblin.prefab` | 敵プレハブ |
| **Enemy Type** | `Goblin` | 敵タイプ |

### ステップ4: プレハブにEnemyDataを設定

1. `Enemy_Orc.prefab`を選択
2. `EnemyBase`コンポーネントの**Enemy Data**に`EnemyData_Orc`を設定
3. `Enemy_Goblin.prefab`を選択
4. `EnemyBase`コンポーネントの**Enemy Data**に`EnemyData_Goblin`を設定

## 動作確認

### テスト手順

1. シーンに`EnemySpawner`を配置
2. `EnemySpawner`の**Available Enemies**に`EnemyData_Orc`と`EnemyData_Goblin`を追加
3. **Spawn Point**を設定（右側の位置）
4. **Spawn Interval**を`3.0`に設定
5. 再生ボタンを押して動作確認：
   - 敵が左方向へ移動するか
   - 敵がプレイヤーキャラクターを検出して攻撃するか
   - 敵がプレイヤー城に到達して攻撃するか

## トラブルシューティング

### 敵が移動しない

- `Rigidbody2D`の**Body Type**が`Kinematic`になっているか確認
- `EnemyController`の**Move Left**が`true`になっているか確認
- `EnemyData`が正しく設定されているか確認

### 敵が攻撃しない

- `Enemy_Orc`（または`Enemy_Goblin`）コンポーネントが追加されているか確認
- タグが`Enemy`に設定されているか確認
- `Collider2D`が正しく設定されているか確認

### 敵が表示されない

- `SpriteRenderer`が追加されているか確認
- スプライトが設定されているか確認
- カメラの位置を確認

## 次のステップ

敵プレハブの作成が完了したら：
1. `EnemySpawner`のセットアップ
2. 動作確認
3. バランス調整（HP、攻撃力、移動速度など）
