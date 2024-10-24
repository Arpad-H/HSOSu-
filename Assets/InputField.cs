using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public HighscoreTable highscoreTable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
        public void ReadString(string s)
        {
            Debug.Log(s);
            highscoreTable = gameObject.AddComponent<HighscoreTable>();
            highscoreTable.AddHighscoreEntry((double) PlayerPrefs.GetInt("Score"), s);
        }
    
}
