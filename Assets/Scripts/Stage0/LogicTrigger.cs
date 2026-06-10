using UnityEngine;
using UnityEngine.UI;

public class LogicTrigger : MonoBehaviour
{
    [SerializeField] private EscapingButton escapingButton;     // 로직 A
    [SerializeField] private ButtonRandomMove buttonRandomMove; // 로직 B
    [SerializeField] private ChatbotPattern chatbotPattern;     // 로직 C

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        //int rand = Random.Range(0, 3);
        int rand = 2; // [테스트] 랜덤 정수 하드코딩 (임유미 추가)

        // 로직 랜덤 발생
        if (rand == 0)
            escapingButton.StartEscaping();
        else if (rand == 1)
            buttonRandomMove.StartSequence();
        else
            chatbotPattern.StartSequence();

        // 타이머 시작
        //TimerManager.GetInstance().StartTimer();

        // 재클릭 방지
        Button btn = GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.interactable = false;
    }
}