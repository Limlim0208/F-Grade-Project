using UnityEngine;
using UnityEngine.UI;

public class LogicTrigger : MonoBehaviour
{
    [SerializeField] private EscapingButton escapingButton;     // 로직 A
    [SerializeField] private ButtonRandomMove buttonRandomMove; // 로직 B
    [SerializeField] private ChatbotPattern chatbotPattern; // 로직 C

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        int rand = Random.Range(0, 3);
        // 로직 랜덤 발생
        if (rand == 0)
        {
            escapingButton.StartEscaping();
        }
        else if (rand == 1)
        {
            buttonRandomMove.StartSequence();
        }
        else
        {
            chatbotPattern.StartSequence();
        }

        GetComponent<Button>().interactable = false;
    }
}