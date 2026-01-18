# 効果音実装ガイド

## 概要

ゲーム内の各種アクション時に効果音を再生する機能の実装手順です。

- 英単語問題の正解・不正解時の効果音
- キャラクター・敵のスポーン時の効果音

## Unity側での作業手順

### ステップ1: 音声ファイルの準備

1. **フォルダ構造の確認**
   - `Assets/Audio/SFX/UI/` フォルダが存在するか確認
   - 存在しない場合は作成：
     - プロジェクトウィンドウで `Assets/Audio/SFX/` を右クリック
     - `Create` → `Folder` を選択
     - フォルダ名を `UI` と入力

2. **効果音ファイルのインポート**
   - 正解時の効果音ファイルを用意（例: `correct_sound.wav`）
   - 不正解時の効果音ファイルを用意（例: `incorrect_sound.wav`）
   - 対応形式: `.wav`, `.mp3`, `.ogg`
   
3. **音声ファイルをUnityにインポート**
   - プロジェクトウィンドウで `Assets/Audio/SFX/UI/` フォルダを開く
   - ファイルエクスプローラー（Finder）から音声ファイルをドラッグ&ドロップ
   - または、`Assets/Audio/SFX/UI/` を右クリック → `Import New Asset...` でファイルを選択

4. **音声ファイルの設定（Inspector）**
   - インポートした音声ファイルをクリック
   - Inspectorで以下の設定を確認・変更：
     - **Load Type**: `Decompress On Load` または `Compressed In Memory`（短い効果音は `Decompress On Load` 推奨）
     - **Compression Format**: `PCM`（高品質）または `Vorbis`（ファイルサイズ削減）
     - **3D Sound**: オフ（2Dサウンドとして再生）

### ステップ2: GameManagerにAudioSourceコンポーネントを追加

1. **GameManagerオブジェクトを選択**
   - ヒエラルキーウィンドウで `GameManager` をクリック

2. **AudioSourceコンポーネントを追加**
   - Inspectorウィンドウで `Add Component` ボタンをクリック
   - 検索欄に `Audio Source` と入力
   - `Audio Source` を選択

3. **AudioSourceの設定**
   - Inspectorで以下の設定を行います：
     - **AudioClip**: 空のまま（コードで動的に設定）
     - **Play On Awake**: オフ（チェックを外す）
     - **Loop**: オフ（チェックを外す）
     - **Volume**: `0.7`（後で調整可能）
     - **Spatial Blend**: `0`（2Dサウンド）
     - **3D Sound Settings**: すべてデフォルト（使用しない）

### ステップ3: WordLearningSystemに効果音を設定

1. **WordLearningSystemコンポーネントを選択**
   - ヒエラルキーウィンドウで `GameManager` を選択（`WordLearningSystem`コンポーネントがアタッチされていることを確認）

2. **Inspectorで効果音を設定**
   - Inspectorで `Word Learning System` コンポーネントを展開
   - **効果音設定**セクションを確認：
     - **Correct Answer Sound**: プロジェクトウィンドウから正解時の効果音ファイルをドラッグ&ドロップ
     - **Incorrect Answer Sound**: プロジェクトウィンドウから不正解時の効果音ファイルをドラッグ&ドロップ
     - **Audio Source**: `GameManager` の `AudioSource` コンポーネントをドラッグ&ドロップ（または、コードが自動検出するため空のままでも可）

### ステップ4: 動作確認

1. **再生モードでテスト**
   - 再生ボタンをクリックしてゲームを開始
   - ゲームモードを選択（English to Japanese または Japanese to English）
   - 英単語問題に正解したときに正解音が再生されることを確認
   - 英単語問題に不正解したときに不正解音が再生されることを確認
   - 時間切れになったときに不正解音が再生されることを確認

2. **音量の調整**
   - 効果音が大きすぎる場合：`GameManager` の `AudioSource` の `Volume` を下げる（例: `0.5`）
   - 効果音が小さすぎる場合：`GameManager` の `AudioSource` の `Volume` を上げる（例: `1.0`）

## トラブルシューティング

### 効果音が再生されない場合

1. **AudioSourceの確認**
   - `GameManager` に `AudioSource` コンポーネントがアタッチされているか確認
   - `AudioSource` の `Mute` がオフになっているか確認

2. **音声ファイルの確認**
   - Inspectorで `Word Learning System` の `Correct Answer Sound` と `Incorrect Answer Sound` が設定されているか確認
   - 音声ファイルが正しくインポートされているか確認（プロジェクトウィンドウでクリックして、Inspectorに情報が表示されるか）

3. **システム音量の確認**
   - Unity Editorの音量設定（Edit → Project Settings → Audio）を確認
   - PCのシステム音量を確認

4. **デバッグログの確認**
   - Consoleウィンドウにエラーメッセージが表示されていないか確認

### AudioSourceが自動的に検出されない場合

- `WordLearningSystem` は `Awake()` で自動的に `AudioSource` を検出・作成します
- もし手動で設定したい場合は、Inspectorで `Audio Source` フィールドに `GameManager` の `AudioSource` をドラッグ&ドロップしてください

## コードの動作

### 効果音再生のタイミング

- **正解時**: `OnCorrectAnswer()` メソッド内で `PlaySound(correctAnswerSound)` が呼ばれます
- **不正解時**: `OnIncorrectAnswer()` メソッド内で `PlaySound(incorrectAnswerSound)` が呼ばれます
- **時間切れ時**: `OnTimeUp()` メソッド内で `PlaySound(incorrectAnswerSound)` が呼ばれます（不正解と同じ音）

### PlaySound() メソッド

```csharp
private void PlaySound(AudioClip clip)
{
    if (audioSource != null && clip != null)
    {
        audioSource.PlayOneShot(clip);
    }
}
```

- `PlayOneShot()` を使用することで、複数の効果音を重ねて再生できます
- `audioSource` または `clip` が `null` の場合は何も再生されません（エラーにならない）

## 次のステップ（オプション）

将来的に追加できる機能：

1. **AudioManagerクラスの作成**
   - すべての効果音を一元管理するシステム
   - BGMと効果音の音量を個別に調整
   - オーディオミキサーの設定

2. **その他の効果音**
   - ~~キャラクター召喚時の効果音~~（実装済み）
   - ~~敵のスポーン時の効果音~~（実装済み）
   - 攻撃時の効果音
   - 城がダメージを受けた時の効果音
   - ゲーム終了時（Win/Lost）の効果音

3. **オーディオ設定UI**
   - 音量調整スライダー
   - BGM/効果音のON/OFF切り替え

---

## スポーン時の効果音設定

### 概要

キャラクター召喚時と敵のスポーン時に効果音を再生する機能です。

### ステップ1: 音声ファイルの準備

1. **フォルダ構造の確認**
   - `Assets/Audio/SFX/` フォルダに以下を作成（必要に応じて）：
     - `Character/` フォルダ（キャラクター召喚時の効果音用）
     - `Enemy/` フォルダ（敵のスポーン時の効果音用）

2. **効果音ファイルのインポート**
   - キャラクター召喚時の効果音ファイルを用意（例: `character_spawn.wav`）
   - 敵のスポーン時の効果音ファイルを用意（例: `enemy_spawn.wav`）
   - 対応形式: `.wav`, `.mp3`, `.ogg`

3. **音声ファイルをUnityにインポート**
   - プロジェクトウィンドウで `Assets/Audio/SFX/Character/` フォルダにキャラクター召喚時の効果音を配置
   - プロジェクトウィンドウで `Assets/Audio/SFX/Enemy/` フォルダに敵のスポーン時の効果音を配置

### ステップ2: CharacterSpawnerにAudioSourceコンポーネントを追加

1. **CharacterSpawnerオブジェクトを選択**
   - ヒエラルキーウィンドウで `GameManager` → `CharacterSpawner` を選択

2. **AudioSourceコンポーネントを追加**
   - Inspectorウィンドウで `Add Component` ボタンをクリック
   - 検索欄に `Audio Source` と入力
   - `Audio Source` を選択

3. **AudioSourceの設定**
   - Inspectorで以下の設定を行います：
     - **AudioClip**: 空のまま（コードで動的に設定）
     - **Play On Awake**: オフ（チェックを外す）
     - **Loop**: オフ（チェックを外す）
     - **Volume**: `0.7`（後で調整可能）
     - **Spatial Blend**: `0`（2Dサウンド）

### ステップ3: EnemySpawnerにAudioSourceコンポーネントを追加

1. **EnemySpawnerオブジェクトを選択**
   - ヒエラルキーウィンドウで `GameManager` → `EnemySpawner` を選択

2. **AudioSourceコンポーネントを追加**
   - Inspectorウィンドウで `Add Component` ボタンをクリック
   - 検索欄に `Audio Source` と入力
   - `Audio Source` を選択

3. **AudioSourceの設定**
   - Inspectorで以下の設定を行います：
     - **AudioClip**: 空のまま（コードで動的に設定）
     - **Play On Awake**: オフ（チェックを外す）
     - **Loop**: オフ（チェックを外す）
     - **Volume**: `0.7`（後で調整可能）
     - **Spatial Blend**: `0`（2Dサウンド）

### ステップ4: CharacterSpawnerに効果音を設定

1. **CharacterSpawnerコンポーネントを選択**
   - ヒエラルキーウィンドウで `GameManager` → `CharacterSpawner` を選択

2. **Inspectorで効果音を設定**
   - Inspectorで `Character Spawner` コンポーネントを展開
   - **効果音設定**セクションを確認：
     - **Spawn Sound**: プロジェクトウィンドウからキャラクター召喚時の効果音ファイルをドラッグ&ドロップ
     - **Audio Source**: `CharacterSpawner` の `AudioSource` コンポーネントをドラッグ&ドロップ（または、コードが自動検出するため空のままでも可）

### ステップ5: EnemySpawnerに効果音を設定

1. **EnemySpawnerコンポーネントを選択**
   - ヒエラルキーウィンドウで `GameManager` → `EnemySpawner` を選択

2. **Inspectorで効果音を設定**
   - Inspectorで `Enemy Spawner` コンポーネントを展開
   - **効果音設定**セクションを確認：
     - **Spawn Sound**: プロジェクトウィンドウから敵のスポーン時の効果音ファイルをドラッグ&ドロップ
     - **Audio Source**: `EnemySpawner` の `AudioSource` コンポーネントをドラッグ&ドロップ（または、コードが自動検出するため空のままでも可）

### ステップ6: 動作確認

1. **再生モードでテスト**
   - 再生ボタンをクリックしてゲームを開始
   - ゲームモードを選択
   - キャラクターを召喚したときにスポーン音が再生されることを確認
   - 敵がスポーンしたときにスポーン音が再生されることを確認

2. **音量の調整**
   - 効果音が大きすぎる場合：`CharacterSpawner` または `EnemySpawner` の `AudioSource` の `Volume` を下げる
   - 効果音が小さすぎる場合：`AudioSource` の `Volume` を上げる

### コードの動作

#### キャラクター召喚時の効果音

- `CharacterSpawner.SpawnCharacter()` メソッド内で、キャラクターがインスタンス化された後に `PlaySpawnSound()` が呼ばれます

```csharp
private void PlaySpawnSound()
{
    if (audioSource != null && spawnSound != null)
    {
        audioSource.PlayOneShot(spawnSound);
    }
}
```

#### 敵のスポーン時の効果音

- `EnemySpawner.SpawnEnemy()` メソッド内で、敵がインスタンス化された後に `PlaySpawnSound()` が呼ばれます

```csharp
private void PlaySpawnSound()
{
    if (audioSource != null && spawnSound != null)
    {
        audioSource.PlayOneShot(spawnSound);
    }
}
```

### トラブルシューティング

#### 効果音が再生されない場合

1. **AudioSourceの確認**
   - `CharacterSpawner` および `EnemySpawner` に `AudioSource` コンポーネントがアタッチされているか確認
   - `AudioSource` の `Mute` がオフになっているか確認

2. **音声ファイルの確認**
   - Inspectorで `Character Spawner` の `Spawn Sound` が設定されているか確認
   - Inspectorで `Enemy Spawner` の `Spawn Sound` が設定されているか確認

3. **AudioSourceの自動検出**
   - `CharacterSpawner` と `EnemySpawner` は `Awake()` で自動的に `AudioSource` を検出・作成します
   - もし手動で設定したい場合は、Inspectorで `Audio Source` フィールドに `AudioSource` コンポーネントをドラッグ&ドロップしてください

---

最終更新日: 2026年1月17日（スポーン時の効果音実装完了）
