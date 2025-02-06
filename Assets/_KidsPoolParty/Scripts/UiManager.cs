using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField]private GameObject navBar;
    [SerializeField] private GameObject _btnNextLevel;
    [SerializeField] private GameObject _btnRestartLevel;
    [SerializeField] private Image _iconWIN;
    [SerializeField] private Image _iconLOSE; 
    [SerializeField] private TextMeshProUGUI _textWin;
    [SerializeField] private TextMeshProUGUI _textLose;
    

    private void Start()
    {
        ShowNavBar();
        HideWinPanel();
        HideLosePanel();
        EventsManager.Instance.OnWinPanel += ShowWinPanel;
        EventsManager.Instance.OnLosePanel += ShowLosePanel;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnWinPanel -= ShowWinPanel;
        EventsManager.Instance.OnLosePanel -= ShowLosePanel;
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
        AnimateWinPanel();
        HideNavBar();
        
    }
    
    public void ShowLosePanel()
    {
        _losePanel.SetActive(true);
        AnimateLosePanel();
        HideNavBar();
    }
    
    public void HideWinPanel()
    {
        _winPanel.SetActive(false);
    }
    
    public void HideLosePanel()
    {
        _losePanel.SetActive(false);
    }
    
    public void NextLevel()
    {
        SoundManager.Instance.PlayClickSound();
        GameManager.Instance.NextLevel();
        HideWinPanel();
    }
    
    public void RestartLevel()
    {
        SoundManager.Instance.PlayClickSound();
        GameManager.Instance.RestartLevel();
        HideLosePanel();
    }
    private void ShowNavBar()
    {
        navBar.SetActive(true);
    }
    
    private void HideNavBar()
    {
        navBar.SetActive(false);
    }

    // Función que anima de forma "juicy" los elementos del panel
    private void AnimateWinPanel()
    {
        // Inicializamos el scale de cada elemento a 0 para iniciar la animación
        _textWin.transform.localScale = Vector3.zero;
        _btnNextLevel.transform.localScale = Vector3.zero;
        _iconWIN.transform.localScale = Vector3.zero;

        // Se animan con un DOScale hasta llegar a 1
        _textWin.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
        _iconWIN.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.4f);

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
    
    private void AnimateLosePanel()
    {
        // Inicializamos el scale de cada elemento a 0 para iniciar la animación
        _textLose.transform.localScale = Vector3.zero;
        _btnRestartLevel.transform.localScale = Vector3.zero;
        _iconLOSE.transform.localScale = Vector3.zero;

        // Se animan con un DOScale hasta llegar a 1
        _textLose.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
        _iconLOSE.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.4f);

        // Para el botón, animamos primero la aparición y luego aplicamos un efecto bounce infinito
        _btnRestartLevel.transform
            .DOScale(1f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.2f)
            .OnComplete(() => {
                // Efecto bounce: escala ligeramente superior y vuelve, en loop infinito
                _btnRestartLevel.transform
                    .DOScale(1.2f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }
    
}