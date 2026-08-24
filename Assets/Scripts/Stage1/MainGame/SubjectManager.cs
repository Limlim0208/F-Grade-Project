using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubjectManager : MonoBehaviour
{
    public static SubjectManager Instance { get; private set; }
    public static SubjectManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SubjectManager");
            obj.AddComponent<SubjectManager>();
        }
        return Instance;
    }

    [Header("메모장 UI (신청해야 하는 과목 표시)")]
    [SerializeField] private Transform memoContentParent;
    [SerializeField] private GameObject memoLineTextPrefab;

    private int requiredSubjectCount = 5;
    private List<GameObject> spawnedMemoLines = new List<GameObject>();

    public List<SubjectData> AllSubjects { get; private set; } = new List<SubjectData>();
    public List<SubjectData> RequiredSubjects { get; private set; } = new List<SubjectData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeSubjects(); // Start() → Awake()로 이동 (OnEnable보다 먼저 실행되도록)
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        if (GameManager.GetInstance().CurrentState == GameManager.GameState.Playing)
        {
            AssignRandomRequiredSubjects();
        }
        else
        {
        }
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing && previousState != GameManager.GameState.Paused)
        {
            AssignRandomRequiredSubjects();
        }
    }

    private void InitializeSubjects()
    {
        AllSubjects = new List<SubjectData>
        {
            new SubjectData(1,  "1학년", "010201", "01", "슬기로운 대학 생활: OT·팀플·벼락치기의 실전", "교양필수", 2, "여출석",       "(화), (목) 15:00 ~ 16:15", "생존교양학부",         36,  0, 0f),
            new SubjectData(2,  "1학년", "211001", "01", "기초 파이썬과 복붙 응용 실습",              "전공필수", 3, "컨트롤 씨브이", "(월), (수) 10:30 ~ 11:45", "컴퓨터응급처치학과",   100, 1, 0f),
            new SubjectData(3,  "1학년", "211002", "02", "기초 파이썬과 복붙 응용 실습",              "전공필수", 3, "컨트롤 씨브이", "(화), (목) 13:30 ~ 14:45", "컴퓨터응급처치학과",   50,  2, 0f),
            new SubjectData(4,  "",     "007001", "01", "애니메이션으로 배우는 일본어 회화",          "교양선택", 3, "성더쿠",       "(금) 13:00 ~ 16:15",      "이차원커뮤니케이션학과", 25, 3, 0f),
            new SubjectData(5,  "",     "007072", "01", "한국사의 이해와 암기론",                    "교양선택", 3, "이국사",       "(월), (수) 9:00 ~ 10:15",  "암기사학과",           200, 4, 0f),
            new SubjectData(6,  "",     "211012", "01", "내 컴퓨터에선 됐는데요: 환경탓의 이해(사이버)", "전공필수", 3, "김배포",       "원격(사이버)",             "무한루프공학과",       300, 5, 0f),
            new SubjectData(7,  "",     "212105", "02", "코드 고고학개론",                          "전공선택", 3, "이주석",       "(월), (수) 10:30 ~ 11:45", "코드인문학부",         40,  6, 0f),
            new SubjectData(8,  "",     "010285", "03", "야근의 미학과 카페인 대사학",               "교양필수", 2, "박퇴근",       "(목) 19:00 ~ 22:15",      "워라밸경영학부",       200, 7, 0f),
            new SubjectData(9,  "",     "212187", "01", "의미론: 기획서 거짓말 통역학",              "전공선택", 2, "나갈라",       "(화), (목) 13:30 ~ 14:45", "요구사항학부",         30,  8, 0f),
            new SubjectData(10, "",     "007014", "02", "재택근무 마우스 흔들기 실기(사이버)",         "교양선택", 3, "안일",         "원격(사이버)",             "출퇴근위장학과",       100, 9, 0f),
        };
    }

    public void AssignRandomRequiredSubjects()
    {

        if (AllSubjects == null || AllSubjects.Count == 0)
        {
            return;
        }

        RequiredSubjects = AllSubjects
            .OrderBy(_ => Random.value)
            .Take(requiredSubjectCount)
            .ToList();

        PopulateMemoUI();
    }

    private void PopulateMemoUI()
    {

        if (memoContentParent == null || memoLineTextPrefab == null)
        {
            return;
        }

        foreach (GameObject line in spawnedMemoLines)
        {
            if (line != null)
                Destroy(line);
        }
        spawnedMemoLines.Clear();

        foreach (SubjectData subject in RequiredSubjects)
        {
            GameObject line = Instantiate(memoLineTextPrefab, memoContentParent);
            TMP_Text text = line.GetComponent<TMP_Text>();
            if (text != null)
                text.text = $"{subject.SubjectName}";
            else
                Debug.LogWarning($"[SubjectManager] {line.name}에 TMP_Text 컴포넌트 없음");

            spawnedMemoLines.Add(line);
        }

    }
}