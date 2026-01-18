# Unity開発標準仕様書

## 命名規則

### C#スクリプト

#### クラス名
- **規則**: PascalCase
- **例**: `PlayerController`, `EnemySpawner`, `GameManager`

#### メソッド名
- **規則**: PascalCase
- **例**: `SpawnCharacter()`, `CalculateDamage()`, `UpdateHealth()`

#### 変数名（public/protected）
- **規則**: PascalCase
- **例**: `CurrentHealth`, `MaxHealth`, `MoveSpeed`

#### 変数名（private）
- **規則**: camelCase（先頭にアンダースコアを付ける場合あり）
- **例**: `currentHealth`, `_maxHealth`, `moveSpeed`

#### 定数
- **規則**: PascalCase（全大文字も可）
- **例**: `MAX_HEALTH`, `DEFAULT_SPEED`, `BaseDamage`

#### インターフェース
- **規則**: PascalCase（先頭に`I`を付ける）
- **例**: `IDamageable`, `IMovable`, `ISpawnable`

#### イベント
- **規則**: PascalCase（`On`で始める）
- **例**: `OnCharacterSpawned`, `OnEnemyDefeated`, `OnGameOver`

### Unityオブジェクト名

#### シーン内オブジェクト
- **規則**: PascalCase（日本語可、ただし推奨は英語）
- **例**: `PlayerCastle`, `EnemyCastle`, `CharacterSpawnPoint`

#### プレハブ名
- **規則**: PascalCase
- **例**: `Character_Warrior`, `Enemy_Orc`, `Projectile_Arrow`

#### アセット名
- **規則**: PascalCase
- **例**: `CharacterData_ScriptableObject`, `GameSettings_Asset`

### フォルダ・ファイル名

#### フォルダ名
- **規則**: PascalCase
- **例**: `Scripts`, `Prefabs`, `Materials`, `Audio`

#### ファイル名
- **規則**: PascalCase（スクリプトはクラス名と一致）
- **例**: `PlayerController.cs`, `EnemySpawner.cs`

## フォルダ構成

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── SceneManager.cs
│   │   └── SaveManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerCastle.cs
│   │   └── ResourceManager.cs
│   ├── Character/
│   │   ├── CharacterBase.cs
│   │   ├── CharacterController.cs
│   │   ├── CharacterData.cs
│   │   └── CharacterTypes/
│   │       ├── Warrior.cs
│   │       ├── Archer.cs
│   │       └── Mage.cs
│   ├── Enemy/
│   │   ├── EnemyBase.cs
│   │   ├── EnemyController.cs
│   │   ├── EnemySpawner.cs
│   │   └── EnemyCastle.cs
│   ├── UI/
│   │   ├── UIManager.cs
│   │   ├── CharacterSelectUI.cs
│   │   ├── ResourceUI.cs
│   │   └── GameOverUI.cs
│   ├── Combat/
│   │   ├── DamageSystem.cs
│   │   ├── HealthSystem.cs
│   │   └── Projectile.cs
│   └── Utils/
│       ├── ObjectPool.cs
│       ├── AudioManager.cs
│       └── Extensions.cs
├── Prefabs/
│   ├── Characters/
│   │   ├── Character_Warrior.prefab
│   │   ├── Character_Archer.prefab
│   │   └── Character_Mage.prefab
│   ├── Enemies/
│   │   ├── Enemy_Orc.prefab
│   │   └── Enemy_Goblin.prefab
│   ├── Castles/
│   │   ├── PlayerCastle.prefab
│   │   └── EnemyCastle.prefab
│   └── UI/
│       ├── CharacterButton.prefab
│       └── HealthBar.prefab
├── Materials/
│   ├── CharacterMaterials/
│   └── EnvironmentMaterials/
├── Textures/
│   ├── Characters/
│   ├── Enemies/
│   └── UI/
├── Audio/
│   ├── BGM/
│   ├── SFX/
│   │   ├── Combat/
│   │   ├── UI/
│   │   └── Character/
│   └── Voice/
├── Animations/
│   ├── Characters/
│   └── Enemies/
├── Scenes/
│   ├── MainMenu.unity
│   ├── GameScene.unity
│   └── StageSelect.unity
├── Data/
│   ├── ScriptableObjects/
│   │   ├── CharacterData.asset
│   │   └── StageData.asset
│   └── Configs/
│       └── GameConfig.json
└── Resources/
    └── (必要に応じて)
```

## タグ（Tags）定義

### 標準タグ

| タグ名 | 用途 |
|--------|------|
| `Player` | プレイヤーキャラクター |
| `Enemy` | 敵キャラクター |
| `PlayerCastle` | プレイヤーの城 |
| `EnemyCastle` | 敵の城 |
| `Projectile` | 投射物（矢、魔法弾など） |
| `Ground` | 地面・道 |
| `SpawnPoint` | キャラクター召喚地点 |
| `UI` | UI要素 |

### カスタムタグ（必要に応じて追加）

| タグ名 | 用途 |
|--------|------|
| `Obstacle` | 障害物 |
| `Collectible` | 収集アイテム |

## レイヤー（Layers）定義

### 標準レイヤー

| レイヤー名 | レイヤー番号 | 用途 |
|-----------|-------------|------|
| `Default` | 0 | デフォルト |
| `Player` | 6 | プレイヤーキャラクター |
| `Enemy` | 7 | 敵キャラクター |
| `Castle` | 8 | 城オブジェクト |
| `Ground` | 9 | 地面・道 |
| `UI` | 5 | UI要素 |
| `Projectile` | 10 | 投射物 |

### レイヤー衝突設定（Physics Settings）

- **Player** vs **Enemy**: 衝突する（物理衝突はしないが、コリジョン検出はする）
- **Player** vs **EnemyCastle**: 衝突する
- **Enemy** vs **PlayerCastle**: 衝突する
- **Projectile** vs **Player**: 衝突しない（プレイヤー投射物は敵に当たる）
- **Projectile** vs **Enemy**: 衝突する

## コンポーネント命名規則

### スクリプトコンポーネント
- クラス名とファイル名を一致させる
- 機能が明確に分かる名前を使用

### 標準コンポーネントの使用

| 用途 | 推奨コンポーネント |
|------|-------------------|
| キャラクター移動 | `Rigidbody2D` + `Collider2D`（2Dの場合）<br>`CharacterController`（3Dの場合） |
| 衝突検出 | `BoxCollider2D` / `CircleCollider2D`（2D）<br>`BoxCollider` / `CapsuleCollider`（3D） |
| アニメーション | `Animator` |
| UI要素 | `Canvas`, `Button`, `Image`, `TextMeshProUGUI` |
| オーディオ | `AudioSource` |

## コーディング規約

### アクセス修飾子
- 明示的に指定する（`public`, `private`, `protected`）
- `[SerializeField]`を使用してprivate変数をインスペクターに表示

### コメント
- 複雑なロジックには必ずコメントを記述
- XMLドキュメントコメントを推奨（publicメソッド）

### エラーハンドリング
- nullチェックを適切に行う
- `Debug.LogError()`でエラーを記録

### パフォーマンス
- `Update()`内での重い処理を避ける
- オブジェクトプーリングを活用
- `FindObjectOfType()`の頻繁な使用を避ける

---

## 最終更新日

2026年1月15日

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-15 | 初版作成 | - |
