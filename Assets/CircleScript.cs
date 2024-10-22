using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
    private float ttl = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ttl = ttl - Time.deltaTime;
        if (ttl <= 0f) Destroy(this.gameObject);
    }
}
