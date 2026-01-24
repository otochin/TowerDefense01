using UnityEngine;
using System.Diagnostics;

/// <summary>
/// macOS標準の音声合成（sayコマンド使用）
/// テキストを読み上げる機能を提供する
/// </summary>
public class MacOSTextToSpeech : MonoBehaviour
{
    [Header("音声設定")]
    [SerializeField] private string englishVoice = "Samantha"; // 英語の音声（デフォルト: Samantha）
    [SerializeField] private string japaneseVoice = "Kyoko"; // 日本語の音声（デフォルト: Kyoko）
    [SerializeField] private bool enableTTS = true; // TTS機能を有効にするか
    [SerializeField, Range(0f, 100f)] private float volume = 100f; // 音量（0-100、afplayで使用）
    
    [Header("設定")]
    [SerializeField] private bool autoDetectLanguage = true; // 言語を自動検出するか
    [SerializeField] private bool useAfplayForVolume = false; // afplayを使用して音量を調整するか（一時ファイルを使用）
    
    // 読み上げ中のプロセス（停止用）
    private Process currentSpeakingProcess;
    
    /// <summary>
    /// テキストを読み上げる
    /// </summary>
    /// <param name="text">読み上げるテキスト</param>
    /// <param name="voice">音声名（nullの場合は自動選択）</param>
    public void Speak(string text, string voice = null)
    {
        // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Speak called. enableTTS: {enableTTS}, text: {text}");
        
        if (!enableTTS || string.IsNullOrEmpty(text))
        {
            // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Speak cancelled. enableTTS: {enableTTS}, text is null or empty: {string.IsNullOrEmpty(text)}");
            return;
        }
        
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // UnityEngine.Debug.Log("[MacOSTextToSpeech] macOS platform detected. Starting speech...");
        
        // 既に読み上げ中の場合は停止
        Stop();
        
        // 音声を選択
        string selectedVoice = voice;
        if (string.IsNullOrEmpty(selectedVoice) && autoDetectLanguage)
        {
            selectedVoice = DetectLanguageAndSelectVoice(text);
        }
        
        if (string.IsNullOrEmpty(selectedVoice))
        {
            selectedVoice = englishVoice; // デフォルトは英語
        }
        
        // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Selected voice: {selectedVoice}, Text: {text}");
        
        // コルーチンで非同期に読み上げを開始
        StartCoroutine(SpeakAsync(text, selectedVoice));
        #else
        // UnityEngine.Debug.Log($"[MacOSTextToSpeech] TTS is only available on macOS. Text: {text}");
        #endif
    }
    
    /// <summary>
    /// テキストの言語を検出して音声を選択
    /// </summary>
    private string DetectLanguageAndSelectVoice(string text)
    {
        // 日本語の文字が含まれているかチェック
        bool isJapanese = System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF]");
        
        return isJapanese ? japaneseVoice : englishVoice;
    }
    
    /// <summary>
    /// テキストを非同期で読み上げる
    /// </summary>
    private System.Collections.IEnumerator SpeakAsync(string text, string voice)
    {
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // テキストに含まれる特殊文字をエスケープ
        string escapedText = text.Replace("\"", "\\\"").Replace("'", "\\'");
        
        // 音量調整が必要でafplayを使用する場合
        if (useAfplayForVolume && volume < 100f)
        {
            yield return StartCoroutine(SpeakWithVolume(text, voice, escapedText));
        }
        else
        {
            // 通常のsayコマンドを使用（音量調整なし）
            string arguments = $"-v {voice} \"{escapedText}\"";
            // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Executing: /usr/bin/say {arguments}");
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/say",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            try
            {
                currentSpeakingProcess = new Process { StartInfo = startInfo };
                currentSpeakingProcess.Start();
                // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Process started. PID: {currentSpeakingProcess.Id}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[MacOSTextToSpeech] Failed to start say command: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }
        
        yield return null;
        #endif
    }
    
    /// <summary>
    /// 音量を指定して音声を再生（sayでファイル生成→afplayで再生）
    /// </summary>
    private System.Collections.IEnumerator SpeakWithVolume(string text, string voice, string escapedText)
    {
        // 一時ファイルのパスを生成
        string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"unity_tts_{System.Guid.NewGuid()}.aiff");
        
        try
        {
            // sayコマンドで音声ファイルを生成
            string sayArguments = $"-v {voice} -o \"{tempFilePath}\" \"{escapedText}\"";
            // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Generating audio file: /usr/bin/say {sayArguments}");
            
            ProcessStartInfo sayInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/say",
                Arguments = sayArguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            Process sayProcess = new Process { StartInfo = sayInfo };
            sayProcess.Start();
            sayProcess.WaitForExit(); // ファイル生成が完了するまで待機
            sayProcess.Dispose();
            
            // ファイルが生成されたか確認
            if (System.IO.File.Exists(tempFilePath))
            {
                // afplayで音量を指定して再生
                float volumeValue = Mathf.Clamp01(volume / 100f); // 0.0-1.0に変換
                string afplayArguments = $"-v {volumeValue} \"{tempFilePath}\"";
                // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Playing with volume: /usr/bin/afplay {afplayArguments}");
                
                ProcessStartInfo afplayInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/afplay",
                    Arguments = afplayArguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                
                currentSpeakingProcess = new Process { StartInfo = afplayInfo };
                currentSpeakingProcess.Start();
                // UnityEngine.Debug.Log($"[MacOSTextToSpeech] afplay started. PID: {currentSpeakingProcess.Id}, Volume: {volumeValue}");
                
                // 再生完了後に一時ファイルを削除（非同期）
                StartCoroutine(DeleteTempFileAfterPlayback(tempFilePath));
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[MacOSTextToSpeech] Audio file was not created: {tempFilePath}");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"[MacOSTextToSpeech] Failed to generate or play audio with volume: {e.Message}");
            // エラー時も一時ファイルを削除
            if (System.IO.File.Exists(tempFilePath))
            {
                try { System.IO.File.Delete(tempFilePath); } catch { }
            }
        }
        
        yield return null;
    }
    
    /// <summary>
    /// 再生完了後に一時ファイルを削除
    /// </summary>
    private System.Collections.IEnumerator DeleteTempFileAfterPlayback(string filePath)
    {
        if (currentSpeakingProcess != null)
        {
            // プロセスの完了を待機（最大10秒）
            int maxWaitSeconds = 10;
            int waitedSeconds = 0;
            while (!currentSpeakingProcess.HasExited && waitedSeconds < maxWaitSeconds)
            {
                yield return new WaitForSeconds(1f);
                waitedSeconds++;
            }
        }
        
        // 一時ファイルを削除
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                System.IO.File.Delete(filePath);
                // UnityEngine.Debug.Log($"[MacOSTextToSpeech] Temporary file deleted: {filePath}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"[MacOSTextToSpeech] Failed to delete temp file: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// 読み上げを停止
    /// </summary>
    public void Stop()
    {
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // 現在の読み上げプロセスを停止
        if (currentSpeakingProcess != null && !currentSpeakingProcess.HasExited)
        {
            try
            {
                currentSpeakingProcess.Kill();
                currentSpeakingProcess.Dispose();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"[MacOSTextToSpeech] Failed to stop process: {e.Message}");
            }
            currentSpeakingProcess = null;
        }
        
        // 実行中のすべてのsayプロセスを停止（念のため）
        try
        {
            ProcessStartInfo killInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/killall",
                Arguments = "say",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process killProcess = new Process { StartInfo = killInfo };
            killProcess.Start();
            killProcess.WaitForExit(1000); // 1秒でタイムアウト
            killProcess.Dispose();
        }
        catch (System.Exception e)
        {
            // killallコマンドが失敗しても続行（sayプロセスが存在しない場合など）
            // UnityEngine.Debug.Log($"[MacOSTextToSpeech] killall say command: {e.Message}");
        }
        #endif
    }
    
    private void OnDestroy()
    {
        // オブジェクトが破棄される際に読み上げを停止
        Stop();
    }
    
    /// <summary>
    /// TTS機能を有効/無効にする
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        enableTTS = enabled;
        
        if (!enabled)
        {
            Stop();
        }
    }
}
