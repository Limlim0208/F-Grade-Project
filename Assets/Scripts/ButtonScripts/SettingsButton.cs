using UnityEngine;

public class SettingsButton : MonoBehaviour
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