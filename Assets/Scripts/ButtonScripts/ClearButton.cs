using UnityEngine;
using UnityEngine.UI;

public class ClearButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        GameManager.GetInstance().OnStageClear();
    }
}