using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomController : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private List<MomBehaviourScript> moms;

    private void Start()
    {
        // Desactivar todas las moms al inicio.
        foreach (var mom in moms)
        {
            mom.gameObject.SetActive(false);
        }
        // Inicia el proceso secuencial
        StartCoroutine(ProcessMoms());
    }

    private IEnumerator ProcessMoms()
    {
        foreach (var mom in moms)
        {
            // Activar la mom actual.
            mom.gameObject.SetActive(true);

            // Mover a la posición de inicio.
            yield return StartCoroutine(mom.MoveTo(startPosition,0.1f));

            // Esperar a que la mamá obtenga a su hijo (_isHaveKid se ponga en true).
            yield return new WaitUntil(() => mom.IsHaveKid);
            yield return new WaitForSeconds(0.4f);

            // Una vez que ya tiene a su hijo, moverla a la posición final.
            yield return StartCoroutine(mom.MoveTo(endPosition,1, false));
            
            StartCoroutine(DelayToDisableMom(mom));
            
            
        }
    }

    private IEnumerator DelayToDisableMom(MomBehaviourScript mom)
    {
        yield return new WaitForSeconds(4f); // Espera 2 segundos en la posición final
        mom.gameObject.SetActive(false);
    }
}