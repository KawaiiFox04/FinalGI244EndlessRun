using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject recordItemPrefab;

    public void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        List<RunRecord> records = HistoryManager.LoadHistory();

        for (int i = records.Count - 1; i >= 0; i--)
        {
            GameObject go = Instantiate(recordItemPrefab, contentParent);
            TextMeshProUGUI[] texts = go.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 3)
            {
                texts[0].text = "Score: " + records[i].score;
                texts[1].text = "Coins: " + records[i].coins;
                texts[2].text = records[i].date;
            }
        }
    }
}