using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ausgabe im Debug-Fenster
        Debug.Log("Ein Objekt hat den Trigger betreten: " + other.gameObject.name);
    }
}
