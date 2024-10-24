using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Text;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D body;
    private CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");
    public float x_velocity;
    public float y_velocity;
    public float speed;
    public float x_tillt;
    public float y_tillt;
    public float drag_Force;
    public float Pressur;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        drag();
        checkMapBorder();
        body.velocity = new Vector2(y_velocity * speed, x_velocity * speed);
        
    }

    public void EventHandler(String[] stdfs)
    {
        this.y_tillt = float.Parse(stdfs[4], en_us.NumberFormat);
        this.x_tillt = float.Parse(stdfs[3], en_us.NumberFormat);
        //this.Pressur = float.Parse(stdfs[5], en_us.NumberFormat);
        this.movement(x_tillt, y_tillt);
    }
    
    private void movement(float x_tillt, float y_tillt)
    {
        if (y_tillt > 100.0f || y_tillt < 84.0f)
        {
            y_velocity = y_velocity + y_tillt * 0.1f;
            if (y_tillt < 84.0f)
            {
                y_velocity = -(y_velocity + (100 - y_velocity));
            }
        }
        else
        {
            y_velocity = y_velocity - y_velocity / 2;
        }
        
        //Max Velocity
        if (y_velocity > 10f)
        {
            y_velocity = 10f;
        }

        if (y_velocity < -10f)
        {
            y_velocity = -10f;
        }
        // Map Boarder
        if ((body.position.x > 7.7 && y_tillt > 100.0f) || (body.position.x < -7.7 && y_tillt < 100.0f))
        {
            y_velocity = 0f;
        }
        
    }
    
    //private void FixedUpdate()
    //{
       // body.velocity = new Vector2(y_velocity * speed, x_velocity * speed);
    //}
    
    private void drag()
    {
        this.y_velocity = this.y_velocity * this.drag_Force;
        if ( this.y_velocity == 0.0000001f){
            this.y_velocity = 0f;
        }
    }
    private void checkMapBorder()
    {
        if (  (body.position.x >= 7.7 && body.position.x <= 7.5) 
            ||(body.position.x < -7.7 && body.position.x >= -7.5))
        {
            y_velocity = 0f;
        }
    }
}
