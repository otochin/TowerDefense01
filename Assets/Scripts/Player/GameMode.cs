/// <summary>
/// ゲームモード列挙型
/// 英単語・英熟語学習の方向性を定義
/// </summary>
public enum GameMode
{
    EnglishToJapanese,      // 英単語→日：英単語を表示し、日本語訳の選択肢から選ぶ
    JapaneseToEnglish,      // 日→英単語：日本語を表示し、英単語の選択肢から選ぶ
    PhraseEnglishToJapanese, // 英熟語→日：英熟語を表示し、日本語訳の選択肢から選ぶ
    PhraseJapaneseToEnglish  // 日→英熟語：日本語を表示し、英熟語の選択肢から選ぶ
}
