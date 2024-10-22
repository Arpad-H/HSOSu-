using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SliderScript: MonoBehaviour
{
    private float _ttl = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _ttl = _ttl - Time.deltaTime;
        if (_ttl <= 0f) Destroy(this.gameObject);
    }
    
}