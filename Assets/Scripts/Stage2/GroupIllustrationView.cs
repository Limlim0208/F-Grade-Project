using UnityEngine;

// Day1 그룹 채팅 배경의 실루엣 -> 본모습 리빌 연출
public class GroupIllustrationView : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSlot
    {
        public string speakerId; // "A", "B", "C"
        public GameObject silhouette;
        public GameObject revealed;
    }

    [SerializeField] private CharacterSlot[] slots;

    void Awake()
    {
        ResetAll();
    }

    // 전부 실루엣 상태로 초기화
    public void ResetAll()
    {
        foreach (var slot in slots)
        {
            if (slot.silhouette != null) slot.silhouette.SetActive(true);
            if (slot.revealed != null) slot.revealed.SetActive(false);
        }
    }

    // 이미 리빌된 캐릭터에 다시 호출해도 안전함
    public void Reveal(string speakerId)
    {
        var slot = System.Array.Find(slots, s => s.speakerId == speakerId);
        if (slot == null) return;

        if (slot.revealed != null) slot.revealed.SetActive(true);
        if (slot.silhouette != null) slot.silhouette.SetActive(false);
    }
}
