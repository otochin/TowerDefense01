# ゲームループシステム仕様書

## 概要

ゲームループシステムは、ゲームの状態管理、シーン遷移、クリア/ゲームオーバー判定、セーブデータの管理を担当します。

## 主要コンポーネント

### 1. GameManager（ゲーム管理）

#### 機能
- ゲーム状態の管理（モード選択、プレイ中、一時停止、ゲームオーバー、クリア）
- ゲームループの制御
- 他のマネージャーへの参照管理

#### Hierarchy構造
```
GameManager (GameObject)
├── ResourceManager (GameObject)
├── WordLearningSystem (GameObject)
├── CharacterSpawner (GameObject)
├── EnemySpawner (GameObject)
└── UIManager (GameObject)
```

#### Required Components
- `GameManager.cs` (スクリプト)
- `DontDestroyOnLoad` (必要に応じて)

#### C# Logic概要
```csharp
public enum GameState
{
    MainMenu,
    ModeSelection,  // ゲームモード選択
    Playing,
    Paused,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    // 現在のゲーム状態
    public GameState CurrentState { get; private set; }
    
    // ゲーム開始
    public void StartGame();
    
    // ゲーム一時停止
    public void PauseGame();
    
    // ゲーム再開
    public void ResumeGame();
    
    // ゲームオーバー
    public void GameOver();
    
    // ゲームクリア
    public void Victory();
    
    // ゲーム状態変更イベント
    public event Action<GameState> OnGameStateChanged;
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `CurrentState` | GameState | 現在のゲーム状態 | ModeSelection |
| `CurrentGameMode` | GameMode | 現在のゲームモード | EnglishToJapanese |
| `TimeScale` | float | ゲームの時間スケール | 1.0f |
| `AutoSaveInterval` | float | 自動保存の間隔（秒） | 30.0f |

### 2. SceneManager（シーン管理）

#### 機能
- シーン間の遷移管理
- シーン読み込み時の初期化処理
- フェードイン/アウト効果

#### Required Components
- `SceneManager.cs` (スクリプト)
- `Image` (フェード用、オプション)

#### C# Logic概要
```csharp
public class SceneManager : MonoBehaviour
{
    // シーンを読み込む
    public void LoadScene(string sceneName);
    
    // シーンを読み込む（非同期）
    public void LoadSceneAsync(string sceneName);
    
    // フェードアウトしてシーン遷移
    public void LoadSceneWithFade(string sceneName, float fadeDuration = 1.0f);
    
    // 現在のシーンを再読み込み
    public void ReloadCurrentScene();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `FadeImage` | Image | フェード用のImage | - |
| `FadeDuration` | float | フェード時間（秒） | 1.0f |

### 3. VictoryCondition（クリア条件判定）

#### 機能
- 敵の城のHPが0になった時の判定
- クリア条件のチェック
- クリア時の処理

#### Hierarchy構造
```
GameManager
└── VictoryCondition (Component)
```

#### Required Components
- `VictoryCondition.cs` (スクリプト)

#### C# Logic概要
```csharp
public class VictoryCondition : MonoBehaviour
{
    // 敵の城への参照
    public EnemyCastle EnemyCastle;
    
    // クリア条件をチェック
    private void CheckVictoryCondition();
    
    // クリア時の処理
    private void OnVictory();
    
    // クリアイベント
    public event Action OnVictoryAchieved;
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `EnemyCastle` | EnemyCastle | 敵の城への参照 | - |
| `CheckInterval` | float | チェック間隔（秒） | 0.5f |

### 4. GameOverCondition（ゲームオーバー条件判定）

#### 機能
- プレイヤーの城のHPが0になった時の判定
- ゲームオーバー条件のチェック
- ゲームオーバー時の処理

#### Hierarchy構造
```
GameManager
└── GameOverCondition (Component)
```

#### Required Components
- `GameOverCondition.cs` (スクリプト)

#### C# Logic概要
```csharp
public class GameOverCondition : MonoBehaviour
{
    // プレイヤーの城への参照
    public PlayerCastle PlayerCastle;
    
    // ゲームオーバー条件をチェック
    private void CheckGameOverCondition();
    
    // ゲームオーバー時の処理
    private void OnGameOver();
    
    // ゲームオーバーイベント
    public event Action OnGameOverTriggered;
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `PlayerCastle` | PlayerCastle | プレイヤーの城への参照 | - |
| `CheckInterval` | float | チェック間隔（秒） | 0.5f |

### 5. SaveManager（セーブデータ管理）

#### 機能
- ゲームデータの保存・読み込み
- プレイヤープログレス管理
- 設定データの保存

#### Required Components
- `SaveManager.cs` (スクリプト)

#### C# Logic概要
```csharp
[System.Serializable]
public class SaveData
{
    public int ClearedStages;
    public List<int> UnlockedCharacters;
    public int TotalMoney;
    public Dictionary<string, int> StageBestScores;
    public SettingsData Settings;
}

public class SaveManager : MonoBehaviour
{
    // データを保存
    public void SaveGame(SaveData data);
    
    // データを読み込み
    public SaveData LoadGame();
    
    // データを削除
    public void DeleteSaveData();
    
    // データが存在するか
    public bool SaveDataExists();
}
```

#### 保存データの構造

```csharp
[System.Serializable]
public class SaveData
{
    // クリアしたステージ数
    public int ClearedStages = 0;
    
    // アンロックされたキャラクターIDのリスト
    public List<int> UnlockedCharacters = new List<int>();
    
    // 累計獲得お金（将来的な通貨システム用）
    public int TotalMoney = 0;
    
    // ステージごとのベストスコア
    public Dictionary<string, int> StageBestScores = new Dictionary<string, int>();
    
    // 設定データ
    public SettingsData Settings = new SettingsData();
    
    // 最終プレイ日時
    public string LastPlayedDate = "";
}

[System.Serializable]
public class SettingsData
{
    // 音量設定
    public float MasterVolume = 1.0f;
    public float BGMVolume = 1.0f;
    public float SFXVolume = 1.0f;
    
    // 画面設定
    public int ScreenWidth = 1920;
    public int ScreenHeight = 1080;
    public bool FullScreen = true;
    
    // 操作設定
    public KeyCode PauseKey = KeyCode.P;
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `SaveFileName` | string | セーブファイル名 | "savegame.dat" |
| `AutoSaveEnabled` | bool | 自動保存を有効にするか | true |
| `AutoSaveInterval` | float | 自動保存の間隔（秒） | 30.0f |

### 6. UIManager（UI管理）

#### 機能
- ゲーム中のUI表示管理
- 一時停止メニュー
- ゲームオーバー/クリア画面の表示

#### Hierarchy構造
```
Canvas
├── GameplayUI (GameObject)
│   ├── ResourceUI (GameObject)
│   └── CharacterSelectUI (GameObject)
├── PauseMenu (GameObject)
│   ├── ResumeButton (Button)
│   ├── SettingsButton (Button)
│   └── QuitButton (Button)
├── GameOverScreen (GameObject)
│   ├── GameOverText (TextMeshProUGUI)
│   ├── RetryButton (Button)
│   └── MainMenuButton (Button)
└── VictoryScreen (GameObject)
    ├── VictoryText (TextMeshProUGUI)
    ├── NextStageButton (Button)
    └── MainMenuButton (Button)
```

#### Required Components
- `UIManager.cs` (スクリプト)
- `Canvas` (UnityEngine.UI.Canvas)
- 各種UI要素（Button, TextMeshProUGUI等）

#### C# Logic概要
```csharp
public class UIManager : MonoBehaviour
{
    // UIパネルの参照
    public GameObject GameplayUI;
    public GameObject PauseMenu;
    public GameObject GameOverScreen;
    public GameObject VictoryScreen;
    
    // 一時停止メニューを表示
    public void ShowPauseMenu();
    
    // 一時停止メニューを非表示
    public void HidePauseMenu();
    
    // ゲームオーバー画面を表示
    public void ShowGameOverScreen();
    
    // クリア画面を表示
    public void ShowVictoryScreen();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `GameplayUI` | GameObject | ゲームプレイ中のUI | - |
| `PauseMenu` | GameObject | 一時停止メニュー | - |
| `GameOverScreen` | GameObject | ゲームオーバー画面 | - |
| `VictoryScreen` | GameObject | クリア画面 | - |

## ゲームフロー

### 通常のゲームフロー

1. **MainMenu** → ステージ選択
2. **GameScene** → **ModeSelection** → ゲームモード選択（「English to Japanese」「Japanese to English」）
3. **ModeSelection** → **Playing** → ゲーム開始（英単語問題が表示される）
4. **Playing** → ゲームプレイ中（英単語問題に答えてお金を獲得、キャラクターを召喚）
5. **Victory** → クリア画面表示 → 次のステージ or メインメニュー
6. **GameOver** → ゲームオーバー画面表示 → リトライ or メインメニュー

### 一時停止フロー

1. **Playing** → `P`キー押下 → **Paused**
2. **Paused** → `P`キー押下 → **Playing**

### シーン遷移フロー

```
MainMenu.unity
    ↓ (ステージ選択)
StageSelect.unity
    ↓ (ステージ開始)
GameScene.unity
    ↓ (クリア/ゲームオーバー)
MainMenu.unity または StageSelect.unity
```

## イベントシステム

### 主要イベント

```csharp
// ゲーム状態変更
public static event Action<GameState> OnGameStateChanged;

// ゲーム開始
public static event Action OnGameStarted;

// ゲーム一時停止
public static event Action OnGamePaused;

// ゲーム再開
public static event Action OnGameResumed;

// ゲームオーバー
public static event Action OnGameOver;

// ゲームクリア
public static event Action OnGameVictory;

// シーン遷移開始
public static event Action<string> OnSceneTransitionStart;

// シーン遷移完了
public static event Action<string> OnSceneTransitionComplete;
```

## 実装の優先順位

1. **GameManager**: ゲーム状態管理の基盤
2. **VictoryCondition / GameOverCondition**: 基本的な勝敗判定
3. **UIManager**: UI表示管理
4. **SceneManager**: シーン遷移機能
5. **SaveManager**: セーブデータ機能

---

## 最終更新日

2026年1月16日

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-15 | 初版作成 | - |
| 2026-01-16 | ゲームモード選択機能の追加仕様を反映（GameStateにModeSelection追加、ゲームフローにモード選択ステップ追加） | - |
