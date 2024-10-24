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
    public float ySpawnpositionoffset;
    private SpriteRenderer spikeRenderer;
    private SpriteRenderer spawnBoxSpride;
    
    
    private float tick = 5f;
    private int amount = 1;
    
    void Start()
    {
       spikeRenderer = Spike.GetComponent<SpriteRenderer>();
       spawnBoxSpride = SpawnBox.GetComponent<SpriteRenderer>();
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
        //Spawn random in SpwanBox
        float spawnPointX = Random.Range((SpawnBox.transform.position.x - SpawnBox.transform.localScale.x / 2),
            SpawnBox.transform.position.x + SpawnBox.transform.localScale.x / 2);
        float spwanPointY = SpawnBox.transform.position.y;
        Vector2 spawnPostion = new Vector2(spawnPointX, spwanPointY + ySpawnpositionoffset);
        spikeRenderer.color = spawnBoxSpride.color;
        Quaternion rotation = Quaternion.Euler(0, 0, 180);
        //Color
        
        Instantiate(Spike, spawnPostion, rotation);
    }
}
