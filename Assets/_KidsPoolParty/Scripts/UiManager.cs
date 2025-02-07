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
        AnimateBtn(_btnNextLevel);
        HideNavBar();
        
    }
    
    public void ShowLosePanel()
    {
        _losePanel.SetActive(true);
        AnimateBtn(_btnRestartLevel);
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
    private void AnimateBtn( GameObject btn)
    {
       
        btn.transform.localScale = Vector3.zero;
        btn.transform
            .DOScale(1f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.2f)
            .OnComplete(() => {
                // Efecto bounce: escala ligeramente superior y vuelve, en loop infinito
                btn.transform
                    .DOScale(1.1f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }
    
    
}