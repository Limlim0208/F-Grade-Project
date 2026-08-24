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

    public bool IsMainGameActive { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// 미니게임(수강신청 준비 미니게임 등) 종료 후 MinigameManager가 호출.
    /// 메인게임(수강신청 화면 등) 진입 트리거.
    public void StartMainGame()
    {
        if (mainGameRoot != null)
            mainGameRoot.SetActive(true);

        IsMainGameActive = true;

        Debug.Log("[MainGameManager] 메인게임 시작됨");
    }

    public void OnMainGameSuccess()
    {
        IsMainGameActive = false;

        Debug.Log("[MainGameManager] 메인게임 성공");
        GameManager.GetInstance().OnStageClear();

    }

    public void OnMainGameFail()
    {
        IsMainGameActive = false;

        Debug.Log("[MainGameManager] 메인게임 실패");
        GameManager.GetInstance().OnGameOver();
    }
}