using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class HighscoreTable : MonoBehaviour
{
    private Transform _entryContainer;
    private Transform _entryTemplate; 
    private List<HighscoreEntry> _highscoreEntryList;
    private List<Transform> _highscoreEntryTransformList;
    private const string Path = "Assets/Highscores/song1.txt";

    private void OnEnable()
    {
        _entryContainer = transform.Find("highscoreEntryContainer");
        _entryTemplate = _entryContainer.Find("highscoreEntryTemplate");

        _entryTemplate.gameObject.SetActive(false);
        getHighscoreList();
        _highscoreEntryTransformList = new List<Transform>();
        int x;
        if (_highscoreEntryList.Count < 5)
        {
            x = _highscoreEntryList.Count;
        }
        else
        {
            x = 5;
        }
        for (var i = 0; i < x; i++)
        {
            var highscoreEntry = _highscoreEntryList[i];
            CreateHighscoreEntryTransform(highscoreEntry, _entryContainer, _highscoreEntryTransformList);
        }
    }

    private void OnDisable()
    {
        for (var i = 0; i < _highscoreEntryList.Count; i++) { Destroy(_highscoreEntryTransformList[i].gameObject); }
    }
    
    private void getHighscoreList()
    {
        
        _highscoreEntryList = new List<HighscoreEntry>();
        var path = Path.Replace("1", PlayerPrefs.GetInt("Song").ToString());
        var lines = File.ReadAllLines(path);
        // Debug.Log(lines.Length);
        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            if (parts.Length == 2 && double.TryParse(parts[1], out var number))
            {
                var playerName = parts[0];
                _highscoreEntryList.Add(new HighscoreEntry(number, playerName));
            }
        }
       
        
        //sort
        for(var i = 0; i < _highscoreEntryList.Count; i++)
        {
            for(var j = i; j < _highscoreEntryList.Count; j++)
            {
                if (_highscoreEntryList[j].score > _highscoreEntryList[i].score)
                {
                    (_highscoreEntryList[i], _highscoreEntryList[j]) = (_highscoreEntryList[j], _highscoreEntryList[i]);
                }
            }
        }   
        
       
    }

    public double getBorderScores()
    {
        getHighscoreList();
        double borderScore; 
        if (_highscoreEntryList.Count > 5)
        {
             borderScore = _highscoreEntryList[0].score;
        }
        else
        {
            borderScore = 0;
        }

        // Debug.Log("HsELC: " + highscoreEntryList.Count);

        return borderScore;

    }



    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        const float templateHeight = 100f;
        // Debug.Log("1");
        
        var entryTransform = Instantiate(_entryTemplate, container);
        var entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        // Debug.Log("1");          
        var playerName = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = playerName;
        // Debug.Log("1");        
        var points = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = points.ToString();
        // Debug.Log("1");
        transformList.Add(entryTransform);
        // Debug.Log("5");
    }
    
    public void AddHighscoreEntry(double score, string playerName)
    {
        
        var path = Path.Replace("1", PlayerPrefs.GetInt("Song").ToString());
        var entry = playerName + " " + score.ToString();
        
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


