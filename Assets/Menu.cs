using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public HighscoreTable highscoreTable;
    public GameObject popUpPanel;
    public GameObject MainMenu;public GameObject Next;public GameObject Previous;public GameObject ScrollView;
    public GameManager gameManager;
    public InputField inputField;
    
    private void Start()
    {
        PlayerPrefs.SetInt("Song", 1);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Awake()
    {
        highscoreTable = new HighscoreTable();
        double borderScores = highscoreTable.getBorderScores();
        double score = (double) PlayerPrefs.GetInt("Score");
        
        if(score >= borderScores && score > 0){
            popUpPanel.transform.Find("scoreText").GetComponent<Text>().text = score.ToString();

            popUpPanel.SetActive(true);
            MainMenu.SetActive(false);
            Next.SetActive(false);
            Previous.SetActive(false);
            ScrollView.SetActive(false);

        }
    }
    
    
    
    
    
    
}
