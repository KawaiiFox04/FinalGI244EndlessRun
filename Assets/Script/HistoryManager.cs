using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunRecord
{
    public int score;
    public int coins;
    public string date;
}

[System.Serializable]
public class RecordWrapper
{
    public List<RunRecord> records = new List<RunRecord>();
}

public static class HistoryManager
{
    private const string SAVE_KEY = "RunHistory";
    private const int MAX_RECORDS = 10;

    public static void SaveRun(int score, int coins)
    {
        RecordWrapper wrapper = LoadWrapper();

        wrapper.records.Add(new RunRecord
        {
            score = score,
            coins = coins,
            date = System.DateTime.Now.ToString("dd/MM/yy HH:mm")
        });

        if (wrapper.records.Count > MAX_RECORDS)
            wrapper.records.RemoveAt(0);

        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    public static List<RunRecord> LoadHistory()
    {
        return LoadWrapper().records;
    }

    private static RecordWrapper LoadWrapper()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        if (string.IsNullOrEmpty(json))
            return new RecordWrapper();
        return JsonUtility.FromJson<RecordWrapper>(json);
    }
}