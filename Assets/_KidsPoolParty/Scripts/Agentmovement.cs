using System.Collections;
using ProjectDawn.Navigation.Astar;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class Agentmovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject momFBX;
    [SerializeField] private AgentAuthoring agentAuthoring;
    [SerializeField] private Transform target;
    [SerializeField] private AgentAstarPathingAuthoring agentAstarPathingAuthoring;
    private bool once;

    public IEnumerator GotoExit()
    {
        yield return new WaitForSeconds(1.5f);

        if (!once)
        {
            // Rotar 180° sobre su pivote en el eje Y
            momFBX.transform.Rotate(Vector3.up, 180);

            // Esperar un poco antes de moverse
            yield return new WaitForSeconds(0.5f);

            // Activar la animación de caminar
            animator.SetBool("walk", true);

            // Ir al destino
            agentAuthoring.SetDestination(target.position);
            
            once = true;
        }
    }
}