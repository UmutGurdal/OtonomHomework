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

    private List<ServerClient> clients;
    private List<int> disconnectIndex;

    [SerializeField] private Transform interactionObject;

    [Header("Server Settings")]
    [SerializeField] private int Port = 55001;
    private TcpListener server;
    private bool serverStarted;

    private float[] myFloat = new float[6];



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
    }

    private void Update()
    {
        if (!serverStarted)
            return;
        if (clients.Count == 0)
            return;

        for (int c = 0; c < clients.Count; c++)
        {

            if (!isConnected(clients[c].tcp))
            {
                clients[c].tcp.Close();
                disconnectIndex.Add(c);
                Debug.Log(clients[c].clientName + " has disconnected from the server");
            }

            else
            {
                NetworkStream s = clients[c].tcp.GetStream();

                if (!s.DataAvailable) return;

                byte[] RecievedString = new byte[sizeof(float) * 8];

                if (RecievedString == null) return;

                s.Read(RecievedString, 0, sizeof(float) * 8);
                myFloat = ConvertBytes2Float(RecievedString);
                Debug.LogError(myFloat[3]);

                MoveCamera(myFloat);

                s.Flush();
            }
        }

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
        if (c != null && c.Client != null && c.Client.Connected) return true;
        else return false;
    }


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

        Vector3 positionVector = new Vector3(x_trans, y_trans, z_trans);

        interactionObject.position = positionVector;


        interactionObject.rotation = Quaternion.AngleAxis(z_rot, Vector3.up) *       // yaw [z]
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