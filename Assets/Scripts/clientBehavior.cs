using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
//using ENCVAL_TEMP = System.Char[];
public class clientBehavior : MonoBehaviour
{

    enum PARSING_RESLULT
    {
        PR_GARBAGE,
        PR_GARBEAGE_OURFAULT,
        PR_ILLOGICAL,
        PR_PROTOMISMATCH,
        PR_PARTRECV,
        PR_TOKENINVALID,
        PR_SUCCESSFUL
    }
    enum MESSAGE_TYPE
    {
        CS_AUTHRQ,
        CS_LOGINRQ,
        CS_MAPJOINRQ,
        CS_GSSJOINRQ,
        CS_HSYNCRQ,
        CS_INVSYNCHRQ,
        CS_ANIMATIONRQ,


        SC_HSYNCRQ,
        SC_TOKENRSP,
    }


    struct Protocol_Msg
    {
        char[] wizardweedchecksum;//size 10
        ushort rqrspandpvr;
        uint message_len;
        char[] tokening;//size 2048
        MESSAGE_TYPE type;
        ulong message_id_or_response_to;
    }

    NetworkingUtils utils;

    [SerializeField] uint connection = 0;
    Address address;
    NetworkingSockets client;
    ushort port; // placeholder for port number
    StatusCallback statusCallback;
    MessageCallback messageCallback;

    void Awake()
    {
        Library.Initialize();
        client = new NetworkingSockets();
        utils = new NetworkingUtils();
        port = 1263;
        print("Awake runs");

    }
    void Start()
    {
        print("Start runs");
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


        // Unsure about the ip(it can change) or the port.
        address.SetAddress("10.230.45.81", port);
        print("Connecting to server");
        connection = client.Connect(ref address);
        Send("Hello, server!");


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


    private void Send(string message)
    {
        print("Sending message: " + message);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        client.SendMessageToConnection(connection, messageBytes);
    }

    public void SayHello()
    {
        print("Sending Hello, server!");
        Send("Hello, server!");
    }

    public void connectToServer()
    {
        print("conection attempt");
        connection = client.Connect(ref address);
    }
}

