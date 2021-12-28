using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net;
using System.IO;

public class Server : MonoBehaviour
{
    //General Init
    private List<ServerClient> clients;
    private List<int> disconnectIndex;

    //Camera Variables
    private Camera MainCam;

    [Header("Server Settings")]
    public int Port = 55001;
    private TcpListener server;
    private bool serverStarted;

    // MatlabVariables
    private float[] myFloat = new float[6];
    private void Awake()
    {
        MainCam = Camera.main;
    }

    // Use this for initialization
    void Start()
    {
        clients = new List<ServerClient>();
        disconnectIndex = new List<int>();

        try
        {
            server = new TcpListener(IPAddress.Any, Port);
            server.Start();

            Startlistening();
            serverStarted = true;
            Debug.Log("Server has started on port:" + Port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error " + e.Message);
        }

        InvokeRepeating("UpdateLoop", 0f, 0.003f);
    }

    private void UpdateLoop()
    {
        if (!serverStarted)
            return;
        if (clients.Count == 0)
            return;

        for (int c = 0; c < clients.Count; c++)
        {
            //Check if clients are connected
            if (!isConnected(clients[c].tcp))
            {
                clients[c].tcp.Close();
                disconnectIndex.Add(c);
                Debug.Log(clients[c].clientName + " has disconnected from the server");
                continue;
            }
            // Check for data from client
            else
            {
                Debug.LogWarning("isConnected = true");
                NetworkStream s = clients[c].tcp.GetStream();
                if (s.DataAvailable)
                {
                    byte[] RecievedString = new byte[sizeof(float) * 8];
                    Debug.LogWarning("data avaliable");

                    if (RecievedString != null)
                    {
                        Debug.LogWarning("string recieved");
                        s.Read(RecievedString, 0, sizeof(float) * 8);
                        myFloat = ConvertBytes2Float(RecievedString);
                        Debug.LogError(myFloat[3]);

                        MoveCamera(myFloat);

                    }
                    s.Flush();
                }
            }
        }

        //Clean up Disconnected Clients
        for (int i = 0; i < disconnectIndex.Count; i++)
        {
            clients.RemoveAt(disconnectIndex[i]);
        }
        disconnectIndex.Clear();
    }

    private float[] ConvertBytes2Float(byte[] byteArray)
    {
        var floatArray = new float[byteArray.Length / sizeof(float)];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    //Checks if client is connected
    private bool isConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            { //Makes sure the client is connected
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {         //Polls the Client for activity
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0); //Checks for response
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    //Begins connection with client
    private void AcceptServerClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        ServerClient NewClient = new ServerClient(listener.EndAcceptTcpClient(ar), null);
        Debug.Log("Someone has connected");
        clients.Add(NewClient);
        Startlistening();
    }

    //Starts listening on server socket
    private void Startlistening()
    {
        server.BeginAcceptTcpClient(AcceptServerClient, server);
    }

    //Try to close all the connections gracefully
    void OnApplicationQuit()
    {
        for (int i = 0; i < clients.Count; i++)
        {
            try
            {
                clients[i].tcp.GetStream().Close();
                clients[i].tcp.Close();
            }
            catch { }
        }
        Debug.Log("Connections Closed");
    }

    //Sends out data
    public void OutgoingData(ServerClient c, byte[] data)
    {
        NetworkStream ClientStream = c.tcp.GetStream();
        try
        {
            ClientStream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not write to client.\n Error:" + e);
        }
    }

    // Matlab used NED right-handed coordinate system
    // +x forward [optical axis]
    // +y right
    // +z down

    // Unity uses a wild left-handed coordinate system
    // +x right
    // +y up
    // +z forward [optical axis]

    //               matlab    unity
    // forward         x        z
    // right           y        x
    // down            z        -y

    private void MoveCamera(float[] pose)
    {
        Debug.LogWarning("MoveCamCalled");
        // x,y,z,yaw[z],pitch[y],roll[x]
        float x_trans = pose[0];
        float y_trans = pose[1];
        float z_trans = pose[2];
        float z_rot = pose[3];
        float y_rot = pose[4];
        float x_rot = pose[5];

        //Vector3 matlabTranslate = new Vector3(y_trans, -z_trans, x_trans);
        // perform translation
        //ClientObj.transform.position = transform.TransformVector(matlabTranslate);

        // perform rotation in yaw, pitch, roll order while converting to left hand coordinate system.
        MainCam.gameObject.transform.rotation = Quaternion.AngleAxis(z_rot, Vector3.up) *       // yaw [z]
                                       Quaternion.AngleAxis(-y_rot, Vector3.right) *    // pitch [y]
                                       Quaternion.AngleAxis(-x_rot, Vector3.forward);  // roll [x]
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;
    public List<GameObject> ClientObj;

    public ServerClient(TcpClient clientSocket, string Name)
    {
        clientName = Name;
        tcp = clientSocket;
        ClientObj = new List<GameObject>();
    }
}