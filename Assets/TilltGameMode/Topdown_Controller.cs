using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Topdown_Controller : MonoBehaviour
{
    private Rigidbody2D body;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");

    public float x_velocity;
    public float y_velocity;
    public float speed;
    public float x_tillt;
    public float y_tillt;
    public float drag_Force;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        udpClient = new UdpClient(8081);
        endPoint = new IPEndPoint(IPAddress.Any, 8081);
    }

    // Update is called once per frame
    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref endPoint);
            string message = Encoding.ASCII.GetString(data);
            // Use the input to control the game
            // Debug.Log("Received: " + message); 
            String[] stdfs = (message.Split(";"));
            y_tillt = float.Parse(stdfs[3], en_us.NumberFormat);
            x_tillt = float.Parse(stdfs[2], en_us.NumberFormat);
            movement(x_tillt, y_tillt);
        }
        else
        {
            checkMapBorder();
        }
        drag();
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(y_velocity * speed, x_velocity * speed);
    }
    
    void OnApplicationQuit()
    {
        udpClient.Close();
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

    private void checkMapBorder()
    {
        if (body.position.x > 7.7 || body.position.x < -7.7)
        {
            y_velocity = 0f;
        }
    }

    private void drag()
    {
        y_velocity = y_velocity * drag_Force;
        if ( y_velocity == 0.0000001f){
            y_velocity = 0f;
        }
    }
}
