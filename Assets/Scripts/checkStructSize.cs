using UnityEngine;
using Valve.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
public class checkStructSize : MonoBehaviour
{

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

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    struct Protocol_Msg
    {
        public byte[] wizardweedchecksum;//size 10
        public byte rqrspandpvr;
        public uint message_len;
        public byte[] tokening;//size 2048
        public MESSAGE_TYPE type;
        public ulong message_id_or_response_to;
    }



    private void CheckStructSize(byte[] message, byte rqrspandpvr, byte[] token, MESSAGE_TYPE type, ulong message_id_or_response_to)
    {
        Protocol_Msg msg = new Protocol_Msg();
        msg.rqrspandpvr = rqrspandpvr;
        msg.message_len = (uint)message.Length;
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
            .ToArray();

        print("Wizardweed checksum length is: " + msg.wizardweedchecksum.Length);
        print("rqrspandpvrBytes length is: " + rqrspandpvrBytes.Length);
        print("messageLengthBytes length is: " + messageLengthBytes.Length);
        print("token length is: " + msg.tokening.Length);
        print("typeBytes length is: " + typeBytes.Length);
        print("messageIDorResp length is: " + messageIdOrResponseToBytes.Length);
        print("fullMsg length is: " + fullMsg.Length);


        //Im missing 2 bytes
        //added 3 with different enum conversion, still one too many

    }

    public void sayHello()
    {
        byte[] randToken = new byte[2048];
        for (int i = 0; i < randToken.Length; i++)
        {
            randToken[i] = (byte)UnityEngine.Random.Range(0, 256);
        }
        CheckStructSize(new byte[] { 0x0 }, 0b1, randToken, MESSAGE_TYPE.CS_LOGINRQ, 0);
    }
}

