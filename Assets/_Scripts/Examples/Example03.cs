using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType 
{
    Matlab,
    LocalKeyboard
}
public class Example03 : MonoBehaviour
{
    [SerializeField] private Transform InteractionObject;
    [SerializeField] private InputType inputType;

    private float speed = 5;
    private Vector2 movement;

    void Update()
    {
        if (!Server.ins.IsConnected) return;

        MoveObject();
    }

    private void MoveObject()
    {
        switch (inputType) 
        {
            case InputType.Matlab:
                Vector3 moveDir = new Vector3(Server.ins.ClientFloats[0], 0, Server.ins.ClientFloats[2]);
                InteractionObject.position += moveDir.normalized * speed * Time.deltaTime;
                break;

            case InputType.LocalKeyboard:
                moveDir = new Vector3(movement.x, 0, movement.y);
                InteractionObject.position += moveDir.normalized * speed * Time.deltaTime;
                break;
        }
    }
}
