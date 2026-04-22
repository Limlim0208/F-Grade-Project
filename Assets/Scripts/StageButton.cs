using UnityEngine;

public class StageButton : MonoBehaviour
{
    [SerializeField] private int stageId; // Inspector에서 스테이지 번호 입력
    [SerializeField] private string customSceneName; // 특수 스테이지용, 비워두면 stageId 사용

    public void OnClick()
    {
        if (!string.IsNullOrEmpty(customSceneName))
            SceneChanger.GetInstance().LoadScene(customSceneName);
        else
            StageManager.GetInstance().SetStage(stageId);
    }
}