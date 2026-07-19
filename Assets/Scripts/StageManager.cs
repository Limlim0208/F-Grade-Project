using UnityEngine;

public class StageProgressManager : MonoBehaviour
{
    public static StageProgressManager Instance { get; private set; }

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

    // 씬 관리 Instance가 없으면 자동 생성
    public static StageProgressManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("StageManager");
            obj.AddComponent<StageProgressManager>();
        }
        return Instance;
    }

    // StageSelectScene에서 스테이지 선택 버튼 클릭 시 호출
    public void SetStage(int stageId)
    {
        CurrentStage = stageId;
        SceneChanger.GetInstance().LoadStage(stageId);
    }

    // 클리어 성공 시 다음 스테이지로 이동
    public void LoadNextStage()
    {
        if (CurrentStage < TotalStages)
        {
            CurrentStage++;
            SceneChanger.GetInstance().LoadScene("StageSelectScene");
        }
        else
        {
            SceneChanger.GetInstance().LoadScene("FinalScene"); // 마지막 스테이지
        }
    }

    // 2026-07-19 임유미 추가
    // 클리어 실패 시 처리 — GameOverPopup의 확인 버튼에서 호출됨
    public void OnStageFailed()
    {
        CurrentStage = 0; // 현재까지 클리어한 스테이지 수 초기화
        SceneChanger.GetInstance().LoadScene("StageSelectScene"); // 스테이지 선택 화면으로 이동
    }

    public void LoadStartScene()
    {
        SceneChanger.GetInstance().LoadScene("StartScene");
    }

}