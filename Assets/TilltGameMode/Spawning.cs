using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawning : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Spike;
    public GameObject SpawnBox;
    private float tick = 5f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tick = tick - Time.deltaTime;
        if (tick < 0)
        {
            spawn();
            tick = 5;
        }
    }

    private void spawn()
    {
        float spawnPointX = Random.Range((SpawnBox.transform.position.x - SpawnBox.transform.localScale.x / 2),
            SpawnBox.transform.position.x + SpawnBox.transform.localScale.x / 2);
        float spwanPointY = SpawnBox.transform.position.y;
        Vector2 spawnPostion = new Vector2(spawnPointX, spwanPointY);
        Quaternion rotation = Quaternion.Euler(0, 0, 180);
        Instantiate(Spike, spawnPostion, rotation);
    }
}
