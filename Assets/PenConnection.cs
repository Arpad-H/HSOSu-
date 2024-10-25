using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

//
// import asyncio
// import socket
//
// from neosmartpen import Pen, dto
// from neosmartpen.pen_protocol.command_spec import SettingCommand
//
//
//     async def main():
// pen: Pen = None
// async for device in Pen.async_search():
// print(device)
// if device.name == "NSN-15":
// pen = await Pen.create(device)
// break
//
// server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
// server_socket.bind(("localhost", 8080))
//
// await pen.accept_paper()
//
// await pen.change_setting(SettingCommand.USE_HOVER, True)
//
// async for event in pen.events():
// if isinstance(event, dto.Dot):
// input_data = f"{event.x};{event.y};{event.tilt_x};{event.tilt_y}"
// server_socket.sendto(input_data.encode(), ("localhost", 8081))
//
// asyncio.run(main())

public class PenConnection : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    public GameObject cursorDot;
     CursorScript cursorScript;
    CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");
    private float orthoSize;
    private float aspectRatio;
    private float height;
    private float width;
    
    private int UDPPort;

    // public Camera camera;


    float left;
    float right;
    float top;
    float bottom;
    public float penX;
    public float penY;
    public float tilltX;
    public float tilltY;
    
    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    
    void Start()
    {
        cursorDot = GameObject.Find("cursorDot");
        cursorScript = cursorDot.GetComponent<CursorScript>();
        udpClient = new UdpClient(8081);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 8081);
    }
    // Vector3 ToUnityCoordinates(int x_osu, int y_osu)
    // {
    //     // Step 1: Normalize the osu coordinates (0 to 1 range)
    //     float x_norm = x_osu / 512f;
    //     float y_norm = y_osu / 384f;
    //
    //     // Step 2: Calculate Unity's camera dimensions
    //     float cameraHeight = 2f * gameCamera.orthographicSize;
    //     float cameraWidth = cameraHeight * gameCamera.aspect;
    //
    //     // Step 3: Map normalized osu coordinates to Unity's world coordinates
    //     float x_unity = (x_norm - 0.5f) * cameraWidth;
    //     float y_unity = (y_norm - 0.5f) * cameraHeight;
    //
    //     return new Vector3(x_unity, y_unity, 0f); // Z is zero for 2D
    // }
    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.ASCII.GetString(data);
            // Use the input to control the game
            // Debug.Log("Received: " + message); 
            String[] stdfs = (message.Split(";"));
            if (stdfs[0] == "UP")
            {
                cursorScript.penDown = false;
            }

            if (stdfs[0] == "DOWN")
            {
                cursorScript.penDown = true;
            }
            else if (stdfs[0] == "POS")
            {
                tilltY = float.Parse(stdfs[4], en_us.NumberFormat);
                tilltX = float.Parse(stdfs[3], en_us.NumberFormat);
                penX = float.Parse(stdfs[2], en_us.NumberFormat);
                penY = float.Parse(stdfs[1], en_us.NumberFormat)-5;
                // Debug.Log("penX: " +penX + "penY: " + penY);
                // Debug.Log("tilltX: " + tilltX + "tilltY: " + tilltY);
                penX = Remap(penX, 0, 83, -620f, 620f);
                penY = Remap(penY, 0, 118, -420f, 420f);
                Vector3 pos = new Vector3(penX, penY, -160f);
           
                //x = -10 + pos.x * 20;
                //y = -5 + pos.y * 10;
                //pos.x = x;
                //pos.y = y;
                cursorDot.transform.localPosition = pos;
                // Debug.Log(pos);
                // Instantiate(cursorDot, pos, Quaternion.identity);
            }
           
        }
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}