using UnityEngine;
using TMPro;

public class TimerUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    // Tiempo en segundos (2 minutos = 120 segundos)
    private float timeRemaining = 20f;
    private bool timerIsRunning = false;

    private void Start()
    {
        // Iniciamos el temporizador
        timerIsRunning = true;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Restamos el tiempo transcurrido desde el último frame
                timeRemaining -= Time.deltaTime;

                // Aseguramos que no baje de 0
                if (timeRemaining < 0)
                    timeRemaining = 0;

                // Convertimos los segundos restantes en minutos y segundos
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);

                // Actualizamos el texto del temporizador en formato MM:SS
                _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                // Cuando el tiempo se ha agotado, detenemos el temporizador
                timerIsRunning = false;
                Debug.Log("¡El tiempo se ha agotado!");
                EventsManager.Instance.LosePanel();
            }
        }
    }
}