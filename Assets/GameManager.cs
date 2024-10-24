using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //useless push
    public Canvas cm;
    CanvasManager canvasManager;
    public int score = 0;
    private void OnEnable()
    {
        canvasManager = cm.GetComponentInChildren<CanvasManager>();
        // canvasManager = cm.GetComponent<CanvasManager>();
        CircleScript.OnObjectDestroyed += HandleObjectDestroyed;
        SpinnerScript.OnSpinnerDestroyed += HandleSpinnerDestroyed;
        Slidercollider.OnSliderDestroyed += HandleSliderDestroyed;
    }
    private static GameManager _instance;
    // Start is called before the first frame update
    private void OnDisable()
    {
        CircleScript.OnObjectDestroyed -= HandleObjectDestroyed;
        SpinnerScript.OnSpinnerDestroyed -= HandleSpinnerDestroyed;
    }

    private void HandleObjectDestroyed(float remainingLifespan)
    {
        int points = Mathf.RoundToInt( 100 * (4 - remainingLifespan)) ;
        score += points;
        canvasManager.UpdateScore(score);
        PlayerPrefs.SetInt("Score", score);
        // Add your functionality here (e.g., updating UI, scores, etc.)
    }

    private void HandleSpinnerDestroyed(int scoreMultiplier)
    {
        score += Math.Abs(scoreMultiplier) * 1000;
        canvasManager.UpdateScore(score);
        PlayerPrefs.SetInt("Score", score);
    }
    private void HandleSliderDestroyed(int incomingScore)
    {
        score += incomingScore;
        canvasManager.UpdateScore(score);
        PlayerPrefs.SetInt("Score", score);
    }
}
