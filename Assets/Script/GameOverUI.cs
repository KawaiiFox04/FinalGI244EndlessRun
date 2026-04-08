// GameOverUI.cs
// ติดไว้ที่ GameObject ใน Scene: GameOver

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "Gameplay";
    public string startSceneName    = "StartScene";

    [Header("UI References")]
    public TextMeshProUGUI currentScoreText;   // คะแนนรอบนี้
    public TextMeshProUGUI bestScoreText;      // คะแนนสูงสุด
    public TextMeshProUGUI compareText;        // "มากกว่า / น้อยกว่า X คะแนน"
    public TextMeshProUGUI newRecordText;      // "NEW RECORD!" (ซ่อนถ้าไม่ใช่ Record)
    public TextMeshProUGUI coinEarnedText;     // เหรียญที่ได้รอบนี้

    [Header("History Panel")]
    public GameObject historyPanel;
    public Transform  historyContent;
    public GameObject historyItemPrefab;

    void Start()
    {
        int current  = GameData.CurrentScore;
        int best     = GameData.BestScore;
        int coinsEarned = GameData.CurrentCoins;

        // คะแนน
        currentScoreText?.SetText($"{current:N0}");
        bestScoreText?.SetText($"Best: {best:N0}");
        coinEarnedText?.SetText($"🪙 +{coinsEarned}");

        // เปรียบเทียบ
        int diff = current - best;
        if (diff > 0)
        {
            compareText?.SetText($"มากกว่า Best เดิม +{diff:N0} คะแนน!");
            newRecordText?.gameObject.SetActive(true);
        }
        else if (diff < 0)
        {
            compareText?.SetText($"น้อยกว่า Best อยู่ {Mathf.Abs(diff):N0} คะแนน");
            newRecordText?.gameObject.SetActive(false);
        }
        else
        {
            compareText?.SetText("เท่ากับ Best Score!");
            newRecordText?.gameObject.SetActive(false);
        }

        // ซ่อน History ก่อน
        historyPanel?.SetActive(false);
    }

    // ---- Buttons ----

    public void OnRestartPressed()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnHistoryPressed()
    {
        bool isOpen = !historyPanel.activeSelf;
        historyPanel?.SetActive(isOpen);
        if (isOpen) LoadHistory();
    }

    // ---- Load History ----
    void LoadHistory()
    {
        if (historyContent == null || historyItemPrefab == null) return;

        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        var records = GameData.GetHistory();

        if (records.Count == 0)
        {
            var empty = Instantiate(historyItemPrefab, historyContent);
            empty.GetComponentInChildren<TextMeshProUGUI>()?.SetText("ยังไม่มีประวัติ");
            return;
        }

        for (int i = 0; i < records.Count; i++)
        {
            var rec  = records[i];
            var item = Instantiate(historyItemPrefab, historyContent);
            var tmps = item.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var t in tmps)
            {
                switch (t.gameObject.name)
                {
                    case "Rank":  t.SetText($"#{i + 1}");              break;
                    case "Score": t.SetText($"Score: {rec.score:N0}"); break;
                    case "Coin":  t.SetText($"🪙 {rec.coins:N0}");     break;
                    case "Date":  t.SetText(rec.date);                 break;
                }
            }
        }
    }
}