using UnityEngine;
using UnityEngine.UI;

public class LogicTrigger : MonoBehaviour
{
    [SerializeField] private EscapingButton escapingButton;     // 로직 A
    [SerializeField] private ButtonRandomMove buttonRandomMove; // 로직 B

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        // 로직 랜덤 발생
        if (Random.Range(0, 2) == 0)
        {
            escapingButton.StartEscaping();
        }
        else
        {
            buttonRandomMove.StartSequence();
        }
    }
}