# 開発ログ

## 現在の実装状況

### 完了項目

- [x] プロジェクト初期設定
- [x] 仕様書作成
  - [x] マスタープロジェクト仕様書
  - [x] Unity開発標準仕様書
  - [x] プレイヤーシステム仕様書
  - [x] ゲームループシステム仕様書
  - [x] 開発ログ
- [x] Phase 1: プロジェクト基盤構築
  - [x] フォルダ構成の作成
  - [x] タグとレイヤーの設定

### 未実装項目

- [ ] コアシステム実装
- [ ] キャラクターシステム実装
- [ ] 敵システム実装
- [ ] UIシステム実装
- [ ] 戦闘システム実装
- [ ] オーディオシステム実装

## 次にやるべきこと（ToDoリスト）

### Phase 1: プロジェクト基盤構築

#### 1.1 フォルダ構成の作成
- [x] `Assets/Scripts/` フォルダ構造を作成
- [x] `Assets/Prefabs/` フォルダ構造を作成
- [x] `Assets/Materials/`, `Assets/Textures/` 等のアセットフォルダを作成
- [x] タグとレイヤーの設定（Unity Editor）

**優先度**: 高  
**見積もり**: 30分

#### 1.2 基本スクリプトの作成
- [x] `HealthSystem.cs` - HP管理システム
- [x] `IDamageable.cs` - ダメージ可能インターフェース
- [x] `ObjectPool.cs` - オブジェクトプーリングシステム
- [x] `Extensions.cs` - ユーティリティ拡張メソッド

**優先度**: 高  
**見積もり**: 2時間

### Phase 2: プレイヤーシステム実装

#### 2.1 リソース管理
- [x] `ResourceManager.cs` の実装
- [x] `ResourceUI.cs` の実装（お金表示）
- [x] 時間経過によるお金増加の実装
- [x] お金の消費・取得機能の実装

**優先度**: 高  
**見積もり**: 3時間

#### 2.2 城システム
- [x] `PlayerCastle.cs` の実装
- [x] `EnemyCastle.cs` の実装
- [x] 城のHPバーUI実装（`CastleHealthBarUI.cs`）
- [x] 城のダメージフラッシュエフェクト実装（攻撃受けた時に一瞬赤くなる）
- [x] Unity Editorでの城システムセットアップ完了
  - `PlayerCastle` GameObject作成と設定完了
  - `EnemyCastle` GameObject作成と設定完了
  - 両方のHPバーUIセットアップ完了
  - 動作確認完了（両方の城、HPバー、HPテキストが正常に表示されることを確認）
  - ダメージフラッシュエフェクトの動作確認完了（攻撃受けた時に赤くフラッシュすることを確認）

**優先度**: 高  
**見積もり**: 4時間

#### 2.3 キャラクター召喚システム
- [x] `CharacterData.cs` (ScriptableObject) の作成
- [x] `CharacterSpawner.cs` の実装
- [x] Unity Editorでのキャラクター召喚システムセットアップ完了
  - `CharacterSpawner` GameObject作成と設定完了
  - `SpawnPoint_Player`作成と設定完了
  - `ResourceManager`との連携設定完了
- [ ] 召喚エフェクト（オプション）

**優先度**: 高  
**見積もり**: 3時間

#### 2.4 キャラクター選択UI
- [x] `CharacterSelectUI.cs` の実装
- [x] `CharacterButton.cs` の実装
- [x] Unity EditorでのUIセットアップ完了
  - `CharacterSelectPanel`の作成完了
  - `CharacterButton`の作成完了（3つ）
  - 各ボタンの子要素（Icon、CostText、LockedOverlay）の作成完了
  - `CharacterData`アセットの作成完了（Warrior、Archer、Mage）
  - `CharacterSelectUI`と`CharacterButton`の連携設定完了
  - 動作確認完了（ボタンクリック、ロック状態、お金の消費が正常に動作することを確認）
- [x] ボタンのクリック処理
- [x] お金が足りない時の視覚的フィードバック

**優先度**: 高  
**見積もり**: 4時間

#### 2.5 英単語学習システム
- [x] `GameMode.cs` - ゲームモード列挙型（EnglishToJapanese / JapaneseToEnglish）の実装
- [x] `WordData.cs` - 英単語データ構造体の実装
- [x] `WordLearningSystem.cs` - 英単語学習システムの実装
  - CSVファイルから英単語データを読み込む機能
  - ランダムな問題の生成
  - 選択肢の生成（正解1つ、重複なし）
  - タイマー機能（カウントダウン）
  - 正解/不正解の判定
  - お金の増加処理（正解時、残りタイマー秒数×倍率）
  - フィードバック表示（正解時「Get X Power!」、不正解時「Wrong!」、時間切れ時「Time over!」）
  - 間違えた問題の再出題機能（2〜3問後に再出題）
  - BGM再生システム（ゲームプレイ中）
  - 効果音再生システム（正解・不正解時）
- [x] `WordQuizUI.cs` - 英単語問題UIの実装
  - 問題表示
  - 選択肢表示（3つ）
  - タイマー表示
  - フィードバック表示
  - 時間切れ時の正解表示
  - 勝利・敗北時の待機画面でQuestionTextとCorrectAnswerTextを表示したままにする機能
- [x] `GameModeSelectUI.cs` - ゲームモード選択UIの実装
- [x] `ResourceManager.cs` - 時間経過によるお金の自動増加を無効化（enableAutoMoneyGenerationフラグ追加）
- [x] `WordLearningSystem.cs` - フィードバックメッセージ変更（「Bad!」→「Wrong!」「Time over!」、正解時の「Good!」削除）
- [x] サンプルの英単語CSVファイル作成（`Assets/Resources/Data/WordData.csv`）

**優先度**: 高  
**見積もり**: 4時間

### Phase 3: キャラクターシステム実装

#### 3.1 キャラクター基底クラス
- [x] `CharacterBase.cs` の実装
- [x] `CharacterMovementController.cs` の実装（旧`CharacterController.cs`からリネーム）
- [x] 移動システムの実装（右方向への自動移動）
- [x] 移動システムの修正（`FixedUpdate`と`MovePosition`を使用）
- [x] ライフゲージシステムの実装
  - `SimpleHealthBar.cs` - SpriteRenderer使用の2Dライフゲージ（実装済みだが、WorldSpaceHealthBarUIを採用）
  - `WorldSpaceHealthBarUI.cs` - World Space Canvas使用のライフゲージ（採用）
  - `CharacterBase`と`EnemyBase`でのライフゲージ自動生成機能
  - ダメージ受けた時の赤色フラッシュエフェクト
  - `HealthBarPrefab`の作成手順書（`docs/HealthBar_WorldSpace_Setup.md`）
- [x] 歩行エフェクトの実装
  - `CharacterMovementController.cs`に歩行エフェクト機能を追加
  - 上下の揺れ（Y軸）のみを実装（Z軸の回転は削除）
  - `LateUpdate`で`rb2D.position`のY座標を変更して歩行エフェクトを適用
  - 移動速度に応じてタイマーを調整し、歩行リズムを表現
  - Inspectorでパラメータ調整可能（揺れ幅、速度、有効/無効の切り替え）
- [ ] アニメーション統合（オプション）

**優先度**: 高  
**見積もり**: 5時間

#### 3.2 キャラクタータイプ実装
- [x] `Warrior.cs` - 近接戦闘型
- [x] `Archer.cs` - 遠距離攻撃型
- [x] `Mage.cs` - 魔法攻撃型
- [x] 各キャラクターのプレハブ作成（Unity Editorで手動設定完了）
  - `Character_Warrior.prefab` 作成完了
  - `Character_Archer.prefab` 作成完了
  - `Character_Mage.prefab` 作成完了
- [x] 城への攻撃機能実装（EnemyCastle/PlayerCastleタグの検出と攻撃）
- [x] キャラクタータイプのコード統一（Archerの構造を基準にWarriorとMageを統一）
- [x] 攻撃時の突進アニメーション実装（Warrior、Archer、Mage）

**優先度**: 中  
**見積もり**: 6時間

#### 3.3 キャラクターデータ設定
- [ ] ScriptableObjectアセットの作成
- [ ] 各キャラクターのステータス設定
- [ ] コスト、HP、攻撃力等のバランス調整

**優先度**: 中  
**見積もり**: 2時間

### Phase 4: 敵システム実装

#### 4.1 敵基底クラス
- [x] `EnemyBase.cs` の実装
- [x] `EnemyData.cs` の実装（ScriptableObject）
- [x] `EnemyController.cs` の実装
- [x] 敵の移動システム（左方向へ）
- [x] ライフゲージシステムの実装（`CharacterBase`と同様）
- [x] 歩行エフェクトの実装
  - `EnemyController.cs`に歩行エフェクト機能を追加
  - 上下の揺れ（Y軸）のみを実装（Z軸の回転は削除）
  - `LateUpdate`で`rb2D.position`のY座標を変更して歩行エフェクトを適用
  - 移動速度に応じてタイマーを調整し、歩行リズムを表現
- [ ] 敵のアニメーション統合（オプション）

**優先度**: 高  
**見積もり**: 4時間

#### 4.2 敵スポナー
- [x] `EnemySpawner.cs` の実装
- [x] 敵のスポーンタイミング設定
- [ ] ウェーブシステム（オプション）

**優先度**: 高  
**見積もり**: 3時間

#### 4.3 敵タイプ実装
- [x] `Enemy_Orc.cs` - 基本敵
- [x] `Enemy_Goblin.cs` - 弱い敵
- [x] 攻撃時の突進アニメーション実装（Enemy_Orc、Enemy_Goblin）
- [x] 敵のプレハブ作成（Unity Editorで手動設定が必要）
  - `Enemy_Orc.prefab` 作成完了
  - `Enemy_Goblin.prefab` 作成完了
  - `Enemy_Archer.prefab` 作成完了（遠距離攻撃型）
  - `Enemy_Mage.prefab` 作成完了（魔法攻撃型）
  - `EnemyData`アセットの作成・調整完了

### Phase 5: 戦闘システム実装

#### 5.1 ダメージシステム
- [ ] `DamageSystem.cs` の実装
- [ ] ダメージ計算ロジック
- [ ] 防御力の実装

**優先度**: 高  
**見積もり**: 3時間

#### 5.2 攻撃システム
- [ ] 近接攻撃の実装
- [ ] 遠距離攻撃の実装
- [ ] `Projectile.cs` の実装（矢、魔法弾等）
- [ ] 攻撃アニメーション

**優先度**: 高  
**見積もり**: 5時間

#### 5.3 衝突検出
- [ ] キャラクター同士の衝突検出
- [ ] 投射物の衝突検出
- [ ] 城への衝突検出

**優先度**: 高  
**見積もり**: 3時間

### Phase 6: ゲームループ実装

#### 6.1 ゲーム終了処理
- [x] `GameEndHandler.cs` の実装
  - 敵の城HPが0→「Win!」表示（緑）
  - プレイヤー城HPが0→「Lost!」表示（赤）
  - タイマー・スポーン・キャラクター・エネミーの動きを停止（Time.timeScale = 0f）
  - Win/Lost表示を維持したままゲームモード選択パネルを表示
- [x] `ResourceManager.ResetMoney()` の実装（Powerリセット用）
- [x] `PlayerCastle.ResetHealth()` / `EnemyCastle.ResetHealth()` の実装（HPリセット用）
- [x] `GameModeSelectUI.ResetGame()` の実装（ゲーム再開時のリセット処理）

**優先度**: 高  
**見積もり**: 3時間

#### 6.2 ゲームマネージャー（将来の拡張用）
- [ ] `GameManager.cs` の実装
- [ ] ゲーム状態管理
- [ ] 一時停止機能

**優先度**: 中  
**見積もり**: 3時間

#### 6.3 勝敗判定（GameEndHandlerに統合済み）
- [x] ゲーム終了判定（GameEndHandler.csに実装済み）
- [ ] `VictoryCondition.cs` の実装（オプション、GameEndHandlerで代替）
- [ ] `GameOverCondition.cs` の実装（オプション、GameEndHandlerで代替）

**優先度**: 中  
**見積もり**: 2時間

#### 6.3 UI管理
- [ ] `UIManager.cs` の実装
- [ ] 一時停止メニュー
- [ ] ゲームオーバー画面
- [ ] クリア画面

**優先度**: 高  
**見積もり**: 4時間

#### 6.4 シーン管理
- [ ] `SceneManager.cs` の実装
- [ ] シーン遷移機能
- [ ] フェード効果（オプション）

**優先度**: 中  
**見積もり**: 2時間

#### 6.5 ステージシステム
- [x] `StageManager.cs` の実装
  - ステージ数の管理（シングルトンパターン）
  - 勝利時のStage進行（`AdvanceStage()`）
  - 敗北時のStageリセット（`ResetStage()`）
  - Stageに応じたスポーン間隔倍率の計算（`GetSpawnIntervalMultiplier()`）
- [x] `StageUI.cs` の実装
  - 画面右上に「Stage: X」を表示
  - StageManagerの変更イベントを購読して自動更新
- [x] `GameEndHandler.cs` 修正
  - 勝利/敗北フラグ（`isVictory`）を追加
  - `IsVictory`プロパティで外部から状態を取得可能に
- [x] `GameModeSelectUI.cs` 修正
  - ゲームモード選択時に`HandleStageManagement()`を呼び出し
  - 勝利時はStageを進める、敗北時はStageをリセット
- [x] `EnemySpawner.cs` 修正
  - `baseSpawnInterval`を保存
  - `StartSpawning()`時にStageに応じてスポーン間隔を調整
  - Stage 1: 1.0倍、Stage 2: 0.9倍、Stage 3: 0.81倍、Stage 4: 0.729倍...
- [x] Unity Editorセットアップガイド作成（`docs/22_StageSystem_Setup.md`）

**優先度**: 高  
**見積もり**: 3時間

#### 6.6 背景システム（完了）
- [x] `BackgroundManager.cs` の実装
  - ステージごとに背景を切り替える機能
  - `StageManager`の`OnStageChanged`イベントを購読して自動切り替え
  - `Background` GameObjectのSpriteRendererを使用したシンプルな背景切り替え機能
  - タイル表示機能を削除し、シンプルなSprite切り替えに変更
- [x] Unity Editorセットアップ完了
  - `BackgroundManager` GameObject作成済み
  - `Background` GameObject作成済み（SpriteRendererコンポーネント付き）
  - `Background Manager`スクリプトアタッチ済み
  - `Stage Backgrounds`にステージ1の背景画像（`maptile_tsuchi_01`）設定済み
  - `Background` GameObjectの自動検出機能実装済み
- [x] Unity Editorセットアップガイド作成（`docs/23_BackgroundSystem_Setup.md`）
- [x] 調査ドキュメント作成（`docs/24_BackgroundTile_Investigation.md`）

**優先度**: 中  
**見積もり**: 2時間（完了）

**実装内容（2026年1月20日）**:
- 背景システムのスクリプト実装完了
- タイル表示機能を削除し、シンプルなSprite切り替え機能に変更
- `Background` GameObjectを自動検出する機能を実装
- ステージ変更時に自動的に背景が切り替わる機能を実装
- Unity Editorでのセットアップ完了

### Phase 7: セーブシステム実装

#### 7.1 セーブマネージャー
- [ ] `SaveManager.cs` の実装
- [ ] セーブデータ構造の実装
- [ ] JSON形式での保存・読み込み

**優先度**: 中  
**見積もり**: 3時間

#### 7.2 プログレス管理
- [ ] クリアステージ数の保存
- [ ] アンロックキャラクターの管理
- [ ] 設定データの保存

**優先度**: 低  
**見積もり**: 2時間

### Phase 8: オーディオシステム実装

#### 8.1 オーディオマネージャー
- [ ] `AudioManager.cs` の実装
- [ ] BGM再生システム
- [ ] SFX再生システム

**優先度**: 中  
**見積もり**: 3時間

#### 8.2 オーディオアセット
- [ ] BGMの追加
- [ ] 戦闘SEの追加
- [ ] UI SEの追加

**優先度**: 低  
**見積もり**: 2時間（アセット準備含む）

### Phase 9: ポリッシュ

#### 9.1 エフェクト
- [ ] 召喚エフェクト
- [ ] 攻撃エフェクト
- [ ] ダメージエフェクト
- [ ] パーティクルシステムの実装

**優先度**: 低  
**見積もり**: 4時間

#### 9.2 UI改善
- [ ] アニメーション追加
- [ ] フィードバック改善
- [ ] 視覚的ポリッシュ

**優先度**: 低  
**見積もり**: 3時間

#### 9.3 バランス調整
- [ ] キャラクターのステータス調整
- [ ] 敵のステータス調整
- [ ] お金の増加率調整
- [ ] 難易度調整

**優先度**: 中  
**見積もり**: 継続的

## 技術的課題・検討事項

### 未解決の課題

1. **2D vs 3D**: ゲームを2Dで実装するか3Dで実装するか決定が必要
   - 推奨: 2D（にゃんこ大戦争に近い見た目）
   - 検討: Sprite2Dを使用

2. **アニメーション**: アニメーションシステムの選択
   - 推奨: Animator Controller + Animation Clip
   - 検討: ドット絵アニメーション vs 3Dモデルアニメーション

3. **パスファインディング**: キャラクターの移動経路
   - 推奨: 単純な右方向への移動（道が一直線の場合）
   - 検討: NavMesh（将来的に複雑な経路が必要な場合）

4. **オブジェクトプーリング**: パフォーマンス最適化
   - 必須: キャラクター、敵、投射物のプーリング実装

### 将来の拡張機能

- [ ] ステージ選択画面
- [ ] キャラクター強化システム
- [ ] アチーブメントシステム
- [ ] ランキングシステム
- [ ] モバイル対応（タッチ操作）

## メモ

### 開発環境
- Unity 2022.3 LTS以降推奨
- Visual Studio / Rider 推奨

### 参考資料
- にゃんこ大戦争のゲームプレイ動画
- Unity公式ドキュメント
- タワーディフェンスゲームの設計パターン

---

## 現在の作業状況（中断時点）

### 実装済み
- Phase 1: プロジェクト基盤構築（完了）
- Phase 8: オーディオシステム実装（効果音・BGM再生システム実装完了）
- Phase 2.1: リソース管理システム（スクリプト実装完了、Unity Editorセットアップ完了）
  - `ResourceManager.cs` - リソース管理システム
  - `ResourceUI.cs` - リソースUI管理
  - Unity Editorでのセットアップ完了（動作確認済み）
- Phase 2.2: 城システム（スクリプト実装完了、Unity Editorセットアップ完了）
  - `PlayerCastle.cs` - プレイヤーの城
  - `EnemyCastle.cs` - 敵の城
  - `CastleHealthBarUI.cs` - 城のHPバーUI
  - 城のダメージフラッシュエフェクト実装（攻撃受けた時に一瞬赤くなる）
  - Unity Editorでのセットアップ完了（動作確認済み：両方の城、HPバー、HPテキストが正常に表示されることを確認）
  - ダメージフラッシュエフェクトの動作確認完了（攻撃受けた時に赤くフラッシュすることを確認）
- Phase 2.3: キャラクター召喚システム（スクリプト実装完了、Unity Editorセットアップ完了）
  - `CharacterData.cs` - キャラクターデータ（ScriptableObject）
  - `CharacterSpawner.cs` - キャラクター召喚システム
  - Unity Editorでのセットアップ完了（`CharacterSpawner`、`SpawnPoint_Player`、`ResourceManager`連携設定済み）
- Phase 2.4: キャラクター選択UI（スクリプト実装完了、Unity Editorセットアップ完了）
  - `CharacterSelectUI.cs` - キャラクター選択UI管理
  - `CharacterButton.cs` - キャラクターボタン管理
  - Unity Editorでのセットアップ完了（動作確認済み：3つのボタン、CharacterDataアセット、UI連携が正常に動作することを確認）
- Phase 3.1: キャラクター基底クラス（スクリプト実装完了、動作確認完了）
  - `CharacterBase.cs` - キャラクター基底クラス
  - `CharacterMovementController.cs` - キャラクター移動コントローラー（移動システム）
  - 動作確認完了（TestCharacterが右方向へ移動することを確認）
- Phase 3.1: ライフゲージシステム（スクリプト実装完了、プレハブ作成完了、動作確認完了）
  - `SimpleHealthBar.cs` - SpriteRenderer使用の2Dライフゲージ（実装済みだが、WorldSpaceHealthBarUIを採用）
  - `WorldSpaceHealthBarUI.cs` - World Space Canvas使用のライフゲージ（採用方式）
  - `CharacterBase`と`EnemyBase`でのライフゲージ自動生成機能
  - ダメージ受けた時の赤色フラッシュエフェクト
  - `HealthBarPrefab`の作成手順書作成（`docs/HealthBar_WorldSpace_Setup.md`）
  - 動作確認完了（キャラクター・エネミーの上部にライフゲージが表示され、ダメージを受けるとFillが減少することを確認）
- Phase 3.2: キャラクタータイプ実装（スクリプト実装完了、プレハブ作成完了、動作確認完了）
  - `Warrior.cs` - 近接戦闘型キャラクター
  - `Archer.cs` - 遠距離攻撃型キャラクター
  - `Mage.cs` - 魔法攻撃型キャラクター
  - 城への攻撃機能実装（EnemyCastle/PlayerCastleタグの検出と攻撃）
  - キャラクタータイプのコード統一（Archerの構造を基準にWarriorとMageを統一、nullチェック追加、不要なデバッグログ削除）
  - Unity Editorでのプレハブ作成完了（`Character_Warrior.prefab`、`Character_Archer.prefab`、`Character_Mage.prefab`）
  - 動作確認完了（キャラクター召喚、お金の消費、画面表示の更新が正常に動作することを確認）
- Phase 4.1: 敵基底クラス（スクリプト実装完了、ライフゲージ実装完了）
  - `EnemyBase.cs` - 敵基底クラス
  - `EnemyData.cs` - 敵データ（ScriptableObject）
  - `EnemyController.cs` - 敵移動コントローラー（左方向への移動システム）
  - ライフゲージシステム実装（`CharacterBase`と同様）
- Phase 4.2: 敵スポナー（スクリプト実装完了）
  - `EnemySpawner.cs` - 敵スポーンシステム
- Phase 4.3: 敵タイプ実装（スクリプト実装完了、プレハブ作成完了）
  - `Enemy_Orc.cs` - 基本敵タイプ
  - `Enemy_Goblin.cs` - 弱い敵タイプ
  - `Enemy_Archer.cs` - 遠距離攻撃型敵タイプ（新規実装）
  - `Enemy_Mage.cs` - 魔法攻撃型敵タイプ（新規実装）
  - 敵プレハブ作成完了（`Enemy_Orc.prefab`、`Enemy_Goblin.prefab`、`Enemy_Archer.prefab`、`Enemy_Mage.prefab`）
  - `EnemyData`アセットの作成・調整完了
- Phase 2.5: 英単語学習システム（スクリプト実装完了、Unity Editorセットアップ完了）
  - `GameMode.cs` - ゲームモード列挙型
  - `WordData.cs` - 英単語データ構造体
  - `WordLearningSystem.cs` - 英単語学習システム（CSV読み込み、問題生成、タイマー、判定）
    - ゲームモード選択後に`StartGame()`が呼ばれるまで問題を生成しないように修正
    - 非アクティブな`WordQuizUI`も検索できるように`FindObjectOfType(true)`を使用
    - 正解時・不正解時・時間切れ時すべてで正解を表示するように修正
  - `WordQuizUI.cs` - 英単語問題UI（問題・選択肢・タイマー表示、フィードバック）
    - `WordQuizPanel`が初期状態で非表示になるように修正（`Awake()`で`SetActive(false)`）
    - `ShowPanel()`メソッドを追加（ゲームモード選択後に表示）
    - 正解表示時に「Correct Answer:」ラベルを削除し、正解の単語のみ表示するように修正
    - `FeedbackPanel`の背景色を、正解時は緑、不正解時は赤に変更する機能を追加
    - 非アクティブな状態でも`TimerText`を検索できるように`UpdateTimer()`内で再検索処理を追加
  - `GameModeSelectUI.cs` - ゲームモード選択UI
    - モード選択後に`WordLearningSystem.StartGame()`を呼び出すように修正
    - 選択されたゲームモードを`WordLearningSystem.SetGameMode()`で設定するように修正
    - ゲームモード選択後にパネルを非表示にするように修正
  - `ResourceManager.cs` - 時間経過によるお金の自動増加を無効化（enableAutoMoneyGenerationフラグ追加）
  - サンプルCSVファイル作成（`Assets/Resources/Data/WordData.csv`）
  - Unity Editorセットアップガイド作成（`docs/09_Japanese_Font_Fix.md` - 日本語フォント文字化け対処法）
  - 間違えた問題リスト機能実装（`IncorrectAnswersListUI.cs`、`IncorrectAnswerListItemUI.cs`）
    - `WordLearningSystem.cs`に間違えた問題の記録機能を追加（`incorrectAnswersCount`ディクショナリ、`OnIncorrectAnswer()`でカウント、`GetIncorrectAnswersSorted()`で回数順にソート）
    - `GameEndHandler.cs`に間違えた問題リスト表示機能を統合（ゲーム終了時に`ShowIncorrectAnswersList()`を呼び出し、`ResetGameState()`でリセット）
    - Unity Editorセットアップガイド作成（`docs/11_IncorrectAnswersList_Setup.md` - 間違えた問題リストUIセットアップ手順）
    - 音声読み上げ機能実装：`IncorrectAnswerListItemUI.cs`に`MacOSTextToSpeech`を使用した音声読み上げ機能を追加（アイテム全体をクリックすると英語を読み上げる）
    - `IncorrectAnswerListItemUI.cs`修正：`IPointerClickHandler`を実装してアイテム全体をクリック可能に変更

### 解決済みの問題
- **ResourceUIの表示問題**: 解決済み
  - 原因1：GameビューのScale設定の問題（最初の問題）
  - 原因2：`ResourcePanel`にも`ResourceUI`コンポーネントがアタッチされていた（重複）
  - 原因3：`Canvas`の`ResourceUI`の`Money Format`に「お金: {0}」が残っていた（日本語文字化け）
  - 解決方法：
    1. GameビューのScaleを調整
    2. `ResourcePanel`の`ResourceUI`コンポーネントを削除（不要だった）
    3. `Canvas`の`ResourceUI`の`Money Format`を「Money: {0}」に変更
  - デバッグログを削除し、コードをクリーンアップ済み
- **城への攻撃ができない問題**: 解決済み
  - 原因：WarriorとMageの`CharacterData`の`AttackRange`が城に届く距離に設定されていなかった（Archerだけが動作していた）
  - 解決方法：
    1. Unity Editorで`CharacterData`アセットの`AttackRange`を確認し、適切な値に設定（Warrior: 2.0以上、Mage: 4.0以上、Archer: 5.0以上を推奨）
    2. WarriorとMageの`FindNearestEnemy()`メソッドに`characterBase`のnullチェックを追加（Archerと同じ構造に統一）
    3. 不要なデバッグログを削除し、コードをクリーンアップ
- **CharacterSpawnerのバグ修正**: 解決済み
  - 問題：お金が足りているのに「お金が足りない」という警告が表示される
  - 原因：`CanSpawnCharacter`メソッドでプレハブが設定されていない場合も`false`を返していたため、お金のチェックが正しく動作していなかった
  - 解決方法：
    1. `CanSpawnCharacter`からプレハブチェックを削除（お金のチェックと利用可能キャラクターリストのチェックのみに変更）
    2. `SpawnCharacter`メソッドでエラーメッセージを分離（お金が足りない場合とプレハブが設定されていない場合で適切なメッセージを表示）
    3. プレハブが設定されていない場合、お金は消費されないように修正
- **CharacterControllerの名前衝突問題**: 解決済み
  - 問題：`CharacterController`という名前がUnityの組み込みコンポーネントと衝突し、`AddComponent`や`GetComponent`が動作しない
  - 解決方法：`CharacterController.cs`を`CharacterMovementController.cs`にリネームし、すべての参照を更新
- **キャラクター移動システムの問題**: 解決済み
  - 問題：`TestCharacter`が右方向へ移動しない（`velocity`は設定されているが、実際には移動していない）
  - 原因：`Kinematic`の`Rigidbody2D`で`Update`内で`velocity`を設定していたため、物理演算と正しく同期されていなかった
  - 解決方法：
    1. 移動処理を`Update`から`FixedUpdate`に変更（物理演算と同期）
    2. `Kinematic`の`Rigidbody2D`では`velocity`の代わりに`MovePosition`を使用
    3. `Time.deltaTime`の代わりに`Time.fixedDeltaTime`を使用
- **ResourceManagerの複数インスタンス問題**: 解決済み
  - 問題：画面左上のお金が増えるだけで減らない。内部のお金は減っているが、表示が更新されない
  - 原因：`ResourceManager`が複数存在し、`CharacterSpawner`と`CharacterSelectUI`が削除される`ResourceManager`インスタンスを参照していた。また、`ResourceUI`は正しい`ResourceManager`インスタンスを参照していたため、お金の消費と表示が同期していなかった
  - 解決方法：
    1. `ResourceManager`をシングルトンパターンに変更（`Instance`プロパティを追加）
    2. `ResourceManager.Awake`で重複インスタンスを自動削除
    3. `CharacterSpawner`と`CharacterSelectUI`の`Start`で強制的に`ResourceManager.Instance`を使用するように修正（Inspectorで設定されていてもシングルトンインスタンスを優先）
    4. `ResourceUI`も`ResourceManager.Instance`を使用するように修正
- **ResourceManagerのinitialMoneyが反映されない問題**: 解決済み
  - 問題：Inspectorで設定した`initialMoney`が反映されない（`currentMoney`がプレハブやシーンに保存された値で初期化される）
  - 原因：`currentMoney`が`[SerializeField]`でシリアライズされているため、プレハブやシーンに保存された値が読み込まれ、`Awake()`で`currentMoney`が既に値を持っている場合に`initialMoney`が反映されない
  - 解決方法：
    1. `Awake()`の80-93行目の条件分岐を修正し、常に`currentMoney = initialMoney;`を実行するように変更
    2. `Start()`でも確実に`initialMoney`を反映するように、`instance == this`の場合に`currentMoney = initialMoney;`を実行

### 次回の作業内容
1. 間違えた問題リスト機能の動作確認・デバッグ
   - ゲーム終了時に間違えた問題リストが正しく表示されるか確認
   - 間違えた問題が回数順（多い順）でソートされているか確認
   - 英語/英熟語、日本語、間違えた回数が正しく表示されるか確認
   - ゲーム再開時にリストがリセットされているか確認
   - 症状が変わらない原因を特定・修正
2. Phase 5（戦闘システム実装）に進む（間違えた問題リスト機能が動作確認完了後）
   - `DamageSystem.cs`の実装
   - 投射物システム（`Projectile.cs`）の実装
   - 衝突検出システムの実装

### 技術的なメモ
- `Money Format`のデフォルト値を英語（`Money: {0}`）に設定済み（日本語文字化け対策）
- `MoneyText`の自動検出機能を実装済み（`GetComponentInChildren`を使用）
- デバッグログは削除済み（本番コードとしてクリーンアップ）
- **重要**: `ResourceUI`コンポーネントは`Canvas`にのみアタッチする（`ResourcePanel`には不要）
- `Money Format`はInspectorで`{0}`または`Money: {0}`に設定（日本語は文字化けするため避ける）
- **城システム**: `PlayerCastle`と`EnemyCastle`は`HealthSystem`コンポーネントを使用してHP管理を行う
- **城システム**: `CastleHealthBarUI`は`HealthSystem`のイベントを購読して自動的にHPバーを更新する
- **城システム**: 防御力（Defense）はダメージ計算時に考慮される（最小ダメージは1）
- **キャラクター召喚システム**: `CharacterData`はScriptableObjectで、`CreateAssetMenu`属性によりUnity Editorでアセットを作成可能
- **キャラクター召喚システム**: `CharacterSpawner`は`ResourceManager`と連携して、お金が足りる場合のみキャラクターを召喚する
- **キャラクター召喚システム**: `CharacterSpawner`は`SpawnPoint`が設定されていない場合、自身のTransform位置を使用する
- **キャラクター召喚システム**: 召喚されたキャラクターには自動的に`Player`タグが設定される（`autoFaceRight`がtrueの場合、右を向く）
- **Unity Editorセットアップ**: `HealthSystem`コンポーネントは実行時に自動追加されるが、Inspector上で手動設定することも可能
- **キャラクター選択UI**: `CharacterSelectUI`は`CharacterSpawner`から利用可能なキャラクターリストを自動取得し、各ボタンに設定する
- **キャラクター選択UI**: `CharacterButton`は`ResourceManager`と連携して、お金が足りない場合に自動的にロック状態を表示する
- **キャラクター選択UI**: `CharacterButton`は子要素（Icon、CostText、LockedOverlay）を自動検出する機能を実装済み
- **キャラクター選択UI**: `HorizontalLayoutGroup`を使用する場合、`Layout Element`コンポーネントで`Preferred Width`と`Preferred Height`を設定することでボタンサイズを制御できる
- **キャラクター選択UI**: `CharacterSpawner`の`CanSpawnCharacter`メソッドはお金のチェックのみを行い、プレハブチェックは`SpawnCharacter`内で行う（エラーメッセージを適切に分離するため）
- **キャラクターシステム**: `CharacterBase`は`CharacterData`からステータスを取得し、`HealthSystem`を使用してHP管理を行う
- **キャラクターシステム**: `CharacterMovementController`は2D（Rigidbody2D）と3D（CharacterController）の両方に対応
- **キャラクターシステム**: `CharacterMovementController`は自動的に右方向へ移動し、死亡時は移動を停止する
- **キャラクターシステム**: `Kinematic`の`Rigidbody2D`を使用する場合、`FixedUpdate`内で`MovePosition`を使用することで物理演算と正しく同期される
- **キャラクターシステム**: `CharacterController`という名前はUnityの組み込みコンポーネントと衝突するため、`CharacterMovementController`にリネーム済み
- **キャラクターシステム**: `CharacterBase`は`IDamageable`を実装し、防御力を考慮したダメージ計算を行う
- **キャラクターシステム**: `IDamageable`インターフェースに`IsDead`プロパティを追加（死亡状態のチェック用）
- **キャラクターシステム**: `Warrior`、`Archer`、`Mage`は攻撃範囲内の敵を自動検出し、攻撃する
- **キャラクターシステム**: 敵が見つかった場合、移動を停止して攻撃する。敵が見つからない場合は移動を再開する
- **キャラクターシステム**: `Warrior`、`Archer`、`Mage`は`EnemyCastle`タグを持つ城も攻撃対象として検出し、攻撃する
- **キャラクターシステム**: `Archer`と`Mage`は投射物システム（Phase 5で実装予定）に対応する構造になっている
- **城システム**: `PlayerCastle`と`EnemyCastle`は攻撃を受けた時に一瞬赤くフラッシュするエフェクトを持つ（キャラクター・エネミーと同じ仕組み）
- **城システム**: 城の`SpriteRenderer`は自分自身または子オブジェクト（`CastleModel`）から自動検出される
- **ライフゲージシステム**: `WorldSpaceHealthBarUI`はWorld Space Canvasを使用して、キャラクター・エネミーの上部に追従するライフゲージを表示する
- **ライフゲージシステム**: `HealthBarPrefab`は`HealthBarPanel`（World Space Canvasの子）を含み、`WorldSpaceHealthBarUI`コンポーネントを`HealthBarPanel`にアタッチする
- **ライフゲージシステム**: `CharacterBase`と`EnemyBase`は`GetComponentInChildren`を使用して、プレハブ内の`WorldSpaceHealthBarUI`コンポーネントを検出する
- **ライフゲージシステム**: ダメージを受けたキャラクター・エネミーは一瞬だけ赤色にフラッシュする（`damageFlashDuration`と`damageFlashColor`で制御）
- **ライフゲージシステム**: `HealthBarPrefab`の`Fill` Imageは`Image Type`を`Filled`、`Fill Method`を`Horizontal`、`Fill Origin`を`Left`に設定する必要がある
- **リソース管理システム**: `ResourceManager`はシングルトンパターンで実装されており、シーンに1つだけ存在する。重複インスタンスは自動的に削除される
- **リソース管理システム**: `ResourceManager.Instance`プロパティを使用して、すべてのコンポーネントが同じインスタンスを参照する
- **リソース管理システム**: `CharacterSpawner`と`CharacterSelectUI`は`Start`で強制的に`ResourceManager.Instance`を使用するため、Inspectorで設定されていてもシングルトンインスタンスが優先される
- **リソース管理システム**: `ResourceUI`は`ResourceManager.Instance`を使用して、お金の変更イベントを購読し、画面表示を更新する
- **リソース管理システム**: `ResourceManager`の`Awake()`と`Start()`で常に`initialMoney`を反映するように修正済み（Inspectorで設定した値が確実に適用される）
- **リソース管理システム**: `ResourceManager`に`isGameStarted`フラグと`StartGame()`メソッドを追加。ゲームモード選択後からお金の自動増加が開始される（`enableAutoMoneyGeneration`が`true`の場合のみ）
- **キャラクター選択UI**: `CharacterSelectUI`に`SetPanelVisible()`と`SetButtonsEnabled()`メソッドを追加。ゲームモード選択前はパネルが非表示になり、ゲームモード選択後に表示される
- **キャラクター選択UI**: `GameModeSelectUI.OnModeSelected()`から`CharacterSelectUI.SetButtonsEnabled(true)`が呼び出され、ゲームモード選択後にキャラクターボタンが有効化される
- **敵システム**: `EnemyBase`は`CharacterBase`と同様の構造で、`IDamageable`を実装し、`HealthSystem`を使用してHP管理を行う
- **敵システム**: `EnemyData`は`CharacterData`と同様の構造で、ScriptableObjectとして実装されている
- **敵システム**: `EnemyController`は`CharacterMovementController`と同様の構造で、左方向への移動を制御する（`moveLeft = true`）
- **敵システム**: `EnemyController`は`Kinematic`の`Rigidbody2D`を使用する場合、`FixedUpdate`内で`MovePosition`を使用することで物理演算と正しく同期される
- **敵システム**: `EnemySpawner`は指定された間隔で敵を自動スポーンし、画面上の敵数を制限する機能を持つ
- **敵システム**: `Enemy_Orc`と`Enemy_Goblin`は攻撃範囲内のプレイヤーキャラクター（`Player`タグ）またはプレイヤー城（`PlayerCastle`タグ）を自動検出し、攻撃する
- **敵システム**: 敵が見つかった場合、移動を停止して攻撃する。敵が見つからない場合は移動を再開する
- **英単語学習システム**: `WordLearningSystem`はゲーム開始時にデータのみ読み込み、`StartGame()`が呼ばれるまで問題を生成しない（ゲームモード選択後に開始）
- **英単語学習システム**: `WordQuizUI`は初期状態で非アクティブ（`Awake()`で`SetActive(false)`）、`ShowPanel()`で表示される
- **英単語学習システム**: `FindObjectOfType<WordQuizUI>(true)`を使用することで、非アクティブなオブジェクトも検索可能
- **英単語学習システム**: 正解時・不正解時・時間切れ時のすべてで正解の単語が表示される（「Correct Answer:」ラベルなし）
- **英単語学習システム**: `FeedbackPanel`の背景色は、正解時は緑（`feedbackPanelCorrectColor`）、不正解時は赤（`feedbackPanelIncorrectColor`）に設定される
- **英単語学習システム**: TextMeshProで日本語を表示する場合、日本語対応のフォントアセットを設定する必要がある（文字化け対策）
- **英単語学習システム**: `GameModeSelectUI`は選択されたゲームモードを`WordLearningSystem.SetGameMode()`で設定し、その後`StartGame()`を呼び出す
- **英単語学習システム**: フィードバックメッセージは、不正解時「Wrong!」、タイムオーバー時「Time over!」、正解時は「Get X Power!」（「Good!」なし）
- **英単語学習システム**: 間違えた問題を2〜3回後に再出題する仕組みを実装（`incorrectQuestionsQueue`で管理、`retryInterval`で間隔を設定可能）
- **英単語学習システム**: 正解時のリワードは残りタイマー秒数×倍率（`powerRewardMultiplier`、デフォルト10）、四捨五入処理あり
- **英単語学習システム**: ゲームプレイ中にBGMをループ再生する機能を実装（`bgmAudioSource`で管理）
- **英単語学習システム**: 正解・不正解時に効果音を再生する機能を実装（`audioSource`で管理）
- **英単語学習システム**: 勝利・敗北時の待機画面で`QuestionText`と`CorrectAnswerText`を表示したままにする
- **ゲーム終了処理**: `GameEndHandler.cs`で城のHPが0になった時の処理を管理。敵の城HPが0→「Win!」（緑）、プレイヤー城HPが0→「Lost!」（赤）。ゲーム終了時はタイマー・スポーン・キャラクター・エネミーの動きを停止（Time.timeScale = 0f）。Win/Lost表示を維持したままゲームモード選択パネルを表示
- **ゲーム再開処理**: `GameModeSelectUI.ResetGame()`でゲーム再開時にPower・城のHPをリセット、フィールド上のキャラクター・エネミーを削除、Time.timeScaleを1fにリセット。`GameEndHandler.ResetGameState()`で`isGameEnded`フラグをリセット
- **エネミーのスポーン**: ゲームモード選択後に`EnemySpawner.StartSpawning()`が呼ばれる。`EnemySpawner.Start()`でautoSpawnを強制的にfalseに設定
- **攻撃時の突進アニメーション**: `Warrior`、`Archer`、`Mage`、`Enemy_Orc`、`Enemy_Goblin`に実装。攻撃時にターゲット方向に少し前進してから元の位置に戻る（コルーチンを使用）
- **城のHPバー**: `CastleHealthBarUI`で`healthBarFill`の検出方法を改善（名前で検索→Fillを含む名前で検索→最初のImage）。Image TypeがFilledでない場合は自動設定
- **オーディオシステム**: 各コンポーネントに`AudioSource`を追加し、効果音を再生（`PlayOneShot()`を使用）
- **効果音システム**: 正解・不正解時（`WordLearningSystem`）、スポーン時（`CharacterSpawner`、`EnemySpawner`）、攻撃時（`Warrior`、`Archer`、`Mage`、`Enemy_Orc`、`Enemy_Goblin`）、城破壊時（`PlayerCastle`、`EnemyCastle`）に効果音を再生
- **BGMシステム**: ゲームプレイ中にBGMをループ再生（`WordLearningSystem`）、勝利時・敗北時の待機画面でBGMをループ再生（`GameEndHandler`）
- **忘却曲線ロジック**: 間違えた問題をキューに追加し、2〜3問後に再出題するシンプルな仕組みを実装（`retryInterval`で間隔を設定可能）
- **ゲーム終了処理**: `HealthSystem.TakeDamage()`で重複発火を防ぐため、既に死亡している場合は`OnHealthDepleted`を発火しないように修正
- **勝利判定バグ修正**: `EnemyCastle`に`isDestroyedHandled`フラグを追加し、重複呼び出しを防止。`HandleHealthChanged()`にフォールバック検出を追加
- **ゲーム再開処理**: `GameEndHandler.ResetGameState()`メソッドを追加し、ゲーム再開時に`isGameEnded`フラグをリセット。`GameModeSelectUI.ResetGame()`から呼び出される
- **正解時のパワー計算**: 残りタイマー秒数×倍率（`powerRewardMultiplier`、デフォルト10）でパワーを計算。少数は四捨五入（`Mathf.RoundToInt()`）
- **間違えた問題リスト機能**: ゲーム終了時に間違えた問題を回数順（多い順）で表示する機能を実装。`WordLearningSystem`で間違えた回数を記録し、`IncorrectAnswersListUI`でリスト表示。`IncorrectAnswerListItemUI`で各アイテムの英語・日本語・回数を表示。アイテム全体をクリックすると`MacOSTextToSpeech`を使用して英語を読み上げる機能を実装。
- **シーン管理**: `GameSceneManager.cs`を作成（シーン間の遷移を管理するクラス）。ただし、現在はタイトルシーンを使用しないため、`SampleScene`のみで完結する構成に戻している。
- **ステージシステム**: `StageManager`でステージ数を管理（シングルトンパターン）。画面右上に`StageUI`で「Stage: X」を表示。勝利後、ゲームモード選択時にStageを進める。敗北後、ゲームモード選択時にStage1にリセット。`EnemySpawner`がStageに応じてスポーン間隔を調整（Stageが1進むごとに0.9倍）。`GameEndHandler`で勝利/敗北フラグを管理し、`GameModeSelectUI`でゲームモード選択時にステージ管理を実行。
- **城破壊時の効果音**: `EnemyCastle`と`PlayerCastle`に城破壊時の効果音機能を追加（`destroyedSound`、`audioSource`）
- **歩行エフェクト**: `CharacterMovementController`と`EnemyController`に歩行エフェクトを実装。上下の揺れ（Y軸）のみを適用し、`LateUpdate`で`rb2D.position`のY座標を変更することで物理演算と競合しないように実装。移動速度に応じてタイマーを調整し、歩行リズムを表現。Inspectorでパラメータ（揺れ幅、速度、有効/無効）を調整可能。Z軸の回転（傾き）は削除し、上下の揺れのみで歩行しているように見えるようにした。
- **背景システム**: `BackgroundManager`でステージごとに背景を切り替える機能を実装。`StageManager`の`OnStageChanged`イベントを購読して自動切り替え。`Background` GameObjectのSpriteRendererを使用したシンプルな背景切り替え機能。タイル表示機能は削除し、シンプルなSprite切り替えに変更。`Background` GameObjectを自動検出する機能を実装（`GameObject.Find("Background")`を使用）。Inspectorで設定されていても、常に`Background` GameObjectのSpriteRendererを参照するように修正。

### 動作確認済み項目
- [x] ResourceManagerの動作確認：お金が時間経過で自動増加することを確認
- [x] ResourceUIの表示確認：画面左上にお金が正しく表示されることを確認
- [x] PlayerCastleとEnemyCastleの動作確認：両方の城がシーンに表示されることを確認
- [x] HPバーUIの動作確認：両方のHPバーが表示され、HPテキストが「1000 / 1000」と表示されることを確認
- [x] HPバーのFill動作確認：HPバーのFillが満タン（Fill Amount = 1）であることを確認
- [x] キャラクター選択UIの動作確認：3つのボタンが画面下部に正しく表示されることを確認
- [x] ボタンのコスト表示確認：各ボタンの`CostText`に正しいコスト（50、75、100）が表示されることを確認
- [x] ボタンのロック状態確認：お金が足りない場合、ボタンがロック状態（グレーアウト）になることを確認
- [x] ボタンクリック動作確認：ボタンをクリックすると、適切なエラーメッセージが表示されることを確認（プレハブ未設定のため）
- [x] CharacterSpawnerのバグ修正確認：お金が足りている場合と足りない場合で、適切なメッセージが表示されることを確認
- [x] キャラクター移動システムの動作確認：TestCharacterが右方向へ移動することを確認（FixedUpdateとMovePositionを使用）
- [x] キャラクター召喚システムの動作確認：Warrior、Archer、Mageが正常にスポーンすることを確認
- [x] お金の消費と表示の同期確認：キャラクターを召喚すると、お金が正しく減り、画面左上の表示も正しく更新されることを確認
- [x] ResourceManagerのシングルトンパターン動作確認：すべてのコンポーネントが同じResourceManagerインスタンスを参照することを確認
- [x] ライフゲージシステムの動作確認：キャラクター・エネミーの上部にライフゲージが表示されることを確認
- [x] ライフゲージのFill動作確認：ダメージを受けるとFillが減少することを確認
- [x] ダメージフラッシュエフェクトの動作確認：ダメージを受けたキャラクター・エネミーが一瞬赤色にフラッシュすることを確認
- [x] GameModeSelectUIの動作確認：ゲームモード選択後に正しいモードが反映されることを確認
- [x] 城への攻撃機能の動作確認：キャラクター・エネミーが城に攻撃できることを確認
- [x] 城のダメージフラッシュエフェクトの動作確認：城が攻撃を受けた時に一瞬赤色にフラッシュすることを確認

2026年1月19日（開発ログ・仕様書更新：間違えた問題リスト機能の音声読み上げ機能実装、IPointerClickHandler実装、GameSceneManager作成、タイトルシーンを使用しない構成への変更を記録）

## 最終更新日

2026年1月20日（背景システム実装完了：BackgroundManager修正、タイル表示機能を削除してシンプルなSprite切り替え機能に変更、Background GameObjectの自動検出機能実装、Unity Editorセットアップ完了）

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-15 | 初版作成、ToDoリスト追加 | - |
| 2026-01-15 | Phase 1.1 フォルダ構成とタグ・レイヤー設定完了 | - |
| 2026-01-15 | Phase 1.2 基本スクリプト作成完了（IDamageable, HealthSystem, ObjectPool, Extensions） | - |
| 2026-01-15 | Phase 2.1 リソース管理システム実装完了（ResourceManager, ResourceUI） | - |
| 2026-01-15 | ResourceUIの表示問題を調査中。デバッグログ追加、Money Formatを英語に変更 | - |
| 2026-01-15 | ResourceUIの表示問題解決。GameビューのScale設定が原因。デバッグログ削除、コードクリーンアップ完了 | - |
| 2026-01-15 | ResourceUIの文字化け問題解決。ResourcePanelの重複コンポーネント削除、Money Formatを英語に変更 | - |
| 2026-01-16 | Phase 2.2 城システム実装完了（PlayerCastle, EnemyCastle, CastleHealthBarUI） | - |
| 2026-01-16 | Phase 2.3 キャラクター召喚システム実装完了（CharacterData, CharacterSpawner） | - |
| 2026-01-16 | Unity Editorでの城システムセットアップ完了。PlayerCastleとEnemyCastleの動作確認完了 | - |
| 2026-01-16 | Unity Editorでのキャラクター召喚システムセットアップ完了。CharacterSpawnerとResourceManagerの連携設定完了 | - |
| 2026-01-16 | Phase 2.4 キャラクター選択UI実装完了（CharacterSelectUI, CharacterButton） | - |
| 2026-01-16 | Unity EditorでのPhase 2.4セットアップ完了。CharacterSelectPanel、3つのCharacterButton、CharacterDataアセット（Warrior、Archer、Mage）の作成完了 | - |
| 2026-01-16 | CharacterSpawnerのバグ修正。CanSpawnCharacterからプレハブチェックを削除し、エラーメッセージを適切に分離 | - |
| 2026-01-16 | Phase 2.4の動作確認完了。ボタンクリック、ロック状態、お金の消費が正常に動作することを確認 | - |
| 2026-01-16 | Phase 3.1 キャラクター基底クラス実装完了（CharacterBase, CharacterMovementController） | - |
| 2026-01-16 | IDamageableインターフェースにIsDeadプロパティを追加 | - |
| 2026-01-16 | Phase 3.2 キャラクタータイプ実装完了（Warrior, Archer, Mage） | - |
| 2026-01-16 | CharacterControllerをCharacterMovementControllerにリネーム（Unity組み込みコンポーネントとの名前衝突を解決） | - |
| 2026-01-16 | キャラクター移動システム修正：FixedUpdateとMovePositionを使用するように変更。TestCharacterの動作確認完了 | - |
| 2026-01-16 | Phase 3.2 キャラクタープレハブ作成完了（Character_Warrior、Character_Archer、Character_Mage） | - |
| 2026-01-16 | ResourceManagerをシングルトンパターンに変更。複数インスタンスの問題を解決 | - |
| 2026-01-16 | CharacterSpawnerとCharacterSelectUIがResourceManager.Instanceを使用するように修正。お金の消費と表示の同期問題を解決 | - |
| 2026-01-16 | Phase 3.2動作確認完了。キャラクター召喚、お金の消費、画面表示の更新が正常に動作することを確認 | - |
| 2026-01-16 | Phase 4.1 敵基底クラス実装完了（EnemyBase, EnemyData, EnemyController） | - |
| 2026-01-16 | Phase 4.2 敵スポナー実装完了（EnemySpawner） | - |
| 2026-01-16 | Phase 4.3 敵タイプ実装完了（Enemy_Orc, Enemy_Goblin） | - |
| 2026-01-17 | Phase 2.5 英単語学習システム実装完了（GameMode, WordData, WordLearningSystem, WordQuizUI, GameModeSelectUI） | - |
| 2026-01-17 | ResourceManagerの時間経過によるお金の自動増加を無効化（enableAutoMoneyGenerationフラグ追加） | - |
| 2026-01-17 | サンプル英単語CSVファイル作成（Assets/Resources/Data/WordData.csv） | - |
| 2026-01-17 | 日本語フォント文字化け対処ガイド作成（docs/09_Japanese_Font_Fix.md） | - |
| 2026-01-17 | WordLearningSystem修正：ゲームモード選択後にゲームが開始されるように修正（StartGame()メソッド追加） | - |
| 2026-01-17 | WordQuizUI修正：WordQuizPanelが初期状態で非表示、ゲームモード選択後に表示されるように修正 | - |
| 2026-01-17 | WordLearningSystem修正：非アクティブなWordQuizUIも検索できるようにFindObjectOfType(true)を使用 | - |
| 2026-01-17 | WordQuizUI修正：正解表示時に「Correct Answer:」ラベルを削除し、正解の単語のみ表示 | - |
| 2026-01-17 | WordLearningSystem修正：正解時・不正解時にも正解を表示するように修正 | - |
| 2026-01-17 | WordQuizUI修正：FeedbackPanelの背景色を、正解時は緑、不正解時は赤に変更する機能を追加 | - |
| 2026-01-17 | ResourceManager修正：Inspectorで設定したinitialMoneyが確実に反映されるように修正（AwakeとStartで初期化） | - |
| 2026-01-17 | ResourceManager修正：ゲームモード選択後からお金の自動増加が開始されるように修正（isGameStartedフラグとStartGame()メソッド追加） | - |
| 2026-01-17 | CharacterSelectUI修正：ゲームモード選択前にパネルを非表示にするように修正（SetPanelVisible()メソッド追加） | - |
| 2026-01-17 | GameModeSelectUI修正：ゲームモード選択後にCharacterSelectUIのパネルを表示するように修正 | - |
| 2026-01-17 | Phase 3.1 ライフゲージシステム実装完了（WorldSpaceHealthBarUI、SimpleHealthBar、ダメージフラッシュエフェクト） | - |
| 2026-01-17 | CharacterBase/EnemyBase修正：ライフゲージ自動生成機能を追加、GetComponentInChildrenで子オブジェクトからコンポーネントを検出 | - |
| 2026-01-17 | GameModeSelectUI修正：選択されたゲームモードをWordLearningSystem.SetGameMode()で設定するように修正 | - |
| 2026-01-17 | HealthBarPrefab作成手順書作成（docs/HealthBar_WorldSpace_Setup.md） | - |
| 2026-01-17 | 城への攻撃機能実装完了（Warrior、Archer、MageがEnemyCastleに攻撃可能、Enemy_OrcがPlayerCastleに攻撃可能） | - |
| 2026-01-17 | WarriorとMageのコード統一（Archerと同じ構造に統一、nullチェック追加、不要なデバッグログ削除） | - |
| 2026-01-17 | 城のダメージフラッシュエフェクト実装完了（PlayerCastle、EnemyCastleが攻撃受けた時に一瞬赤くなる） | - |
| 2026-01-17 | CastleHealthBarUI修正：healthBarFillの検出方法改善、Image TypeがFilledでない場合の自動設定を追加 | - |
| 2026-01-17 | 英単語学習システム修正：フィードバックメッセージ変更（「Bad!」→「Wrong!」「Time over!」、正解時の「Good!」削除） | - |
| 2026-01-17 | ゲーム終了処理実装完了（GameEndHandler.cs）：敵の城HPが0→「Win!」、プレイヤー城HPが0→「Lost!」、タイマー・スポーン・キャラクター・エネミーの動きを停止、Win/Lost表示を維持したままゲームモード選択パネルを表示 | - |
| 2026-01-17 | ResourceManager修正：ResetMoney()メソッド追加（Powerリセット用） | - |
| 2026-01-17 | PlayerCastle/EnemyCastle修正：ResetHealth()メソッド追加（HPリセット用） | - |
| 2026-01-17 | GameModeSelectUI修正：ResetGame()メソッド追加（ゲーム再開時のリセット処理：Power・城のHP・フィールド上のキャラクター・エネミー削除） | - |
| 2026-01-17 | EnemySpawner修正：ゲームモード選択後にエネミーがスポーンするように変更（autoSpawnのデフォルト値をfalseに、Start()で強制的にfalseに設定） | - |
| 2026-01-17 | GameModeSelectUI修正：ゲームモード選択後にEnemySpawner.StartSpawning()を呼び出すように修正 | - |
| 2026-01-17 | WordQuizUI修正：ShowPanel()で非表示になったUI要素（タイマー・問題・選択肢）を再表示する処理を追加 | - |
| 2026-01-17 | 攻撃時の突進アニメーション実装完了（Warrior、Archer、Mage、Enemy_Orc、Enemy_Goblinに実装） | - |
| 2026-01-17 | オーディオシステム実装完了：正解・不正解時の効果音（WordLearningSystem）、スポーン時の効果音（CharacterSpawner、EnemySpawner）、攻撃時の効果音（Warrior、Archer、Mage、Enemy_Orc、Enemy_Goblin） | - |
| 2026-01-17 | 忘却曲線ロジック実装：間違えた問題を2〜3回後に再出題する仕組みを追加（WordLearningSystem） | - |
| 2026-01-17 | 正解時のリワードを10から30に変更（WordLearningSystem.moneyRewardOnCorrect） | - |
| 2026-01-17 | BGMシステム実装完了：ゲームプレイ中のBGM（WordLearningSystem）、勝利時・敗北時の待機画面BGM（GameEndHandler） | - |
| 2026-01-17 | WordQuizUI修正：勝利・敗北時の待機画面でQuestionTextとCorrectAnswerTextを表示したままにする修正 | - |
| 2026-01-18 | 敵の城のライフが0なのに勝利画面にならない問題の修正（EnemyCastle、HealthSystem、GameEndHandlerにデバッグログ追加、フォールバック検出追加） | - |
| 2026-01-18 | EnemyCastle修正：isDestroyedHandledフラグ追加（重複呼び出し防止）、HandleHealthChanged()にフォールバック検出追加 | - |
| 2026-01-18 | GameEndHandler修正：ResetGameState()メソッド追加（ゲーム再開時にisGameEndedフラグをリセット） | - |
| 2026-01-18 | GameModeSelectUI修正：ResetGame()内でGameEndHandler.ResetGameState()を呼び出すように修正 | - |
| 2026-01-18 | EnemyCastle修正：ResetHealth()でisDestroyedHandledフラグをリセットするように修正 | - |
| 2026-01-18 | WordLearningSystem修正：正解時のパワー計算を残りタイマー秒数×倍率に変更（moneyRewardOnCorrectをpowerRewardMultiplierに変更、四捨五入処理追加） | - |
| 2026-01-18 | EnemyCastle修正：城破壊時の効果音機能追加（destroyedSound、audioSource） | - |
| 2026-01-18 | PlayerCastle修正：城破壊時の効果音機能追加（destroyedSound、audioSource） | - |
| 2026-01-18 | IncorrectAnswerListItemUI修正：MacOSTextToSpeechを使用した音声読み上げ機能を追加（アイテム全体をクリックすると英語を読み上げる） | - |
| 2026-01-18 | IncorrectAnswerListItemUI修正：IPointerClickHandlerを実装してアイテム全体をクリック可能に変更 | - |
| 2026-01-18 | GameSceneManager作成：シーン管理システムを作成（タイトルシーンとゲームシーン間の遷移を管理）。ただし、最終的にタイトルシーンは使用しない方針に変更 | - |
| 2026-01-18 | GameEndHandler/GameModeSelectUI修正：タイトルシーンを使用しない構成に戻す（SampleSceneのみで完結） | - |
| 2026-01-18 | CharacterMovementController修正：歩行エフェクト実装完了（上下の揺れのみ、Z軸回転は削除、LateUpdateでrb2D.positionのY座標を変更して歩行しているように見えるエフェクトを実装） | - |
| 2026-01-18 | EnemyController修正：歩行エフェクト実装完了（上下の揺れのみ、Z軸回転は削除、LateUpdateでrb2D.positionのY座標を変更して歩行しているように見えるエフェクトを実装） | - |
| 2026-01-18 | 間違えた問題リスト機能実装完了（IncorrectAnswersListUI、IncorrectAnswerListItemUI、WordLearningSystemの記録機能、GameEndHandlerとの統合、Unity Editorセットアップガイド作成） | - |
| 2026-01-18 | GameEndHandler修正：ResetGameState()に間違えた問題リストの非表示とカウントリセット処理を追加 | - |
| 2026-01-19 | 開発ログ・仕様書更新：間違えた問題リスト機能の音声読み上げ機能実装、IPointerClickHandler実装、GameSceneManager作成、タイトルシーンを使用しない構成への変更を記録 | - |
| 2026-01-19 | ステージシステム実装完了：StageManager作成（ステージ数の管理、勝利時の増加、敗北時のリセット）、StageUI作成（画面右上に「Stage: X」を表示）、GameEndHandler修正（勝利/敗北フラグの管理）、GameModeSelectUI修正（ゲームモード選択時にステージ管理を実行）、EnemySpawner修正（Stageに応じてスポーン間隔を0.9倍に調整）、Unity Editorセットアップガイド作成（docs/22_StageSystem_Setup.md） | - |
| 2026-01-19 | 背景システム実装開始：BackgroundManager.cs作成（ステージごとの背景切り替え、タイル表示機能）、Unity Editorセットアップガイド作成（docs/23_BackgroundSystem_Setup.md）、調査ドキュメント作成（docs/24_BackgroundTile_Investigation.md）。問題：DrawModeがTiledからSlicedに変わってしまう、タイルが2枚分しか表示されない。原因調査中：Mesh TypeはFull Rectに設定済み、Background Rendererの参照設定が必要。次のステップ：Background Rendererの参照設定、Unity Editorで手動でDrawModeをTiledに設定、Sizeプロパティを適切に設定 | - |
| 2026-01-20 | 背景システム実装完了：BackgroundManager修正（タイル表示機能を削除、Background GameObjectのSpriteRendererを使用したシンプルな背景切り替え機能に変更）、Background GameObjectの自動検出機能実装（GameObject.Find("Background")を使用）、Awake()とUpdateBackground()で常にBackground GameObjectを検索して正しいSpriteRendererを参照するように修正、ステージ変更時に自動的に背景が切り替わる機能を実装、Unity Editorセットアップ完了 | - |