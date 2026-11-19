using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using ClientStructs;
using static parsecontruct;
public class clientBehavior : MonoBehaviour
{
    string ip = "10.230.44.134";

    byte rqr = 0b00010000;
    byte rsp = 0b00000000;

    NetworkingUtils utils;

    uint connection = 0;
    Address address;
    NetworkingSockets client;
    ushort port;
    byte protoVer = 0b00000000;
    StatusCallback statusCallback;
    MessageCallback messageCallback;


    void Awake()
    {
        Library.Initialize();
        client = new NetworkingSockets();
        utils = new NetworkingUtils();
        port = 1263;

    }
    void Start()
    {
        statusCallback = (ref StatusInfo info) =>
        {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;


                case ConnectionState.Connected:
                    Debug.Log("Client connected to server - ID: " + connection);
                    break;


                case ConnectionState.ClosedByPeer:
                case ConnectionState.ProblemDetectedLocally:
                    client.CloseConnection(connection);
                    Debug.Log("Client disconnected from server");
                    break;
            }
        };


        utils.SetStatusCallback(statusCallback);


        address.SetAddress(ip, port);
        connectToServer();

        messageCallback = (in NetworkingMessage netMessage) =>
        {
            Debug.Log("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
        };
    }


    void Update()
    {
        client.RunCallbacks();

        client.ReceiveMessagesOnConnection(connection, messageCallback, 20);
    }


    public void sendMessageToServerTest()
    {
        byte[] randToken = new byte[2048];
        for (int i = 0; i < randToken.Length; i++)
        {
            randToken[i] = (byte)UnityEngine.Random.Range(0, 256);
        }
        client.SendMessageToConnection(connection,constructProtocolMsg(new byte[] { 0x0 }, rqr, randToken, MESSAGE_TYPE.CS_LOGINRQ, 0,protoVer));
    }
    public void connectToServer()
    {
        print("conection attempt");
        connection = client.Connect(ref address);
    }

    

}

