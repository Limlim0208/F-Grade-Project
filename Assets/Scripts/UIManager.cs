using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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

    public static UIManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("UIManager");
            obj.AddComponent<UIManager>();
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