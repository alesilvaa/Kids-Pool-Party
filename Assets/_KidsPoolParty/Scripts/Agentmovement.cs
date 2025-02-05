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

    public IEnumerator GotoExit(Transform target, float timeDelay, bool isRotate = false )
    {
        yield return new WaitForSeconds(timeDelay);
        
        
        if (isRotate)
        {
            // Calcular la rotación final (180° sobre el eje Y)
            Quaternion finalRotation = momFBX.transform.rotation * Quaternion.Euler(0, -180, 0);

            // Rotar suavemente durante 1 segundo (ajusta la duración según necesites)
            yield return StartCoroutine(RotateSmooth(momFBX.transform, finalRotation, 0.19f));
        }
        

        

        // Activar la animación de caminar
        momBehaviourScript.ActivateAnimation(State.walk, true);

        // Ir al destino
        agentAuthoring.SetDestination(target.position);
        yield return new WaitForSeconds(2.25f);
        momBehaviourScript.ActivateAnimation(State.walk, false);
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