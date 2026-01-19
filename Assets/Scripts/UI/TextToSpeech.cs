using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// テキストを音声で読み上げる機能
/// </summary>
public static class TextToSpeech
{
    /// <summary>
    /// テキストを音声で読み上げる
    /// </summary>
    /// <param name="text">読み上げるテキスト</param>
    /// <param name="language">言語コード（例: "en-US", "ja-JP"）</param>
    public static void Speak(string text, string language = "en-US")
    {
        if (string.IsNullOrEmpty(text))
        {
            UnityEngine.Debug.LogWarning("[TextToSpeech] Text is null or empty. Cannot speak.");
            return;
        }

        UnityEngine.Debug.Log($"[TextToSpeech] Speaking: '{text}' (language: {language})");

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // macOSの場合: sayコマンドを使用（Process.Startで実行）
        try
        {
            string voice = language.StartsWith("en") ? "Alex" : "Kyoko"; // 英語はAlex、日本語はKyoko
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "/usr/bin/say",
                Arguments = $"-v {voice} \"{text}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            Process process = Process.Start(psi);
            if (process != null)
            {
                UnityEngine.Debug.Log($"[TextToSpeech] Started say process for: '{text}'");
                // プロセスが終了するまで待たない（非同期実行）
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) =>
                {
                    UnityEngine.Debug.Log($"[TextToSpeech] say process finished for: '{text}'");
                    process.Dispose();
                };
            }
            else
            {
                UnityEngine.Debug.LogError("[TextToSpeech] Failed to start say process.");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[TextToSpeech] Failed to speak on macOS: {ex.Message}\n{ex.StackTrace}");
        }
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Windowsの場合: PowerShellのAdd-Typeを使用してSystem.Speech.Synthesisを呼び出す
        try
        {
            // PowerShellでは、シングルクォート内のシングルクォートは2つ重ねる必要がある
            string escapedText = text.Replace("'", "''");
            string command = $"Add-Type -AssemblyName System.Speech; $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; $speak.Speak('{escapedText}')";
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            
            Process process = Process.Start(psi);
            if (process != null)
            {
                // プロセスが終了するまで待たない（非同期実行）
                process.EnableRaisingEvents = true;
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[TextToSpeech] Failed to speak on Windows: {ex.Message}");
        }
#elif UNITY_ANDROID
        // Androidの場合: AndroidのTTSを使用
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass ttsClass = new AndroidJavaClass("android.speech.tts.TextToSpeech"))
        {
            AndroidJavaObject tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", currentActivity, null);
            tts.Call("setLanguage", new AndroidJavaObject("java.util.Locale", language));
            tts.Call("speak", text, 0, null, null);
        }
#elif UNITY_IOS
        // iOSの場合: AVSpeechSynthesizerを使用（ネイティブプラグインが必要）
        UnityEngine.Debug.LogWarning("[TextToSpeech] iOS TTS is not implemented yet. Please use a native plugin.");
#else
        UnityEngine.Debug.LogWarning($"[TextToSpeech] TTS is not supported on this platform: {Application.platform}");
#endif
    }
}
