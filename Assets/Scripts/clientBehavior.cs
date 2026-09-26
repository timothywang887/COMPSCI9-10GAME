using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
        public char[] wizardweedchecksum;//size 10
        public byte rqrspandpvr;
        public uint message_len;
        public char[] tokening;//size 2048
        public MESSAGE_TYPE type;
        public ulong message_id_or_response_to;
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


        // Unsure about the ip(it can change) or the port.
        address.SetAddress("10.230.45.81", port);
        print("Connecting to server");
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


    private void SendMessageToServer(string message, byte rqrspandpvr, char[] token, MESSAGE_TYPE type, ulong message_id_or_response_to)
    {
        print("Sending message: " + message);
        Protocol_Msg msg = new Protocol_Msg();
        msg.rqrspandpvr = rqrspandpvr;
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        msg.message_len = (uint)messageBytes.Length;
        msg.tokening = token;
        msg.type = type;
        msg.message_id_or_response_to = message_id_or_response_to;

        byte[] wizardweedchecksumBytes = Encoding.UTF8.GetBytes(msg.wizardweedchecksum);
        byte[] messageLengthBytes = Encoding.UTF8.GetBytes(msg.message_len.ToString());
        byte[] tokeningBytes = Encoding.UTF8.GetBytes(msg.tokening);
        byte[] typeBytes = Encoding.UTF8.GetBytes(msg.type.ToString());
        byte[] messageIdOrResponseToBytes = Encoding.UTF8.GetBytes(msg.message_id_or_response_to.ToString());

        byte[] fullMsg = wizardweedchecksumBytes
            .Concat(messageIdOrResponseToBytes)
            .Concat(messageLengthBytes)
            .Concat(tokeningBytes)
            .Concat(typeBytes)
            .Concat(messageIdOrResponseToBytes)
            .Concat(messageBytes)
            .ToArray();

        client.SendMessageToConnection(connection, fullMsg);
    }


    public void connectToServer()
    {
        print("conection attempt");
        connection = client.Connect(ref address);
    }
}

