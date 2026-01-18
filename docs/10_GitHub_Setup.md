# GitHub管理セットアップ手順

## 概要

このドキュメントでは、Unityプロジェクト（TowerDefense01）をGitHubで管理するための詳しい手順を説明します。

## 前提条件

- GitHubアカウントが既に取得済みであること
- Gitがローカル環境にインストールされていること
- コマンドライン（ターミナル）が使用できること

## 手順

### ステップ1: Unity用の.gitignoreファイルの確認

プロジェクトルート（`TowerDefense01/`）に`.gitignore`ファイルが存在することを確認してください。このファイルにより、以下の不要なファイルがGit管理から除外されます：

- `Library/` - Unityが自動生成するファイル
- `Temp/` - 一時ファイル
- `Logs/` - ログファイル
- `obj/` - ビルド中間ファイル
- `*.csproj`, `*.sln` - IDEが自動生成するプロジェクトファイル（必要に応じて）
- `UserSettings/` - ユーザー固有の設定ファイル
- `.DS_Store` - macOSのシステムファイル

**重要**: `.gitignore`はプロジェクトルートに配置されている必要があります。

### ステップ2: GitHubでリポジトリを作成

1. GitHubのウェブサイト（https://github.com）にログインします
2. 画面右上の「+」ボタンをクリックし、「New repository」を選択します
3. 以下の情報を入力します：
   - **Repository name**: `TowerDefense01`（または任意の名前）
   - **Description**: 「タワーディフェンス × 英単語学習ゲーム」など、プロジェクトの説明
   - **Visibility**: `Public`（公開）または`Private`（非公開）を選択
4. **「Initialize this repository with a README」はチェックしないでください**（既存のプロジェクトをプッシュするため）
5. 「Create repository」ボタンをクリックします

### ステップ3: ローカルでGitリポジトリを初期化

ターミナル（またはコマンドプロンプト）を開き、プロジェクトのルートディレクトリに移動します：

```bash
cd /Users/arie-yoshiaki/Documents/game02/TowerDefense01
```

既にGitリポジトリが初期化されているか確認します：

```bash
git status
```

もし「not a git repository」と表示される場合は、Gitリポジトリを初期化します：

```bash
git init
```

メインブランチを`main`に設定します（GitHubの標準）：

```bash
git checkout -b main
```

または、既に`master`ブランチがある場合は：

```bash
git branch -M main
```

### ステップ4: ファイルをステージングしてコミット

プロジェクトのすべてのファイルをステージングします：

```bash
git add .
```

最初のコミットを作成します：

```bash
git commit -m "Initial commit: Unity project setup with tower defense game"
```

コミットメッセージは、プロジェクトの内容を表す適切なものに変更してください。

### ステップ5: GitHubリポジトリに接続してプッシュ

GitHubで作成したリポジトリのURLを確認します。リポジトリページに表示される「Quick setup」セクションにURLが表示されています。

リモートリポジトリを追加します（`YOUR_USERNAME`を実際のGitHubユーザー名に置き換えてください）：

```bash
git remote add origin https://github.com/YOUR_USERNAME/TowerDefense01.git
```

リモートリポジトリが正しく設定されたか確認します：

```bash
git remote -v
```

ファイルをGitHubにプッシュします：

```bash
git push -u origin main
```

**注意**: 初回のプッシュ時、GitHubの認証情報を求められる場合があります。以下のいずれかの方法で認証できます：

1. **Personal Access Token（推奨）**: GitHubの設定でPersonal Access Tokenを作成し、パスワードの代わりに使用
2. **Git Credential Manager**: 認証情報を保存して自動認証

### ステップ6: README.mdの作成（オプション）

プロジェクトルートに`README.md`ファイルを作成し、プロジェクトの説明を記載することを推奨します。

`README.md`の例：

```markdown
# TowerDefense01

にゃんこ大戦争を参考にしたタワーディフェンスゲームに、英単語学習機能を組み合わせた教育ゲームです。

## 概要

- **ジャンル**: タワーディフェンス × 英単語学習
- **プラットフォーム**: Steam（PC）
- **エンジン**: Unity 2022.3 LTS以降

## 機能

- 英単語問題に答えてPowerを獲得
- Powerを使ってキャラクターを召喚
- 自動戦闘システム
- 敵の城を破壊して勝利

## ドキュメント

詳細な仕様は`docs/`フォルダ内のドキュメントを参照してください。

- `docs/00_Master_Project.md` - マスタープロジェクト仕様書
- `docs/99_Development_Log.md` - 開発ログ
```

`README.md`を追加してコミット・プッシュします：

```bash
git add README.md
git commit -m "Add README.md"
git push
```

## 今後の運用

### 定期的なコミットとプッシュ

作業が完了したら、定期的にコミットとプッシュを行います：

```bash
# 変更を確認
git status

# 変更をステージング
git add .

# コミット（適切なメッセージを付ける）
git commit -m "機能追加: 歩行エフェクト実装完了"

# プッシュ
git push
```

### コミットメッセージのベストプラクティス

明確で理解しやすいコミットメッセージを使用します：

- `[ADD]` - 新機能の追加
- `[FIX]` - バグ修正
- `[UPDATE]` - 既存機能の更新
- `[REFACTOR]` - リファクタリング
- `[DOC]` - ドキュメント更新
- `[SETUP]` - 設定・セットアップ関連

例：
```
[ADD] 歩行エフェクト実装完了（CharacterMovementController、EnemyController）
[FIX] 城のHPバー表示問題を修正
[UPDATE] 正解時のパワー計算ロジックを変更（残りタイマー秒数×倍率）
```

### ブランチ戦略（オプション）

大規模な開発やチーム開発では、ブランチ戦略を採用することを推奨します：

```bash
# 機能開発用ブランチを作成
git checkout -b feature/walking-effect

# 作業完了後、mainブランチにマージ
git checkout main
git merge feature/walking-effect
git push
```

### プル（Pull）で最新状態を取得

他の環境や他の開発者との同期が必要な場合：

```bash
git pull origin main
```

## トラブルシューティング

### プッシュ時に認証エラーが発生する

- Personal Access Tokenが正しく設定されているか確認
- `git remote set-url origin`でリモートURLを確認・修正

### プッシュ時に競合が発生する

- まず`git pull`で最新の状態を取得してから、再度`git push`を試す
- 競合が発生した場合は、手動で解決してからコミット・プッシュ

### 大容量ファイルが含まれている

Unityプロジェクトには大容量ファイル（音声、画像など）が含まれる場合があります。`.gitignore`に適切なパターンを追加するか、Git LFS（Large File Storage）の使用を検討してください。

## 参考リンク

- [Git公式ドキュメント](https://git-scm.com/doc)
- [GitHub公式ドキュメント](https://docs.github.com/ja)
- [Unity用.gitignoreテンプレート](https://github.com/github/gitignore/blob/main/Unity.gitignore)

---

## 最終更新日

2026年1月18日
