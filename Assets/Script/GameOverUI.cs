using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "Gameplay";

    [Header("UI References")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI compareText;
    public TextMeshProUGUI newRecordText;
    public TextMeshProUGUI coinEarnedText;

    [Header("History Panel")]
    public GameObject historyPanel;
    public Transform  historyContent;
    public GameObject historyItemPrefab;

    private void Start()
    {
        int current     = GameData.CurrentScore;
        int best        = GameData.BestScore;
        int coinsEarned = GameData.CurrentCoins;

        if (currentScoreText != null) currentScoreText.SetText(current.ToString("N0"));
        if (bestScoreText    != null) bestScoreText.SetText($"Best: {best:N0}");
        if (coinEarnedText   != null) coinEarnedText.SetText($"Coin: +{coinsEarned}");

        int diff = current - best;
        if (diff > 0)
        {
            if (compareText   != null) compareText.SetText($"มากกว่า Best เดิม +{diff:N0} คะแนน!");
            if (newRecordText != null) newRecordText.gameObject.SetActive(true);
        }
        else if (diff < 0)
        {
            if (compareText   != null) compareText.SetText($"น้อยกว่า Best อยู่ {Mathf.Abs(diff):N0} คะแนน");
            if (newRecordText != null) newRecordText.gameObject.SetActive(false);
        }
        else
        {
            if (compareText   != null) compareText.SetText("เท่ากับ Best Score!");
            if (newRecordText != null) newRecordText.gameObject.SetActive(false);
        }

        if (historyPanel != null) historyPanel.SetActive(false);
    }

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
        if (historyPanel == null) return;
        bool isOpen = !historyPanel.activeSelf;
        historyPanel.SetActive(isOpen);
        if (isOpen) LoadHistory();
    }

    private void LoadHistory()
    {
        if (historyContent == null || historyItemPrefab == null) return;

        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        var records = GameData.GetHistory();

        if (records.Count == 0)
        {
            var empty = Instantiate(historyItemPrefab, historyContent);
            var tmp   = empty.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.SetText("ยังไม่มีประวัติ");
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
                    case "Coin":  t.SetText($"Coin: {rec.coins:N0}");  break;
                    case "Date":  t.SetText(rec.date);                 break;
                }
            }
        }
    }
}