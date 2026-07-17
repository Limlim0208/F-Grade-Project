using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static GameManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("GameManager");
            obj.AddComponent<GameManager>();
        }
        return Instance;
    }

    public enum GameState { Playing, Paused, StageClear, GameOver }
    public GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // 플레이 상태 관리 함수들
    public void StartGame()
    {
        TimerManager.GetInstance().StartTimer(); // 타이머 시작
        ChangeState(GameState.Playing);
        Debug.Log("[GameManager] StartGame → GameState: Playing");
    }

    public void OnStageClear()
    {
        TimerManager.GetInstance().StopTimer(); // 타이머 정지
        ChangeState(GameState.StageClear);
        Debug.Log("[GameManager] OnStageClear → GameState: StageClear");
    }

    public void OnGameOver()
    {
        TimerManager.GetInstance().StopTimer(); // 타이머 정지
        ChangeState(GameState.GameOver);
        Debug.Log("[GameManager] OnGameOver → GameState: GameOver");
    }

    public void PauseGame()
    {
        TimerManager.GetInstance().PauseTimer(); // 타이머 일시정지
        Time.timeScale = 0;
        ChangeState(GameState.Paused);
        Debug.Log("[GameManager] PauseGame → GameState: Paused");
    }

    public void ResumeGame()
    {
        TimerManager.GetInstance().ResumeTimer(); // 타이머 재개
        Time.timeScale = 1;
        ChangeState(GameState.Playing);
        Debug.Log("[GameManager] ResumeGame → GameState: Playsing");
    }

    // 스테이지 재시작 (현재 씬 재로드)
    public void RestartStage()
    {
        Time.timeScale = 1; // Paused 상태였을 경우 시간 흐름 복구 

        Debug.Log("[GameManager] RestartStage → 씬 재로드");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ChangeState(GameState newState) => CurrentState = newState;
}