using UnityEngine;
using TMPro;
using System.Collections;

public class TimerUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private float timeRemaining = 120f;
    private bool timerIsRunning = false;

    private void Start()
    {
        timerIsRunning = true;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (timerIsRunning && timeRemaining > 0)
        {
            timeRemaining -= 1f;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            _timerText.text = $"{minutes:00}:{seconds:00}";

            yield return new WaitForSeconds(1f); // Espera 1 segundo antes de la siguiente actualización
        }

        timerIsRunning = false;
        Debug.Log("¡El tiempo se ha agotado!");

        if (!GameManager.Instance.isWinPlayer)
        {
            EventsManager.Instance.LosePanel();
        }
    }
}