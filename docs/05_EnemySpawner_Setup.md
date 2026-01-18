# EnemySpawnerセットアップガイド

## 概要

敵が出現するためには、シーンに`EnemySpawner`を配置し、適切に設定する必要があります。

## セットアップ手順

### ステップ1: EnemySpawnerの作成

1. Hierarchyで右クリック → `Create Empty`
2. 名前を`EnemySpawner`に変更
3. 位置を設定（通常は画面右側、敵の城の近く）
   - 例: `(10, 0, 0)`（右側）

### ステップ2: EnemySpawnerコンポーネントの追加

1. `EnemySpawner`を選択
2. `Add Component` → `Scripts` → `EnemySpawner`

### ステップ3: EnemySpawnerの設定

#### 3.1 基本設定

Inspectorで`EnemySpawner`コンポーネントを確認し、以下を設定：

| 項目 | 値 | 説明 |
|------|-----|------|
| **Auto Spawn** | `true` | 自動的に敵をスポーン開始 |
| **Spawn Interval** | `3.0` | スポーン間隔（秒） |
| **Limit Enemies** | `false` | 敵数制限（必要に応じて`true`に） |
| **Max Enemies On Screen** | `10` | 最大敵数（`Limit Enemies`が`true`の場合） |

#### 3.2 Available Enemiesの設定

1. **Available Enemies**リストのサイズを設定（例: `2`）
2. 各要素に`EnemyData`アセットを設定：
   - Element 0: `EnemyData_Orc`
   - Element 1: `EnemyData_Goblin`

**重要**: `EnemyData`アセットの`Enemy Prefab`フィールドに、対応するプレハブ（`Enemy_Orc.prefab`、`Enemy_Goblin.prefab`）が設定されている必要があります。

#### 3.3 Spawn Pointの設定（オプション）

- **Spawn Point**が空の場合、`EnemySpawner`自身の位置が使用されます
- 特定の位置からスポーンしたい場合は、空のGameObjectを作成し、そのTransformを設定

### ステップ4: EnemyDataアセットの確認

各`EnemyData`アセットで以下を確認：

1. `EnemyData_Orc`を選択
2. **Enemy Prefab**に`Enemy_Orc.prefab`が設定されているか確認
3. 同様に`EnemyData_Goblin`も確認

### ステップ5: 動作確認

1. 再生ボタンを押す
2. Consoleを確認（`Window` → `General` → `Console`）
3. 3秒ごとに敵がスポーンされることを確認

## トラブルシューティング

### 敵が出現しない

#### チェック1: Consoleの確認

Consoleに以下の警告が出ていないか確認：
- `[EnemySpawner] No available enemies to spawn.`
  → **解決方法**: `Available Enemies`リストに`EnemyData`を追加

- `[EnemySpawner] EnemyPrefab is not set for {EnemyName}.`
  → **解決方法**: `EnemyData`アセットの`Enemy Prefab`にプレハブを設定

#### チェック2: Auto Spawnの確認

- `EnemySpawner`の**Auto Spawn**が`true`になっているか確認

#### チェック3: Available Enemiesの確認

- `Available Enemies`リストが空でないか確認
- 各`EnemyData`が正しく設定されているか確認

#### チェック4: EnemyDataのEnemyPrefabの確認

- `EnemyData`アセットの**Enemy Prefab**フィールドにプレハブが設定されているか確認
- プレハブが`Assets/Prefabs/Enemies/`フォルダに存在するか確認

#### チェック5: プレハブの構造確認

敵プレハブに以下のコンポーネントが正しく追加されているか確認：
- `EnemyBase`
- `EnemyController`
- `Enemy_Orc`（または`Enemy_Goblin`）
- `Rigidbody2D`
- `Collider2D`

### 敵が出現するが移動しない

- `EnemyController`の**Move Left**が`true`になっているか確認
- `Rigidbody2D`の**Body Type**が`Kinematic`になっているか確認
- `EnemyData`の**Move Speed**が`0`でないか確認

### 敵が出現するが攻撃しない

- 敵のタグが`Enemy`に設定されているか確認
- `Enemy_Orc`（または`Enemy_Goblin`）コンポーネントが追加されているか確認
- `Collider2D`が正しく設定されているか確認

## デバッグ方法

### デバッグログの確認

`EnemySpawner`は以下の状況でConsoleにログを出力します：

- `[EnemySpawner] No available enemies to spawn.` - 利用可能な敵がない
- `[EnemySpawner] EnemyData is null.` - EnemyDataがnull
- `[EnemySpawner] EnemyPrefab is not set for {EnemyName}.` - プレハブが設定されていない

### 手動で敵をスポーンする

テスト用に、`EnemySpawner`の`SpawnEnemy`メソッドを直接呼び出すこともできます：

```csharp
// テスト用スクリプト
public class TestEnemySpawner : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public EnemyData testEnemyData;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemySpawner.SpawnEnemy(testEnemyData);
        }
    }
}
```

## 次のステップ

`EnemySpawner`のセットアップが完了したら：
1. 敵のスポーン間隔を調整
2. 敵のステータスをバランス調整
3. ウェーブシステムの実装（オプション）
