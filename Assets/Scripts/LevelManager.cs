using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavedData
{
    private static readonly SavedData instance = new SavedData();

    private SavedData()
    {
        // TODO: load saved data from files
    }

    public static SavedData GetInstance()
    {
        return instance;
    }

    public string QuerySavedData(string saveID, string path)
    {
        return "";
    }

    public void UpdateSavedData(string saveID, string path, string data)
    {

    }

    public void WriteBack()
    {
        // write data back to file
    }
}

public class LevelManager : MonoBehaviour
{
    public class LevelInfo
    {
        public readonly string objectName;
        public readonly string levelName;
        public readonly string sceneName;
        public int score;
        public LevelInfo(string mObjectName,
                string mLevelName, string mSceneName, int mScore = 0)
        {
            objectName = mObjectName;
            levelName = mLevelName;
            sceneName = mSceneName;
            score = mScore;
        }
    }
    List<LevelInfo> levels = new List<LevelInfo>();
    public static string selectedLevel = "";

    public int RegisterLevel(LevelBlock level)
    {
        GameObject obj = level.gameObject;
        string objectName = obj.name;
        string levelName = GameObject.Find(objectName + "Canvas/LevelName").GetComponent<Text>().text;
        string sceneName = level.sceneName;
        levels.Add(new LevelInfo(objectName, levelName, sceneName));
        return levels.Count - 1;
    }

    public void OnLevelClicked(int index)
    {
        selectedLevel = levels[index].sceneName;
        SceneManager.LoadScene("GameScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        // should load score data from local flles here？

    }

    // Update is called once per frame
    void Update()
    {

    }
}
