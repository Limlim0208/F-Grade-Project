using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int CurrentStage { get; private set; } = 0; // 스테이지 번호 0으로 초기화
    public int TotalStages { get; private set; } = 5; // 마지막 스테이지 번호

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Instance가 없으면 자동 생성
    public static StageManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("StageManager");
            obj.AddComponent<StageManager>();
        }
        return Instance;
    }

    // StageSelectScene에서 스테이지 선택 시 호출
    public void SetStage(int stageId)
    {
        CurrentStage = stageId;
        SceneChanger.GetInstance().LoadStage(stageId);
    }

    // 클리어 성공 시 다음 스테이지로
    public void LoadNextStage()
    {
        if (CurrentStage < TotalStages)
        {
            CurrentStage++;
            SceneChanger.GetInstance().LoadStage(CurrentStage);
        }
        else
        {
            SceneChanger.GetInstance().LoadScene("FinalScene"); // 마지막 스테이지
        }
    }

    // 클리어 실패 시 시작화면으로
    public void LoadStartScene()
    {
        CurrentStage = 0; // 초기화
        SceneChanger.GetInstance().LoadScene("StartScene");
    }
}