using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using System.IO;
using System.Text;


public class Communication : MonoBehaviour
{
    TcpListener listener;
    [SerializeField] private String msg;

    // Start is called before the first frame update
    void Start()
    {
        // LISTEN TO MATLAB. Set up unity listening to matlab

        listener = new TcpListener(IPAddress.Parse("192.168.0.10"), 55001);
        listener.Start();
        Debug.Log("is listening");

    }

    // Update is called once per frame
    void Update()
    {
        if (!listener.Pending()) return;

        print("socket comes");
        TcpClient client = listener.AcceptTcpClient();
        NetworkStream ns = client.GetStream();
        StreamReader reader = new StreamReader(ns);
        msg = reader.ReadToEnd();
        Debug.Log(msg);
    }
}
