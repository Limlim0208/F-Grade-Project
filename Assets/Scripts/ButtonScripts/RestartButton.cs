using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickRestart);
    }

    void OnClickRestart()
    {
        GameManager.GetInstance().RestartStage();
    }
}
