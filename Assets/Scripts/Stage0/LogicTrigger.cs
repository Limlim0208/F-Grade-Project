using UnityEngine;
using UnityEngine.UI;

public class LogicTrigger : MonoBehaviour
{
    [SerializeField] private EscapingButton escapingButton;     // 로직 A
    [SerializeField] private ButtonRandomMove buttonRandomMove; // 로직 B
    [SerializeField] private ChatbotPattern chatbotPattern;     // 로직 C

    void Start()
    {
    }

    public void OnClicked()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
            escapingButton.StartEscaping();
        else if (rand == 1)
            buttonRandomMove.StartSequence();
        else
            chatbotPattern.StartSequence();

        Button btn = GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.interactable = false;
    }
}