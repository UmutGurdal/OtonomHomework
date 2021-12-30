using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Example04 : MonoBehaviour
{
    [SerializeField] private Transform InteractionObject, Target;

    
    private float speed = 5;
    private Vector2 movement;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = InteractionObject.gameObject.GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        agent.SetDestination(Target.position);
    }
}
