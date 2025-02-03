using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FollowMouseAndClickEffect : MonoBehaviour
{
    [SerializeField] private RectTransform handImage; // Referencia a la imagen de la mano
    [SerializeField] private Canvas canvas; // Referencia al canvas
    [SerializeField] private float clickEffectScale = 0.8f; // Escala más pequeña al hacer clic
    [SerializeField] private float clickEffectDuration = 0.2f; // Duración del efecto

    private void Update()
    {
        FollowMouse();

        // Detectar clic y aplicar el efecto
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickEffect();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            UnplayClickEffect();
        }
    }

    private void FollowMouse()
    {
        // Obtener la posición del mouse en coordenadas de pantalla
        Vector2 mousePosition = Input.mousePosition;

        // Convertir la posición del mouse a coordenadas locales del Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        // Mover la imagen de la mano a las coordenadas locales calculadas
        handImage.anchoredPosition = localPoint;
    }

    private void PlayClickEffect()
    {
        // Achicar la imagen usando DoTween
        handImage.DOScale(clickEffectScale, clickEffectDuration)
            .SetEase(Ease.OutCubic);
    }
    
    private void UnplayClickEffect()
    {
        // Restaurar el tamaño original
        handImage.DOScale(1f, clickEffectDuration).SetEase(Ease.OutCubic);
    }
}