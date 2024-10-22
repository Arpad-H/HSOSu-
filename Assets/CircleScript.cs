using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
    public delegate void ObjectDestroyed(float remainingLifespan);

    public static event ObjectDestroyed OnObjectDestroyed;


    public GameObject border;
    private float ttl = 2f;
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
        ttl = ttl - Time.deltaTime;

        border.transform.localScale = Vector3.Lerp(Vector3.one, initBorderScale, ttl / maxLifespan);

        if (ttl <= 0f)
        {
            Destroy(this.gameObject);
        }


        if (Input.GetKeyDown("space"))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hovered = true;
        Debug.Log("Triggered by: " + other.name);
        // Add your functionality here
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        hovered = false;
    }

    private void Interact()
    {
        if (hovered)
        {
            OnObjectDestroyed?.Invoke(ttl);
        }

        Destroy(this.gameObject);
    }


    private void OnDestroy()
    {
            //TODO play sprite anim
    }
}