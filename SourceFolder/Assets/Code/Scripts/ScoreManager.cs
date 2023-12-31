using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore
{
    public string playerName;
    public double time;
}

public class ScoreManager : MonoBehaviour
{
    private void Awake()
    {
        if (scoreInstance == null)
        {
            scoreInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public const int EntryCount = 5;

    public struct ScoreEntry
    {
        public string playerName;
        public double time;

        public ScoreEntry(string playerName, double time)
        {
            this.playerName = playerName;
            this.time = time;
        }
    }

    private static ScoreManager scoreInstance;

    public static ScoreManager Instance
    {
        get
        {
            if (scoreInstance == null)
            {
                scoreInstance = FindObjectOfType<ScoreManager>();
                if (scoreInstance == null)
                {
                    GameObject gameObject = new GameObject("ScoreManager");
                    scoreInstance = gameObject.AddComponent<ScoreManager>();
                }
            }
            return scoreInstance;
        }
    }

    private List<ScoreEntry> mEntries;

    private List<ScoreEntry> Entries
    {
        get
        {
            if (mEntries == null)
            {
                mEntries = new List<ScoreEntry>();
                LoadScores();
            }
            return mEntries;
        }
    }

    private void SortScores()
    {
        mEntries.Sort((a, b) => a.time.CompareTo(b.time));
    }

    private void LoadScores()
    {
        mEntries = new List<ScoreEntry>();

        for (int i = 0; i < EntryCount; ++i)
        {
            ScoreEntry entry;
            entry.playerName = PlayerPrefs.GetString("scoreboard" + "[" + i + "].name", "");
            entry.time = PlayerPrefs.GetFloat("scoreboard" + "[" + i + "].time", 0f);
            mEntries.Add(entry);
        }

        SortScores();
    }

    private void SaveScores()
    {
        for (int i = 0; i < EntryCount; ++i)
        {
            var entry = mEntries[i];
            PlayerPrefs.SetString("scoreboard" + "[" + i + "].name", entry.playerName);
            PlayerPrefs.SetFloat("scoreboard" + "[" + i + "].time", (float)entry.time);
        }

        PlayerPrefs.Save();
    }

    public ScoreEntry GetEntry(int index)
    {
        return Entries[index];
    }

    public List<ScoreEntry> GetScores()
    {
        return Entries;
    }

    public void Record(string playerName, double time)
    {
        ScoreEntry newEntry = new ScoreEntry(playerName, time);
        Entries.Add(newEntry);
        SortScores();
        Entries.RemoveAt(Entries.Count - 1);
        SaveScores();
    }

    public bool IsPlayerScoreTop5(double playerScore)
    {
        LoadScores();
        return Entries.Count < EntryCount || Entries[EntryCount - 1].time > playerScore;
    }

    public void ClearLeaderboard()
    {
        mEntries.Clear();
        for (int i = 0; i < EntryCount; ++i)
        {
            ScoreEntry emptyEntry = new ScoreEntry("", 99999);
            mEntries.Add(emptyEntry);
        }
        SaveScores();
    }
}
