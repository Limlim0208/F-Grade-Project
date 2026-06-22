using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private GameObject settingsCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static SettingsManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("UIManager");
            obj.AddComponent<SettingsManager>();
        }
        return Instance;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        if (GameManager.GetInstance().CurrentState == GameManager.GameState.Playing)
            GameManager.GetInstance().PauseGame();
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        if (GameManager.GetInstance().CurrentState == GameManager.GameState.Paused)
            GameManager.GetInstance().ResumeGame();
    }
}