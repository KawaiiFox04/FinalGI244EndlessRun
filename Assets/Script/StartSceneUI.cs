// StartSceneUI.cs
// ติดไว้ที่ GameObject ใน Scene: StartScene

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

    [Header("Coin Display (มุมขวาบน)")]
    public TextMeshProUGUI totalCoinText;

    [Header("Option Panel")]
    public GameObject optionPanel;
    public Toggle     musicToggle;
    public Toggle     sfxToggle;

    [Header("History Panel (มุมซ้าย)")]
    public GameObject         historyPanel;       // Panel ที่มี ScrollView
    public Transform          historyContent;     // Content ใน ScrollRect
    public GameObject         historyItemPrefab;  // Prefab แต่ละแถว History

    void Start()
    {
        // แสดง Coin ปัจจุบัน
        RefreshCoinDisplay();

        // ตั้งค่า Toggle ตาม Setting ที่บันทึกไว้
        if (musicToggle != null) musicToggle.isOn = GameData.IsMusicOn;
        if (sfxToggle   != null) sfxToggle.isOn   = GameData.IsSfxOn;

        // ปิด Panel ก่อน
        optionPanel?.SetActive(false);

        // โหลด History
        LoadHistory();

        // Hook Buttons
        startButton? .onClick.AddListener(OnStartPressed);
        optionButton?.onClick.AddListener(OnOptionPressed);

        if (musicToggle != null) musicToggle.onValueChanged.AddListener(v => GameData.IsMusicOn = v);
        if (sfxToggle   != null) sfxToggle.onValueChanged.AddListener  (v => GameData.IsSfxOn   = v);
    }

    // ----------------------------------------------------------------
    void RefreshCoinDisplay()
    {
        totalCoinText?.SetText($"🪙 {GameData.TotalCoins:N0}");
    }

    // ----------------------------------------------------------------
    //  History
    // ----------------------------------------------------------------
    void LoadHistory()
    {
        if (historyContent == null || historyItemPrefab == null) return;

        // ลบของเก่าก่อน
        foreach (Transform child in historyContent)
            Destroy(child.gameObject);

        var records = GameData.GetHistory();

        if (records.Count == 0)
        {
            // แสดงข้อความ "ยังไม่มีประวัติ"
            var empty = Instantiate(historyItemPrefab, historyContent);
            var texts = empty.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].SetText("ยังไม่มีประวัติการเล่น");
            return;
        }

        for (int i = 0; i < records.Count; i++)
        {
            var rec  = records[i];
            var item = Instantiate(historyItemPrefab, historyContent);
            var tmps = item.GetComponentsInChildren<TextMeshProUGUI>();

            // Prefab ควรมี TMP อย่างน้อย 3 ตัว: Rank | Score | Coin | Date
            // ตั้งชื่อ GameObject ใน Prefab ว่า "Rank", "Score", "Coin", "Date"
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

    // ----------------------------------------------------------------
    //  Button Handlers
    // ----------------------------------------------------------------
    public void OnStartPressed()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnOptionPressed()
    {
        bool isOpen = !optionPanel.activeSelf;
        optionPanel?.SetActive(isOpen);
    }

    public void OnCloseOptionPressed()
    {
        optionPanel?.SetActive(false);
    }
}