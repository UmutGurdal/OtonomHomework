using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example01 : MonoBehaviour
{
    [SerializeField] private Transform InteractionObject;

    void Update()
    {
        if (!Server.ins.IsConnected) return;

        RotateObject();
    }

    private void RotateObject() 
    {
        InteractionObject.rotation = Quaternion.Euler(Server.ins.ClientFloats[3], Server.ins.ClientFloats[4], Server.ins.ClientFloats[5]);
    }
}
