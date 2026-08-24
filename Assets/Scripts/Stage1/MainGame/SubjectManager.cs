using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubjectManager : MonoBehaviour
{

    [Header("메모장 UI (신청해야 하는 과목 표시)")]
    [SerializeField] private Transform memoContentParent;
    [SerializeField] private GameObject memoLineTextPrefab;

    private int requiredSubjectCount = 5;
    private List<GameObject> spawnedMemoLines = new List<GameObject>();

    [Header("전체 과목표 UI (10개 행 전부 표시)")]
    [SerializeField] private Transform chartContentParent;
    [SerializeField] private GameObject subjectChartRowPrefab;
    public List<SubjectData> AllSubjects { get; private set; } = new List<SubjectData>();
    public List<SubjectData> RequiredSubjects { get; private set; } = new List<SubjectData>();

    private List<SubjectChartRow> spawnedChartRows = new List<SubjectChartRow>();

    [Header("신청 완료 과목 UI")]
    [SerializeField] private Transform registeredContentParent;
    [SerializeField] private GameObject registeredSubjectRowPrefab;
    [SerializeField] private TMP_Text registeredCountText; // "신청과목수: {n}" 표시

    public List<SubjectData> RegisteredSubjects { get; private set; } = new List<SubjectData>();
    private List<RegisteredSubjectRow> spawnedRegisteredRows = new List<RegisteredSubjectRow>();


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

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeSubjects(); // Start() → Awake()로 이동 (OnEnable보다 먼저 실행되도록)
        PopulateSubjectChart();
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

    private float applicantTickInterval = 0.2f;
    private float applicantTickTimer = 0f;

    [Header("스테이지 결과 판정")]
    [SerializeField] private float finalJudgeDelaySeconds = 30f; // 전체 신청시간 종료 후 최종 판정까지 대기 시간
    private float finalJudgeOffsetSeconds; // AllSubjects 중 가장 늦은 마감 + finalJudgeDelaySeconds
    private bool hasJudgedStageResult = false;

    void Update()
    {
        if (GameManager.GetInstance().CurrentState != GameManager.GameState.Playing)
            return;

        if (!ServerClockManager.GetInstance().IsRegistrationOpen)
            return;

        float elapsed = ServerClockManager.GetInstance().SecondsSinceRegistrationOpened;

        // 최종 판정: 전체 신청시간 종료 + finalJudgeDelaySeconds 경과 시 단 한 번 실행
        if (!hasJudgedStageResult && elapsed >= finalJudgeOffsetSeconds)
        {
            hasJudgedStageResult = true;
            JudgeStageResult();
        }

        applicantTickTimer += Time.deltaTime;
        if (applicantTickTimer < applicantTickInterval)
            return;
        applicantTickTimer = 0f;

        for (int i = 0; i < AllSubjects.Count; i++)
        {
            bool changed = AllSubjects[i].UpdateApplicants(elapsed);

            if (changed && i < spawnedChartRows.Count && spawnedChartRows[i] != null)
                spawnedChartRows[i].RefreshApplicantText();
        }

        foreach (RegisteredSubjectRow row in spawnedRegisteredRows)
        {
            if (row != null)
                row.RefreshApplicantText();
        }
    }


    private void InitializeSubjects()
    {
        AllSubjects = new List<SubjectData>
        {
            new SubjectData(1,  "1학년", "010201", "01", "슬기로운 대학 생활: OT·팀플·벼락치기의 실전", "교양필수", 2, "여출석",       "(화), (목) 15:00 ~ 16:15", "생존교양학부",         36,  0, 5f),
            new SubjectData(2,  "1학년", "211001", "01", "기초 파이썬과 복붙 응용 실습",              "전공필수", 3, "컨트롤 씨브이", "(월), (수) 10:30 ~ 11:45", "컴퓨터응급처치학과",   100, 1, 5f),
            new SubjectData(3,  "1학년", "211002", "02", "기초 파이썬과 복붙 응용 실습",              "전공필수", 3, "컨트롤 씨브이", "(화), (목) 13:30 ~ 14:45", "컴퓨터응급처치학과",   50,  2, 1f),
            new SubjectData(4,  "",     "007001", "01", "애니메이션으로 배우는 일본어 회화",          "교양선택", 3, "성더쿠",       "(금) 13:00 ~ 16:15",      "이차원커뮤니케이션학과", 25, 3, 2f),
            new SubjectData(5,  "",     "007072", "01", "한국사의 이해와 암기론",                    "교양선택", 3, "이국사",       "(월), (수) 9:00 ~ 10:15",  "암기사학과",           200, 4, 8f),
            new SubjectData(6,  "",     "211012", "01", "내 컴퓨터에선 됐는데요: 환경탓의 이해(사이버)", "전공필수", 3, "김배포",       "원격(사이버)",             "무한루프공학과",       300, 5, 5f),
            new SubjectData(7,  "",     "212105", "02", "코드 고고학개론",                          "전공선택", 3, "이주석",       "(월), (수) 10:30 ~ 11:45", "코드인문학부",         40,  6, 3f),
            new SubjectData(8,  "",     "010285", "03", "야근의 미학과 카페인 대사학",               "교양필수", 2, "박퇴근",       "(목) 19:00 ~ 22:15",      "워라밸경영학부",       200, 7, 10f),
            new SubjectData(9,  "",     "212187", "01", "의미론: 기획서 거짓말 통역학",              "전공선택", 2, "나갈라",       "(화), (목) 13:30 ~ 14:45", "요구사항학부",         30,  8, 6f),
            new SubjectData(10, "",     "007014", "02", "재택근무 마우스 흔들기 실기(사이버)",         "교양선택", 3, "안일",         "원격(사이버)",             "출퇴근위장학과",       100, 9, 7f),
        };
    }

    public void AssignRandomRequiredSubjects()
    {
        if (AllSubjects == null || AllSubjects.Count == 0)
            return;

        RequiredSubjects = AllSubjects
            .OrderBy(_ => Random.value)
            .Take(requiredSubjectCount)
            .ToList();

        PopulateMemoUI();
        InitializeApplicantSimulation();

        // 전체 과목 중 가장 늦은 마감 시각 + 대기시간 = 최종 판정 시각
        finalJudgeOffsetSeconds = AllSubjects.Max(s => s.DeadlineOffsetSeconds) + finalJudgeDelaySeconds;
        hasJudgedStageResult = false;
    }

    private void InitializeApplicantSimulation()
    {
        foreach (SubjectData subject in AllSubjects)
            subject.GenerateApplicantArrivals();
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

    public void PopulateSubjectChart()
    {
        Debug.Log($"[SubjectManager] PopulateSubjectChart 시작 - AllSubjects.Count = {AllSubjects.Count}");

        if (chartContentParent == null || subjectChartRowPrefab == null)
        {
            Debug.LogWarning("[SubjectManager] chartContentParent 또는 subjectChartRowPrefab 미연결");
            return;
        }

        // 기존에 우리가 생성한 행만 제거 (원래 있던 기본 오브젝트는 그대로 둠)
        foreach (SubjectChartRow row in spawnedChartRows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }
        spawnedChartRows.Clear();

        foreach (SubjectData subject in AllSubjects)
        {
            GameObject rowObj = Instantiate(subjectChartRowPrefab, chartContentParent);
            SubjectChartRow row = rowObj.GetComponent<SubjectChartRow>();

            if (row == null)
            {
                Debug.LogWarning($"[SubjectManager] {rowObj.name}에 SubjectChartRow 컴포넌트 없음");
                continue;
            }

            row.Bind(subject);
            spawnedChartRows.Add(row);
        }

        Debug.Log($"[SubjectManager] PopulateSubjectChart 완료 - {spawnedChartRows.Count}개 행 생성됨");
    }

    /// 신청 성공 판정 로직(추후 구현)에서 호출. 중복 등록 방지 포함.
    /// 신청 성공 판정 로직(추후 구현)에서 호출. 중복 등록 방지 포함.
    public void AddRegisteredSubject(SubjectData subject)
    {
        if (subject == null || RegisteredSubjects.Contains(subject))
            return;

        RegisteredSubjects.Add(subject);
        Debug.Log($"[SubjectManager] '{subject.SubjectName}' 신청 완료 목록에 추가됨 (현재 {RegisteredSubjects.Count}개)");

        PopulateRegisteredUI();
    }

    /// RegisteredSubjectRow의 취소 버튼에서 호출. 신청 완료 목록에서 제거.
    /// RegisteredSubjectRow의 취소 버튼에서 호출. 신청 완료 목록에서 제거.
    public void RemoveRegisteredSubject(int subjectIndex)
    {
        SubjectData subject = RegisteredSubjects.FirstOrDefault(s => s.SubjectIndex == subjectIndex);
        if (subject == null)
        {
            Debug.LogWarning($"[SubjectManager] RemoveRegisteredSubject: subjectIndex={subjectIndex}를 신청 완료 목록에서 찾을 수 없음");
            return;
        }

        RegisteredSubjects.Remove(subject);
        Debug.Log($"[SubjectManager] '{subject.SubjectName}' 신청 완료 목록에서 취소됨 (현재 {RegisteredSubjects.Count}개)");

        PopulateRegisteredUI();
        ShowResultPopup("신청 취소", $"'{subject.SubjectName}' 신청이 취소되었습니다.");
    }

    private void PopulateRegisteredUI()
    {
        if (registeredContentParent == null || registeredSubjectRowPrefab == null)
        {
            Debug.LogWarning("[SubjectManager] registeredContentParent 또는 registeredSubjectRowPrefab 미연결");
            UpdateRegisteredCountText();
            return;
        }

        // 기존에 우리가 만든 행만 제거 (원래 있던 기본 오브젝트는 유지)
        foreach (RegisteredSubjectRow row in spawnedRegisteredRows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }
        spawnedRegisteredRows.Clear();

        foreach (SubjectData subject in RegisteredSubjects)
        {
            GameObject rowObj = Instantiate(registeredSubjectRowPrefab, registeredContentParent);
            RegisteredSubjectRow row = rowObj.GetComponent<RegisteredSubjectRow>();

            if (row == null)
            {
                Debug.LogWarning($"[SubjectManager] {rowObj.name}에 RegisteredSubjectRow 컴포넌트 없음");
                continue;
            }

            row.Bind(subject);
            spawnedRegisteredRows.Add(row);
        }

        UpdateRegisteredCountText();
    }

    private void UpdateRegisteredCountText()
    {
        if (registeredCountText != null)
            registeredCountText.text = $"신청과목수: {RegisteredSubjects.Count}";
    }

    /// 버튼 클릭 시 SubjectChartRow가 호출. index로 원본 SubjectData 역참조.
    /// 신청 가능 시간대인지 + 마감 시각(DeadlineOffsetSeconds) 기준 성공/실패 판정.
    /// TODO: RequiredSubjects 포함 여부(관련 과목인지) 체크는 추후 구현 예정
    public void OnApplyButtonClicked(int subjectIndex)
    {
        SubjectData subject = AllSubjects.FirstOrDefault(s => s.SubjectIndex == subjectIndex);
        if (subject == null)
        {
            Debug.LogWarning($"[SubjectManager] subjectIndex={subjectIndex}에 해당하는 과목을 찾을 수 없음");
            return;
        }

        // 1) 신청 가능 시간대인지 체크
        if (!ServerClockManager.GetInstance().IsRegistrationOpen)
        {
            Debug.Log($"[SubjectManager] '{subject.SubjectName}' 신청 시도 — 아직 신청 불가 시간대");
            ShowResultPopup("신청 불가", "수강신청 가능 시간이 아닙니다.");
            return;
        }

        // 2) 마감 시각 판정 (신청 오픈 후 경과 시간 <= 과목별 마감 오프셋이면 성공)
        float elapsedSeconds = ServerClockManager.GetInstance().SecondsSinceRegistrationOpened;
        bool isSuccess = elapsedSeconds <= subject.DeadlineOffsetSeconds;

        if (!isSuccess)
        {
            Debug.Log($"[SubjectManager] '{subject.SubjectName}' 신청 실패 — 마감 시각 초과 (경과 {elapsedSeconds:F1}s > 마감 {subject.DeadlineOffsetSeconds:F1}s)");
            ShowResultPopup("신청 실패", $"'{subject.SubjectName}' 수강인원이 초과되었습니다.");
            return;
        }

        // 3) 성공 → 신청 완료 목록에 등록
        // NOTE: RequiredSubjects 포함 여부와 무관하게 일단 등록됨. 무관 과목 여부는 스테이지 클리어 판정 단계에서 별도 처리 예정.
        AddRegisteredSubject(subject);
        Debug.Log($"[SubjectManager] '{subject.SubjectName}' 신청 성공 (경과 {elapsedSeconds:F1}s <= 마감 {subject.DeadlineOffsetSeconds:F1}s)");
        ShowResultPopup("신청 완료", $"'{subject.SubjectName}' 신청되었습니다.");
    }

    /// 신청 결과 안내 팝업 (Confirm 버튼 단일 구성)
    private void ShowResultPopup(string title, string content)
    {
        var info = new PopupInfo.Builder()
            .SetTitle(title)
            .SetContent(content)
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }

    /// 전체 신청시간 종료 + finalJudgeDelaySeconds 후 호출됨.
    /// RequiredSubjects의 모든 과목이 RegisteredSubjects에 (과목명 기준으로) 포함돼있는지만 판정하고 결과를 MainGameManager에 위임.
    private void JudgeStageResult()
    {
        bool allRegistered = RequiredSubjects.All(required =>
            RegisteredSubjects.Any(registered => registered.SubjectName == required.SubjectName));

        Debug.Log(allRegistered
            ? "[SubjectManager] 필수 과목 전부 신청 완료"
            : "[SubjectManager] 필수 과목 중 누락된 과목 있음");

        if (allRegistered)
            MainGameManager.GetInstance().OnMainGameSuccess();
        else
            MainGameManager.GetInstance().OnMainGameFail();
    }
}