using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField]private GameObject navBar;
    [SerializeField] private GameObject _btnNextLevel;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _textWin;
    

    private void Start()
    {
        HideWinPanel();
        EventsManager.Instance.OnWinPanel += ShowWinPanel;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnWinPanel -= ShowWinPanel;
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
        AnimateWinPanel();
    }
    
    public void HideWinPanel()
    {
        _winPanel.SetActive(false);
    }
    
    public void NextLevel()
    {
        SoundManager.Instance.PlayClickSound();
        GameManager.Instance.NextLevel();
        HideWinPanel();
    }

    // Función que anima de forma "juicy" los elementos del panel
    private void AnimateWinPanel()
    {
        // Inicializamos el scale de cada elemento a 0 para iniciar la animación
        _textWin.transform.localScale = Vector3.zero;
        _btnNextLevel.transform.localScale = Vector3.zero;
        _icon.transform.localScale = Vector3.zero;

        // Se animan con un DOScale hasta llegar a 1
        _textWin.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
        _icon.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.4f);

        // Para el botón, animamos primero la aparición y luego aplicamos un efecto bounce infinito
        _btnNextLevel.transform
            .DOScale(1f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.2f)
            .OnComplete(() => {
                // Efecto bounce: escala ligeramente superior y vuelve, en loop infinito
                _btnNextLevel.transform
                    .DOScale(1.2f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }
}