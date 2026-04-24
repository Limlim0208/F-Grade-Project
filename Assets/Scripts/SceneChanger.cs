using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static SceneChanger GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SceneChanger");
            obj.AddComponent<SceneChanger>();
        }
        return Instance;
    }

    // 스테이지 이름으로 씬 호출
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 스테이지 번호로 씬 호출
    public void LoadStage(int stageId)
    {
        SceneManager.LoadScene("Stage_" + stageId.ToString("D2"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}