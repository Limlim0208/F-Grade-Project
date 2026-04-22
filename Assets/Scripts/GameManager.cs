using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Paused, StageClear, GameOver }
    public GameState CurrentState { get; private set; }

    [Header("타이머")]
    public float timeLimit = 60f;
    public float TimeRemaining { get; private set; }
    public bool IsTimerRunning { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        TimeRemaining = timeLimit;
        IsTimerRunning = true;
        ChangeState(GameState.Playing);
    }

    void Update()
    {
        if (!IsTimerRunning) return;
        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0)
        {
            TimeRemaining = 0;
            OnGameOver();
        }
    }

    public void OnStageClear()
    {
        IsTimerRunning = false;
        ChangeState(GameState.StageClear);
        StageManager.Instance.LoadNextStage(); // 다음 스테이지로
    }

    public void OnGameOver()
    {
        IsTimerRunning = false;
        ChangeState(GameState.GameOver);
        StageManager.Instance.LoadStartScene(); // 시작화면으로
    }

    public void PauseGame() { IsTimerRunning = false; Time.timeScale = 0; ChangeState(GameState.Paused); }
    public void ResumeGame() { IsTimerRunning = true; Time.timeScale = 1; ChangeState(GameState.Playing); }

    void ChangeState(GameState newState) => CurrentState = newState;
}