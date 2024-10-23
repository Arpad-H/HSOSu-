using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate; 
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;
    public string path;
    
    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);
        
        /*string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        highscoreEntryList = highscores.highscoreEntryList;*/
        

        highscoreEntryList = new List<HighscoreEntry>(); 
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            string[] parts = line.Split(' ');
            if (parts.Length == 2 && double.TryParse(parts[1], out double number))
            {
                string name = parts[0];
                highscoreEntryList.Add(new HighscoreEntry(number, name));
            }
        }
       
        
        //sort
        for(int i = 0; i < highscoreEntryList.Count; i++)
        {
             for(int j = i; j < highscoreEntryList.Count; j++)
                    {
                        if (highscoreEntryList[j].score > highscoreEntryList[i].score)
                        {
                            HighscoreEntry tmp = highscoreEntryList[i];
                            highscoreEntryList[i] = highscoreEntryList[j];
                            highscoreEntryList[j] = tmp;
                        }
                    }
        }
        
        highscoreEntryTransformList = new List<Transform>();
        for (int i = 0; i < 5; i++/*HighscoreEntry highscoreEntry in highscoreEntryList*/)
        {
            HighscoreEntry highscoreEntry = highscoreEntryList[i];
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }
    
    
    
    
    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 100f;
        
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
                    
        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;
                    
        double points = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = points.ToString();
        
        transformList.Add(entryTransform);
    }
    
    private void AddHighscoreEntry(double score, string name)
    {
        HighscoreEntry highscoreEntry = new HighscoreEntry(score, name);
        
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        
        highscores.highscoreEntryList.Add(highscoreEntry);
        
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable",json);
        PlayerPrefs.Save();
    }
    
    
    
    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList= new List<HighscoreEntry>();
        
    }
    
    
    //single entry
    [System.Serializable]
    private class HighscoreEntry 
    {
        public double score;
        public string name;
        
        public HighscoreEntry(double score, string name)
        {
            this.score = score;
            this.name = name;
        }
    }

   /* public class NameValueReader
    {
        public static List<HighscoreEntry> ReadNameNumberPairs(string path)
        {
            var list = new List<HighscoreEntry>();
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');

                if (parts.Length == 2 && double.TryParse(parts[1], out double number))
                {
                    string name = parts[0];
                    list.Add(new HighscoreEntry(number, name));
                }

                return list;
            }
        }

    }*/
}


