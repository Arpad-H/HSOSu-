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
    CultureInfo en_us = CultureInfo.GetCultureInfo( "en-US" );
    void Start()
    {
        udpClient = new UdpClient(8081);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 8081);
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.ASCII.GetString(data);

            // Use the input to control the game
            Debug.Log("Received: " + message); 
            String[] stdfs = (message.Split(";"));
            float x = float.Parse(stdfs[0], en_us.NumberFormat);
            float y = float.Parse(stdfs[1], en_us.NumberFormat);
            Vector3 pos  = new Vector3(x,y,0.0f);
            cursorDot.transform.position = pos;
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
