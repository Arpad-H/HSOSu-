using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DefaultNamespace;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SpinnerScript: MonoBehaviour, Interactable
{
    public delegate void SpinnerDestroyed(int score);

    public static event SpinnerDestroyed OnSpinnerDestroyed;

    public GameObject t;
    public GameObject cursor;
    private CursorScript cursorScript;
    private float _ttl = 5f;
    Vector3 lastPosition;
    bool pressed = false;

    private float totalDegTravelled = 0;
    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("cursorDot");
        cursorScript = cursor.GetComponent<CursorScript>();
      
    }

    // Update is called once per frame
    private void Update()
    {
        pressed = cursorScript.penDown;
        _ttl -= Time.deltaTime;
        //  transform.Rotate(0f, 0f, 40 * Time.deltaTime);
        if (_ttl < 0)
        { 
            
            int scoreMultiplier =Mathf.FloorToInt( totalDegTravelled / 360);
            OnSpinnerDestroyed?.Invoke(scoreMultiplier);
            Destroy(t.gameObject);
        }

        if (pressed)
        {
            Vector3 currentPos = cursor.transform.position.normalized;
            float angle = -Vector3.SignedAngle(currentPos, lastPosition, Vector3.forward);
            if (angle != 0)  Debug.Log(angle);;
           
            totalDegTravelled += angle;
            transform.Rotate(0, 0, angle);
            lastPosition = currentPos;
          
        }

        
    }
    
    public void KeyDown()
    { 
    }

    public HitObject GetTypeOfHitObject()
    {
        return new HitObject();
    }

    public void KeyUp()
    {  
    }

    public void SetTTl(float i)
    {
        _ttl = i;
    }
}