using UnityEngine;

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
    }

    public void OnStageClear()
    {
        TimerManager.GetInstance().StopTimer(); // 타이머 정지
        ChangeState(GameState.StageClear);
        StageManager.GetInstance().LoadNextStage();
    }

    public void OnGameOver()
    {
        TimerManager.GetInstance().StopTimer(); // 타이머 정지
        ChangeState(GameState.GameOver);
        StageManager.GetInstance().LoadStartScene();
    }

    public void PauseGame()
    {
        TimerManager.GetInstance().PauseTimer(); // 타이머 일시정지
        Time.timeScale = 0;
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        TimerManager.GetInstance().ResumeTimer(); // 타이머 재개
        Time.timeScale = 1;
        ChangeState(GameState.Playing);
    }

    void ChangeState(GameState newState) => CurrentState = newState;
}