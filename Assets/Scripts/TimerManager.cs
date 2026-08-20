using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    public static TimerManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("TimerManager");
            obj.AddComponent<TimerManager>();
        }
        return Instance;
    }
    public float timeLimit = 60f;
    public float TimeRemaining { get; private set; }
    public bool IsTimerRunning { get; private set; }
    [SerializeField] private Text timerText; // �ð� ǥ�ÿ� �ؽ�Ʈ (��ä�� ����)
    [SerializeField] private Image timerBar; // Ÿ�̸� �� �̹��� (��ä�� ����)
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���
    }

    public void StartTimer()
    {
        if (timerText == null)
        {
            GameObject textObj = GameObject.Find("TimerText");
            if (textObj != null)
                timerText = textObj.GetComponent<Text>();
        }
        if (timerBar == null)
        {
            GameObject barObj = GameObject.Find("TimerBar");
            if (barObj != null)
                timerBar = barObj.GetComponent<Image>();
        }

        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (timerBar != null)
            timerBar.gameObject.SetActive(true);

        TimeRemaining = timeLimit;
        IsTimerRunning = true;
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
    }
    public void PauseTimer() => IsTimerRunning = false;
    public void ResumeTimer() => IsTimerRunning = true;

    void Update()
    {
        if (!IsTimerRunning) return;
        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0)
            TimeRemaining = 0; 

        // UI ������Ʈ
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt((TimeRemaining * 100f) % 100f);
            timerText.text = string.Format("남은 접속 시간 {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
        UpdateTimerBar();

        // Ÿ�̸� ���� 0 ���ϰ� ���� �� ���� ���� ����
        if (TimeRemaining <= 0)
        {
            StopTimer();
            GameManager.GetInstance().OnGameOver();
        }
    }

    // ���� �ð��� ���� Ÿ�̸� ���� ���� ��ȭ (��ä�� ����)
    private void UpdateTimerBar()
    {
        timerBar.fillAmount = TimeRemaining / timeLimit;
        if (TimeRemaining <= 15f)
        {
            timerBar.color = new Color32(255, 61, 0, 255);
            BGMManager.GetInstance().PlayBGM("stage0_fast"); // ���� ���� ���� ��� ��ȯ (Stage0 ����)
        }
        else if (TimeRemaining <= 30f)
            timerBar.color = new Color32(255, 160, 0, 255); 
        else
            timerBar.color = new Color32(29, 233, 182, 255); 
    }

    // �ð� ���� �Լ� (��ä�� ����)
    public void ReduceTime(float amount)
    {
        TimeRemaining = Mathf.Max(0, TimeRemaining - amount);
    }

    // �� �ε�� ������ Ÿ�̸� �ʱ�ȭ (��ä�� ����)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopTimer();
        // �� ������ timerText �ٽ� ã��
        GameObject textObj = GameObject.Find("TimerText"); 
        if (textObj != null)
            timerText = textObj.GetComponent<Text>();

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        GameObject barObj = GameObject.Find("TimerBar"); 
        if (barObj != null) 
            timerBar = barObj.GetComponent<Image>(); 
        if (timerBar != null) 
            timerBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ����
    }
}