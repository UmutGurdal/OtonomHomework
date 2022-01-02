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
    [SerializeField] private float speed = 5, rotSpeed = 45;

    private float rot;
    private Vector2 movement;
    private Rigidbody rb;

    private void Awake()
    {
        rb = InteractionObject.gameObject.GetComponent<Rigidbody>();    
    }

    private void Update()
    {
        switch (inputType)
        {

            case InputType.Matlab:
                movement.x = Server.ins.ClientFloats[0];
                movement.y = Server.ins.ClientFloats[2];
                break;

            case InputType.LocalKeyboard:
                movement.x = Input.GetAxis("Horizontal");
                movement.y = Input.GetAxis("Vertical");
                break;
        }
    }

    void FixedUpdate()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        Vector3 moveDir;

        moveDir = InteractionObject.transform.forward * movement.y;
        rot += transform.rotation.y + (movement.x * rotSpeed * Time.deltaTime);

        InteractionObject.rotation = Quaternion.Euler(0, rot, 0);

        rb.AddForce(moveDir.normalized * speed, ForceMode.Force);
    }
}
