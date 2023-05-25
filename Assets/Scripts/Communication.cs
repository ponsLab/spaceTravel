using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Connection with Python
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Communication : MonoBehaviour
{
    // Setting for socket
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    private TcpClient client;
    private Thread mThread;
    private NetworkStream nwStream;
    public static Vector3 receivedPos = new Vector3(0, 0, 0);
    public static bool running;

    // Start is called before the first frame update
    void Start()
    {
        //start thread(Get data from Python)
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetInfo()
    {
        client = new TcpClient(connectionIP, connectionPort);
        running = true;
        while (running)
        {
            SendAndReceiveData();
            //print(receivedPos);
        }
    }


    void SendAndReceiveData()
    {
        nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            //---Using received data---
            receivedPos = StringToVector3(dataReceived); //<-- assigning receivedPos value from Python
            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(receivedPos.y.ToString()); //Converting string to byte data
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}