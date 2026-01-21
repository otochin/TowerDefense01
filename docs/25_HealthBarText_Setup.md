# HealthBarPrefabにHPテキスト表示を追加する手順

## 概要

HealthBarPrefabに最大HPと現在のHPの数字を表示する機能を追加します。

## 現在の構造

```
HealthBarPrefab (Root)
└── Canvas
    └── HealthBarPanel
        ├── Background
        └── Fill
```

## 手順

### 1. HealthBarPrefabを開く

1. Projectウィンドウで `Assets/Prefabs/UI/HealthBarPrefab.prefab` をダブルクリック
2. Prefab Modeで編集できるようにする

### 2. HealthBarPanelにTextMeshProUGUIを追加

1. Hierarchyで `HealthBarPanel` を選択
2. 右クリック → `UI` → `Text - TextMeshPro` を選択
3. 作成されたGameObjectの名前を `HealthText` に変更

### 3. HealthTextの設定

1. `HealthText` を選択
2. Inspectorで以下の設定を行う：

#### RectTransform設定
- **Anchors**: Center（中央）
- **Position**: X=0, Y=0, Z=0（HealthBarPanelの中央）
- **Width**: 100（適宜調整）
- **Height**: 20（適宜調整）

#### TextMeshProUGUI設定
- **Text**: `100 / 100`（プレースホルダー、実行時に自動更新されます）
- **Font Size**: 12-14（適宜調整）
- **Alignment**: Center（中央揃え）
- **Color**: 白または見やすい色
- **Auto Size**: 必要に応じて有効化

### 4. WorldSpaceHealthBarUIコンポーネントの設定

1. `HealthBarPanel` を選択（WorldSpaceHealthBarUIコンポーネントがアタッチされているはず）
2. Inspectorで `WorldSpaceHealthBarUI` コンポーネントを確認：
   - **Show Health Text**: ✅ チェック（デフォルトでtrueになっています）
   - **Health Format**: `{0} / {1}`（現在のHP / 最大HPの形式）
   - **Health Bar Text**: `HealthText` をドラッグ&ドロップ（自動検出も可能）

### 5. 最終的な構造

```
HealthBarPrefab (Root)
└── Canvas
    └── HealthBarPanel (WorldSpaceHealthBarUIコンポーネント付き)
        ├── Background
        ├── Fill
        └── HealthText (TextMeshProUGUI) ← 新規追加
```

### 6. プレハブを保存

1. Prefab Modeの上部にある `Open Prefab` ボタンをクリックして通常のシーンに戻る
2. 変更が自動的に保存されます

## 動作確認

1. ゲームを実行
2. キャラクターまたはエネミーの上部にHPバーが表示されることを確認
3. HPバーの上または下に「現在のHP / 最大HP」の形式で数字が表示されることを確認
4. ダメージを受けると、数字が更新されることを確認

## 注意事項

- `WorldSpaceHealthBarUI`コンポーネントは`HealthBarPanel`にアタッチされている必要があります
- `Show Health Text`が`true`になっていることを確認してください
- テキストの位置やサイズは、ゲーム中に調整してください
- テキストが見づらい場合は、背景色やアウトラインを追加することを検討してください

## トラブルシューティング

### テキストが表示されない場合

1. `HealthBarPanel`の`WorldSpaceHealthBarUI`コンポーネントで`Show Health Text`が`true`になっているか確認
2. `HealthText`が`HealthBarPanel`の子要素になっているか確認
3. `HealthText`がアクティブになっているか確認
4. コンソールにエラーメッセージがないか確認

### テキストの位置がおかしい場合

1. `HealthText`のRectTransformのAnchorsとPositionを確認
2. HealthBarPanelのサイズに応じて位置を調整
