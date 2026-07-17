using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void OnClickOpen()
    {
        SettingsManager.GetInstance().OpenSettings();
    }

    public void OnClickClose()
    {
        SettingsManager.GetInstance().CloseSettings();
    }
}