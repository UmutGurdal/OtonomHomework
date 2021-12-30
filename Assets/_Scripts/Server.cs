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
    public static Server ins;

    private List<ServerClient> clients;
    private List<int> disconnectIndex;

    [Header("Server Settings")]
    [SerializeField] private int Port = 55001;
    private TcpListener server;
    private bool serverStarted;

    [Space][Header("PublicVariables")]
    public bool IsConnected;
    public float[] ClientFloats = new float[6];

    private void Awake()
    {
        ins = this;
    }

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
            CheckConnection(clients[c].tcp);
            if (!IsConnected)
            {
                clients[c].tcp.Close();
                disconnectIndex.Add(c);
                Debug.Log(clients[c].clientName + " has disconnected from the server");
            }

            else
            {
                NetworkStream s = clients[c].tcp.GetStream();

                if (!s.DataAvailable) return;

                byte[] RecievedString = new byte[sizeof(float) * ClientFloats.Length];

                if (RecievedString == null) return;

                s.Read(RecievedString, 0, sizeof(float) * ClientFloats.Length);
                ClientFloats = ConvertBytes2Float(RecievedString);
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

    
    private void CheckConnection(TcpClient c)
    {
        if (c != null && c.Client != null && c.Client.Connected) IsConnected = true;
        else IsConnected = false;
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