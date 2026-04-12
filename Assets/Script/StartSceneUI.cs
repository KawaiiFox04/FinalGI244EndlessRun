using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartSceneUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "Gameplay";

    [Header("Main Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button historyButton;

    [Header("Coin Display")]
    public TextMeshProUGUI totalCoinText;

    [Header("Option Panel")]
    public GameObject optionPanel;
    public Toggle     musicToggle;
    public Toggle     sfxToggle;

    [Header("History Panel")]
    public GameObject historyPanel;
    public Transform  historyContent;
    public GameObject historyItemPrefab;

    private void Start()
    {
        RefreshCoinDisplay();

        if (musicToggle != null) musicToggle.isOn = GameData.IsMusicOn;
        if (sfxToggle   != null) sfxToggle.isOn   = GameData.IsSfxOn;

        // เล่นเพลง StartScene
        if (AudioManager.Instance != null) AudioManager.Instance.PlayStartBGM();

        if (optionPanel  != null) optionPanel.SetActive(false);
        if (historyPanel != null) historyPanel.SetActive(false);

        if (startButton   != null) startButton.onClick.AddListener(OnStartPressed);
        if (optionButton  != null) optionButton.onClick.AddListener(OnOptionPressed);
        if (historyButton != null) historyButton.onClick.AddListener(OnHistoryPressed);

        if (musicToggle != null) musicToggle.onValueChanged.AddListener(v =>
        {
            GameData.IsMusicOn = v;
            if (AudioManager.Instance != null) AudioManager.Instance.SetMusic(v);
        });
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(v =>
        {
            GameData.IsSfxOn = v;
            if (AudioManager.Instance != null) AudioManager.Instance.SetSFX(v);
        });
    }

    // ================================================================
    //  Coin
    // ================================================================
    private void RefreshCoinDisplay()
    {
        if (totalCoinText != null)
            totalCoinText.SetText($"Coin: {GameData.TotalCoins:N0}");
    }

    // ================================================================
    //  History
    // ================================================================
    public void OnHistoryPressed()
    {
        if (historyPanel == null) return;
        historyPanel.SetActive(true);
        LoadHistory();
    }

    public void OnCloseHistoryPressed()
    {
        if (historyPanel != null)
            historyPanel.SetActive(false);
    }

    private void LoadHistory()
    {
        if (historyContent == null || historyItemPrefab == null) return;

        // ลบของเก่าออกก่อน
        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        var records = GameData.GetHistory();

        if (records.Count == 0)
        {
            var empty = Instantiate(historyItemPrefab, historyContent);
            var tmps  = empty.GetComponentsInChildren<TextMeshProUGUI>();
            if (tmps.Length > 0) tmps[0].SetText("No history yet");
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

    // ================================================================
    //  Option
    // ================================================================
    public void OnOptionPressed()
    {
        if (optionPanel == null) return;
        optionPanel.SetActive(!optionPanel.activeSelf);
    }

    public void OnCloseOptionPressed()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    // ================================================================
    //  Start Game
    // ================================================================
    public void OnStartPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(gameplaySceneName);
    }
}