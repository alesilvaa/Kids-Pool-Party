using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomBehaviourScript : MonoBehaviour
{
    public SOMoms mom;
    [SerializeField] private GameObject hands;
    private string _name;
    private Agentmovement agentmovement;
    private void Awake()
    {
        _name = mom.name;
        agentmovement = GetComponent<Agentmovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
            KidBehaviourScript kid = other.GetComponent<KidBehaviourScript>();
        if (other.CompareTag("Movable"))
        {
            if (kid.kidNames == _name)
            {
                Debug.Log("Mom: " + _name + " found her kid: " + kid.name);
                StartCoroutine(kid.JumpToHerMom(hands.transform));
                StartCoroutine(agentmovement.GotoExit());
            }
            else
            {
                Debug.Log("Mom: " + _name + " found a kid: " + kid.name + " but it's not hers");
            }
        }
    }
}
