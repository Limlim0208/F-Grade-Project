using UnityEngine;

public class QuickButton : MonoBehaviour
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