using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;
    [SerializeField] private float lateralAmplitude = 0.5f;
    [SerializeField] private float lateralFrequency = 1f;
    [SerializeField] private float forwardAmplitude = 0.5f;
    [SerializeField] private float forwardFrequency = 1f;

    private Vector3 startPos;

    private void Start()
    {
        if (obstacle == null)
        {
            obstacle = transform.GetChild(0).gameObject; 
        }
        startPos = obstacle.transform.position;
    }

    private void Update()
    {
        float time = Time.time * Mathf.PI * 2;
        float yOffset = floatAmplitude * Mathf.Sin(time * floatFrequency);
        float xOffset = lateralAmplitude * Mathf.Sin(time * lateralFrequency);
        float zOffset = forwardAmplitude * Mathf.Cos(time * forwardFrequency);

        obstacle.transform.position = startPos + new Vector3(xOffset, yOffset, zOffset);
    }
}