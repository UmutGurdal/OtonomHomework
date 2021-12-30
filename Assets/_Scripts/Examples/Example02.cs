using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example02 : MonoBehaviour
{
    [SerializeField] private Transform InteractionObject;

    void Update()
    {
        if (!Server.ins.IsConnected) return;

        MoveObject();
    }

    private void MoveObject()
    {
        InteractionObject.position = new Vector3(Server.ins.ClientFloats[3], Server.ins.ClientFloats[4], Server.ins.ClientFloats[5]);
    }
}
