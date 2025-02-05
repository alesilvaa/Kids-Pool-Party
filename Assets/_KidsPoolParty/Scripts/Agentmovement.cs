using System;
using System.Collections;
using DG.Tweening;
using ProjectDawn.Navigation.Astar;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class Agentmovement : MonoBehaviour
{
    [SerializeField] private GameObject momFBX;
    [SerializeField] private AgentAuthoring agentAuthoring;
    [SerializeField] private AgentAstarPathingAuthoring agentAstarPathingAuthoring;
    private bool once;
    private MomBehaviourScript momBehaviourScript;

    private void Start()
    {
        momBehaviourScript = GetComponent<MomBehaviourScript>();
    }

    private IEnumerator RotateSmooth(Transform transformToRotate, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transformToRotate.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transformToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
    }

    public IEnumerator GotoExit(Transform target, float timeDelay, bool isRotate = false)
    {
        // Espera el tiempo de retardo
        yield return new WaitForSeconds(timeDelay);

        // Si se requiere rotación, la ejecuta suavemente
        if (isRotate)
        {
            // Calcula la rotación final (180° sobre el eje Y)
            Quaternion finalRotation = momFBX.transform.rotation * Quaternion.Euler(0, 120, 0);
            // Rota suavemente durante 0.19 segundos (ajusta la duración si es necesario)
            yield return StartCoroutine(RotateSmooth(momFBX.transform, finalRotation, 0.19f));
        }

        // Activa la animación de caminar
        momBehaviourScript.ActivateAnimation(State.walk, true);

        // Define la duración del movimiento (ajusta según sea necesario)
        float moveDuration = 2.25f;

        // Mueve el transform hacia la posición destino usando DOMove
        transform.DOMove(target.position, moveDuration).OnComplete(() =>
        {
            // Al completarse el movimiento, desactiva la animación de caminar
            momBehaviourScript.ActivateAnimation(State.walk, false);
        });

        // Espera a que se complete el movimiento para finalizar la corrutina
        yield return new WaitForSeconds(moveDuration);
    }

    
    public IEnumerator GoToNextSlot(Transform target)
    {
        yield return new WaitForSeconds(0.1f);
        momBehaviourScript.ActivateAnimation(State.walk, true);

        transform.DOMove(target.position, .4f).OnComplete(() =>
        {
            momBehaviourScript.ActivateAnimation(State.walk, false);
        });
    }


    
}