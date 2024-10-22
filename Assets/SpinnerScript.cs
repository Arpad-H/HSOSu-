using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript: MonoBehaviour
{
    private float _ttl = 8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _ttl -= Time.deltaTime;
        transform.Rotate(0f, 0f, 40 * Time.deltaTime);
        if (_ttl <= 0f) Destroy(this.gameObject);
    }


}