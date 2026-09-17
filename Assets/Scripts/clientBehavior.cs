using UnityEngine;
using ValveSockets;
using System;
using System.Text;
public class clientBehavior : MonoBehaviour
{
    uint connection = 0;
    Address address = new Address();

    int port = 67; // placeholder for port number
    void Start()
    {
        utils.SetStatusCallback(status);
        address.SetAddress("10.230.41.9"/*I think this is the correct IP address, unsure, espcially if it may change*/, port);
        connection = client.Connect(ref address);
    }

    void Update()
    {
        if (VALVESOCKETS_SPAN)
        {
            MessageCallback message = (in NetworkingMessage netMessage) =>{
		        Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
            };
        }
        else
        {
            const int maxMessages = 20;

            NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        }

        if(!Console.KeyAvailable)
        {
            client.RunCallbacks();

            if (VALVESOCKETS_SPAN)
            {
                client.ReceiveMessagesOnConnection(connection, message,20);
            }
            else
            {
                int netMessagesCount = client.ReceiveMessagesOnConnection(connection, netMessages, maxMessages);

                if(netMessagesCount > 0)
                {
                    for (int i = 0; i < netMessagesCount; i++)
                    {
                        Console.WriteLine("Message received from server - Channel ID: " + netMessages.channel + ", Data length: " + netMessages.length);

                        netMessage.Destroy();
                    }
                }
            }
        }
        
    }
    NetworkingSockets client = new NetworkingSockets();



    StatusCallback status = (ref StatusInfo info) => {
        switch (info.connectionInfo.state) {
            case ConnectionState.None:
                break;

            case ConnectionState.Connected:
                Console.WriteLine("Client connected to server - ID: " + connection);
                break;

            case ConnectionState.ClosedByPeer:
            case ConnectionState.ProblemDetectedLocally:
                client.CloseConnection(connection);
                Console.WriteLine("Client disconnected from server");
                break;
        }
    };

    private void sendMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        sockets.SendMessageToConnection(connection, messageBytes);
    }



}
