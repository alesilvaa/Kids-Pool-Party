using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle,
    walk,
    Jumping
}

public class MomBehaviourScript : MonoBehaviour
{
    public SOMoms mom;
    [SerializeField] private GameObject hands;
    [SerializeField] private Animator animator;
    private string _name;
    private Agentmovement agentmovement;
    private bool _isReadyToGo = false;
    private bool _isHaveKid = false;
    
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
                StartCoroutine(kid.DisableMovement());
                StartCoroutine(kid.JumpToHerMom(hands.transform));
                // Indicamos que ya encontró a su hijo y así se cumple la condición en el controlador.
                _isReadyToGo = true;
                _isHaveKid = true;
            }
            else
            {
                Debug.Log("Mom: " + _name + " found a kid: " + kid.name + " but it's not hers");
            }
        }
    }
    
    public void ActivateAnimation(State state, bool value)
    {
        // Convertir el enum a cadena para que coincida con el nombre del parámetro en el Animator.
        animator.SetBool(state.ToString(), value);
    }
    

    // Nuevo método para mover la mamá a una posición sin requerir el flag _isReadyToGo.
    public IEnumerator MoveTo(Transform target,float timeDelay ,bool isRotate = false)
    {
        yield return StartCoroutine(agentmovement.GotoExit(target,timeDelay ,isRotate));
    }
    
    public IEnumerator MoveToNexSlot(Transform target)
    {
        yield return StartCoroutine(agentmovement.GoToNextSlot(target));
    }

    // Propiedad para que MomController pueda leer el estado de _isHaveKid.
    public bool IsHaveKid
    {
        get { return _isHaveKid; }
    }
}
