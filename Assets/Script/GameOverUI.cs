using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "Gameplay";
    public string startSceneName    = "StartScene";

    [Header("Score UI")]
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
        SetupScoreDisplay();
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverBGM();
        
        if (historyPanel != null)
            historyPanel.SetActive(true);
        LoadHistory();
    }
    
    private void SetupScoreDisplay()
    {
        int current     = GameData.CurrentScore;
        int best        = GameData.BestScore;
        int coinsEarned = GameData.CurrentCoins;
        
        if (currentScoreText != null)
            currentScoreText.SetText($"Score: {current:N0}");
        
        if (bestScoreText != null)
            bestScoreText.SetText($"Best: {best:N0}");
        
        if (coinEarnedText != null)
            coinEarnedText.SetText($"Coin: +{coinsEarned}");
        
        int diff = current - best;
        if (diff > 0)
        {
            if (compareText   != null) compareText.SetText($"Better than Best +{diff:N0} pts!");
            if (newRecordText != null) newRecordText.gameObject.SetActive(true);
        }
        else if (diff < 0)
        {
            if (compareText   != null) compareText.SetText($"Below Best by {Mathf.Abs(diff):N0} pts");
            if (newRecordText != null) newRecordText.gameObject.SetActive(false);
        }
        else
        {
            if (compareText   != null) compareText.SetText("Matched Best Score!");
            if (newRecordText != null) newRecordText.gameObject.SetActive(false);
        }
    }
    
    public void OnRestartPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnMainMenuPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
        SceneManager.LoadScene(startSceneName);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    public void OnCloseHistoryPressed()
    {
        if (historyPanel != null)
            historyPanel.SetActive(false);
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
            if (tmp != null) tmp.SetText("No history yet");
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