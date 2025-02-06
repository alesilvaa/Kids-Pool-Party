using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]  private GameObject obstacle;
    // Amplitud y velocidad del movimiento vertical (flotación)
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    // Amplitud y velocidad del movimiento lateral (izquierda/derecha)
    [SerializeField] private float lateralAmplitude = 0.5f;
    [SerializeField] private float lateralFrequency = 1f;

    // Amplitud y velocidad del movimiento hacia adelante y atrás
    [SerializeField] private float forwardAmplitude = 0.5f;
    [SerializeField] private float forwardFrequency = 1f;

    // Posición inicial del obstáculo
    private Vector3 startPos;

    private void Start()
    {
        // Guardamos la posición inicial para usarla como referencia
        startPos = obstacle.transform.position;
    }

    private void Update()
    {
        // Calculamos el desplazamiento vertical (flotación suave)
        float yOffset = floatAmplitude * Mathf.Sin(Time.time * floatFrequency * Mathf.PI * 2);

        // Calculamos el desplazamiento lateral (movimiento a izquierda y derecha)
        float xOffset = lateralAmplitude * Mathf.Sin(Time.time * lateralFrequency * Mathf.PI * 2);

        // Calculamos el desplazamiento en el eje Z (movimiento hacia adelante y atrás)
        float zOffset = forwardAmplitude * Mathf.Cos(Time.time * forwardFrequency * Mathf.PI * 2);

        // Aplicamos la nueva posición sumando los offsets a la posición inicial
        obstacle.transform.position = startPos + new Vector3(xOffset, yOffset, zOffset);
    }
}