using UnityEngine;
using ValveSockets;
using System;
using System.Text;
using ENCVAL_TEMP = System.Char[2048];
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
        char[10] wizardweedchecksum;
        ushort rqrspandpvr;
        uint message_len;
        ENVAL_TEMP tokening;
        MESSAGE_TYPE type;
        ulong message_id_or_response_to;
    }

    uint connection = 0;
    Address address = new Address();
    NetworkingSockets client = new NetworkingSockets();
    int port = 67; // placeholder for port number
    StatusCallback statusCallback;
    MessageCallback messageCallback;

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
        address.SetAddress("10.230.41.9", port);
        connection = client.Connect(ref address);

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

    private void SendMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        client.SendMessageToConnection(connection, messageBytes);
    }
}