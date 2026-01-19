# 間違えた問題リスト機能 動作テストガイド

## テスト概要

ゲーム終了時に表示される「間違えた問題リスト」機能の動作をテストします。

## テスト前の確認事項

### 1. UIのセットアップ確認

以下の要素が正しくセットアップされているか確認してください：

- [ ] `IncorrectAnswersListPanel`がHierarchyの`Canvas`の子として存在する
- [ ] `IncorrectAnswersListPanel`が初期状態で非表示（非アクティブ）になっている
- [ ] `IncorrectAnswersListUI`コンポーネントが`IncorrectAnswersListPanel`にアタッチされている
- [ ] `IncorrectAnswersListUI`の参照が正しく設定されている：
  - `List Panel`: `IncorrectAnswersListPanel`自身
  - `Title Text`: `TitleText`（`IncorrectAnswersListPanel`の子）
  - `Scroll Rect`: `ScrollView`のScrollRectコンポーネント
  - `Content Parent`: `ScrollView/Viewport/Content`
  - `ListItem Prefab`: `Assets/Prefabs/UI/IncorrectAnswerListItem`プレハブ
- [ ] `IncorrectAnswerListItem`プレハブが正しく作成されている
- [ ] `GameEndHandler`の`Incorrect Answers List UI`フィールドに`IncorrectAnswersListPanel`が設定されている

### 2. スクリプトの確認

以下のスクリプトが正しく実装されているか確認してください：

- [ ] `WordLearningSystem.cs`の間違えた問題記録機能が実装されている
- [ ] `IncorrectAnswersListUI.cs`が実装されている
- [ ] `IncorrectAnswerListItemUI.cs`が実装されている
- [ ] `GameEndHandler.cs`の`ShowIncorrectAnswersList()`が実装されている

## テスト項目

### テスト1: 間違えた問題の記録確認

**目的**: 間違えた問題が正しく記録されているか確認

**手順**:
1. ゲームを開始（ゲームモードを選択）
2. 英単語問題で**意図的に間違えた選択肢を選ぶ**（最低3問）
3. 同じ問題を複数回間違える（最低2問を2回ずつ間違える）
4. 時間切れで間違える問題も作成する（制限時間を待つ）

**確認事項**:
- [ ] コンソールに間違えた問題のカウントログが表示される
  - `[WordLearningSystem] Incorrect answer count for '...': X`
- [ ] 時間切れ時にもカウントログが表示される
  - `[WordLearningSystem] Time-over answer count for '...': X`

**期待結果**:
- 間違えた問題が正しくカウントされる
- 時間切れ時も間違えた問題としてカウントされる

---

### テスト2: ゲーム終了時のリスト表示確認

**目的**: ゲーム終了時に間違えた問題リストが表示されるか確認

**手順**:
1. テスト1を実行（間違えた問題を作成）
2. ゲームを終了（敵の城のHPを0にする、またはプレイヤー城のHPを0にする）
3. ゲーム終了画面（「Win!」または「Lost!」）が表示されるまで待つ

**確認事項**:
- [ ] `IncorrectAnswersListPanel`が表示される
- [ ] タイトルが「間違えた問題リスト (X件)」と表示される（Xは間違えた問題の数）
- [ ] リストアイテムが表示される
- [ ] 各アイテムに英語/英熟語、日本語、間違えた回数（×X回）が表示される
- [ ] コンソールにリスト表示のログが表示される
  - `[IncorrectAnswersListUI] Showed X incorrect answers.`

**期待結果**:
- ゲーム終了時に間違えた問題リストが正しく表示される
- 間違えた問題がない場合はリストが表示されない（または空の状態で表示される）

---

### テスト3: 回数順（多い順）ソート確認

**目的**: 間違えた問題が回数順（多い順）でソートされているか確認

**手順**:
1. ゲームを開始
2. 複数の問題を間違える（回数を変える）：
   - 問題A: 3回間違える
   - 問題B: 1回間違える
   - 問題C: 2回間違える
3. ゲームを終了

**確認事項**:
- [ ] リストアイテムが回数順（多い順）で並んでいる
  - 問題A（×3回）が一番上
  - 問題C（×2回）が2番目
  - 問題B（×1回）が3番目

**期待結果**:
- 間違えた問題が回数順（多い順）で正しくソートされている

---

### テスト4: スクロール機能確認

**目的**: リストが長い場合にスクロールできるか確認

**手順**:
1. ゲームを開始
2. 多くの問題を間違える（10問以上）
3. ゲームを終了
4. リストが表示されたら、マウスホイールでスクロール

**確認事項**:
- [ ] リストがスクロールできる
- [ ] スクロールバーが表示される（リストが長い場合）

**期待結果**:
- リストがスクロール可能である
- スクロールバーが正しく表示される

---

### テスト5: ゲーム再開時のリセット確認

**目的**: ゲーム再開時にリストがリセットされるか確認

**手順**:
1. テスト2を実行（間違えた問題リストが表示される）
2. ゲームモード選択パネルからゲームモードを再選択
3. ゲームが再開される
4. 再度ゲームを終了

**確認事項**:
- [ ] ゲーム再開時に`IncorrectAnswersListPanel`が非表示になる
- [ ] ゲーム再開後、間違えた問題のカウントがリセットされる
- [ ] 再度ゲームを終了した時、新しい間違えた問題リストが表示される（前回のリストが残っていない）

**期待結果**:
- ゲーム再開時にリストが正しくリセットされる
- 間違えた問題のカウントがリセットされる

---

### テスト6: 間違いがない場合の動作確認

**目的**: 間違えた問題がない場合の動作を確認

**手順**:
1. ゲームを開始
2. すべての問題に正解する（間違えない）
3. ゲームを終了

**確認事項**:
- [ ] `IncorrectAnswersListPanel`が表示されない（または空の状態で表示される）
- [ ] コンソールにエラーが表示されない

**期待結果**:
- 間違えた問題がない場合、リストが表示されない（または空の状態で表示される）

---

## トラブルシューティング

### 問題1: リストが表示されない

**症状**: ゲーム終了時にリストが表示されない

**確認事項**:
1. `IncorrectAnswersListPanel`がHierarchyに存在するか確認
2. `IncorrectAnswersListUI`コンポーネントがアタッチされているか確認
3. `GameEndHandler`の`Incorrect Answers List UI`フィールドに`IncorrectAnswersListPanel`が設定されているか確認
4. コンソールにエラーメッセージが表示されていないか確認

**解決方法**:
- `GameEndHandler`の`Incorrect Answers List UI`フィールドに`IncorrectAnswersListPanel`を設定
- `IncorrectAnswersListUI`の各参照が正しく設定されているか確認

---

### 問題2: リストアイテムが表示されない

**症状**: リストパネルは表示されるが、アイテムが表示されない

**確認事項**:
1. `IncorrectAnswersListUI`の`ListItem Prefab`にプレハブが設定されているか確認
2. `Content Parent`が`ScrollView/Viewport/Content`を指しているか確認
3. `IncorrectAnswerListItem`プレハブが正しく作成されているか確認
4. `IncorrectAnswerListItem`プレハブに`IncorrectAnswerListItemUI`コンポーネントがアタッチされているか確認

**解決方法**:
- `IncorrectAnswersListUI`の`ListItem Prefab`に`IncorrectAnswerListItem`プレハブを設定
- `Content Parent`が正しいオブジェクトを指しているか確認
- `IncorrectAnswerListItem`プレハブの構造を確認（`EnglishText`、`JapaneseText`、`CountText`が存在するか）

---

### 問題3: 間違えた問題がカウントされない

**症状**: 間違えた問題を選んでもカウントが増えない

**確認事項**:
1. コンソールにカウントログが表示されるか確認
2. `WordLearningSystem`の`OnIncorrectAnswer()`メソッドが呼ばれているか確認
3. 時間切れ時もカウントされているか確認（`OnTimeUp()`メソッド）

**解決方法**:
- `WordLearningSystem`のデバッグログを確認
- 時間切れ時のカウント処理が追加されているか確認（最新の実装では追加済み）

---

### 問題4: リストがソートされていない

**症状**: 間違えた問題が回数順でソートされていない

**確認事項**:
1. `WordLearningSystem.GetIncorrectAnswersSorted()`メソッドが正しく実装されているか確認
2. コンソールにソート処理のログが表示されるか確認

**解決方法**:
- `GetIncorrectAnswersSorted()`メソッドの実装を確認（`OrderByDescending`を使用して回数が多い順にソート）

---

## テスト完了後の確認事項

すべてのテストが完了したら、以下を確認してください：

- [ ] すべてのテスト項目が完了している
- [ ] エラーや警告がコンソールに表示されていない
- [ ] リストが正しく表示される
- [ ] リストが正しくソートされる
- [ ] ゲーム再開時にリストがリセットされる
- [ ] 間違えた問題がない場合の動作が正しい

---

## テスト結果の記録

テストを実行したら、以下を記録してください：

- **テスト日**: YYYY-MM-DD
- **テスト実施者**: （名前）
- **テスト結果**: 
  - テスト1: ✅ / ❌
  - テスト2: ✅ / ❌
  - テスト3: ✅ / ❌
  - テスト4: ✅ / ❌
  - テスト5: ✅ / ❌
  - テスト6: ✅ / ❌
- **発見された問題**: 
  - （問題の説明）
- **修正内容**: 
  - （修正した内容）

---

これでテストは完了です！
