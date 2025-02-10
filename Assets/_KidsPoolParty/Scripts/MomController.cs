using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Necesario para el uso de eventos

public class MomController : MonoBehaviour
{
    private bool noMoreMoms = false;

    [Header("Posiciones")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    
    [Header("Configuración de Moms y Slots")]
    [SerializeField] private List<MomBehaviourScript> moms;
    [SerializeField] private Transform prefabSlotQueue;
    [SerializeField] private float slotSpacing = 1.0f; // Distancia entre cada slot

    [Header("Configuración de Instanciación")]
    [SerializeField] private bool usarEjeX = false; // Si es true se instanciará en el eje X; de lo contrario, en el eje Z.
    [SerializeField] private bool instanciarEnPositivo = true; // Si es true se instanciará en dirección positiva, si no, en negativa.

    // Lista para almacenar los slots instanciados
    private List<Transform> slots = new List<Transform>();

    // Propiedad para acceder a noMoreMoms si se necesita desde fuera
    public bool noMoreMomsActive => noMoreMoms;

    // Evento que se disparará cuando este MomController termine su proceso.
    public event Action OnMomFinished;

    private void Start()
    {
        // Instanciar los slots para la cantidad de moms, colocándolos uno detrás de otro en el eje seleccionado.
        for (int i = 0; i < moms.Count; i++)
        {
            float direccion = instanciarEnPositivo ? 1f : -1f;
            Vector3 offset = usarEjeX 
                ? new Vector3(i * slotSpacing * direccion, 0, 0) 
                : new Vector3(0, 0, i * slotSpacing * direccion);
            Vector3 slotPos = startPosition.position + offset;

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

            // Una vez que ya tiene a su hijo, moverla a la posición final.
            StartCoroutine(currentMom.MoveTo(endPosition, .5f, true));
            currentMom.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DelayToDisableMom(currentMom));
            moms.RemoveAt(0);
            yield return new WaitForSeconds(.5f);
            
            // Mover cada mamá restante al slot correspondiente (cada una avanza al siguiente slot).
            for (int i = 0; i < moms.Count; i++)
            {
                StartCoroutine(moms[i].MoveToNexSlot(slots[i].transform));
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        // Marca que este controlador ha terminado y dispara el evento.
        noMoreMoms = true;
        OnMomFinished?.Invoke();
    }

    private IEnumerator DelayToDisableMom(MomBehaviourScript mom)
    {
        yield return new WaitForSeconds(3f); // Espera 3 segundos en la posición final
        mom.gameObject.SetActive(false);
    }
}
