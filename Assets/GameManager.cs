using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //useless push
    public Canvas cm;
    CanvasManager canvasManager;
    public int score = 0;
    private void OnEnable()
    {
        canvasManager = cm.GetComponent<CanvasManager>();
        CircleScript.OnObjectDestroyed += HandleObjectDestroyed;
    }
    private static GameManager _instance;
    // Start is called before the first frame update
    private void OnDisable()
    {
        CircleScript.OnObjectDestroyed -= HandleObjectDestroyed;
    }

    private void HandleObjectDestroyed(float remainingLifespan)
    {
        int points = Mathf.RoundToInt( 100 * (4 - remainingLifespan)) ;
        score += points;
        canvasManager.UpdateScore(score);
        // Add your functionality here (e.g., updating UI, scores, etc.)
    }
}
