using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    
    public bool penDown = false;
    public GameObject outerCircle;
    GameObject currentHoveredObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        outerCircle.transform.Rotate(0, 0 ,50 * Time.deltaTime);
        
        
        if (Input.GetKeyDown("space"))
        {
            if (currentHoveredObject != null)
            {
                Interactable interactable = currentHoveredObject.GetComponent<Interactable>();
               interactable.KeyDown();
                // switch (interactable.GetTypeOfHitObject())
                // {
                //     case (HitObject.Dot):
                //         break;
                //     case (HitObject.Slider):
                //         break;
                //     case (HitObject.Spinner):
                //         break;
                // }
            }
            
        }

        if (Input.GetKeyUp("space"))
        {
            if (currentHoveredObject != null)
            {
                Interactable interactable = currentHoveredObject.GetComponent<Interactable>();
                interactable.KeyUp();
            }
        }
    }

    public void setLocation(Vector3 pos)
    {
        transform.position = pos;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        currentHoveredObject = other.gameObject;
       
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        interactable.KeyUp();
        currentHoveredObject = null;
    }
    
}