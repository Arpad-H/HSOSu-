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
    private string path = "Assets/Highscores/song1.txt";
    
    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);
        
        getHighscoreList();
        highscoreEntryTransformList = new List<Transform>();
        int x;
        if (highscoreEntryList.Count < 5)
        {
            x = highscoreEntryList.Count;
        }
        else
        {
            x = 5;
        }
        for (int i = 0; i < x; i++)
        {
            HighscoreEntry highscoreEntry = highscoreEntryList[i];
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void getHighscoreList()
    {
        
        highscoreEntryList = new List<HighscoreEntry>();
        path = path.Replace("1", PlayerPrefs.GetInt("Song").ToString());
        string[] lines = File.ReadAllLines(path);
        Debug.Log(lines);
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
        
       
    }

    public double getBorderScores()
    {
        getHighscoreList();
        double borderScore; 
        if (highscoreEntryList.Count > 5)
        {
             borderScore = highscoreEntryList[0].score;
        }
        else
        {
            borderScore = 0;
        }

        Debug.Log("HsELC: " + highscoreEntryList.Count);

        return borderScore;

    }



    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 100f;
        Debug.Log("1");
        
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        Debug.Log("1");          
        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;
        Debug.Log("1");        
        double points = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = points.ToString();
        Debug.Log("1");
        transformList.Add(entryTransform);
        Debug.Log("5");
    }
    
    public void AddHighscoreEntry(double score, string name)
    {
        
        path.Replace("1", PlayerPrefs.GetInt("Song").ToString());
        string entry = name + " " + score.ToString();
        
       File.AppendAllLines(path, new[] { entry });
    }
    
    
    
    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList= new List<HighscoreEntry>();
        
    }
    
    
    //single entry
    [System.Serializable]
    public class HighscoreEntry 
    {
        public double score;
        public string name;
        
        public HighscoreEntry(double score, string name)
        {
            this.score = score;
            this.name = name;
        }
    }

}


