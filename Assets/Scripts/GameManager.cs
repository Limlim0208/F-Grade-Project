using System;
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

    // 임유미 추가: 상태가 바뀔 때마다 (이전 상태, 새 상태)를 전달
    public static event Action<GameState, GameState> OnGameStateChanged;

    // 임유미 수정: GameState 변경 시 브로드캐스트
    void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return; // 같은 상태로 중복 변경 방지

        GameState previousState = CurrentState;
        CurrentState = newState;
        Debug.Log($"[GameManager] GameState: {previousState} → {newState}");

        OnGameStateChanged?.Invoke(previousState, newState);
    }

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
    }

    public void OnGameOver()
    {
        TimerManager.GetInstance().StopTimer(); // 타이머 정지
        ChangeState(GameState.GameOver);
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

    // 스테이지 재시작 (현재 씬 재로드)
    public void RestartStage()
    {
        Time.timeScale = 1; // Paused 상태였을 경우 시간 흐름 복구 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}