using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class MultiplePencontroller : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");
    private Player_Controller Player0_Controller;
    private Player_Controller Player1_Controller;
    private Player_Controller Player2_Controller;
    private Player_Controller Player3_Controller;
    
    
    //Player Objekts
    public GameObject Player0;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Socked connection
        udpClient = new UdpClient(8081);
        endPoint = new IPEndPoint(IPAddress.Any, 8081);
        //Get Player Components
        Player0_Controller = Player0.GetComponent<Player_Controller>();
        Player1_Controller = Player1.GetComponent<Player_Controller>();
        Player2_Controller = Player2.GetComponent<Player_Controller>();
        Player3_Controller = Player3.GetComponent<Player_Controller>();
        InvokeRepeating("ProcessEvents", 0f, 1f / 150f); // fuert unabhängig von den fps die angebene Funktion 150 mal die Sekunde aus
    }

    // Update is called once per frame
    void ProcessEvents()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref endPoint);
            string message = Encoding.ASCII.GetString(data);
            String[] stdfs = (message.Split(";"));
            Debug.Log(stdfs[4]);
            switch (stdfs[0])
            {
                case "Pen0":
                    Player0_Controller.EventHandler(stdfs);
                    break;
                case "Pen1":
                    Player1_Controller.EventHandler(stdfs);
                    break;
                case "Pen2":
                    Player2_Controller.EventHandler(stdfs);
                    break;
                case "Pen3":
                    Player3_Controller.EventHandler(stdfs);
                    break;
                default:
                    Debug.Log("Unknown pennumber");
                    break;
            }
            
            
        }
    }
    
    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
