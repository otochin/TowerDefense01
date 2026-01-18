# 城への攻撃ができない問題のトラブルシューティングガイド

## 問題の症状

- Archerだけが城（EnemyCastle/PlayerCastle）に攻撃できる
- WarriorとMageは城に攻撃できない

## Unity Editorでの確認手順

### 1. EnemyCastleのGameObject確認

1. Unity Editorで `SampleScene` を開く
2. Hierarchyウィンドウで `EnemyCastle` GameObjectを選択
3. Inspectorウィンドウで以下を確認：

#### タグの確認
- **Tag**: `EnemyCastle` に設定されているか確認
  - もし `Untagged` の場合は、ドロップダウンから `EnemyCastle` を選択

#### Collider2Dの確認
- **BoxCollider2D** コンポーネントがアタッチされているか確認
  - もしアタッチされていない場合は、`Add Component` → `Physics 2D` → `Box Collider 2D` を選択
- **BoxCollider2D** の設定：
  - **Is Trigger**: `false` (チェックが入っていないこと)
  - **Size**: `(1, 1)` 以上（推奨: `(2, 2)` 以上）
  - **Offset**: 任意（城の中央に来るように調整）

#### EnemyCastleスクリプトの確認
- **EnemyCastle** スクリプトがアタッチされているか確認
  - もしアタッチされていない場合は、`Add Component` → `Scripts` → `Enemy` → `EnemyCastle` を選択
- **EnemyCastle** スクリプトの設定：
  - **Max Health**: 任意（例: `1000`）
  - **Defense**: 任意（例: `0`）

### 2. PlayerCastleのGameObject確認

1. Hierarchyウィンドウで `PlayerCastle` GameObjectを選択
2. Inspectorウィンドウで以下を確認：

#### タグの確認
- **Tag**: `PlayerCastle` に設定されているか確認
  - もし `Untagged` の場合は、ドロップダウンから `PlayerCastle` を選択

#### Collider2Dの確認
- **BoxCollider2D** コンポーネントがアタッチされているか確認
  - もしアタッチされていない場合は、`Add Component` → `Physics 2D` → `Box Collider 2D` を選択
- **BoxCollider2D** の設定：
  - **Is Trigger**: `false` (チェックが入っていないこと)
  - **Size**: `(1, 1)` 以上（推奨: `(2, 2)` 以上）
  - **Offset**: 任意（城の中央に来るように調整）

#### PlayerCastleスクリプトの確認
- **PlayerCastle** スクリプトがアタッチされているか確認
  - もしアタッチされていない場合は、`Add Component` → `Scripts` → `Player` → `PlayerCastle` を選択
- **PlayerCastle** スクリプトの設定：
  - **Max Health**: 任意（例: `1000`）
  - **Defense**: 任意（例: `0`）

### 3. CharacterDataのAttackRange確認（重要）

Archerだけが城に攻撃できる場合、**最も可能性が高い原因**は、WarriorとMageの`AttackRange`が城に届く距離に設定されていないことです。

#### 確認手順

1. Projectウィンドウで `Assets/Data/ScriptableObjects/` フォルダを開く
2. 各キャラクターの `CharacterData` アセットを選択：
   - `Warrior` (CharacterData)
   - `Archer` (CharacterData)
   - `Mage` (CharacterData)
3. Inspectorウィンドウで **Attack Range** の値を確認

#### 推奨設定値

城の位置を考慮した推奨設定値：

| キャラクター | Attack Range | 説明 |
|------------|-------------|------|
| Warrior | `1.5` 以上（推奨: `2.0`） | 近接攻撃型なので比較的短い |
| Archer | `5.0` 以上（推奨: `6.0`） | 遠距離攻撃型なので長い |
| Mage | `3.0` 以上（推奨: `4.0`） | 魔法攻撃型なので中距離 |

#### 注意点

- EnemyCastleの位置が `x: 8` にある場合、プレイヤーキャラクターは右方向に進みます
- キャラクターが城の近く（攻撃範囲内）に到達した時にのみ、城を検出できます
- WarriorとMageのAttackRangeが小さすぎると、城に到達しても攻撃範囲内に入らず、検出されません

#### 設定方法

1. Projectウィンドウで `Warrior` (CharacterData) アセットを選択
2. Inspectorウィンドウで **Attack Range** の値を `2.0` 以上に設定
3. 同様に `Mage` (CharacterData) アセットの **Attack Range** を `4.0` 以上に設定
4. 変更を保存（自動保存されます）

### 4. プレハブの設定確認

もしキャラクターがプレハブから生成されている場合：

1. Projectウィンドウで `Assets/Prefabs/Characters/` フォルダを開く
2. 各キャラクターのプレハブを選択：
   - `Character_Warrior.prefab`
   - `Character_Archer.prefab`
   - `Character_Mage.prefab`
3. Inspectorウィンドウで **CharacterBase** コンポーネントの設定を確認：
   - **Character Data**: 正しい `CharacterData` アセットが設定されているか
   - 設定されていない場合は、対応する `CharacterData` アセットをドラッグ＆ドロップ

### 5. 動作確認

1. Unity Editorで再生ボタンを押す
2. ゲームモードを選択
3. WarriorとMageを召喚
4. キャラクターが城の近くに到達した時に、コンソールに以下のようなログが表示されるか確認：
   - `[Warrior] EnemyCastle tag detected! GameObject: EnemyCastle`
   - `[Mage] EnemyCastle tag detected! GameObject: EnemyCastle`
5. これらのログが表示されない場合：
   - Warrior/Mageの`AttackRange`が小さすぎる可能性があります
   - `AttackRange`を大きくしてみてください

### 6. 攻撃範囲の可視化（デバッグ用）

Unity Editorで攻撃範囲を可視化する方法：

1. Sceneビューでキャラクターを選択
2. キャラクターの`Warrior`、`Archer`、または`Mage`コンポーネントの`OnDrawGizmosSelected`メソッドが実装されていることを確認
3. 選択したキャラクターの周りに円が表示され、これが攻撃範囲です
4. この円が城（EnemyCastle）に届いているか確認

## よくある問題と解決方法

### 問題1: タグが設定されていない

**症状**: ログに `[Warrior] EnemyCastle tag detected!` が表示されない

**解決方法**:
1. EnemyCastle/PlayerCastleのGameObjectを選択
2. InspectorウィンドウでTagを確認
3. `EnemyCastle` / `PlayerCastle` タグに設定する

### 問題2: Collider2Dがアタッチされていない、またはIs Triggerがtrue

**症状**: `Physics2D.OverlapCircleAll` で城が検出されない

**解決方法**:
1. EnemyCastle/PlayerCastleのGameObjectを選択
2. Inspectorウィンドウで`BoxCollider2D`コンポーネントを確認
3. アタッチされていない場合は追加
4. **Is Trigger**が`false`になっていることを確認（チェックが入っていないこと）

### 問題3: AttackRangeが小さすぎる

**症状**: キャラクターが城の近くに到達しても、攻撃範囲内に入らない

**解決方法**:
1. `CharacterData`アセットの`Attack Range`を確認
2. Warrior: `2.0`以上、Mage: `4.0`以上に設定
3. Archerより小さい値でも問題ありませんが、城に届く距離に設定する必要があります

### 問題4: レイヤー衝突設定の問題

**症状**: 物理衝突が発生しない

**解決方法**:
1. `Edit` → `Project Settings` → `Physics 2D` を開く
2. `Layer Collision Matrix` を確認
3. `Player` レイヤーと `Castle` レイヤーが衝突するように設定されていることを確認

## 確認チェックリスト

- [ ] EnemyCastleのタグが `EnemyCastle` に設定されている
- [ ] PlayerCastleのタグが `PlayerCastle` に設定されている
- [ ] EnemyCastleに`BoxCollider2D`がアタッチされている
- [ ] PlayerCastleに`BoxCollider2D`がアタッチされている
- [ ] EnemyCastleの`BoxCollider2D`の`Is Trigger`が`false`
- [ ] PlayerCastleの`BoxCollider2D`の`Is Trigger`が`false`
- [ ] EnemyCastleに`EnemyCastle`スクリプトがアタッチされている
- [ ] PlayerCastleに`PlayerCastle`スクリプトがアタッチされている
- [ ] Warriorの`CharacterData`の`Attack Range`が`2.0`以上
- [ ] Mageの`CharacterData`の`Attack Range`が`4.0`以上
- [ ] Archerの`CharacterData`の`Attack Range`が`5.0`以上
- [ ] 各キャラクタープレハブに正しい`CharacterData`アセットが設定されている

## デバッグログの確認

ゲーム実行中に以下のログが表示されるか確認：

### 正常な場合のログ例

```
[Warrior] EnemyCastle tag detected! GameObject: EnemyCastle
[Warrior] IDamageable found on EnemyCastle. IsDead: False
[Warrior] EnemyCastle distance: 1.50, AttackRange: 2.00
[Warrior] EnemyCastle selected as target! Distance: 1.50
[Warrior] Target found! Stopping movement.
[Warrior] Warrior attacked EnemyCastle for 10 damage!
```

### 問題がある場合のログ例

```
[Warrior] Found 0 colliders in range 1.50. Position: (7.50, 0.04, 0.00)
```

この場合、`AttackRange`が小さすぎる可能性があります。
