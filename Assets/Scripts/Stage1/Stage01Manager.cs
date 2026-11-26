using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class Stage01Manager : MonoBehaviour
{
    [Header("게임오버 UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button gameOverConfirmButton;

    [Header("클리어 UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private Button clearConfirmButton;
    [SerializeField] private Transform clearSubjectListParent;
    [SerializeField] private ClearSubjectRow clearSubjectRowPrefab;

    private bool hasResultHandled = false;

    private void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (clearPanel != null)
            clearPanel.SetActive(false);

        if (gameOverConfirmButton != null)
            gameOverConfirmButton.onClick.AddListener(HandleGameOverConfirmClicked);
        else
            Debug.LogWarning("[Stage01Manager] gameOverConfirmButton이 Inspector에 연결되어 있지 않음");

        if (clearConfirmButton != null)
            clearConfirmButton.onClick.AddListener(HandleClearConfirmClicked);
        else
            Debug.LogWarning("[Stage01Manager] clearConfirmButton이 Inspector에 연결되어 있지 않음");
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    [Header("테스트용")]
    [SerializeField] private bool isForceClearTest = false;

    private void Start()
    {
        GameManager.GetInstance().StartGame();

        if (isForceClearTest)
            StartCoroutine(LoadForceClearTest());
    }

    private System.Collections.IEnumerator LoadForceClearTest()
    {
        // 다른 매니저들의 Awake/OnEnable이 끝나도록 잠깐 대기
        yield return new WaitForSeconds(1f);

        if (SubjectManager.Instance != null && SubjectManager.Instance.RequiredSubjects.Count == 0)
            SubjectManager.Instance.AssignRandomRequiredSubjects();

        GameManager.GetInstance().OnStageClear();
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        // 결과(클리어/게임오버)는 한 판에 한 번만 처리
        if (hasResultHandled) return;

        if (current == GameManager.GameState.StageClear)
        {
            hasResultHandled = true;
            BGMManager.GetInstance().PlayBGM("clear_bgm");

            UpdateClearSubjectList();

            if (clearPanel != null)
                clearPanel.SetActive(true);
        }
        else if (current == GameManager.GameState.GameOver)
        {
            hasResultHandled = true;
            BGMManager.GetInstance().PlayBGM("fail_bgm");

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }

    private void HandleGameOverConfirmClicked()
    {
        StageProgressManager.GetInstance().OnStageFailed();
    }

    private void HandleClearConfirmClicked()
    {
        StageProgressManager.GetInstance().LoadNextStage();
    }

    private void UpdateClearSubjectList()
    {
        if (clearSubjectListParent == null || clearSubjectRowPrefab == null)
        {
            Debug.LogWarning("[Stage01Manager] clearSubjectListParent 또는 clearSubjectRowPrefab 미연결");
            return;
        }

        if (SubjectManager.Instance == null)
        {
            Debug.LogWarning("[Stage01Manager] SubjectManager가 씬에 없어 과목 목록을 표시할 수 없음");
            return;
        }

        // 기존 행 제거 (재호출 대비)
        foreach (Transform child in clearSubjectListParent)
            Destroy(child.gameObject);

        foreach (SubjectData subject in SubjectManager.Instance.RequiredSubjects)
        {
            ClearSubjectRow row = Instantiate(clearSubjectRowPrefab, clearSubjectListParent);
            row.Bind(subject.SubjectName);
        }
    }
}