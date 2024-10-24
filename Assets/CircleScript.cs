using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class CircleScript : MonoBehaviour, Interactable
{
    public delegate void ObjectDestroyed(float remainingLifespan);

    public static event ObjectDestroyed OnObjectDestroyed;


    public GameObject border;
    public GameObject hitbox;
    public GameObject filling;
    private float ttl = 5f;
    private float maxLifespan;
    private Vector3 initBorderScale;

    bool hovered = false;

    // Start is called before the first frame update
    void Start()
    {
        maxLifespan = ttl;
        initBorderScale = border.transform.localScale;
    }

    // Update is called once per frame
    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0f) Destroy(this.gameObject);
        ttl = ttl - Time.deltaTime;

        border.transform.localScale = Vector3.Lerp(hitbox.transform.localScale, initBorderScale, ttl / maxLifespan);
        

       
    }
    

    public void KeyDown()
    {
            OnObjectDestroyed?.Invoke(ttl);
            Destroy(this.gameObject);
      
    }

    public void SetColor(Color c)
    {
        border.GetComponent<SpriteRenderer>().color = c;
        //hitbox.GetComponent<SpriteRenderer>().color = c;
        filling.GetComponent<SpriteRenderer>().color = c;
        
    }

    public HitObject GetTypeOfHitObject()
    {
        return HitObject.Dot;
    }

    public void KeyUp()
    {
        
    }

    private void OnDestroy()
    {
            //TODO play sprite anim
    }
}