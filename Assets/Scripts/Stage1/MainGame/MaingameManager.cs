using System.Collections;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }
    public static MainGameManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("MainGameManager");
            obj.AddComponent<MainGameManager>();
        }
        return Instance;
    }

    [Header("메인게임 UI 참조")]
    [SerializeField] private GameObject mainGameRoot; // 메인게임 전체를 켜고 끌 루트 오브젝트

    [Header("랜덤 셧다운")]
    [SerializeField] private GameObject shutdownScreen;   // 메인게임 위에 덮을 셧다운 화면
    [SerializeField] private float minShutdownDelay = 10f;
    [SerializeField] private float maxShutdownDelay = 20f;

    public bool IsMainGameActive { get; private set; }
    public bool IsShutdownActive => shutdownScreen != null && shutdownScreen.activeInHierarchy;

    private Coroutine shutdownRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// 서버 대기가 끝난 뒤 ServerWaitingPopup이 호출.
    public void StartMainGame()
    {
        if (mainGameRoot != null)
            mainGameRoot.SetActive(true);

        IsMainGameActive = true;

        Debug.Log("[MainGameManager] 메인게임 시작됨");

        StartRandomShutdownTimer();
    }

    private void StartRandomShutdownTimer()
    {
        StopShutdownTimer();

        if (shutdownScreen == null)
        {
            Debug.LogWarning("[MainGameManager] shutdownScreen이 Inspector에 연결되어 있지 않아 셧다운 타이머를 시작하지 않습니다.");
            return;
        }

        shutdownRoutine = StartCoroutine(RandomShutdownRoutine());
    }

    private IEnumerator RandomShutdownRoutine()
    {
        float delay = Random.Range(minShutdownDelay, maxShutdownDelay);
        Debug.Log($"[MainGameManager] 랜덤 셧다운 타이머 설정: {delay:F1}초 후 셧다운");

        yield return new WaitForSeconds(delay);

        shutdownRoutine = null;

        // 메인게임이 이미 끝났으면 셧다운을 띄우지 않는다
        if (!IsMainGameActive) yield break;

        shutdownScreen.SetActive(true);
    }

    public void OnMainGameSuccess()
    {
        IsMainGameActive = false;
        StopShutdownTimer();

        Debug.Log("[MainGameManager] 메인게임 성공");
        GameManager.GetInstance().OnStageClear();
    }

    public void OnMainGameFail()
    {
        IsMainGameActive = false;
        StopShutdownTimer();

        Debug.Log("[MainGameManager] 메인게임 실패");
        GameManager.GetInstance().OnGameOver();
    }

    private void StopShutdownTimer()
    {
        if (shutdownRoutine != null)
        {
            StopCoroutine(shutdownRoutine);
            shutdownRoutine = null;
        }
    }
}