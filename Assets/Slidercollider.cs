using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Slidercollider : MonoBehaviour, Interactable
{
    public delegate void SliderDestroyed(int score);

    public static event SliderDestroyed OnSliderDestroyed;

    public GameObject sliderParent;
    bool dragging = false;
    private float ttl;
   
    void Start()
    {
        sliderParent.GetComponent<SliderScript>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
   

    public HitObject GetTypeOfHitObject()
    {
        return HitObject.Slider;
    }

    public void KeyUp()
    {
        
        Destroy(sliderParent);
    }

    public void KeyDown()
    {
        dragging = true;
    }

    public void OnDestroy()
    {
        if (dragging)
        {
            float ttl = sliderParent.GetComponent<SliderScript>().GetTTL();
            float maxTtl = sliderParent.GetComponent<SliderScript>().GetMaxTTL();
            int score =  Mathf.RoundToInt((maxTtl - ttl) * 100) ;
            OnSliderDestroyed?.Invoke(score);
            Destroy(sliderParent);
            //finished movemnt award points based on distance travlled
        }
    }
}
