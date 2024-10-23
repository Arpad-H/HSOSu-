using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject != null)
        {
            Destroy(this.gameObject, 20f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
     }
}
