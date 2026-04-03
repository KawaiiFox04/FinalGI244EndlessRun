using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject historyPanel;
    public GameObject optionPanel;

    void Start()
    {
        ShowMain();
    }

    public void OnPlayButton()
    {
        GameManager.instance.ResetSession();
        SceneManager.LoadScene("GamePlay");
    }

    public void OnHistoryButton()
    {
        mainPanel.SetActive(false);
        historyPanel.SetActive(true);
        historyPanel.GetComponent<HistoryUI>().Refresh();
    }

    public void OnOptionButton()
    {
        mainPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        historyPanel.SetActive(false);
        optionPanel.SetActive(false);
    }
}