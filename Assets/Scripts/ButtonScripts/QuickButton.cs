using UnityEngine;

public class QuickButton : MonoBehaviour
{
    public void OnClickOpen()
    {
        UIManager.GetInstance().OpenSettings();
    }

    public void OnClickClose()
    {
        UIManager.GetInstance().CloseSettings();
    }
}