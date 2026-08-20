using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string sceneName;   // 씬 이름으로 이동할 때
    [SerializeField] private int stageId = -1;   // 스테이지 번호로 이동할 때 (-1이면 미사용)

    // 외부(StageSelectButton)에서 stageId 읽을 수 있게 getter 추가
    public int StageId => stageId;
    public void OnClick()
    {
        if (stageId >= 0)
            StageProgressManager.GetInstance().SetStage(stageId);      // 스테이지 번호로 이동
        else if (!string.IsNullOrEmpty(sceneName))
            SceneChanger.GetInstance().LoadScene(sceneName);   // 씬 이름으로 이동
    }
}