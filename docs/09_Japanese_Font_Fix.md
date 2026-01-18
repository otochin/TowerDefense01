# 日本語文字化け対処ガイド

## 問題

TextMeshProで日本語を表示すると文字化けが発生する場合があります。これは、デフォルトのTextMeshProフォントアセットに日本語のグリフ（文字形状データ）が含まれていないためです。

## 対処方法

### 方法1: TextMeshProフォントアセットに日本語を追加（推奨）

#### 手順

1. **フォントファイルを用意する**
   - Noto Sans JP（Google Fontsからダウンロード可能）
   - または、システムにインストールされている日本語フォント（Meiryo、MS ゴシックなど）

2. **TextMeshPro Font Asset Creatorを開く**
   - Unity Editorメニュー: `Window` → `TextMeshPro` → `Font Asset Creator`

3. **フォント設定を行う**
   - `Source Font File`: 日本語フォントファイル（.ttf/.otf）を選択
   - `Atlas Resolution`: `2048 x 2048` または `4096 x 4096`（日本語は多くの文字があるため）
   - `Character Set`: `Custom Characters` を選択

4. **使用する日本語文字を指定**
   - `Custom Character List`に以下の文字列をコピー&ペースト：
   ```
   あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンりんご本猫犬卵魚緑家氷ジャケット凧ライオン月鼻オレンジペン女王うさぎ太陽木傘バイオリン水レントゲン黄色シマウマ
   ```
   - または、CSVファイルに含まれるすべての文字を確認して入力

5. **フォントアセットを生成**
   - `Generate Font Atlas`をクリック
   - `Save`または`Save as...`をクリック
   - 保存場所: `Assets/TextMesh Pro/Resources/Fonts & Materials/`
   - ファイル名: `NotoSansJP SDF` など

6. **UI要素にフォントアセットを設定**
   - Hierarchyで各TextMeshProUGUI要素を選択：
     - `QuestionText`
     - `ChoiceButton_1` → `Text (TMP)`（子オブジェクト）
     - `ChoiceButton_2` → `Text (TMP)`（子オブジェクト）
     - `ChoiceButton_3` → `Text (TMP)`（子オブジェクト）
     - `FeedbackText`
     - `CorrectAnswerText`
   - Inspectorで`Font Asset`を、作成した日本語フォントアセットに変更

#### 注意点

- フォントアセットのサイズが大きくなることがあります（日本語は文字数が多いため）
- `Atlas Resolution`を大きくするほど、より多くの文字を含められますが、メモリ使用量も増えます
- `Character Set`で`ASCII`のみ選択すると、日本語は表示されません

### 方法2: CSVファイルのエンコーディングを確認

CSVファイルが正しくUTF-8で保存されているか確認してください。

#### 確認方法（macOS/Linux）

```bash
file -I Assets/Resources/Data/WordData.csv
```

出力が `charset=utf-8` でない場合は、UTF-8で保存し直してください。

#### 確認方法（Windows）

1. テキストエディタ（VS Code、Notepad++など）で開く
2. 右下のエンコーディング表示を確認
3. UTF-8でない場合は、エンコーディングを変更して保存

### 方法3: TMP Settingsでデフォルトフォントを変更

1. Projectウィンドウで `Assets/TextMesh Pro/Resources/TMP Settings.asset` を選択
2. Inspectorで `Default Font Asset` を、日本語対応のフォントアセットに変更
3. これにより、新しく作成するTextMeshProUGUI要素は自動的に日本語フォントを使用します

## 推奨手順のまとめ

1. Noto Sans JPをダウンロード（https://fonts.google.com/noto）
2. TextMeshPro Font Asset Creatorで日本語フォントアセットを作成
3. UI要素に個別にフォントアセットを設定
4. CSVファイルがUTF-8であることを確認
5. 動作確認（Playボタンでテスト）

## トラブルシューティング

### 文字が表示されない

- フォントアセットにその文字が含まれているか確認
- `Custom Character List`に文字を追加して再生成

### 一部の文字が表示されない

- `Atlas Resolution`を大きくする（4096x4096など）
- または、使用する文字を制限して複数のフォントアセットに分ける

### フォントが重い

- 必要な文字のみを含める（`Custom Character List`を使用）
- `Atlas Resolution`を適切なサイズに設定（必要最小限に）

---

## 最終更新日

2026年1月17日

## 変更履歴

| 日付 | 変更内容 | 変更者 |
|------|----------|--------|
| 2026-01-17 | 初版作成 | - |
