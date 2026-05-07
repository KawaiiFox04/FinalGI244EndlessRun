using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunRecord
{
    public int   score;
    public int   coins;
    public string date;

    public RunRecord(int s, int c)
    {
        score = s;
        coins = c;
        date  = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}

public static class GameData
{
    public static int CurrentScore;
    public static int CurrentCoins;
    
    public static int BestScore
    {
        get => PlayerPrefs.GetInt("BestScore", 0);
        set { PlayerPrefs.SetInt("BestScore", value); PlayerPrefs.Save(); }
    }

    public static int TotalCoins
    {
        get => PlayerPrefs.GetInt("TotalCoins", 0);
        set { PlayerPrefs.SetInt("TotalCoins", value); PlayerPrefs.Save(); }
    }
    
    private const string HistoryKey = "RunHistory";
    private const int    MaxHistory = 20;

    public static List<RunRecord> GetHistory()
    {
        string json = PlayerPrefs.GetString(HistoryKey, "");
        if (string.IsNullOrEmpty(json)) return new List<RunRecord>();
        
        var wrapper = JsonUtility.FromJson<RecordListWrapper>(json);
        return wrapper?.records ?? new List<RunRecord>();
    }

    public static void SaveRecord(RunRecord record)
    {
        var list = GetHistory();
        list.Insert(0, record);
        if (list.Count > MaxHistory) list.RemoveAt(list.Count - 1);

        var wrapper = new RecordListWrapper { records = list };
        PlayerPrefs.SetString(HistoryKey, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    public static void ClearHistory()
    {
        PlayerPrefs.DeleteKey(HistoryKey);
        PlayerPrefs.Save();
    }
    
    public static bool IsMusicOn
    {
        get => PlayerPrefs.GetInt("MusicOn", 1) == 1;
        set { PlayerPrefs.SetInt("MusicOn", value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static bool IsSfxOn
    {
        get => PlayerPrefs.GetInt("SfxOn", 1) == 1;
        set { PlayerPrefs.SetInt("SfxOn", value ? 1 : 0); PlayerPrefs.Save(); }
    }
    
    [System.Serializable]
    private class RecordListWrapper
    {
        public List<RunRecord> records;
    }
}