using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using ClientStructs;
public class clientBehavior : MonoBehaviour
{
    string ip = "10.230.44.134";

    byte rqr = 0b00010000;
    byte rsp = 0b00000000;

    NetworkingUtils utils;

    [SerializeField] uint connection = 0;
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


        // Unsure about the ip(it can change) or the port.
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


    private void SendMessageToServer(byte[] message, byte rqrspandpvr, byte[] token, MESSAGE_TYPE type, ulong message_id_or_response_to)
    {

        
        Protocol_Msg msg = new Protocol_Msg();
        msg.rqrspandpvr = (byte)(rqrspandpvr|protoVer);
        msg.message_len = (uint)message.Length +2075;
        msg.tokening = token;
        msg.type = type;
        msg.message_id_or_response_to = message_id_or_response_to;
        msg.wizardweedchecksum = Encoding.UTF8.GetBytes("WIZARDWEED");

        byte[] rqrspandpvrBytes = new byte[] { rqrspandpvr };
        byte[] messageLengthBytes = BitConverter.GetBytes(msg.message_len);
        byte[] typeBytes = BitConverter.GetBytes((int)msg.type);
        byte[] messageIdOrResponseToBytes = BitConverter.GetBytes(msg.message_id_or_response_to);

        byte[] fullMsg = msg.wizardweedchecksum
            .Concat(rqrspandpvrBytes)
            .Concat(messageLengthBytes)
            .Concat(msg.tokening)
            .Concat(typeBytes)
            .Concat(messageIdOrResponseToBytes)
            .Concat(message)
            .ToArray();

        client.SendMessageToConnection(connection, fullMsg);

    }

    public void sayHello()
    {
        byte[] randToken = new byte[2048];
        for (int i = 0; i < randToken.Length; i++)
        {
            randToken[i] = (byte)UnityEngine.Random.Range(0, 256);
        }
        SendMessageToServer(new byte[] { 0x0 }, rqr, randToken, MESSAGE_TYPE.CS_LOGINRQ, 0);
    }
    public void connectToServer()
    {
        print("conection attempt");
        connection = client.Connect(ref address);
    }

    

}

