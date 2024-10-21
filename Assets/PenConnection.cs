using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PenConnection : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;

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
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
