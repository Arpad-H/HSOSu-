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
// input_data = f"{event.x};{event.y}"
// server_socket.sendto(input_data.encode(), ("localhost", 8081))
//
// asyncio.run(main())

public class PenConnection : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    public GameObject cursorDot;
    CultureInfo en_us = CultureInfo.GetCultureInfo("en-US");
    private float orthoSize;
    private float aspectRatio;
    private float height;
    private float width;
    // public Camera camera;


    float left;
    float right;
    float top;
    float bottom;

    void Start()
    {
        cursorDot = GameObject.Find("cursorDot");
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
            float x = float.Parse(stdfs[1], en_us.NumberFormat);
            float y = float.Parse(stdfs[0], en_us.NumberFormat)-5;
            Debug.Log("x: " +x + "y: " + y);
            x = Remap(x, 0, 83, -8.4f, 9.4f);
            y = Remap(y, 0, 118, -4.5f, 5.5f);
            Vector3 pos = new Vector3(x, y, 0.0f);
            //x = -10 + pos.x * 20;
            //y = -5 + pos.y * 10;
            //pos.x = x;
            //pos.y = y;
            cursorDot.transform.localPosition = pos;
            cursorDot.transform.rotation = Quaternion.Euler(0, 0, 0);
            // Debug.Log(pos);
            // Instantiate(cursorDot, pos, Quaternion.identity);
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