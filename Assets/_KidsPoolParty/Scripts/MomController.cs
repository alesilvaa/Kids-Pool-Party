using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomController : MonoBehaviour
{
    [Header("Posiciones")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    
    [Header("Configuración de Moms y Slots")]
    [SerializeField] private List<MomBehaviourScript> moms;
    [SerializeField] private Transform prefabSlotQueue;
    [SerializeField] private float slotSpacing = 1.0f; // Distancia entre cada slot en Z

    // Lista para almacenar los slots instanciados
    private List<Transform> slots = new List<Transform>();

    private void Start()
    {
        // Instanciar los slots para la cantidad de moms, colocándolos uno detrás de otro en Z.
        for (int i = 0; i < moms.Count; i++)
        {
            // La posición de cada slot se calcula a partir de startPosition, avanzando en Z.
            Vector3 slotPos = startPosition.position + new Vector3(0, 0, i * slotSpacing);
            Transform slotInstance = Instantiate(prefabSlotQueue, slotPos, Quaternion.identity, transform);
            slots.Add(slotInstance);
        }

        // Ubicar a cada mamá en su slot correspondiente.
        for (int i = 0; i < moms.Count; i++)
        {
            moms[i].transform.position = slots[i].position;
        }

        // Inicia el proceso secuencial de las moms.
        StartCoroutine(ProcessMoms());
    }

    private IEnumerator ProcessMoms()
    {
        // Mientras queden moms en la cola.
        while (moms.Count > 0)
        {
            // La primera mamá en la lista es la que se procesa.
            MomBehaviourScript currentMom = moms[0];
            yield return new WaitUntil(() => currentMom.IsHaveKid);
            yield return new WaitForSeconds(0.2f);

            // Una vez que ya tiene a su hijo, moverla a la posición final.
            StartCoroutine(currentMom.MoveTo(endPosition, .5f, true));
            currentMom.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DelayToDisableMom(currentMom));
            moms.RemoveAt(0);
            yield return new WaitForSeconds(.8f);
            
            // Mover cada mamá restante al slot correspondiente (cada una avanza al siguiente slot).
            for (int i = 0; i < moms.Count; i++)
            {
                // Se asume que el slot i de la lista slots corresponde a la posición destino.
                StartCoroutine(moms[i].MoveToNexSlot(slots[i].transform));
            }
            // Opcional: esperar un momento antes de procesar la siguiente mamá.
            yield return new WaitForSeconds(0.5f);
        }
        
        // Cuando ya no quedan madres, se llama a la función OnWinPanel.
        EventsManager.Instance.WinPanel();
        SoundManager.Instance.PlayWinSound();
    }

    private IEnumerator DelayToDisableMom(MomBehaviourScript mom)
    {
        yield return new WaitForSeconds(3f); // Espera 4 segundos en la posición final
        mom.gameObject.SetActive(false);
    }
}
