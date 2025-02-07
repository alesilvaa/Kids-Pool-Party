using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomsManager : MonoBehaviour
{
    [SerializeField] private List<MomController> _moms;

    // Contador para saber cuántos MomController han finalizado
    private int momsFinished = 0;

    private void Awake()
    {
        // Suscribirse al evento de cada MomController
        foreach (var mom in _moms)
        {
            mom.OnMomFinished += MomFinished;
        }
    }

    // Método que se invoca cada vez que un MomController termina
    private void MomFinished()
    {
        momsFinished++;
        if(momsFinished >= _moms.Count)
        {
            NoMoreMoms();
        }
    }

    private void NoMoreMoms()
    {
        if (!GameManager.Instance.isLosePlayer)
        {
            // Cuando ya no quedan madres, se llama a la función OnWinPanel y se reproduce el sonido de victoria.
            EventsManager.Instance.WinPanel();
            SoundManager.Instance.PlayWinSound();
        }
    }
}