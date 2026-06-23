using UnityEngine;

public class TimerStarter : MonoBehaviour
{
    void Start()
    {
        TimerManager.GetInstance().StartTimer();
    }
}