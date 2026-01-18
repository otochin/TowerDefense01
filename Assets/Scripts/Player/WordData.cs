using System;
using UnityEngine;

/// <summary>
/// 英単語データ構造体
/// CSVファイルから読み込まれる英単語と日本語訳のペア
/// 忘却曲線に基づく学習履歴も管理
/// </summary>
[System.Serializable]
public class WordData
{
    public string English;
    public string Japanese;
    
    // 忘却曲線用の学習履歴
    [System.NonSerialized] public int incorrectCount = 0; // 間違えた回数
    [System.NonSerialized] public int correctCount = 0; // 正解した回数
    [System.NonSerialized] public float lastCorrectTime = 0f; // 最後に正解した時刻（ゲーム開始からの経過時間）
    [System.NonSerialized] public float nextReviewTime = 0f; // 次に復習すべき時刻（ゲーム開始からの経過時間）
    
    public WordData(string english, string japanese)
    {
        English = english;
        Japanese = japanese;
        incorrectCount = 0;
        correctCount = 0;
        lastCorrectTime = 0f;
        nextReviewTime = 0f;
    }
    
    /// <summary>
    /// 正解した時に呼ばれる（復習間隔を長くする）
    /// </summary>
    public void OnCorrect(float currentTime)
    {
        correctCount++;
        lastCorrectTime = currentTime;
        
        // 忘却曲線に基づく復習間隔の設定
        // 1回正解→30秒、2回正解→2分、3回正解→5分、4回以上→10分
        float reviewInterval = correctCount switch
        {
            1 => 30f,   // 30秒
            2 => 120f,  // 2分
            3 => 300f,  // 5分
            _ => 600f   // 10分以上は10分間隔
        };
        
        nextReviewTime = currentTime + reviewInterval;
        
        // 正解が続けば間違えた回数をリセット（少し）
        if (correctCount >= 2)
        {
            incorrectCount = Mathf.Max(0, incorrectCount - 1);
        }
    }
    
    /// <summary>
    /// 不正解時に呼ばれる（優先度を高くする）
    /// </summary>
    public void OnIncorrect(float currentTime)
    {
        incorrectCount++;
        // 不正解した場合はすぐに復習できるように
        nextReviewTime = currentTime;
    }
    
    /// <summary>
    /// 問題として選択される優先度を計算（高いほど優先される）
    /// </summary>
    public float GetPriority(float currentTime)
    {
        // 間違えた回数が多いほど高い優先度
        float incorrectWeight = incorrectCount * 10f;
        
        // 復習時刻が過ぎている場合は優先度を上げる
        float reviewWeight = (currentTime >= nextReviewTime) ? 5f : 0f;
        
        // 正解回数が多いほど優先度を下げる（ただし、復習時刻が来たら復習する）
        float correctPenalty = correctCount > 3 ? -2f : 0f;
        
        return incorrectWeight + reviewWeight + correctPenalty;
    }
}
