// GameData.cs
// ไฟล์กลางเก็บข้อมูลที่ต้องส่งข้าม Scene
// ไม่ต้องติดกับ GameObject ไหนเลย เป็นแค่ Static Class

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunRecord
{
    public int   score;
    public int   coins;
    public string date;   // วันเวลาที่เล่น

    public RunRecord(int s, int c)
    {
        score = s;
        coins = c;
        date  = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}

public static class GameData
{
    // ---- รอบปัจจุบัน (ส่งไป GameOver) ----
    public static int CurrentScore;
    public static int CurrentCoins;

    // ---- ข้อมูลถาวร (PlayerPrefs) ----
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

    // ---- History (เก็บสูงสุด 20 รอบ) ----
    private const string HistoryKey = "RunHistory";
    private const int    MaxHistory = 20;

    public static List<RunRecord> GetHistory()
    {
        string json = PlayerPrefs.GetString(HistoryKey, "");
        if (string.IsNullOrEmpty(json)) return new List<RunRecord>();

        // Unity JsonUtility ไม่ support List โดยตรง ต้องห่อ
        var wrapper = JsonUtility.FromJson<RecordListWrapper>(json);
        return wrapper?.records ?? new List<RunRecord>();
    }

    public static void SaveRecord(RunRecord record)
    {
        var list = GetHistory();
        list.Insert(0, record);   // ใส่ไว้หัว (ล่าสุดก่อน)
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

    // ---- Audio Settings ----
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

    // ---- Wrapper สำหรับ JsonUtility ----
    [System.Serializable]
    private class RecordListWrapper
    {
        public List<RunRecord> records;
    }
}