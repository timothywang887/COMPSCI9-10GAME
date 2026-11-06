using UnityEngine;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Numerics;
public class encrypttest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string hexstring = "00af33d96bbf22dbaea1d5c17ed04cc77c75ac91e629e2c32eedf6e0adacb976df8b83a42cb67329c7e68cd7c590b3cf4daa9fa81f1f2d717bd98bc07fed76f83825a72c7fc74df3242a4fd69344d3eb6b381f8bc6df12a471b17cc8023afea9049d7f0073635774c9ae343f6a23bdbfe93582769c9f35b624d02a2930a07395ac1554ed65a85be10cd75677e01f6e92b74490d8b6a0b54106dbadb13016fd287d4f66d271f9cf97bc256fa887ea4f29f14f0ddb32ce5015f6acb21a83da564fe39ef89a0d1f47775800c8c20d365db22fe67e4928ac46153df4d771fdae45b97925a9d54ef0be9130e9795061ab4aad65c79e28ec467ba75059491c19e7d5a689";

    byte[] modulusBytes; 

    byte[] exponent = new byte[] { 0x01, 0x00, 0x01 };
    private string plaintext = "Hello, World!";

    RSAParameters rsaParams = new RSAParameters();
    
    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

    void Awake()
    {
        modulusBytes = StringToByteArray(hexstring);
    }

    void Start()
    {
        rsaParams.Modulus = modulusBytes;
        rsaParams.Exponent = exponent;
        rsa.ImportParameters(rsaParams);

        print("Plaintext: " + plaintext);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] encryptedBytes = rsa.Encrypt(plaintextBytes, false);
        print("Encrypted: " + Encoding.UTF8.GetString(encryptedBytes));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static byte[] StringToByteArray(String hex)
    {
    int NumberChars = hex.Length;
    byte[] bytes = new byte[NumberChars / 2];
    for (int i = 0; i < NumberChars; i += 2)
        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
    return bytes;
    }

}
