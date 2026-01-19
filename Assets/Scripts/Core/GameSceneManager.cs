using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン管理システム
/// シーン間の遷移を管理する
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    [Header("シーン名設定")]
    [SerializeField] private string titleSceneName = "TitleScene"; // タイトルシーンの名前
    [SerializeField] private string gameSceneName = "SampleScene"; // ゲームシーンの名前
    
    private static GameSceneManager instance;
    
    public static GameSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSceneManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameSceneManager");
                    instance = go.AddComponent<GameSceneManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// タイトルシーンに遷移
    /// </summary>
    public void LoadTitleScene()
    {
        Debug.Log($"[GameSceneManager] Loading title scene: {titleSceneName}");
        Time.timeScale = 1f; // Time.timeScaleをリセット
        UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
    }
    
    /// <summary>
    /// ゲームシーンに遷移
    /// </summary>
    public void LoadGameScene()
    {
        Debug.Log($"[GameSceneManager] Loading game scene: {gameSceneName}");
        Time.timeScale = 1f; // Time.timeScaleをリセット
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
    
    /// <summary>
    /// 現在のシーンを再読み込み（もう一度やる、次のステージへ）
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"[GameSceneManager] Reloading current scene: {currentSceneName}");
        Time.timeScale = 1f; // Time.timeScaleをリセット
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }
    
    /// <summary>
    /// シーン名を設定（Inspectorで設定できない場合に使用）
    /// </summary>
    public void SetSceneNames(string titleScene, string gameScene)
    {
        titleSceneName = titleScene;
        gameSceneName = gameScene;
    }
}
