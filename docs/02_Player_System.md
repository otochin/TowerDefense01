# プレイヤーシステム仕様書

## 概要

プレイヤーシステムは、リソース管理、英単語学習、キャラクター召喚、城の管理を担当するシステムです。

## 主要コンポーネント

### 1. ResourceManager（リソース管理）

#### 機能
- Powerの消費・取得管理（英単語問題に正解することでPowerを獲得、時間経過による自動増加はなし）
- PowerのUI更新

#### Hierarchy構造
```
GameManager
└── ResourceManager (GameObject)
    └── ResourceUI (Canvas/UI)
```

#### Required Components
- `ResourceManager.cs` (スクリプト)

#### C# Logic概要
```csharp
public class ResourceManager : MonoBehaviour
{
    // 現在のPower
    public int CurrentMoney { get; private set; }
    
    // Powerの増加速度（秒間）
    public float MoneyGenerationRate = 1.0f;
    
    // Powerの増加量（1回あたり）
    public int MoneyPerGeneration = 10;
    
    // ゲームが開始されたかどうか（ゲームモード選択後からtrueになる）
    private bool isGameStarted = false;
    
    // Powerを追加
    public void AddMoney(int amount);
    
    // Powerを消費（可能かチェック）
    public bool TrySpendMoney(int amount);
    
    // Powerが足りるかチェック
    public bool CanAfford(int amount);
    
    // ゲームを開始する（ゲームモード選択後に呼び出す）
    // これにより、Powerの自動増加が開始される（enableAutoMoneyGenerationがtrueの場合）
    public void StartGame();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `InitialMoney` | int | 初期Power（ゲーム開始時のPower） | 100 |
| `MoneyGenerationRate` | float | Powerが増える間隔（秒）（※時間経過による自動増加は無効化、英単語問題に正解することでPowerを獲得） | 1.0 |
| `MoneyPerGeneration` | int | 1回あたりの増加量（※使用しない、英単語問題の正解時のPowerはWordLearningSystemで設定） | 10 |
| `MaxMoney` | int | Powerの上限（-1で無制限） | -1 |
| `EnableAutoMoneyGeneration` | bool | 時間経過によるPowerの自動増加を有効にするか（英単語学習機能ではfalse） | false |

#### 動作フロー
- **ゲームモード選択前**: `isGameStarted = false`のため、Powerの自動増加は行われない（`enableAutoMoneyGeneration`が`true`でも）
- **ゲームモード選択後**: `WordLearningSystem.StartGame()`から`ResourceManager.StartGame()`が呼び出され、`isGameStarted = true`になる。これにより、Powerの自動増加が開始される（`enableAutoMoneyGeneration`が`true`の場合のみ）

### 2. CharacterSpawner（キャラクター召喚）

#### 機能
- キャラクターの召喚処理
- 召喚位置の管理
- 召喚可能なキャラクターのリスト管理

#### Hierarchy構造
```
GameManager
└── CharacterSpawner (GameObject)
    ├── SpawnPoint_Player (GameObject)
    │   └── SpawnEffect (ParticleSystem) [オプション]
    └── CharacterDataList (ScriptableObject参照)
```

#### Required Components
- `CharacterSpawner.cs` (スクリプト)
- `Transform` (SpawnPoint用)

#### C# Logic概要
```csharp
public class CharacterSpawner : MonoBehaviour
{
    // 召喚可能なキャラクターリスト
    public List<CharacterData> AvailableCharacters;
    
    // 召喚地点
    public Transform SpawnPoint;
    
    // キャラクターを召喚
    public GameObject SpawnCharacter(CharacterData characterData);
    
    // 召喚可能かチェック（Powerが足りるか）
    public bool CanSpawnCharacter(CharacterData characterData);
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `AvailableCharacters` | List<CharacterData> | 召喚可能なキャラクターリスト | - |
| `SpawnPoint` | Transform | 召喚地点 | - |
| `SpawnOffset` | Vector3 | 召喚位置のオフセット | (0, 0, 0) |
| `AutoFaceRight` | bool | 召喚時に右を向くか | true |

### 3. PlayerCastle（プレイヤーの城）

#### 機能
- 城のHP管理
- 敵からのダメージ処理
- 城の破壊判定

#### Hierarchy構造
```
PlayerCastle (GameObject)
├── CastleModel (GameObject) [3DモデルまたはSprite]
├── HealthBar (UI)
│   ├── HealthBarFill (Image)
│   └── HealthBarText (TextMeshProUGUI)
└── CastleCollider (Collider)
```

#### Required Components
- `PlayerCastle.cs` (スクリプト)
- `Collider` (BoxCollider2D または BoxCollider)
- `HealthSystem.cs` (スクリプト)

#### C# Logic概要
```csharp
public class PlayerCastle : MonoBehaviour, IDamageable
{
    // HPシステム
    private HealthSystem healthSystem;
    
    // ダメージを受ける
    public void TakeDamage(int damage);
    
    // HPが0になった時の処理
    private void OnDestroyed();
    
    // HP更新イベント
    public event Action<int, int> OnHealthChanged;
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `MaxHealth` | int | 最大HP | 1000 |
| `CurrentHealth` | int | 現在のHP | 1000 |
| `Defense` | int | 防御力（ダメージ軽減） | 0 |
| `HealthBarUI` | GameObject | HPバーのUI | - |

### 4. CharacterSelectUI（キャラクター選択UI）

#### 機能
- 画面下部にキャラクターアイコンを表示
- タップ/クリックでキャラクターを召喚
- Powerが足りない場合の視覚的フィードバック
- ゲームモード選択前はパネル全体が非表示になり、ボタンを押すことができない
- ゲームモード選択後にパネルが表示され、ゲームが開始される

#### Hierarchy構造
```
Canvas
└── CharacterSelectPanel (GameObject)
    ├── CharacterButton_1 (Button)
    │   ├── Icon (Image)
    │   ├── CostText (TextMeshProUGUI)
    │   └── LockedOverlay (Image) [お金が足りない時]
    ├── CharacterButton_2 (Button)
    │   └── ...
    └── ...
```

#### Required Components
- `CharacterSelectUI.cs` (スクリプト)
- `Button` (UnityEngine.UI.Button)
- `Image` (アイコン用)
- `TextMeshProUGUI` (コスト表示用)

#### C# Logic概要
```csharp
public class CharacterSelectUI : MonoBehaviour
{
    // キャラクターボタンのリスト
    public List<CharacterButton> CharacterButtons;
    
    // ゲームが開始されたかどうか（ゲームモード選択後からtrueになる）
    private bool isGameStarted = false;
    
    // ボタンクリック時の処理
    private void OnCharacterButtonClicked(CharacterData characterData);
    
    // UI更新（Powerが変わった時）
    public void UpdateUI(int currentMoney);
    
    // ボタンの有効/無効を更新
    private void UpdateButtonStates();
    
    // パネルの表示/非表示を設定（ゲームモード選択後に呼び出す）
    public void SetPanelVisible(bool visible);
    
    // ボタンの有効/無効を設定（ゲームモード選択後に呼び出す）
    public void SetButtonsEnabled(bool enabled);
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `CharacterButtons` | List<CharacterButton> | キャラクターボタンのリスト | - |
| `ButtonPrefab` | GameObject | ボタンのプレハブ | - |
| `ButtonSpacing` | float | ボタン間の間隔 | 10.0f |

#### 動作フロー
- **ゲームモード選択前**: `Start()`で`SetPanelVisible(false)`が呼び出され、`CharacterSelectPanel`が非表示になる
- **ゲームモード選択後**: `GameModeSelectUI.OnModeSelected()`から`SetButtonsEnabled(true)`が呼び出され、`CharacterSelectPanel`が表示される
- **Powerの変更時**: `UpdateButtonStates()`が呼び出され、Powerが足りるかどうかでボタンの有効/無効が更新される（ゲームが開始されている場合のみ）

### 5. CharacterButton（キャラクターボタン）

#### 機能
- 個別のキャラクターボタンの管理
- クリック時の処理
- コスト表示とロック状態の管理

#### Required Components
- `CharacterButton.cs` (スクリプト)
- `Button` (UnityEngine.UI.Button)
- `Image` (アイコン用)
- `TextMeshProUGUI` (コスト表示用)

#### C# Logic概要
```csharp
public class CharacterButton : MonoBehaviour
{
    // キャラクターデータ
    public CharacterData CharacterData;
    
    // ボタンがクリックされた時のイベント
    public UnityEvent<CharacterData> OnButtonClicked;
    
    // ボタンの有効/無効を設定
    public void SetInteractable(bool interactable);
    
    // ロック状態を表示
    public void SetLocked(bool locked);
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `CharacterData` | CharacterData | このボタンに対応するキャラクターデータ | - |
| `IconImage` | Image | キャラクターアイコン | - |
| `CostText` | TextMeshProUGUI | コスト表示テキスト | - |
| `LockedOverlay` | Image | ロック時のオーバーレイ | - |



### 6. WordLearningSystem（英単語学習システム）

#### 機能
- CSVファイルから英単語データを読み込む
- ランダムな問題を生成
- 選択肢を生成（正解1つ、重複なし）
- タイマー機能（カウントダウン）
- 正解/不正解の判定
- Powerの増加処理（正解時）
- フィードバック表示（「Good! Get X Power!」「Bad!」）

#### Hierarchy構造
```
GameManager
└── WordLearningSystem (GameObject)
Canvas
└── WordQuizPanel (GameObject) [初期状態で非アクティブ]
    ├── TimerText (TextMeshProUGUI)
    ├── QuestionText (TextMeshProUGUI)
    ├── ChoicesPanel (GameObject)
    │   ├── ChoiceButton_1 (Button)
    │   │   └── Text (TMP)
    │   ├── ChoiceButton_2 (Button)
    │   │   └── Text (TMP)
    │   └── ChoiceButton_3 (Button)
    │       └── Text (TMP)
    └── FeedbackPanel (GameObject) [Imageコンポーネント付き]
        ├── FeedbackText (TextMeshProUGUI)
        └── CorrectAnswerText (TextMeshProUGUI)
```

#### Required Components
- `WordLearningSystem.cs` (スクリプト)
- `WordQuizUI.cs` (スクリプト)

#### C# Logic概要
```csharp
public enum GameMode
{
    EnglishToJapanese,  // 英→日
    JapaneseToEnglish   // 日→英
}

[System.Serializable]
public class WordData
{
    public string English;
    public string Japanese;
}

public class WordLearningSystem : MonoBehaviour
{
    // ゲームモード
    public GameMode CurrentGameMode;
    
    // CSVファイルパス
    public string WordDataCsvPath = "Data/WordData.csv";
    
    // Powerの増加量（正解時）
    public int MoneyRewardOnCorrect = 10;
    
    // 制限時間（秒）
    public float AnswerTimeLimit = 10.0f;
    
    // 正解表示時間（時間切れ時、秒）
    public float CorrectAnswerDisplayTime = 5.0f;
    
    // 英単語データリスト
    private List<WordData> wordDataList;
    
    // 現在の問題
    private WordData currentQuestion;
    private List<string> currentChoices;
    private int correctChoiceIndex;
    private float currentTimer;
    
    // 問題を読み込む
    public void LoadWordDataFromCsv(string csvPath);
    
    // ランダムな問題を生成
    public void GenerateRandomQuestion();
    
    // 選択肢を生成（正解1つ、重複なし）
    public List<string> GenerateChoices(WordData question);
    
    // ゲームモードを設定
    public void SetGameMode(GameMode mode);
    
    // ゲームを開始する（ゲームモード選択後に呼び出す）
    public void StartGame();
    
    // 選択肢をクリックした時の処理
    public void OnChoiceSelected(int choiceIndex);
    
    // タイマー更新
    private void UpdateTimer();
    
    // 次の問題へ
    public void NextQuestion();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `CurrentGameMode` | GameMode | 現在のゲームモード | EnglishToJapanese |
| `WordDataCsvPath` | string | 英単語データCSVファイルのパス | "Data/WordData.csv" |
| `MoneyRewardOnCorrect` | int | 正解時のPowerの増加量 | 10 |
| `AnswerTimeLimit` | float | 回答制限時間（秒） | 10.0 |
| `CorrectAnswerDisplayTime` | float | 時間切れ時の正解表示時間（秒） | 5.0 |

#### CSVファイル形式
```
English,Japanese
apple,りんご
book,本
cat,猫
```

### 7. WordQuizUI（英単語問題UI）

#### 機能
- 画面中央に問題を表示
- 選択肢を3つ表示
- タイマーを問題の上部に表示
- 正解/不正解のフィードバック表示（「Good! Get X Power!」「Bad!」）
- 正解時・不正解時・時間切れ時に正解を表示（ラベルなしで正解の単語のみ）
- FeedbackPanelの背景色を変更（正解時は緑、不正解時は赤）
- パネルの表示/非表示制御（初期状態で非アクティブ、ゲームモード選択後に表示される）

#### Hierarchy構造
```
Canvas
└── WordQuizPanel (GameObject)
    ├── TimerText (TextMeshProUGUI)
    ├── QuestionText (TextMeshProUGUI)
    ├── ChoicesPanel (GameObject)
    │   ├── ChoiceButton_1 (Button)
    │   ├── ChoiceButton_2 (Button)
    │   └── ChoiceButton_3 (Button)
    ├── FeedbackPanel (GameObject)
    │   ├── FeedbackText (TextMeshProUGUI)
    │   └── CorrectAnswerText (TextMeshProUGUI)
```

#### Required Components
- `WordQuizUI.cs` (スクリプト)
- `TextMeshProUGUI` (問題・タイマー・フィードバック表示用)
- `Button` (選択肢ボタン用)

#### C# Logic概要
```csharp
public class WordQuizUI : MonoBehaviour
{
    // UI要素の参照
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI QuestionText;
    public List<Button> ChoiceButtons;
    public TextMeshProUGUI FeedbackText;
    public TextMeshProUGUI CorrectAnswerText;
    public GameObject FeedbackPanel;
    
    // 色設定
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color feedbackPanelCorrectColor = new Color(0f, 1f, 0f, 0.3f); // 緑（半透明）
    public Color feedbackPanelIncorrectColor = new Color(1f, 0f, 0f, 0.3f); // 赤（半透明）
    
    // WordLearningSystemへの参照
    private WordLearningSystem wordLearningSystem;
    
    // パネルを表示する（ゲームモード選択後に呼び出す）
    public void ShowPanel();
    
    // パネルを非表示にする
    public void HidePanel();
    
    // タイマーを更新
    public void UpdateTimer(float remainingTime);
    
    // 問題を表示
    public void DisplayQuestion(string question);
    
    // 選択肢を表示
    public void DisplayChoices(List<string> choices);
    
    // フィードバックを表示（正解時・不正解時の両方で正解も表示）
    public void ShowFeedback(string feedback, bool isCorrect);
    
    // 正解を表示（ラベルなしで正解の単語のみ）
    public void ShowCorrectAnswer(string correctAnswer);
    
    // フィードバックを非表示
    public void HideFeedback();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `TimerText` | TextMeshProUGUI | タイマー表示テキスト | - |
| `QuestionText` | TextMeshProUGUI | 問題表示テキスト | - |
| `ChoiceButtons` | List<Button> | 選択肢ボタンのリスト（3つ） | - |
| `FeedbackText` | TextMeshProUGUI | フィードバック表示テキスト | - |
| `CorrectAnswerText` | TextMeshProUGUI | 正解表示テキスト（正解時・不正解時・時間切れ時に表示） | - |
| `FeedbackPanel` | GameObject | フィードバックパネル（Imageコンポーネント付き） | - |
| `CorrectColor` | Color | 正解時のテキスト色 | Color.green |
| `IncorrectColor` | Color | 不正解時のテキスト色 | Color.red |
| `FeedbackPanelCorrectColor` | Color | 正解時のパネル背景色（緑、半透明） | new Color(0f, 1f, 0f, 0.3f) |
| `FeedbackPanelIncorrectColor` | Color | 不正解時のパネル背景色（赤、半透明） | new Color(1f, 0f, 0f, 0.3f) |

### 8. GameModeSelectUI（ゲームモード選択UI）

#### 機能
- ゲーム開始時にゲームモードを選択
- 「English to Japanese」と「Japanese to English」の2つのモードから選択
- モード選択後に`WordLearningSystem.StartGame()`を呼び出してゲームを開始

#### Hierarchy構造
```
Canvas
└── GameModeSelectPanel (GameObject)
    ├── TitleText (TextMeshProUGUI)
    ├── ModeButton_EnglishToJapanese (Button)
    └── ModeButton_JapaneseToEnglish (Button)
```

#### Required Components
- `GameModeSelectUI.cs` (スクリプト)
- `Button` (モード選択ボタン用)
- `TextMeshProUGUI` (タイトル表示用)

#### C# Logic概要
```csharp
public class GameModeSelectUI : MonoBehaviour
{
    // モード選択ボタン
    public Button EnglishToJapaneseButton;
    public Button JapaneseToEnglishButton;
    
    // モード選択時のイベント
    public UnityEvent<GameMode> OnGameModeSelected;
    
    // モードを選択
    public void OnModeSelected(GameMode mode);
    
    // UIを表示
    public void Show();
    
    // UIを非表示
    public void Hide();
}
```

#### インスペクター変数
| 変数名 | 型 | 説明 | デフォルト値 |
|--------|-----|------|------------|
| `EnglishToJapaneseButton` | Button | 「English to Japanese」モード選択ボタン | - |
| `JapaneseToEnglishButton` | Button | 「Japanese to English」モード選択ボタン | - |

## システム間の連携

### データフロー

1. **GameModeSelectUI** → **WordLearningSystem**: ゲームモードを選択、`StartGame()`を呼び出してゲーム開始
2. **WordLearningSystem** → **WordQuizUI**: `ShowPanel()`でパネルを表示し、問題と選択肢を表示
3. **WordQuizUI** → **WordLearningSystem**: 選択肢クリック時の処理
4. **WordLearningSystem** → **ResourceManager**: 正解時にPowerを追加
5. **ResourceManager** → **CharacterSelectUI**: Powerが変更されたらUIを更新
6. **CharacterSelectUI** → **CharacterSpawner**: ボタンクリックでキャラクターを召喚
7. **CharacterSpawner** → **ResourceManager**: 召喚時にコストを消費
8. **Enemy** → **PlayerCastle**: 敵が城に到達したらダメージを与える

### イベントシステム

```csharp
// ゲームモードが選択された時
public static event Action<GameMode> OnGameModeSelected;

// 英単語問題が生成された時
public static event Action<WordData> OnQuestionGenerated;

// 問題に正解した時
public static event Action OnQuestionAnsweredCorrectly;

// 問題に不正解した時
public static event Action OnQuestionAnsweredIncorrectly;

// Powerが変更された時
public static event Action<int> OnMoneyChanged;

// キャラクターが召喚された時
public static event Action<CharacterData> OnCharacterSpawned;

// 城のHPが変更された時
public static event Action<int, int> OnCastleHealthChanged;

// 城が破壊された時
public static event Action OnPlayerCastleDestroyed;
```

## 実装の優先順位

1. **GameModeSelectUI**: ゲームモード選択UI（ゲーム開始時）
2. **WordLearningSystem**: 英単語学習システムの基盤
3. **WordQuizUI**: 英単語問題UI
4. **ResourceManager**: リソース管理の基盤（時間経過による自動増加は無効化）
5. **PlayerCastle**: 城のHP管理
6. **CharacterSpawner**: 基本的な召喚機能
7. **CharacterSelectUI**: UI実装
8. **CharacterButton**: ボタンの詳細実装

---

## 最終更新日

2026年1月17日（WordLearningSystem、WordQuizUIの仕様更新）

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-15 | 初版作成 | - |
| 2026-01-16 | 英単語学習機能の追加仕様を反映（ResourceManagerの自動増加を無効化、WordLearningSystem、WordQuizUI、GameModeSelectUIの追加） | - |
| 2026-01-17 | WordLearningSystemにStartGame()メソッド追加、ゲームモード選択後にゲームが開始される仕様を反映 | - |
| 2026-01-17 | WordQuizUIにShowPanel()/HidePanel()メソッド追加、初期状態で非アクティブの仕様を反映 | - |
| 2026-01-17 | WordQuizUIにFeedbackPanelの背景色変更機能追加、正解時は緑・不正解時は赤の仕様を反映 | - |
| 2026-01-17 | 正解表示を正解時・不正解時・時間切れ時のすべてで表示し、ラベルなしで正解の単語のみ表示する仕様を反映 | - |
