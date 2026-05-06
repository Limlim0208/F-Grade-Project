using UnityEngine;
using UnityEngine.UI;

public class GameOverButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        GameManager.GetInstance().OnGameOver();
    }
}