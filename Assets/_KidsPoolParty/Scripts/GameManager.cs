using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private void Awake()
    {
        // Implementar el patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void NextLevel()
    {
        // Obtiene el índice de la escena actual y le suma 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("Next scene index: " + nextSceneIndex);
        
        // Verifica si el índice de la siguiente escena es mayor o igual que la cantidad total de escenas
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("No more scenes, returning to first scene {SceneManager.sceneCountInBuildSettings");
            // Si no hay más escenas, vuelve a la primera escena (índice 0)
            nextSceneIndex = 0;
        }
        
        // Carga la siguiente escena
        SceneManager.LoadScene(nextSceneIndex);
    }
    
    public void RestartLevel()
    {
        // Obtiene el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Recarga la escena actual
        SceneManager.LoadScene(currentSceneIndex);
    }
}
