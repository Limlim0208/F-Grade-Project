using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public static PlayerDataManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("PlayerDataManager");
            obj.AddComponent<PlayerDataManager>();
        }
        return Instance;
    }

    public string PlayerName { get; private set; } = "플레이어";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            PlayerName = name;
    }
}
